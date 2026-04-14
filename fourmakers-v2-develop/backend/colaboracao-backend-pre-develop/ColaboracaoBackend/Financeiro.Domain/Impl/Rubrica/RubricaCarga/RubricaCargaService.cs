using System.Globalization;
using System.Net;
using System.Text;
using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Aws.Infra.Interfaces;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Extension;
using Colaboracao.Helper.Util;
using Core.Domain.Apontamento;
using Core.Domain.Financeiro.Rubrica;
using Core.Domain.Usuario;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using Financeiro.Domain.Interfaces.NotaFiscal;
using Financeiro.Domain.Interfaces.Rubrica.RubricaCarga;
using Financeiro.Domain.Interfaces.Rubrica.RubricaColaborador;
using Newtonsoft.Json;
using SRS.Infra.Constantes;

using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Rubrica.RubricaCarga;

[LogDomainClass]
public class RubricaCargaService : IRubricaCargaService
{
    private readonly static string DESCRICAO_ENTIDADE = "Carga Rubrica";
    private readonly IUploadFilesClient _uploadFilesClient;
    private readonly ITokens _token;
    private readonly ICurriculoClient _curriculoClient;
    private readonly IRubricaCargaRepository _rubricaCargaRepository;
    private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly IQueueProducer _queueProducer;
    private readonly IDBConnectionUnitOfWork _unitOfWork;
    private readonly IRubricaColaboradorService _rubricaColaboradorService;
    private readonly INotaFiscalService _notaFiscalService;
    private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;
    private readonly string _curriculoApiToken = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN");

    public RubricaCargaService(IUploadFilesClient uploadFilesClient, ITokens token, ICurriculoClient curriculoClient, IRubricaCargaRepository rubricaCargaRepository, IUsuarioColaboradorRepository usuarioColaboradorRepository, ILoteRepository loteRepository, IQueueProducer queueProducer, IDBConnectionUnitOfWork unitOfWork, IRubricaColaboradorService rubricaColaboradorService, INotaFiscalService notaFiscalService, IRestricaoDeAcessoService restricaoDeAcessoService)
    {
        _uploadFilesClient = uploadFilesClient;
        _token = token;
        _curriculoClient = curriculoClient;
        _rubricaCargaRepository = rubricaCargaRepository;
        _usuarioColaboradorRepository = usuarioColaboradorRepository;
        _loteRepository = loteRepository;
        _queueProducer = queueProducer;
        _unitOfWork = unitOfWork;
        _rubricaColaboradorService = rubricaColaboradorService;
        _notaFiscalService = notaFiscalService;
        _restricaoDeAcessoService = restricaoDeAcessoService;
    }

    private static readonly List<string> ListaDeCodigosXlsx =
    [
        "Plano Odonto",
        "Plano UNIMED MENSALIDADE",
        "Plano UNIMED MENSALIDADE TITULAR"
    ];
    public async Task<ApiGenericResult<RubricaCargaStatusDTO>> CriarSumarioRubricaCarga(RubricaCargaInput input, string codigoInternoColaborador, int orgId)
    {
        var apiGenericResult = new ApiGenericResult<RubricaCargaStatusDTO>();
        try
        {
                var byteFile = input.Base64File.Base64ToByteArray();
                var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(codigoInternoColaborador, orgId);
                if (usuario == null)
                {
                    throw new InvalidOperationException($"Usuário com CPF {codigoInternoColaborador} não encontrado na organização {orgId}");
                }

                var dataHora = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                var nomeArquivo = $"rubrica-carga/{codigoInternoColaborador}_{dataHora}.{input.Base64File.Tipo.ToString()}";
                
                await _uploadFilesClient.UploadFile(nomeArquivo, byteFile);
                

                var quantidadePaginas = PdfManipulationUtil.ObterQuantidadePaginas(byteFile);
                var loteId = await _loteRepository.CriarLoteAsync(
                    quantidadePaginas, 
                    nomeArquivo, 
                    orgId, 
                    usuario.UsuarioId, 
                    TipoFilaEnum.RUBRICA_CARGA,
                    JsonConvert.SerializeObject(new SumarioRubricaCargaDTO()
                    {
                        RubricaId = input.RubricaId,
                        MesInicial = input.MesInicial,
                        AnoFinal = input.AnoFinal,
                        CodDiretoria = input.CodDiretoria,
                        
                    })
                );
                
                await ProcessarRubricaCargaAsync(loteId, codigoInternoColaborador, orgId, input.Base64File.Tipo);
                return apiGenericResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar sumário da carga: {ex.Message}", ex);
            }
    }

    private async Task<bool> ProcessarRubricaCargaAsync(string loteId, string codigoColaborador, int orgId, ArquivoTipoEnum tipo)
    {
        var lotes = await _rubricaCargaRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.RUBRICA_CARGA);
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
            var sumario = new SumarioRubricaCargaDTO();
            if (!string.IsNullOrEmpty(lote.SumarioRubricaCargaString))
            {
                lote.SumarioRubricaCarga = JsonConvert.DeserializeObject<SumarioRubricaCargaDTO>(lote.SumarioRubricaCargaString);
                sumario =  lote.SumarioRubricaCarga;
            }
            
            var rubricaTemplate =
                await _rubricaCargaRepository.ObterTemplateRubricaPorRubricaId(sumario.RubricaId);

            if (rubricaTemplate == null)
            {
                throw new ValidationException("Não foi possível encontrar um template para essa rubrica.");
            }
            
            var paginas = new List<byte[]>();

            if (ListaDeCodigosXlsx.Contains(rubricaTemplate.NomeTemplate))
            {
                if (tipo != ArquivoTipoEnum.xlsx)
                {
                    throw new ValidationException("Formato do documento deve ser em xlsx.");
                }
                paginas.Add(pdfBytes);
            }
            else
            {
                if (tipo != ArquivoTipoEnum.pdf)
                {
                    throw new ValidationException("Formato do documento deve ser em pdf.");
                }
                paginas = PdfManipulationUtil.DividirPdfEmPaginas(pdfBytes);
            }
            // Dividir PDF em páginas
            var queueUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("AWS_SQS_QUEUE_RUBRICA");
            
            // Primeira transação: Inserir todos os registros na tabela
            _unitOfWork.BeginTransaction();
            
           
            var codigoCarga = await _rubricaCargaRepository.CriarRubricaCargaLoteInicial(lote.Pdf, sumario.CodDiretoria, sumario.RubricaId, rubricaTemplate.Id, codigoColaborador, orgId, sumario.MesInicial, sumario.AnoFinal);
            if (codigoCarga == null)
            {
                throw new ValidationException("Não foi possivel gerar uma carga inicial.");
            }
            await _loteRepository.AtualizarLoteAsync(loteId, true, null);
            
            
            try
            {
                var itensLote = new List<(string itemLoteId, string nomeArquivoPagina, FilaMessageRubricaCargaDTO mensagem, Dictionary<string, string> messageAttributes)>();
                
                foreach (var pagina in paginas)
                {
                    // Salvar página no S3
                    var dataHora = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                    var indicePagina = paginas.IndexOf(pagina) + 1;
                    var nomeArquivoPagina = $"rubrica-carga/paginas/{loteId}_{paginas.IndexOf(pagina) + 1}_{dataHora}.{tipo.ToString()}";
                    await _uploadFilesClient.UploadFile(nomeArquivoPagina, pagina);

                    // Inserir item de lote
                    var itemLoteId = await _loteRepository.InserirItemLoteAsync(loteId, nomeArquivoPagina);

                    var mensagem = new FilaMessageRubricaCargaDTO()
                    {
                        RubricaId = sumario.RubricaId,
                        AnoFinal = sumario.AnoFinal,
                        MesInicial = sumario.MesInicial,
                        CodDiretoria = sumario.CodDiretoria,
                        OrgId = orgId,
                        LoteId = loteId,
                        ItemLoteId = itemLoteId,
                        UsuarioId = usuario.UsuarioId,
                        CodigoColaboradorSolicitante = codigoColaborador,
                        PdfPath = nomeArquivoPagina,
                        IsLastPage = indicePagina == paginas.Count,
                        IsFirstPage = indicePagina == 1,
                        RubricaTemplate = rubricaTemplate,
                        RubricaCargaLogId = codigoCarga
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
                
                foreach (var (_, _, mensagem, messageAttributes) in itensLote)
                {
                    var sucesso = await _queueProducer.SendMessageAsync(queueUrl, JsonConvert.SerializeObject(mensagem), messageAttributes);
                    if (!sucesso.HttpStatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception($"Erro ao enviar página {messageAttributes["pagina"]} para a fila SQS");
                    }
                }

                // foreach (var item in itensLote)
                // {
                //     await ProcessarItemRubricaCargaAsync(item.mensagem, codigoColaborador, orgId, item.itemLoteId, loteId);
                // }
                
                _unitOfWork.Commit();
            }
            catch (System.Exception)
            {
                _unitOfWork.SafeRollback();
                throw;
            }

            return true;
    }

    public async Task<bool> ProcessarItemRubricaCargaAsync(FilaMessageRubricaCargaDTO mensagem, string codigoColaboradorRequest, int orgId, string itemLoteId, string loteId)
    {
       // _unitOfWork.BeginTransaction();
        try
        {
            try
            {
                using var httpClient = new HttpClient();
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("TOKEN_SISTEMA_COLABORACAO")));
                var pdfUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SERVICE_MEDIA_BASE_URL").Replace("$1", token) + mensagem.PdfPath;
                var pdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);
                
                var curriculoToken = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN");
                var base46File = Convert.ToBase64String(pdfBytes);

                List<AnaliseGenericoResult> valoresAnalise;

                if (mensagem.IsFirstPage)
                {
                    await _rubricaCargaRepository.AtualizarRubricaCargaLogAsync(mensagem.RubricaCargaLogId, "", "processamento", 0, 0);
                }

                try
                {
                    valoresAnalise = mensagem.RubricaTemplate.NomeTemplate switch
                    {
                        "Carga CLAMED - R-TRADE" => (await _curriculoClient.AnaliseGenerico(base46File, curriculoToken, mensagem.RubricaTemplate.MapeamentoCampos, mensagem.RubricaTemplate.InstrucoesAdicionais)).Dados.Funcionarios,
                        "Plano Saude Unimed" => await AnalisarEProcessarUnimed(base46File, curriculoToken),
                        "Plano Saude Amil" => await AnalisarEProcessarAmil(base46File, curriculoToken),
                        "Plano Saude Porto Seguro Odonto" => await AnalisarEProcessarPortoSeguroOdonto(base46File, curriculoToken),
                        "Plano Saude PROFARMA" => (await _curriculoClient.AnaliseProfarma(base46File, curriculoToken)).Dados.Data,
                        "Plano Odonto" => await AnalisarEProcessarPlanoOdonto(base46File, curriculoToken),
                        "Plano UNIMED MENSALIDADE" => await AnalisarEProcessarUnimedMensalidade(base46File, curriculoToken),
                        "Plano UNIMED MENSALIDADE TITULAR" => await AnalisarEProcessarUnimedMensalidadeTitular(base46File, curriculoToken),
                        _ => throw new ValidationException(
                            "Não foi possível analisar a rubrica, template nao existente")
                    };
                }
                catch(Exception ex)
                {
                    throw;
                }

                foreach (var analise in valoresAnalise)
                {
                    try
                    {
                        var colaborador =
                            await _rubricaCargaRepository.IdentificarColaboradorDetalhadoPorCampo(analise.Codigo,
                                mensagem.RubricaTemplate.CodigoAlternativaTipo, orgId);
                        if (colaborador == null)
                        {
                            await LogErroColaboradorNaoEncontradoAsync(mensagem.RubricaCargaLogId, analise,
                                analise.Codigo);
                            continue;
                        }

                        if (colaborador.CodigoDiretoria is null)
                        {
                            await LogErroColaboradorNaoExisteOrgAsync(mensagem.RubricaCargaLogId, analise,
                                colaborador.CodigoInternoColaborador, mensagem.RubricaTemplate.CodigoAlternativaTipo);
                            continue;
                        }

                        if (colaborador.CodigoDiretoria != mensagem.CodDiretoria)
                        {
                            await LogErroDiretoriaDiferenteAsync(mensagem.RubricaCargaLogId, analise,
                                colaborador.CodigoInternoColaborador, mensagem.RubricaTemplate.CodigoAlternativaTipo,
                                mensagem.CodDiretoria);
                            continue;
                        }

                        try
                        {
                            var rubricaColaboradorInput = new RubricaColaboradorInput()
                            {
                                CodigoCargaRubrica = mensagem.RubricaCargaLogId,
                                Valor = analise.Valor.ParseDecimalUniversal(),
                                AnoInicial = mensagem.AnoFinal,
                                MesInicial = mensagem.MesInicial,
                                CodigoInternoColaborador = Guid.Parse(colaborador.CodigoInternoColaborador),
                                CodigoRubricaFrequencia = "UNICA",
                                RubricaId = Guid.Parse(mensagem.RubricaId),
                                CodigoInternoColaboradorAlteracao = codigoColaboradorRequest,
                                AnoFinal = null,
                                MesFinal = null,
                                Ativo = true,
                                OrgId = orgId,
                                Observacao = ""
                            };
                            var result = await _rubricaColaboradorService.InserirRubricaColaborador(rubricaColaboradorInput, codigoColaboradorRequest, orgId, false);
                            
                            await _rubricaCargaRepository.SalvarLogItemAsync(
                                mensagem.RubricaCargaLogId,
                                analise,
                                "sucesso",
                                mensagem.RubricaTemplate.CodigoAlternativaTipo,
                                colaborador.CodigoInternoColaborador,
                                result.Retorno.Id.ToString(),
                                null
                            );
                        }
                        catch (Exception ex)
                        {
                            await _rubricaCargaRepository.SalvarLogItemAsync(
                                "",
                                analise,
                                "erro_salvamento",
                                mensagem.RubricaTemplate.CodigoAlternativaTipo,
                                colaborador.CodigoInternoColaborador,
                                null,
                                ex.Message
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        await LogErroGeralAsync(mensagem.RubricaCargaLogId, analise, ex.Message);
                    }
                }
                
                if (mensagem.IsLastPage)
                {
                    await FinalizarLoteAsync(loteId, mensagem.IsLastPage, mensagem.RubricaCargaLogId);
                }
               // _unitOfWork.Commit();
                
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
                await FinalizarLoteAsync(loteId, mensagem.IsLastPage, mensagem.RubricaCargaLogId);
                // _unitOfWork.Commit();
                return false;
            }
            catch (Exception ex)
            {
                await _loteRepository.AtualizarItemLoteAsync(itemLoteId, false, JsonConvert.SerializeObject(new RetornoProcessamentoMensagemFolhaPontoDTO{
                    Erros = new List<string> { "Erro desconhecido no processamento da rubrica do colaborador " },
                    StackTrace = ex.Message + " - " + ex.StackTrace,
                    ProcessadoComSucesso = false,
                    RelatorioFolhaPonto = null
                }), DateTime.Now);
                await FinalizarLoteAsync(loteId, mensagem.IsLastPage, mensagem.RubricaCargaLogId);
                // _unitOfWork.Commit();
                return false;
            }
            
        }
        catch (Exception ex)
        {
            // _unitOfWork.SafeRollback();
            throw;
        }
    }

    private async Task LogErroDiretoriaDiferenteAsync( string codigoCargaRubrica, object item, string codigoInternoColaborador, string tipoIdentificacao, string codDiretoria)
    {
        await _rubricaCargaRepository.SalvarLogItemAsync(
            codigoCargaRubrica,
            item,
            "erro_colaborador_diretoria_diferente",
            tipoIdentificacao,
            codigoInternoColaborador,
            null,
            $"Colaborador pertence a diretoria diferente de: {codDiretoria}"
        );
    }

    private async Task LogErroColaboradorNaoExisteOrgAsync( string codigoCargaRubrica, object item, string codigoInternoColaborador, string tipoIdentificacao)
    {
        await _rubricaCargaRepository.SalvarLogItemAsync(
            codigoCargaRubrica,
            item,
            "erro_colaborador_nao_existe_na_org",
            tipoIdentificacao,
            codigoInternoColaborador,
            null,
            "Colaborador não existe na organização informada"
        );
    }

    private async Task LogErroColaboradorNaoEncontradoAsync( string codigoCargaRubrica, object item, string codigo)
    {

        await _rubricaCargaRepository.SalvarLogItemAsync(
            codigoCargaRubrica,
            item,
            "erro_colaborador_nao_encontrado",
            "nao_encontrado",
            null,
            null,
            $"Colaborador não encontrado para código: {codigo}"
        );
    }

    private async Task LogErroGeralAsync( string codigoCargaRubrica, object item, string erroMsg)
    {
        await _rubricaCargaRepository.SalvarLogItemAsync(
            codigoCargaRubrica,
            item,
            "erro_processamento_geral",
            "erro_geral",
            null,
            null,
            $"Erro geral ao processar item: {erroMsg}"
        );
    }

    private async Task<List<AnaliseGenericoResult>> AnalisarEProcessarAmil(string base64pdf, string token)
    {
        List<AnaliseAmilResult> result = (await _curriculoClient.AnaliseAmil(base64pdf, token)).Dados.Data;

        var retorno = new List<AnaliseGenericoResult>();

        foreach (var item in result)
        {
            var titular = item.Titular?.Trim() ?? "";

            // Pula se não for string válida ou for numérica
            if (string.IsNullOrWhiteSpace(titular) || int.TryParse(titular, out _))
                continue;

            if (!double.TryParse(item.TotalFamilia?.Replace(",", "."), 
                                 System.Globalization.NumberStyles.Any, 
                                 System.Globalization.CultureInfo.InvariantCulture, 
                                 out double valorFloat))
            {
                valorFloat = 0;
            }

            if (valorFloat <= 0) continue;

            retorno.Add(new AnaliseGenericoResult
            {
                Codigo = titular,
                Valor = valorFloat.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                Descricao = "AMIL"
            });
        }

        return retorno;
    }

    private async Task<List<AnaliseGenericoResult>> AnalisarEProcessarUnimed(string base64pdf, string token)
    {
        List<AnaliseUnimedResult> result = (await _curriculoClient.AnaliseUnimed(base64pdf, token)).Dados.Data;

        var grupos = new Dictionary<string, (string Nome, string CpfTitular, List<AnaliseUnimedResult> Itens)>();

        foreach (var item in result)
        {
            var codigoBase = (item.Codigo ?? "").Length >= 14 ? item.Codigo.Substring(0, 14) : item.Codigo ?? "";
            if (!grupos.ContainsKey(codigoBase))
            {
                grupos[codigoBase] = (item.Nome, null, new List<AnaliseUnimedResult>());
            }

            // CPF titular pela rubrica especial
            if (item.Rubrica == "Mensalidade Titular Fx Etaria")
            {
                grupos[codigoBase] = (grupos[codigoBase].Nome, item.Cpf, grupos[codigoBase].Itens);
            }

            grupos[codigoBase].Itens.Add(item);
        }

        var retorno = new List<AnaliseGenericoResult>();

        foreach (var grupo in grupos.Values)
        {
            foreach (var item in grupo.Itens)
            {
                if (item.Rubrica == "Mensalidade Titular Fx Etaria")
                    continue;

                var cpf = string.IsNullOrWhiteSpace(grupo.CpfTitular) ? item.Cpf?.Trim() : grupo.CpfTitular?.Trim();
                if (string.IsNullOrWhiteSpace(cpf))
                    continue;

                if (!double.TryParse(item.Valor?.Replace(",", "."), 
                                     System.Globalization.NumberStyles.Any, 
                                     System.Globalization.CultureInfo.InvariantCulture, 
                                     out double valorNum))
                    continue;

                if (valorNum < 0)
                    continue;

                retorno.Add(new AnaliseGenericoResult
                {
                    Codigo = cpf,
                    Valor = valorNum.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                    Descricao = $"{item.Nome} - {item.Rubrica}"
                });
            }
        }

        return retorno;
    }

    private async Task<List<AnaliseGenericoResult>> AnalisarEProcessarPortoSeguroOdonto(string base64pdf, string token)
    {
        List<AnalisePortoSeguroOdontoResult> result = (await _curriculoClient.AnalisePortoSeguroOdonto(base64pdf, token)).Dados.Data;

        var grupos = new Dictionary<string, (string Nome, string CpfTitular, List<AnalisePortoSeguroOdontoResult> Itens)>();

        foreach (var item in result)
        {
            var codigoBase = (item.Codigo ?? "").Length >= 9 ? item.Codigo.Substring(0, 9) : item.Codigo ?? "";
            if (!grupos.ContainsKey(codigoBase))
            {
                grupos[codigoBase] = (item.Nome, null, new List<AnalisePortoSeguroOdontoResult>());
            }

            // CPF titular pela rubrica especial
            if (item.Rubrica == "Total Prêmio Titulares")
            {
                grupos[codigoBase] = (grupos[codigoBase].Nome, item.Cpf, grupos[codigoBase].Itens);
            }

            grupos[codigoBase].Itens.Add(item);
        }

        var retorno = new List<AnaliseGenericoResult>();

        foreach (var grupo in grupos.Values)
        {
            foreach (var item in grupo.Itens)
            {
                if (item.Rubrica == "Total Prêmio Titulares")
                    continue;

                var cpf = string.IsNullOrWhiteSpace(grupo.CpfTitular) ? item.Cpf?.Trim() : grupo.CpfTitular?.Trim();
                if (string.IsNullOrWhiteSpace(cpf))
                    continue;

                if (!double.TryParse(item.Valor?.Replace(",", "."), 
                                     System.Globalization.NumberStyles.Any, 
                                     System.Globalization.CultureInfo.InvariantCulture, 
                                     out double valorNum))
                    continue;

                if (valorNum < 0)
                    continue;

                retorno.Add(new AnaliseGenericoResult
                {
                    Codigo = cpf,
                    Valor = valorNum.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                    Descricao = $"{item.Nome} - {item.Rubrica}"
                });
            }
        }

        return retorno;
    }
    
    private async Task<List<AnaliseGenericoResult>> AnalisarEProcessarPlanoOdonto(string base64xlsx, string token)
    {
        var fields = new
        {
            identificacaoDoBeneficiario = "identificaçãoDoBeneficiário",
            carteirinhaDoTitular = "carteirinhaDoTitular",
            numeroDaCarteirinha = "númeroDaCarteirinha",
            nomeDoBeneficiario = "nomeDoBeneficiário"
        };

        List<string> requiredFields =
        [
            "identificacaoDoBeneficiario",
            "numeroDaCarteirinha",
            "nomeDoBeneficiario"
        ];
        List<AnalisePlanoOdonto> result = (await _curriculoClient.AnaliseXlsxGenerico<List<AnalisePlanoOdonto>>(base64xlsx, fields, requiredFields,  token));

        var grupos = new Dictionary<long, List<AnalisePlanoOdonto>>();

        var retorno = new List<AnaliseGenericoResult>();

        // Primeiro passo: Criar grupos dos titulares (identificacaoDoBeneficiario == "T")
        foreach (var item in result.Where(x => x.IdentificacaoDoBeneficiario == "T"))
        {
            if (item.NumeroDaCarteirinha.HasValue())
            {
                if (!grupos.ContainsKey(item.NumeroDaCarteirinha))
                {
                    grupos[item.NumeroDaCarteirinha] = new List<AnalisePlanoOdonto>();
                }
                grupos[item.NumeroDaCarteirinha].Add(item);
            }
        }

        // Segundo passo: Adicionar dependentes aos grupos dos titulares
        foreach (var item in result.Where(x => x.IdentificacaoDoBeneficiario != "T"))
        {
            if (item.CarteirinhaDoTitular.HasValue &&
                grupos.ContainsKey(item.CarteirinhaDoTitular.Value))
            {
                grupos[item.CarteirinhaDoTitular.Value].Add(item);
            }
        }

        // Terceiro passo: Gerar o retorno individual para cada beneficiário
        foreach (var grupo in grupos.Values)
        {
            foreach (var beneficiario in grupo)
            {
                retorno.Add(new AnaliseGenericoResult
                {
                    Codigo = beneficiario.NomeDoBeneficiario,
                    Valor = "18.90",
                    Descricao = beneficiario.IdentificacaoDoBeneficiario == "T" ? "Titular" :
                        "Dependente"
                });
            }
        }

        return retorno;
    }

    private async Task<List<AnaliseGenericoResult>> AnalisarEProcessarUnimedMensalidade(string base64xlsx, string token)
    {
        var fields = new
        {
            carteirinha = "carteirinha",
            nome = "nomeDoSegurado",
            parentesco = "parentesco",
            descricao = "descrição",
            valor = "mensalidadeSaúde"
        };

        List<string> requiredFields =
        [
            "carteirinha",
            "nome",
            "parentesco"
        ];
        List<AnaliseUnimedMensalidadeResult> result = (await _curriculoClient.AnaliseXlsxGenerico<List<AnaliseUnimedMensalidadeResult>>(base64xlsx, fields, requiredFields, token, 1));

        // Filtrar apenas registros com descrição "MENSALIDADE"
        var registrosMensalidade = result
            .Where(x => !string.IsNullOrWhiteSpace(x.Descricao) &&
                       x.Descricao.Trim().Equals("MENSALIDADE", StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Agrupar por carteirinha (primeiros 13 dígitos)
        var grupos = new Dictionary<string, (string NomeTitular, List<AnaliseUnimedMensalidadeResult> Itens)>();

        foreach (var item in registrosMensalidade)
        {
            // Converter carteirinha para string e pegar os primeiros 13 dígitos
            var carteirinhaStr = item.Carteirinha.ToString();
            var carteirinhaBase = carteirinhaStr.Length >= 13 ? carteirinhaStr.Substring(0, 13) : carteirinhaStr;

            if (!grupos.ContainsKey(carteirinhaBase))
            {
                grupos[carteirinhaBase] = (null, new List<AnaliseUnimedMensalidadeResult>())!;
            }

            // Se for titular, guardar o nome
            if (!string.IsNullOrWhiteSpace(item.Parentesco) &&
                item.Parentesco.Trim().Equals("TITULAR", StringComparison.OrdinalIgnoreCase))
            {
                grupos[carteirinhaBase] = (item.Nome, grupos[carteirinhaBase].Itens);
            }

            grupos[carteirinhaBase].Itens.Add(item);
        }

        var retorno = new List<AnaliseGenericoResult>();

        // Processar cada grupo
        foreach (var grupo in grupos.Values)
        {
            // Se não houver titular identificado, pular o grupo
            if (string.IsNullOrWhiteSpace(grupo.NomeTitular))
                continue;

            // Adicionar apenas dependentes (não titulares)
            foreach (var item in grupo.Itens)
            {
                // Pular titulares
                if (!string.IsNullOrWhiteSpace(item.Parentesco) &&
                    item.Parentesco.Trim().Equals("TITULAR", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Validar valor
                if (item.Valor <= 0)
                    continue;

                retorno.Add(new AnaliseGenericoResult
                {
                    Codigo = grupo.NomeTitular,  // Nome do titular
                    Descricao = item.Descricao,  // Descrição do dependente
                    Valor = item.Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                });
            }
        }

        return retorno;
    }
    
    private async Task<List<AnaliseGenericoResult>> AnalisarEProcessarUnimedMensalidadeTitular(string base64xlsx, string token)
    {
        var fields = new
        {
            cpfTitular = "cpfTitular",
            nomeTitular = "nomeTitular",
            nomeDependente = "nomeDependente",
            valor = "vlrParticipacao",
        };

        List<string> requiredFields =
        [
            "cpfTitular",
            "nomeTitular",
            "nomeDependente",
            "valor"
        ];
        List<AnaliseUnimedMensalidadeTitularResult> result = (await _curriculoClient.AnaliseXlsxGenerico<List<AnaliseUnimedMensalidadeTitularResult>>(base64xlsx, fields, requiredFields,  token));

        // Agrupar por CPF do titular
        var grupos = new Dictionary<long, (string NomeTitular, List<AnaliseUnimedMensalidadeTitularResult> Itens)>();

        foreach (var item in result)
        {
            // Validar CPF titular
            if (item.CpfTitular <= 0)
                continue;

            if (!grupos.ContainsKey(item.CpfTitular))
            {
                grupos[item.CpfTitular] = (item.NomeTitular, new List<AnaliseUnimedMensalidadeTitularResult>());
            }

            grupos[item.CpfTitular].Itens.Add(item);
        }

        var retorno = new List<AnaliseGenericoResult>();

        // Processar cada grupo (por CPF do titular)
        foreach (var grupo in grupos.Values)
        {
            // Validar se tem nome do titular
            if (string.IsNullOrWhiteSpace(grupo.NomeTitular))
                continue;

            // Adicionar todos os dependentes do grupo
            foreach (var item in grupo.Itens)
            {
                // Validar valor
                if (item.Valor <= 0)
                    continue;

                // Validar nome do dependente
                if (string.IsNullOrWhiteSpace(item.NomeDependente))
                    continue;

                retorno.Add(new AnaliseGenericoResult
                {
                    Codigo = grupo.NomeTitular,      // Nome do titular
                    Descricao = item.NomeDependente, // Nome do dependente
                    Valor = item.Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                });
            }
        }

        return retorno;
    }
    

    
    private async Task FinalizarLoteAsync(string loteId, bool isLastPage, string cargaId)
    {
        var totalItensLote = await _loteRepository.GetTotalItensLoteAsync(loteId);
        var totalItensProcessados = await _loteRepository.GetTotalItensProcessadosLoteAsync(loteId);
        if(totalItensProcessados == totalItensLote)
        {
            await _loteRepository.AtualizarLoteAsync(loteId, true, DateTime.Now);
        }

        if (isLastPage)
        {
            var status = "sucesso";
            string mensagemError = null;

            var listaDeAnalises = new List<AnaliseGenericoResult>();
            var itens = await _rubricaCargaRepository.ObterItensLogPorCodigoCarga(cargaId);
            foreach (var item in itens)
            {
                var json = JsonConvert.DeserializeObject<AnaliseGenericoResult>(item.JsonItemTentativa);
                if (json != null) listaDeAnalises.Add(json);
            }

            if (itens.All(x => x.StatusProcessamento != "sucesso"))
            {
                status = "erro";
                mensagemError = "Finalizado com erros";
            }
            
            var quantidadeRegistrosSucesso = itens.Count(x => x.StatusProcessamento == "sucesso");

            var listaDeAnalizesUnificadasJson = JsonConvert.SerializeObject(listaDeAnalises);
                
            await _rubricaCargaRepository.AtualizarRubricaCargaLogAsync(cargaId, listaDeAnalizesUnificadasJson, status, itens.Count, quantidadeRegistrosSucesso, mensagemError);

        }
    }

    public async Task<ApiGenericResult<RubricaCargaStatusDTO>> InserirCarga(RubricaCargaInput input, string cpfRequest, string token, int orgId)
    {
        var apiGenericResult = new ApiGenericResult<RubricaCargaStatusDTO>();
        try
        {
            var (uploadFile, relativePath) = await UploadAwsRubrica(input.Base64File, input.RubricaId, token);

            var result = await _curriculoClient.InserirRubricaCarga(uploadFile,relativePath, input.RubricaId,"UNICA", input.MesInicial, input.AnoFinal, cpfRequest, input.CodDiretoria, _curriculoApiToken, orgId);
            apiGenericResult.Retorno = (await ObterCargaPorCodigoCarga(result.CodigoCargaRubrica)).Retorno;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
        }
        return apiGenericResult;
    }
    
    public async Task<ApiGenericResult<RubricaCargaConsultaResult>> ObterStatusCargaPorId(string cargaRubricaId)
    {
        var apiGenericResult = new ApiGenericResult<RubricaCargaConsultaResult>();
        try
        {
            var result = await _curriculoClient.BuscarStatusCargaPorId(cargaRubricaId, _curriculoApiToken);
                
            apiGenericResult.Retorno = result;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
        }
        return apiGenericResult;
    }
    
    private async Task<(string, string)> UploadAwsRubrica(Base64DTO input, string rubricaId, string token)
    {
        var file = input.Base64ToByteArray();
        if (file.Length == 0)
        {
            throw new ArgumentException("Erro ao tentar converter documento");
        }
                
        var type = input.GetTypeName() == "pdf" ? "pdf" : "png";
        var relativePath =
            VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_RUBRICAS) +
            "rubrica_" +
            rubricaId +
            "_" +
            DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "." + type;
        var basePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
        var path = basePath + relativePath;
        var tokenBase64 = _token.Base64(token);
        var authorizedPath = path.Replace("$1", tokenBase64);

        await _uploadFilesClient.UploadFile(relativePath, file);

        return (
            authorizedPath,
            relativePath
        );
    }

    public async Task<ApiGenericResult<List<RubricaCargaStatusDTO>>> ListarHistoricoDeCargaRecentes(int orgId, string cpfRequest)
    {
        var apiGenericResult = new ApiGenericResult<List<RubricaCargaStatusDTO>>();
        try
        {
            var restricaoDiretoria = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
            var result = await _rubricaCargaRepository.ListarHistoricoDeCargaRecentes(orgId, restricaoDiretoria);
                
            apiGenericResult.Retorno = result;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
        }
        return apiGenericResult;
    }
    
    public async Task<ApiGenericResult<List<RubricaCargaStatusDTO>>> ListarCargasPorOrg(int orgId, string codigoDiretoria, string rubricaId, int mes, int ano)
    {
        var apiGenericResult = new ApiGenericResult<List<RubricaCargaStatusDTO>>();
        try
        {
            var result = await _rubricaCargaRepository.ListarCargasPorOrg(orgId, codigoDiretoria, rubricaId, mes, ano);
                
            apiGenericResult.Retorno = result;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
        }
        return apiGenericResult;
    }
    
    public async Task<ApiGenericResult<RubricaCargaStatusDTO>> ObterCargaPorCodigoCarga(string codigoCarga)
    {
        var apiGenericResult = new ApiGenericResult<RubricaCargaStatusDTO>();
        try
        {
            var result = await _rubricaCargaRepository.ObterCargaPorCodigoCarga(codigoCarga);
                
            apiGenericResult.Retorno = result;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
        }
        return apiGenericResult;
    }
    
    public async Task<ApiGenericResult<IEnumerable<RubricaCargaOrigemPdfDTO>>> ObterPDFCargaPorMesEAno(int orgId, string codigoInternoColaborador, int mes, int ano)
    {
        var apiGenericResult = new ApiGenericResult<IEnumerable<RubricaCargaOrigemPdfDTO>>();
        try
        {
            var result = await _rubricaCargaRepository.ObterPDFCargaPorMesEAno(orgId, codigoInternoColaborador, mes, ano);
                
            apiGenericResult.Retorno = result;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
        }
        return apiGenericResult;
    }
}