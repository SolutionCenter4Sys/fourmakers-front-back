using Apontamento.Domain.Interfaces.Service;
using Colaboracao.Core;
using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiClient.Domain.Interfaces;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Domain.Apontamento;
using Core.Domain.Usuario;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office2019.Drawing.Chart;
using Aws.Infra.Interfaces;
using System.Net;
using Colaboracao.Core.Interfaces;
using System.Net.Http;
using Core.Domain.Projeto;
using Colaboracao.Core.Exceptions;

using Logs.Infra.Attributes;

namespace Apontamento.Domain.Impl.Service
{
    [LogDomainClass]
    public class FolhaPontoService : IFolhaPontoService
    {
        private readonly IApontamentoService _apontamentoService;
        private readonly IStringLocalizer<ApontamentoMessage> _stringLocalizer;
        private readonly ICurriculoClient _curriculoClient;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IFolhaPontoRepository _folhaPontoRepository;
        private readonly ILoteRepository _loteRepository;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly IQueueProducer _queueProducer;
        private readonly IDBConnectionUnitOfWork _unitOfWork;
        private readonly IAtividadeProjetoRepository _atividadeProjetoRepository;
    
        public FolhaPontoService(
            IApontamentoService apontamentoService,
            IStringLocalizer<ApontamentoMessage> stringLocalizer,
            ICurriculoClient curriculoClient,
            IUsuarioColaboradorRepository usuarioColaboradorRepository,
            IFolhaPontoRepository folhaPontoRepository,
            ILoteRepository loteRepository,
            IUploadFilesClient uploadFilesClient,
            IQueueProducer queueProducer,
            IDBConnectionUnitOfWork unitOfWork,
            IAtividadeProjetoRepository atividadeProjetoRepository)
        {
            _apontamentoService = apontamentoService;
            _stringLocalizer = stringLocalizer;
            _curriculoClient = curriculoClient;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _folhaPontoRepository = folhaPontoRepository;
            _loteRepository = loteRepository;
            _uploadFilesClient = uploadFilesClient;
            _queueProducer = queueProducer;
            _unitOfWork = unitOfWork;
            _atividadeProjetoRepository = atividadeProjetoRepository;
        }

        public async Task<bool> ProcessarFolhaPontoAsync(string loteId, string codigoColaborador, int orgId)
        {
            var lotes = await _folhaPontoRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.FOLHAPONTO);
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
            if (!string.IsNullOrEmpty(lote.SumarioFolhaPontoString))
            {
                lote.SumarioFolhaPonto = JsonConvert.DeserializeObject<SumarioFolhaPontoDTO>(lote.SumarioFolhaPontoString);
                cnpj = lote.SumarioFolhaPonto.Cnpj;
            }
            // Dividir PDF em páginas
            var paginas = PdfManipulationUtil.DividirPdfEmPaginas(pdfBytes);
            var queueUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("AWS_SQS_QUEUE_FOLHAPONTO");
            
            // Primeira transação: Inserir todos os registros na tabela
            _unitOfWork.BeginTransaction();
            await _loteRepository.AtualizarLoteAsync(loteId, true, null);
            try
            {
                var itensLote = new List<(string itemLoteId, string nomeArquivoPagina, FilaMessageDTO mensagem, Dictionary<string, string> messageAttributes)>();
                
                foreach (var pagina in paginas)
                {
                    // Salvar página no S3
                    var dataHora = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                    var nomeArquivoPagina = $"folhaponto/paginas/{loteId}_{paginas.IndexOf(pagina) + 1}_{dataHora}.pdf";
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
                        PdfPath = nomeArquivoPagina
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

        public async Task<bool> ProcessarItemFolhaPontoAsync(FilaMessageDTO mensagem, string codigoColaboradorRequest, int orgId, string itemLoteId, string loteId)
        {
            _unitOfWork.BeginTransaction();
            try
            {  
                try
                {
                    // Buscar o PDF do S3
                    using var httpClient = new HttpClient();
                    var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("TOKEN_SISTEMA_COLABORACAO")));
                    var pdfUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL").Replace("$1", token) + mensagem.PdfPath;
                    var pdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);

                    var relatorioPonto = await _curriculoClient.AnaliseColaboradorFolhaPonto("application/pdf", Convert.ToBase64String(pdfBytes), VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
                    if(relatorioPonto.RelatorioPonto?.Funcionario == null)
                    {
                        throw new ValidationException("Não foi possível analisar a folha de ponto");
                    }
                    if(relatorioPonto.RelatorioPonto.Funcionario.Cpf == null)
                    {
                        throw new ValidationException("Não foi possível analisar a folha de ponto, pois o CPF do colaborador não foi encontrado");
                    }
                    if(relatorioPonto.RelatorioPonto.Empresa?.CnpjCpf == null)
                    {
                        throw new ValidationException("Não foi possível analisar a folha de ponto, pois o CNPJ da empresa não foi encontrado");
                    }
                    relatorioPonto.RelatorioPonto.Empresa.CnpjCpf = new string(relatorioPonto.RelatorioPonto.Empresa.CnpjCpf.Where(char.IsDigit).ToArray());
                    relatorioPonto.RelatorioPonto.Funcionario.Cpf = new string(relatorioPonto.RelatorioPonto.Funcionario.Cpf.Where(char.IsDigit).ToArray());
                    var unidades = await _folhaPontoRepository.ListarUnidadesPorOrg(orgId);
                    string codigoInterno = _usuarioColaboradorRepository.GetCodigoInternoByDocumentoEOrg(relatorioPonto.RelatorioPonto.Funcionario.Cpf, orgId);
                    if (codigoInterno == null)
                    {
                        if(unidades.Any(u => u.Key == relatorioPonto.RelatorioPonto.Empresa.CnpjCpf) == false)
                        {
                            throw new ValidationException("Não foi possível analisar a folha de ponto do colaborador " + relatorioPonto.RelatorioPonto.Funcionario.Cpf + ", pois a empresa não foi encontrada. Empresa no arquivo: " + relatorioPonto.RelatorioPonto.Empresa.CnpjCpf + " - Empresas encontradas: " + unidades.Select(u => u.Key).Aggregate((a, b) => a + ", " + b));
                        }
                        codigoInterno = Guid.NewGuid().ToString();
                        var colaboradorDto = new DataTransferObject.Domain.Colaborador.ColaboradorDTO
                        {
                            Cpf = codigoInterno,
                            NomeCompleto = relatorioPonto.RelatorioPonto.Funcionario.Nome, 
                            DocumentoColaborador = relatorioPonto.RelatorioPonto.Funcionario.Cpf,
                            Matricula = relatorioPonto.RelatorioPonto.Funcionario.Matricula,
                            FlagAtivo = true
                        };
                        var colaboradorOrgDto = new DataTransferObject.Domain.Org.ColaboradorOrgDTO
                        {
                            Cpf = codigoInterno,
                            OrgId = orgId,
                            CodColaborador = relatorioPonto.RelatorioPonto.Funcionario.Matricula,
                            Ativo = true,
                            DataAdmissao = String.IsNullOrEmpty(relatorioPonto.RelatorioPonto.Funcionario.DataAdmissao) ? null : DateTime.ParseExact(relatorioPonto.RelatorioPonto.Funcionario.DataAdmissao, "dd/MM/yyyy", null),
                            Cargo = relatorioPonto.RelatorioPonto.Funcionario.Cargo,
                            CodCargo = relatorioPonto.RelatorioPonto.Funcionario.Cargo,
                            Diretoria = unidades.FirstOrDefault(u => u.Key == relatorioPonto.RelatorioPonto.Empresa.CnpjCpf).Value,
                            CodDiretoria = relatorioPonto.RelatorioPonto.Empresa.CnpjCpf,
                            Departamento = "",
                            CodDepartamento = "",
                            ModeloContratacao = "CLT",
                            CodigoModeloContratacao = "CLT"
                        };
                        _usuarioColaboradorRepository.InsertColaboradorSemUsuario(colaboradorDto);
                        _usuarioColaboradorRepository.UpsertColaboradorOrg(colaboradorDto, colaboradorOrgDto);
                    }

                    // 1. Obter competência (mês/ano) do relatório ponto
                    var competencia = relatorioPonto.RelatorioPonto.Resumo;
                    int mes = 0;
                    int ano = 0;
                    if (relatorioPonto.RelatorioPonto.RegistrosDiarios != null && relatorioPonto.RelatorioPonto.RegistrosDiarios.Count > 0)
                    {
                        var dataPrimeiroRegistro = relatorioPonto.RelatorioPonto.RegistrosDiarios[0].Data;
                        // data vem no formato "01/04/2025 ter"
                        var dataSplit = dataPrimeiroRegistro.Split(' ')[0];
                        var data = DateTime.ParseExact(dataSplit, "dd/MM/yyyy", null);
                        mes = data.Month;
                        ano = data.Year;
                    }
                    else
                    {
                        throw new ValidationException("Não foi possível identificar a competência do relatório ponto.");
                    }

                    // 2. Buscar e deletar todos os apontamentos do colaborador na competência
                    var projetosAtividades = await _apontamentoService.ListarProjetosComAtividadesPorColaborador(codigoColaboradorRequest, orgId, codigoInterno);
                    var apontamentosExistentes = await _apontamentoService.ListarApontamentosPorVigencia(mes, ano, codigoColaboradorRequest, "", orgId, codigoInterno);
                    if (apontamentosExistentes?.ColaboradorApontamentos != null)
                    {
                        foreach (var ap in apontamentosExistentes.ColaboradorApontamentos)
                        {
                            await _apontamentoService.DeletarApontamentoColaborador(ap.TbColaboradorApontamentoId, codigoColaboradorRequest, orgId, DateTime.Now, true);
                        }
                    }

                    // 3. Inserir apontamentos do relatório ponto
                    foreach (var registro in relatorioPonto.RelatorioPonto.RegistrosDiarios)
                    {
                        // Mapear ProjetoId e AtividadeId com validação específica
                        var projeto = projetosAtividades?.FirstOrDefault(p => p.Cod_projeto == relatorioPonto.RelatorioPonto.Empresa.CnpjCpf);
                        if (projeto == null)
                            throw new ValidationException($"Não foi possível encontrar o projeto para a empresa {relatorioPonto.RelatorioPonto.Empresa.CnpjCpf}");

                        string descricaoAtividade = "";
                        if (registro.HorarioContratual != null && registro.HorarioContratual.Contains('-') && registro.HorarioContratual.Contains('|'))
                        {
                            // Formato '00:00 - 00:00 | 00:00 - 00:00'
                            descricaoAtividade = "OPERAÇÕES";
                        }
                        else if (registro.HorarioContratual != null && registro.HorarioContratual != "Descanso Semanal")
                        {
                            // Usar o próprio horarioContratual como descrição da atividade
                            descricaoAtividade = registro.HorarioContratual;
                        }
                        else
                        {
                            // Para "Descanso Semanal" ou outros casos, pular este registro. Registro Vazio não deve ser inserido
                            continue;
                        }

                        // Buscar atividade existente ou criar nova
                        var atividade = projeto.Atividades?.FirstOrDefault(a => a.Descricao.Equals(descricaoAtividade, StringComparison.OrdinalIgnoreCase));
                        if (atividade == null)
                        {
                            // Criar nova atividade
                            var novasAtividades = await _atividadeProjetoRepository.CadastroAtividades(orgId, new List<string> { descricaoAtividade }, projeto.Cod_projeto);
                            var novaAtividadeProjeto = novasAtividades.FirstOrDefault(a => a.Descricao.Equals(descricaoAtividade, StringComparison.OrdinalIgnoreCase));
                            if (novaAtividadeProjeto != null)
                            {
                                atividade = new DataTransferObject.Domain.Apontamento.AtividadeDTO
                                {
                                    Id = novaAtividadeProjeto.Id.ToString(),
                                    Descricao = novaAtividadeProjeto.Descricao
                                };
                                projeto.Atividades.Add(atividade);
                            }
                        }

                        if (atividade == null)
                            throw new ValidationException($"Não foi possível encontrar/criar a atividade {descricaoAtividade} para o projeto {projeto.Cod_projeto}");

                        // Converter hTrab (ex: "08:41") para minutos
                        long horas = 0;
                        if (!string.IsNullOrEmpty(registro.Rendimento?.HTrab))
                        {
                            var partes = registro.Rendimento.HTrab.Split(':');
                            
                            if (partes.Length == 2 && long.TryParse(partes[0], out var h) && long.TryParse(partes[1], out var m))
                            {
                                    horas = h * 60 + m;
                            }
                        }
                        
                        var dataApontamento = DateTime.ParseExact(registro.Data.Split(' ')[0], "dd/MM/yyyy", null);
                        await _apontamentoService.ApontarHorasCarga(
                            codigoColaboradorRequest,
                            orgId,
                            projeto.Cod_projeto,
                            atividade.Id,
                            horas,
                            dataApontamento.ToString("yyyy-MM-dd HH:mm:ss"),
                            0, // diaQuebraSemana
                            false, // deveSomarApontamentoDia
                            codigoInterno,
                            registro.Justificativa
                        );
                    }
                    var competenciaString = $"{mes:D2}/{ano:D4}";
                    await _folhaPontoRepository.DeletarFolhasPontoColaboradorAsync(codigoInterno, competenciaString, orgId);
                    await _folhaPontoRepository.InserirFolhaPontoColaboradorAsync(new FolhaPontoColaboradorDTO{
                        TbItemLoteId = itemLoteId,
                        CodigoInternoColaborador = codigoInterno,
                        ObjetoFolhaPontoString = JsonConvert.SerializeObject(relatorioPonto),
                        Competencia = competenciaString,
                        Cnpj = relatorioPonto.RelatorioPonto.Empresa.CnpjCpf,
                    });
                    await _loteRepository.AtualizarItemLoteAsync(itemLoteId, true, JsonConvert.SerializeObject(new RetornoProcessamentoMensagemFolhaPontoDTO{
                        Erros = new List<string> { },
                        StackTrace = "",
                        ProcessadoComSucesso = true,
                        RelatorioFolhaPonto = relatorioPonto
                    }), DateTime.Now);
                    
                    await FinalizarLoteAsync(loteId);
                    
                    _unitOfWork.Commit();
                    return true;
                }
                catch (ValidationException ex)
                {
                    await _loteRepository.AtualizarItemLoteAsync(itemLoteId, false, JsonConvert.SerializeObject(new RetornoProcessamentoMensagemFolhaPontoDTO{
                        Erros = new List<string> { ex.Message },
                        StackTrace = ex.StackTrace,
                        ProcessadoComSucesso = false,
                        RelatorioFolhaPonto = null
                    }), DateTime.Now);
                    await FinalizarLoteAsync(loteId);
                    _unitOfWork.Commit();
                    return false;
                }
                catch (Exception ex)
                {
                    await _loteRepository.AtualizarItemLoteAsync(itemLoteId, false, JsonConvert.SerializeObject(new RetornoProcessamentoMensagemFolhaPontoDTO{
                        Erros = new List<string> { "Erro desconhecido no processamento da folha de ponto do colaborador " },
                        StackTrace = ex.Message + " - " + ex.StackTrace,
                        ProcessadoComSucesso = false,
                        RelatorioFolhaPonto = null
                    }), DateTime.Now);
                    await FinalizarLoteAsync(loteId);
                    _unitOfWork.Commit();
                    return false;
                }
                
            }
            catch (Exception ex)
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

        public async Task<SumarioFolhaPontoResult> ObterSumarioFolhaPontoAsync(byte[] arquivoPdf, string codigoColaborador, int orgId)
        {
            try
            {
                byte[] primeiraPaginaBytes = PdfManipulationUtil.ExtrairPrimeiraPagina(arquivoPdf);
                
                string base64Pdf = Convert.ToBase64String(primeiraPaginaBytes);
                string tokenAcesso = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN");
                var sumario = await _curriculoClient.SumarioFolhaPonto("application/pdf", base64Pdf, tokenAcesso);

                var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(codigoColaborador, orgId);
                if (usuario == null)
                {
                    throw new InvalidOperationException($"Usuário com CPF {codigoColaborador} não encontrado na organização {orgId}");
                }

                var dataHora = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                var nomeArquivo = $"folhaponto/{codigoColaborador}_{dataHora}.pdf";
                
                await _uploadFilesClient.UploadFile(nomeArquivo, arquivoPdf);
                

                sumario.QuantidadeItens = PdfManipulationUtil.ObterQuantidadePaginas(arquivoPdf);
                var loteId = await _loteRepository.CriarLoteAsync(
                    sumario.QuantidadeItens, 
                    nomeArquivo, 
                    orgId, 
                    usuario.UsuarioId, 
                    TipoFilaEnum.FOLHAPONTO,
                    JsonConvert.SerializeObject(sumario)
                );

                var resultado = new SumarioFolhaPontoResult();
                resultado.Id = loteId;
                resultado.Cnpj = sumario.Cnpj;
                resultado.Competencia = sumario.Competencia;
                resultado.Empresa = sumario.Empresa;
                resultado.QuantidadeItens = sumario.QuantidadeItens;
                resultado.QuantidadeItensProcessados = 0;
                resultado.QuantidadeItensProcessadosComErro = 0;
                
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar sumário da folha ponto: {ex.Message}", ex);
            }
        }



        public async Task<List<LoteFilaResult>> BuscarLotesPorOrgAsync(int orgId)
        {
            try
            {
                var result = new List<LoteFilaResult>();
                var lotes = await _folhaPontoRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.FOLHAPONTO);
                
                // Deserializar o sumário JSON para cada lote
                foreach (var lote in lotes)
                {
                    if (!string.IsNullOrEmpty(lote.SumarioFolhaPontoString))
                    {
                        lote.SumarioFolhaPonto = JsonConvert.DeserializeObject<SumarioFolhaPontoDTO>(lote.SumarioFolhaPontoString);
                        lote.SumarioFolhaPonto.QuantidadeItensProcessados =  await _loteRepository.GetTotalItensProcessadosLoteAsync(lote.Id);
                        lote.SumarioFolhaPonto.QuantidadeItensProcessadosComErro = await _loteRepository.GetTotalItensProcessadosComErroLoteAsync(lote.Id);
                    }
                    result.Add(new LoteFilaResult()
                    {
                        Id = lote.Id,
                        Status = ObterStatusLote(lote),
                        SumarioFolhaPonto = lote.SumarioFolhaPonto,
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

        public string ObterStatusLote(LoteFilaFolhaDTO lote)
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

        public async Task<DetalhesLoteFolhaPontoResult> DetalharLoteAsync(string loteId)
        {
            try
            {
                var loteRaw = await _folhaPontoRepository.DetalharLoteAsync(loteId);
                if(loteRaw == null)
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não encontrado");
                }
                var sumario = JsonConvert.DeserializeObject<SumarioFolhaPontoDTO>(loteRaw.SumarioFolhaPontoString);
                var lote = new SumarioFolhaPontoResult
                {
                    Id = loteRaw.Id,
                    Cnpj = sumario.Cnpj,
                    Competencia = sumario.Competencia,
                    Empresa = sumario.Empresa,
                    QuantidadeItens = loteRaw.QuantidadeDePaginas,
                    QuantidadeItensProcessados = await _loteRepository.GetTotalItensProcessadosLoteAsync(loteId),
                    QuantidadeItensProcessadosComErro = await _loteRepository.GetTotalItensProcessadosComErroLoteAsync(loteId)
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
                        var respostaProcessamento = JsonConvert.DeserializeObject<RetornoProcessamentoMensagemFolhaPontoDTO>(item.Retorno);
                        if(item.Sucesso)
                        {
                            itemLote.Sucesso = true;
                            itemLote.Cpf = respostaProcessamento.RelatorioFolhaPonto.RelatorioPonto.Funcionario.Cpf;
                            itemLote.Nome = respostaProcessamento.RelatorioFolhaPonto.RelatorioPonto.Funcionario.Nome;
                            itemLote.Cargo = respostaProcessamento.RelatorioFolhaPonto.RelatorioPonto.Funcionario.Cargo;
                            itemLote.Matricula = respostaProcessamento.RelatorioFolhaPonto.RelatorioPonto.Funcionario.Matricula;
                        }
                        else
                        {
                            itemLote.Sucesso = false;
                            itemLote.Erro = respostaProcessamento.Erros[0];
                        }
                        
                    }
                    
                    itens.Add(itemLote);
                }
                return new DetalhesLoteFolhaPontoResult
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

        public async Task<ReprocessarItensFolhaPontoResult> ReprocessarItensFolhaPontoAsync(List<string> itensLoteId, string codigoColaborador, int orgId)
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

            var queueUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("AWS_SQS_QUEUE_FOLHAPONTO");
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

                var lote = await _folhaPontoRepository.DetalharLoteAsync(item.LoteId);
                if (lote == null)
                {
                    naoEncontrados.Add(itemLoteId);
                    continue;
                }

                string cnpj = "";
                if (!string.IsNullOrEmpty(lote.SumarioFolhaPontoString))
                {
                    lote.SumarioFolhaPonto = JsonConvert.DeserializeObject<SumarioFolhaPontoDTO>(lote.SumarioFolhaPontoString);
                    if (lote.SumarioFolhaPonto != null)
                    {
                        cnpj = lote.SumarioFolhaPonto.Cnpj;
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
                    PdfPath = item.FilePath
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

            return new ReprocessarItensFolhaPontoResult
            {
                TotalSolicitados = itensLoteId.Count,
                TotalEnfileirados = totalEnfileirados,
                ItensNaoEncontrados = naoEncontrados
            };
        }

        public async Task DeletarLoteAsync(string loteId)
        {
            try
            {
                var lote = await _folhaPontoRepository.DetalharLoteAsync(loteId);
                if(lote == null)
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
                await _loteRepository.DeletarLoteAsync(loteId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar lote: {ex.Message}", ex);
            }
        }
    }
} 