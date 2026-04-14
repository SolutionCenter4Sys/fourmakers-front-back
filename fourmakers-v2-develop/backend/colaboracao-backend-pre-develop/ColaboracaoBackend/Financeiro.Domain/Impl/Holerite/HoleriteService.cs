using Financeiro.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Financeiro.Holerite;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiClient.Domain.Interfaces;
using System.Text;
using Newtonsoft.Json;
using Aws.Infra.Interfaces;
using System.Net;
using Colaboracao.Core.Interfaces;
using System.Net.Http;
using Core.Domain.Financeiro.Holerite;
using Core.Domain.Usuario;
using Core.Domain.Apontamento;
using LoteFilaResult = DataTransferObject.Domain.Financeiro.Holerite.LoteFilaResult;
using Financeiro.Domain.Interfaces.Holerite;
using Colaboracao.Core.Exceptions;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using Core.Domain.Financeiro.Rubrica;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;

using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Holerite
{
    [LogDomainClass]
    public class HoleriteService : IHoleriteService
    {
        private readonly IStringLocalizer<ApontamentoMessage> _stringLocalizer;
        private readonly ICurriculoClient _curriculoClient;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IHoleriteRepository _holeriteRepository;
        private readonly ILoteRepository _loteRepository;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly IQueueProducer _queueProducer;
        private readonly IDBConnectionUnitOfWork _unitOfWork;
        private readonly IRubricaColaboradorRepository _rubricaColaboradorRepository;
        private readonly IRubricaRepository _rubricaRepository;

        public HoleriteService(
            IStringLocalizer<ApontamentoMessage> stringLocalizer,
            ICurriculoClient curriculoClient,
            IUsuarioColaboradorRepository usuarioColaboradorRepository,
            IHoleriteRepository holeriteRepository,
            ILoteRepository loteRepository,
            IUploadFilesClient uploadFilesClient,
            IQueueProducer queueProducer,
            IDBConnectionUnitOfWork unitOfWork,
            IRubricaColaboradorRepository rubricaColaboradorRepository,
            IRubricaRepository rubricaRepository)
        {
            _stringLocalizer = stringLocalizer;
            _curriculoClient = curriculoClient;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _holeriteRepository = holeriteRepository;
            _loteRepository = loteRepository;
            _uploadFilesClient = uploadFilesClient;
            _queueProducer = queueProducer;
            _unitOfWork = unitOfWork;
            _rubricaColaboradorRepository = rubricaColaboradorRepository;
            _rubricaRepository = rubricaRepository;
        }

        public async Task<bool> ProcessarHoleriteAsync(string loteId, string codigoColaborador, int orgId, TipoProcessamentoHoleriteEnum tipoProcessamento)
        {
            var lotes = await _holeriteRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.HOLERITE);
            if (lotes.Count == 0)
            {
                throw new InvalidOperationException($"Não foi encontrado nenhum lote para a organização {orgId}");
            }

            var lote = lotes.FirstOrDefault(l => l.Id == loteId);
            if (lote == null)
            {
                throw new InvalidOperationException($"Lote com ID {loteId} não encontrado");
            }

            if(lote.AprovadoParaProcessamento)
            {
                throw new InvalidOperationException($"Lote com ID {loteId} já foi aprovado para processamento");
            }

            if(lote.DataFinalizacao.HasValue)
            {
                throw new InvalidOperationException($"Lote com ID {loteId} já foi finalizado");
            }

            var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(codigoColaborador, orgId);
            if (usuario == null)
            {
                throw new InvalidOperationException($"Usuário com CPF {codigoColaborador} não encontrado na organização {orgId}");
            }

            // Buscar o PDF do S3
            using var httpClient = new HttpClient();
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("TOKEN_SISTEMA_COLABORACAO")));
            var pdfUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL").Replace("$1", token) + lote.Pdf;
            var pdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);
            
            // Extrair CNPJ do sumário
            string cnpj = "";
            if (!string.IsNullOrEmpty(lote.SumarioHoleriteString))
            {
                lote.SumarioHolerite = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(lote.SumarioHoleriteString);
                cnpj = lote.SumarioHolerite.Cnpj;
            }

            // Dividir PDF em páginas
            var paginas = PdfManipulationUtil.DividirPdfEmPaginas(pdfBytes);
            var queueUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("AWS_SQS_QUEUE_HOLERITE");
            
            // Primeira transação: Inserir todos os registros na tabela
            _unitOfWork.BeginTransaction();
            lote.SumarioHolerite.Adiantamento = tipoProcessamento == TipoProcessamentoHoleriteEnum.Adiantamento;
            lote.SumarioHolerite.Ferias = tipoProcessamento == TipoProcessamentoHoleriteEnum.Ferias;
            lote.SumarioHolerite.DecimoTerceiro = tipoProcessamento == TipoProcessamentoHoleriteEnum.DecimoTerceiro;
            lote.SumarioHolerite.DecimoTerceiroAdiantamento = tipoProcessamento == TipoProcessamentoHoleriteEnum.DecimoTerceiroAdiatamento;
            lote.SumarioHolerite.InformeDeRendimentos = tipoProcessamento == TipoProcessamentoHoleriteEnum.InformeRendimentos;
            await _loteRepository.AtualizarLoteAsync(loteId, true, null, JsonConvert.SerializeObject(lote.SumarioHolerite));
            try
            {
                var itensLote = new List<(string itemLoteId, string nomeArquivoPagina, FilaMessageDTO mensagem, Dictionary<string, string> messageAttributes)>();
                
                foreach (var pagina in paginas)
                {
                    // Salvar página no S3
                    var dataHora = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                    var nomeArquivoPagina = $"holerite/paginas/{loteId}_{paginas.IndexOf(pagina) + 1}_{dataHora}.pdf";
                    await _uploadFilesClient.UploadFile(nomeArquivoPagina, pagina);

                    // Inserir item de lote
                    var itemLoteId = await _loteRepository.InserirItemLoteAsync(loteId, nomeArquivoPagina);

                    var mensagem = new FilaMessageDTO
                    {
                        Cnpj = cnpj,
                        OrgId = orgId,
                        LoteId = loteId,
                        ItemLoteId = itemLoteId,
                        UsuarioId = usuario.UsuarioId,
                        CodigoColaboradorSolicitante = codigoColaborador,
                        PdfPath = nomeArquivoPagina,
                        TipoProcessamento = tipoProcessamento.ToString()
                    };

                    var messageAttributes = new Dictionary<string, string>
                    {
                        { "loteId", loteId },
                        { "itemLoteId", itemLoteId },
                        { "orgId", orgId.ToString() },
                        { "pagina", (paginas.IndexOf(pagina) + 1).ToString() }
                    };

                    itensLote.Add((itemLoteId, nomeArquivoPagina, mensagem, messageAttributes));
                }
                
                // Segunda fase: Enviar todos os itens para a fila SQS
                foreach (var (itemLoteId, nomeArquivoPagina, mensagem, messageAttributes) in itensLote)
                {
                    var sucesso = await _queueProducer.SendMessageAsync(queueUrl, JsonConvert.SerializeObject(mensagem), messageAttributes);
                    if (!sucesso.HttpStatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception($"Erro ao enviar página {messageAttributes["pagina"]} para a fila SQS");
                    }
                }
                _unitOfWork.Commit();
            }
            catch (System.Exception)
            {
                _unitOfWork.SafeRollback();
                throw;
            }

            return true;
        }

        public async Task<bool> ProcessarItemHoleriteAsync(FilaMessageDTO mensagem, string codigoColaboradorRequest, int orgId, string itemLoteId, string loteId)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                try
                {
                    var lote = await _holeriteRepository.DetalharLoteAsync(loteId);
                    if (lote == null)
                    {
                        throw new InvalidOperationException($"Lote com ID {loteId} não encontrado");
                    }

                    using var httpClient = new HttpClient();
                    var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("TOKEN_SISTEMA_COLABORACAO")));
                    var pdfUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL").Replace("$1", token) + mensagem.PdfPath;
                    var pdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);

                    var holeriteAnalise = await _curriculoClient.AnaliseHoleriteColaborador("application/pdf", Convert.ToBase64String(pdfBytes), VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
                    if (holeriteAnalise.Data?.Holerite == null)
                    {
                        throw new ValidationException("Não foi possível analisar o holerite");
                    }
                    if (holeriteAnalise.Data.Holerite.Funcionario?.Cpf == null)
                    {
                        throw new ValidationException("Não foi possível analisar o holerite, pois o CPF do colaborador não foi encontrado");
                    }
                    if (holeriteAnalise.Data.Holerite.Empresa?.Cnpj == null)
                    {
                        throw new ValidationException("Não foi possível analisar o holerite, pois o CNPJ da empresa não foi encontrado");
                    }

                    string codigoInterno = _usuarioColaboradorRepository.GetCodigoInternoByDocumentoEOrg(holeriteAnalise.Data.Holerite.Funcionario.Cpf, orgId);
                    if (codigoInterno == null)
                    {
                        throw new ValidationException($"Não foi possível analisar o holerite, pois o Colaborador {holeriteAnalise.Data.Holerite.Funcionario.Nome} não foi encontrado. CPF: {holeriteAnalise.Data.Holerite.Funcionario.Cpf}");
                    }

                    var competencia = holeriteAnalise.Data.Holerite.Periodo.Referencia; // Formato MM/YYYY

                    if (mensagem.TipoProcessamento == TipoProcessamentoHoleriteEnum.InformeRendimentos.ToString())
                    {
                        var detalheItem = await DetalharLoteAsync(loteId);
                        competencia = detalheItem.Lote.Competencia;

                    }
                    
                    var holeriteJaExiste = await _holeriteRepository.ExisteHoleriteColaboradorPorItemLoteIdAsync(itemLoteId);

                    if (holeriteJaExiste)
                    {
                        await _holeriteRepository.AtualizarHoleriteColaboradorAsync(itemLoteId, JsonConvert.SerializeObject(holeriteAnalise.Data));
                    }
                    else
                    {
                        if (mensagem.TipoProcessamento == TipoProcessamentoHoleriteEnum.Adiantamento.ToString())
                        {
                            var rubrica = await _rubricaRepository.ObterRubricaPorCodigoAsync("5501", orgId);
                            if (rubrica == null)
                            {
                                throw new ValidationException("Não foi possível inserir o adiantamento salarial, pois a rubrica não foi encontrada");
                            }

                            var mesReferencia = int.Parse(holeriteAnalise.Data.Holerite.Periodo.Referencia.Split('/')[0]);
                            var anoReferencia = int.Parse(holeriteAnalise.Data.Holerite.Periodo.Referencia.Split('/')[1]);
                            var valorProventos = decimal.Parse(holeriteAnalise.Data.Holerite.Totais.Proventos, CultureInfo.GetCultureInfo("pt-BR"));

                            var rubricasExistentes = await _rubricaColaboradorRepository.ListarRubricasColaboradorAsync(
                                orgId,
                                null,
                                codigoInterno,
                                mesReferencia,
                                anoReferencia,
                                rubrica.Id.ToString(),
                                0,
                                1
                            );
                            var rubricaExistente = rubricasExistentes?.FirstOrDefault();

                            if (rubricaExistente != null)
                            {
                                var resultAtualizar = await _rubricaColaboradorRepository.AtualizarRubricaColaboradorAsync(new RubricaColaboradorInput
                                {
                                    Id = rubricaExistente.Id,
                                    AnoInicial = rubricaExistente.AnoInicial,
                                    MesInicial = rubricaExistente.MesInicial,
                                    AnoFinal = rubricaExistente.AnoFinal,
                                    MesFinal = rubricaExistente.MesFinal,
                                    CodigoInternoColaborador = rubricaExistente.CodigoInternoColaborador,
                                    RubricaId = rubricaExistente.RubricaId,
                                    Valor = valorProventos,
                                    CodigoRubricaFrequencia = rubricaExistente.CodigoRubricaFrequencia,
                                    CodigoInternoColaboradorAlteracao = codigoColaboradorRequest,
                                    Ativo = true,
                                    OrgId = orgId,
                                    Observacao = rubricaExistente.Observacao
                                });
                                if (resultAtualizar == null)
                                {
                                    throw new ValidationException("Não foi possível atualizar o adiantamento salarial, pois houve um erro ao atualizar a rubrica");
                                }
                            }
                            else
                            {
                                var resultInserir = await _rubricaColaboradorRepository.InserirRubricaColaboradorAsync(new RubricaColaboradorInput
                                {
                                    AnoInicial = anoReferencia,
                                    MesInicial = mesReferencia,
                                    CodigoInternoColaborador = new Guid(codigoInterno),
                                    RubricaId = rubrica.Id,
                                    Valor = valorProventos,
                                    CodigoRubricaFrequencia = "UNICA",
                                    CodigoInternoColaboradorAlteracao = codigoColaboradorRequest,
                                    Ativo = true,
                                    OrgId = orgId,
                                    Observacao = "",
                                    Id = Guid.NewGuid()
                                });
                                if (resultInserir == null)
                                {
                                    throw new ValidationException("Não foi possível inserir o adiantamento salarial, pois houve um erro ao inserir a rubrica");
                                }
                            }
                        }
                        else if (mensagem.TipoProcessamento == TipoProcessamentoHoleriteEnum.Mensal.ToString())
                        {
                            await _holeriteRepository.DeletarHoleritesColaboradorAsync(codigoInterno, competencia, orgId);
                        }

                        await _holeriteRepository.InserirHoleriteColaboradorAsync(new HoleriteColaboradorDTO
                        {
                            TbItemLoteId = itemLoteId,
                            CodigoInternoColaborador = codigoInterno,
                            ObjetoHoleriteString = JsonConvert.SerializeObject(holeriteAnalise.Data),
                            Competencia = competencia,
                            Cnpj = holeriteAnalise.Data.Holerite.Empresa.Cnpj,
                            Adiantamento = mensagem.TipoProcessamento == TipoProcessamentoHoleriteEnum.Adiantamento.ToString(),
                            Ferias = mensagem.TipoProcessamento == TipoProcessamentoHoleriteEnum.Ferias.ToString(),
                            DecimoTerceiro = mensagem.TipoProcessamento == TipoProcessamentoHoleriteEnum.DecimoTerceiro.ToString(),
                            DecimoTerceiroAdiantamento = mensagem.TipoProcessamento == TipoProcessamentoHoleriteEnum.DecimoTerceiroAdiatamento.ToString(),
                            InformeDeRendimentos = mensagem.TipoProcessamento == TipoProcessamentoHoleriteEnum.InformeRendimentos.ToString()
                        });
                    }

                    await _loteRepository.AtualizarItemLoteAsync(itemLoteId, true, JsonConvert.SerializeObject(new RetornoProcessamentoMensagemHoleriteDTO
                    {
                        Erros = new List<string> { },
                        StackTrace = "",
                        ProcessadoComSucesso = true,
                        Holerite = holeriteAnalise.Data.Holerite
                    }), DateTime.Now);

                    _unitOfWork.Commit();
                    await FinalizarLoteAsync(loteId);
                    return true;
                }
                catch (Exception ex)
                {
                    await _loteRepository.AtualizarItemLoteAsync(itemLoteId, false, JsonConvert.SerializeObject(new RetornoProcessamentoMensagemHoleriteDTO
                    {
                        Erros = new List<string> { ex.Message },
                        StackTrace = ex.StackTrace,
                        ProcessadoComSucesso = false,
                        Holerite = null
                    }), DateTime.Now);
                    _unitOfWork.Commit();
                    await FinalizarLoteAsync(loteId);
                    return false;
                }
            }
            catch (Exception)
            {
                _unitOfWork.SafeRollback();
                throw;
            }
        }

        private async Task FinalizarLoteAsync(string loteId)
        {
            var totalItensLote = await _loteRepository.GetTotalItensLoteAsync(loteId);
            var totalItensProcessados = await _loteRepository.GetTotalItensProcessadosLoteAsync(loteId);
            if(totalItensProcessados == totalItensLote)
            {
                await _loteRepository.AtualizarLoteAsync(loteId, true, DateTime.Now);
            }
        }

        public async Task<ReprocessarItensHoleriteResult> ReprocessarItensHoleriteAsync(List<string> itensLoteId, string codigoColaborador, int orgId)
        {
            if (itensLoteId == null || itensLoteId.Count == 0)
            {
                throw new InvalidOperationException("Nenhum item de lote informado para reprocessamento");
            }

            var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(codigoColaborador, orgId);
            if (usuario == null)
            {
                throw new InvalidOperationException($"Usuário com CPF {codigoColaborador} não encontrado na organização {orgId}");
            }

            var queueUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("AWS_SQS_QUEUE_HOLERITE");
            var naoEncontrados = new List<string>();
            var totalEnfileirados = 0;

            foreach (var itemLoteId in itensLoteId)
            {
                var item = await _loteRepository.ObterItemLotePorIdAsync(itemLoteId);
                if (item == null)
                {
                    naoEncontrados.Add(itemLoteId);
                    continue;
                }

                var lote = await _holeriteRepository.DetalharLoteAsync(item.LoteId);
                if (lote == null)
                {
                    naoEncontrados.Add(itemLoteId);
                    continue;
                }

                string cnpj = "";
                string tipoProcessamento = TipoProcessamentoHoleriteEnum.Mensal.ToString();
                if (!string.IsNullOrEmpty(lote.SumarioHoleriteString))
                {
                    lote.SumarioHolerite = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(lote.SumarioHoleriteString);
                    if (lote.SumarioHolerite != null)
                    {
                        cnpj = lote.SumarioHolerite.Cnpj;

                        if (lote.SumarioHolerite.Adiantamento == true)
                            tipoProcessamento = TipoProcessamentoHoleriteEnum.Adiantamento.ToString();
                        else if (lote.SumarioHolerite.Ferias == true)
                            tipoProcessamento = TipoProcessamentoHoleriteEnum.Ferias.ToString();
                        else if (lote.SumarioHolerite.DecimoTerceiro == true)
                            tipoProcessamento = TipoProcessamentoHoleriteEnum.DecimoTerceiro.ToString();
                        else if (lote.SumarioHolerite.DecimoTerceiroAdiantamento == true)
                            tipoProcessamento = TipoProcessamentoHoleriteEnum.DecimoTerceiroAdiatamento.ToString();
                        else if (lote.SumarioHolerite.InformeDeRendimentos == true)
                            tipoProcessamento = TipoProcessamentoHoleriteEnum.InformeRendimentos.ToString();
                    }
                }

                await _loteRepository.ResetarItemLoteAsync(itemLoteId);

                var mensagem = new FilaMessageDTO
                {
                    Cnpj = cnpj,
                    OrgId = orgId,
                    LoteId = item.LoteId,
                    ItemLoteId = itemLoteId,
                    UsuarioId = usuario.UsuarioId,
                    CodigoColaboradorSolicitante = codigoColaborador,
                    PdfPath = item.FilePath,
                    TipoProcessamento = tipoProcessamento
                };

                var messageAttributes = new Dictionary<string, string>
                {
                    { "loteId", item.LoteId },
                    { "itemLoteId", itemLoteId },
                    { "orgId", orgId.ToString() }
                };

                var sucesso = await _queueProducer.SendMessageAsync(queueUrl, JsonConvert.SerializeObject(mensagem), messageAttributes);
                if (sucesso.HttpStatusCode.Equals(System.Net.HttpStatusCode.OK))
                {
                    totalEnfileirados++;
                }
            }

            return new ReprocessarItensHoleriteResult
            {
                TotalSolicitados = itensLoteId.Count,
                TotalEnfileirados = totalEnfileirados,
                ItensNaoEncontrados = naoEncontrados
            };
        }

        public async Task<SumarioHoleriteResult> ObterSumarioHoleriteAsync(byte[] arquivoPdf, string codigoColaborador, int orgId)
        {
            try
            {
                byte[] primeiraPaginaBytes = PdfManipulationUtil.ExtrairPrimeiraPagina(arquivoPdf);
                
                string base64Pdf = Convert.ToBase64String(primeiraPaginaBytes);
                string tokenAcesso = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN");
                
                var sumarioResult = await _curriculoClient.SumarioHolerite("application/pdf", base64Pdf, tokenAcesso);
                
                //Caso Férias primeira pagina assinatura
                if (sumarioResult.Data.Competencia.IsNullOrEmpty() || sumarioResult.Data.Competencia == "null")
                {
                    var paginas = PdfManipulationUtil.ObterQuantidadePaginas(arquivoPdf);
                    if (paginas == 2)
                    {
                        var paginasEmBytes = PdfManipulationUtil.DividirPdfEmPaginas(arquivoPdf);
                        arquivoPdf = paginasEmBytes[1];
                        base64Pdf = Convert.ToBase64String(arquivoPdf);
                        sumarioResult =
                            await _curriculoClient.SumarioHolerite("application/pdf", base64Pdf, tokenAcesso);
                    }
                }

                var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(codigoColaborador, orgId);
                if (usuario == null)
                {
                    throw new InvalidOperationException($"Usuário com CPF {codigoColaborador} não encontrado na organização {orgId}");
                }

                var dataHora = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                var nomeArquivo = $"holerite/{codigoColaborador}_{dataHora}.pdf";
                
                await _uploadFilesClient.UploadFile(nomeArquivo, arquivoPdf);
                var sumario = sumarioResult.Data;
                sumario.Adiantamento = false;
                sumario.Ferias = false;
                sumario.DecimoTerceiro = false;
                sumario.DecimoTerceiroAdiantamento = false;
                var quantidadeItens = PdfManipulationUtil.ObterQuantidadePaginas(arquivoPdf);
                var loteId = await _loteRepository.CriarLoteAsync(
                    quantidadeItens, 
                    nomeArquivo, 
                    orgId, 
                    usuario.UsuarioId, 
                    TipoFilaEnum.HOLERITE,
                    JsonConvert.SerializeObject(sumario)
                );

                var resultado = new SumarioHoleriteResult();
                resultado.Id = loteId;
                resultado.Cnpj = sumario.Cnpj;
                resultado.Competencia = sumario.Competencia;
                resultado.Empresa = sumario.Empresa;
                resultado.QuantidadeItens = quantidadeItens;
                resultado.QuantidadeItensProcessados = 0;
                resultado.QuantidadeItensProcessadosComErro = 0;
                
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar sumário do holerite: {ex.Message}", ex);
            }
        }



        public async Task<List<LoteFilaResult>> BuscarLotesPorOrgAsync(int orgId)
        {
            try
            {
                var result = new List<LoteFilaResult>();
                var lotes = await _holeriteRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.HOLERITE);
                
                // Deserializar o sumário JSON para cada lote
                foreach (var lote in lotes)
                {
                    if (!string.IsNullOrEmpty(lote.SumarioHoleriteString))
                    {
                        lote.SumarioHolerite = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(lote.SumarioHoleriteString);
                        if(lote.SumarioHolerite != null && lote.SumarioHolerite.Adiantamento == null)
                        {
                            lote.SumarioHolerite.Adiantamento = false;
                        }
                        if(lote.SumarioHolerite != null && lote.SumarioHolerite.Ferias == null)
                        {
                            lote.SumarioHolerite.Ferias = false;
                        }
                        if(lote.SumarioHolerite != null && lote.SumarioHolerite.DecimoTerceiro == null)
                        {
                            lote.SumarioHolerite.DecimoTerceiro = false;
                        }
                        if(lote.SumarioHolerite != null && lote.SumarioHolerite.DecimoTerceiroAdiantamento == null)
                        {
                            lote.SumarioHolerite.DecimoTerceiroAdiantamento = false;
                        }
                        if(lote.SumarioHolerite != null && lote.SumarioHolerite.InformeDeRendimentos == null)
                        {
                            lote.SumarioHolerite.InformeDeRendimentos = false;
                        }
                    }
                    result.Add(new LoteFilaResult()
                    {
                        Id = lote.Id,
                        Status = ObterStatusLote(lote),
                        SumarioHolerite = lote.SumarioHolerite,
                        DataCriacao = lote.DataCriacao,
                        DataFinalizacao = lote.DataFinalizacao,
                        QuantidadeDePaginas = lote.QuantidadeDePaginas,
                        Pdf = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL") + lote.Pdf,
                        AprovadoParaProcessamento = lote.AprovadoParaProcessamento
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar lotes da organização: {ex.Message}", ex);
            }
        }

        public string ObterStatusLote(LoteFilaHoleriteDTO lote)
        {
            if (lote.DataFinalizacao.HasValue)
            {
                return "Finalizado";
            }
            else if (lote.AprovadoParaProcessamento)
            {
                return "Em processamento";
            }
            return "Pendente";
        }

        public async Task<DetalhesLoteHoleriteResult> DetalharLoteAsync(string loteId)
        {
            try
            {
                var loteRaw = await _holeriteRepository.DetalharLoteAsync(loteId);
                if(loteRaw == null)
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não encontrado");
                }
                var sumario = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(loteRaw.SumarioHoleriteString);
            
                var lote = new SumarioHoleriteResult
                {
                    Id = loteRaw.Id,
                    Cnpj = sumario.Cnpj,
                    Competencia = sumario.Competencia,
                    Empresa = sumario.Empresa,
                    QuantidadeItens = loteRaw.QuantidadeDePaginas,
                    QuantidadeItensProcessados = await _loteRepository.GetTotalItensProcessadosLoteAsync(loteId),
                    QuantidadeItensProcessadosComErro = await _loteRepository.GetTotalItensProcessadosComErroLoteAsync(loteId),
                    Adiantamento = sumario.Adiantamento ?? false,
                    Ferias = sumario.Ferias ?? false,
                    DecimoTerceiro = sumario.DecimoTerceiro ?? false,
                    DecimoTerceiroAdiantamento = sumario.DecimoTerceiroAdiantamento ?? false
                };

                var itensRaw = await _loteRepository.ListarItensLoteAsync(loteId);
                var itens = new List<ItemLoteColaboradorDTO>();
                foreach (var item in itensRaw)
                {
                    var itemLote = new ItemLoteColaboradorDTO
                    {
                        Id = item.Id,
                        DataCriacao = item.DataCriacao,
                        DataFinalizacao = item.DataFinalizacao,
                        FilePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL") + item.FilePath,
                        Sucesso = item.Sucesso,
                        Retorno = item.Retorno
                    };
                    if(item.DataFinalizacao != null)
                    {
                        var respostaProcessamento = JsonConvert.DeserializeObject<RetornoProcessamentoMensagemHoleriteDTO>(item.Retorno);
                        if(item.Sucesso)
                        {
                            itemLote.Sucesso = true;
                            itemLote.Cpf = respostaProcessamento.Holerite.Funcionario.Cpf;
                            itemLote.Nome = respostaProcessamento.Holerite.Funcionario.Nome;
                            itemLote.Cargo = respostaProcessamento.Holerite.Funcionario.Funcao;
                            itemLote.Matricula = respostaProcessamento.Holerite.Funcionario.Matricula;
                        }
                        else
                        {
                            itemLote.Sucesso = false;
                            itemLote.Erro = respostaProcessamento.Erros[0];
                        }
                        
                    }
                    
                    itens.Add(itemLote);
                }
                return new DetalhesLoteHoleriteResult
                {
                    Lote = lote,
                    Itens = itens
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao detalhar lote: {ex.Message}", ex);
            }
        }

        public async Task DeletarLoteAsync(string loteId)
        {
            try
            {
                var lote = await _holeriteRepository.DetalharLoteAsync(loteId);
                if(lote == null)
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não encontrado");
                }

                if (!string.IsNullOrEmpty(lote.SumarioHoleriteString))
                {
                    var sumario = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(lote.SumarioHoleriteString);
                    if (sumario?.Adiantamento == true)
                    {
                        await _holeriteRepository.DeletarHoleritesAdiantamentoPorLoteAsync(loteId);
                    }
                    if (sumario?.Ferias == true)
                    {
                        await _holeriteRepository.DeletarHoleritesFeriasPorLoteAsync(loteId);
                    }
                    if (sumario?.DecimoTerceiro == true)
                    {
                        await _holeriteRepository.DeletarHoleritesDecimoTerceiroPorLoteAsync(loteId);
                    }
                    if (sumario?.DecimoTerceiroAdiantamento == true)
                    {
                        await _holeriteRepository.DeletarHoleritesAdiantamentoDecimoTerceiroPorLoteAsync(loteId);
                    }
                    if (sumario?.InformeDeRendimentos == true)
                    {
                        await _holeriteRepository.DeletarHoleritesInformeDeRendimentosPorLoteAsync(loteId);
                    }
                    else
                    {
                         if(lote.AprovadoParaProcessamento)
                        {
                            throw new InvalidOperationException($"Lote com ID {loteId} já foi aprovado para processamento");
                        }
                        if(lote.DataFinalizacao.HasValue)
                        {
                            throw new InvalidOperationException($"Lote com ID {loteId} já foi finalizado");
                        }
                    }
                }
                else {
                    if(lote.AprovadoParaProcessamento)
                    {
                        throw new InvalidOperationException($"Lote com ID {loteId} já foi aprovado para processamento");
                    }
                    if(lote.DataFinalizacao.HasValue)
                    {
                        throw new InvalidOperationException($"Lote com ID {loteId} já foi finalizado");
                    }
                }

                
                await _loteRepository.DeletarLoteAsync(loteId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar lote: {ex.Message}", ex);
            }
        }

        public async Task DeletarLoteAdiantamentoAsync(string loteId, int orgId)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                // Buscar lotes da organização para validação
                var lotesOrg = await _holeriteRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.HOLERITE);
                var loteEncontrado = lotesOrg.FirstOrDefault(l => l.Id == loteId);
                
                if(loteEncontrado == null)
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não encontrado ou não pertence à organização {orgId}");
                }

                // Validar se é um lote de adiantamento
                if (!string.IsNullOrEmpty(loteEncontrado.SumarioHoleriteString))
                {
                    var sumario = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(loteEncontrado.SumarioHoleriteString);
                    if (sumario?.Adiantamento != true)
                    {
                        throw new InvalidOperationException($"Lote com ID {loteId} não é um lote de adiantamento");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não possui sumário válido");
                }

                // Deletar holerites de adiantamento
                await _holeriteRepository.DeletarHoleritesAdiantamentoPorLoteAsync(loteId);

                // Deletar o lote (isso deletará automaticamente os itens de lote devido à cascade)
                await _loteRepository.DeletarLoteAsync(loteId);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.SafeRollback();
                throw new Exception($"Erro ao deletar lote de adiantamento: {ex.Message}", ex);
            }
        }
        
        public async Task DeletarLoteFeriasAsync(string loteId, int orgId)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                // Buscar lotes da organização para validação
                var lotesOrg = await _holeriteRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.HOLERITE);
                var loteEncontrado = lotesOrg.FirstOrDefault(l => l.Id == loteId);
                
                if(loteEncontrado == null)
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não encontrado ou não pertence à organização {orgId}");
                }

                // Validar se é um lote de adiantamento
                if (!string.IsNullOrEmpty(loteEncontrado.SumarioHoleriteString))
                {
                    var sumario = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(loteEncontrado.SumarioHoleriteString);
                    if (sumario?.Ferias != true)
                    {
                        throw new InvalidOperationException($"Lote com ID {loteId} não é um lote de férias");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não possui sumário válido");
                }

                // Deletar holerites de adiantamento
                await _holeriteRepository.DeletarHoleritesFeriasPorLoteAsync(loteId);

                // Deletar o lote (isso deletará automaticamente os itens de lote devido à cascade)
                await _loteRepository.DeletarLoteAsync(loteId);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.SafeRollback();
                throw new Exception($"Erro ao deletar lote de férias: {ex.Message}", ex);
            }
        }
        
        public async Task DeletarLoteDecimoTerceiroAsync(string loteId, int orgId)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                // Buscar lotes da organização para validação
                var lotesOrg = await _holeriteRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.HOLERITE);
                var loteEncontrado = lotesOrg.FirstOrDefault(l => l.Id == loteId);
                
                if(loteEncontrado == null)
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não encontrado ou não pertence à organização {orgId}");
                }

                // Validar se é um lote de adiantamento
                if (!string.IsNullOrEmpty(loteEncontrado.SumarioHoleriteString))
                {
                    var sumario = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(loteEncontrado.SumarioHoleriteString);
                    if (sumario?.DecimoTerceiro != true)
                    {
                        throw new InvalidOperationException($"Lote com ID {loteId} não é um lote de décimo terceiro");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não possui sumário válido");
                }

                // Deletar holerites de adiantamento
                await _holeriteRepository.DeletarHoleritesDecimoTerceiroPorLoteAsync(loteId);

                // Deletar o lote (isso deletará automaticamente os itens de lote devido à cascade)
                await _loteRepository.DeletarLoteAsync(loteId);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.SafeRollback();
                throw new Exception($"Erro ao deletar lote de décimo terceiro: {ex.Message}", ex);
            }
        }
        
        public async Task DeletarLoteAdiantamentoDecimoTerceiroAsync(string loteId, int orgId)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                // Buscar lotes da organização para validação
                var lotesOrg = await _holeriteRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.HOLERITE);
                var loteEncontrado = lotesOrg.FirstOrDefault(l => l.Id == loteId);
                
                if(loteEncontrado == null)
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não encontrado ou não pertence à organização {orgId}");
                }

                // Validar se é um lote de adiantamento
                if (!string.IsNullOrEmpty(loteEncontrado.SumarioHoleriteString))
                {
                    var sumario = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(loteEncontrado.SumarioHoleriteString);
                    if (sumario?.DecimoTerceiroAdiantamento != true)
                    {
                        throw new InvalidOperationException($"Lote com ID {loteId} não é um lote de adiantamento do décimo terceiro");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não possui sumário válido");
                }

                // Deletar holerites de adiantamento
                await _holeriteRepository.DeletarHoleritesAdiantamentoDecimoTerceiroPorLoteAsync(loteId);

                // Deletar o lote (isso deletará automaticamente os itens de lote devido à cascade)
                await _loteRepository.DeletarLoteAsync(loteId);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.SafeRollback();
                throw new Exception($"Erro ao deletar lote de adiantamento do décimo terceiro: {ex.Message}", ex);
            }
        }
        public async Task DeletarLoteInformeDeRendimentoAsync(string loteId, int orgId)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                // Buscar lotes da organização para validação
                var lotesOrg = await _holeriteRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.HOLERITE);
                var loteEncontrado = lotesOrg.FirstOrDefault(l => l.Id == loteId);
                
                if(loteEncontrado == null)
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não encontrado ou não pertence à organização {orgId}");
                }

                // Validar se é um lote de adiantamento
                if (!string.IsNullOrEmpty(loteEncontrado.SumarioHoleriteString))
                {
                    var sumario = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(loteEncontrado.SumarioHoleriteString);
                    if (sumario?.DecimoTerceiroAdiantamento != true)
                    {
                        throw new InvalidOperationException($"Lote com ID {loteId} não é um lote de adiantamento do décimo terceiro");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não possui sumário válido");
                }

                // Deletar holerites de adiantamento
                await _holeriteRepository.DeletarHoleritesInformeDeRendimentosPorLoteAsync(loteId);

                // Deletar o lote (isso deletará automaticamente os itens de lote devido à cascade)
                await _loteRepository.DeletarLoteAsync(loteId);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.SafeRollback();
                throw new Exception($"Erro ao deletar lote de adiantamento do décimo terceiro: {ex.Message}", ex);
            }
        }
    }
} 