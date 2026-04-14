using System.Text;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Extension;
using Core.Domain.Financeiro.NotaFiscal;
using Core.Domain.Financeiro.Rubrica;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Org;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.NF.NotaFiscalRubrica;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaboradorLiberacao;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Util.Enum;
using Financeiro.Domain.Impl.Rubrica.Rubrica.Constants;
using Financeiro.Domain.Interfaces.NotaFiscal;
using Microsoft.IdentityModel.Tokens;
using SRS.Infra.Constantes;
using TemplateOrg.Constantes;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.NotaFiscal;

[LogDomainClass]
public class NotaFiscalService(
    INotaFiscalRepository notaFiscalRepository, 
    INotaFiscalLogRepository notaFiscalLogRepository, 
    INotaFiscalRubricaRepository notaFiscalRubricaRepository,
    IDBConnectionUnitOfWork dbConnectionUnitOfWork, 
    IUploadFilesClient uploadFileClient,
    IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
    ITemplateRepository templateRepository,
    IEnvioEmail envioEmail,
    IRubricaColaboradorRepository rubricaColaboradorRepository,
    IOrgRepository orgRepository,
    IRestricaoDeAcessoService restricaoDeAcessoService,
    IRubricaColaboradorLiberacaoNfRepository rubricaColaboradorLiberacaoNfRepository,
    IUsuarioColaboradorRepository usuarioColaboradorRepository,
    ICurriculoClient curriculoClient) : INotaFiscalService
{
    private readonly HashSet<NotaFiscalStatusEnum> _statusPermitidosParaManipulacaoDeNf =
    [
        NotaFiscalStatusEnum.EM_PREPARACAO,
        NotaFiscalStatusEnum.AGUARDA_EMISSAO_DA_NF
    ];
    
    private readonly HashSet<NotaFiscalStatusEnum> _statusPermitidosParaCancelamentoEnvioNF =
    [
        NotaFiscalStatusEnum.NF_EM_ANALISE
    ];
    
    private readonly HashSet<NotaFiscalStatusEnum> _statusPermitidosParaReprovacaoDeNf =
    [
        NotaFiscalStatusEnum.NF_EM_ANALISE
    ];
    
    private readonly HashSet<NotaFiscalStatusEnum> _statusPermitidosParaAprovacaoDeNf =
    [
        NotaFiscalStatusEnum.NF_EM_ANALISE
    ];

    private readonly HashSet<NotaFiscalStatusEnum> _statusPermitidosParaPagamentoDeNf =
    [
        NotaFiscalStatusEnum.NF_APROVADA
    ];

    public async Task<ApiGenericResult<IEnumerable<NotaFiscalResult>>> ListarNotasFiscaisPorVigenciaComColaboradoresSemNFAsync(string? filtro, NotaFiscalStatusEnum? statusId, int? mes, int? ano, string? codDiretoria, string? documentoColaborador, string? codigoInternoColaborador, int cursor, int limite, int orgId, string? cpfGestor, string numeroNf = "")
    {
        var buscaNfs = await ListarNotasFiscaisPorVigenciaAsync(filtro, statusId, mes, ano, codDiretoria, documentoColaborador, codigoInternoColaborador, cursor, limite, orgId, cpfGestor, numeroNf);
        
       if (statusId == NotaFiscalStatusEnum.AGUARDA_EMISSAO_DA_NF || statusId == null || statusId == 0)
        {
            try
            {
                var rubricasNaoUsadas =
                    await rubricaColaboradorLiberacaoNfRepository
                        .ListarRubricasLiberacaoNaoUsadasPorVigenciaAsync(
                            mes,
                            ano,
                            orgId,
                            codigoInternoColaborador,
                            codDiretoria);

                var rubricasAgrupadas = rubricasNaoUsadas
                    .GroupBy(r => new
                    {
                        r.CodigoInternoColaborador,
                        r.NomeColaborador,
                        r.VigenciaMes,
                        r.VigenciaAno
                    })
                    .Select(g => new NotaFiscalResult
                    {
                        NomeColaborador = g.Key.NomeColaborador,
                        VigenciaMes = g.Key.VigenciaMes,
                        VigenciaAno = g.Key.VigenciaAno,
                        NumeroNf = null,
                        SumarioValorTotalDeRubricas = g.Sum(x => x.Valor),
                        NotaFiscalStatusId = NotaFiscalStatusEnum.AGUARDA_EMISSAO_DA_NF,
                        NotaFiscalStatusDescricao = "Aguarda Emissão da NF",
                        Rubricas = g.Select(x => new NotaFiscalRubricaResult
                        {
                            Natureza = x.Natureza,
                            RubricaColaboradorId = x.RubricaColaboradorId,
                            RubricaDescricao = x.Descricao,
                            Valor = x.Valor,
                            CodigoRubrica = "",
                            Tipo = ""
                        }).ToList()
                    })
                    .ToList();

                buscaNfs.Retorno = buscaNfs.Retorno
                    .Concat(rubricasAgrupadas)
                    .ToList();
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Notas Fiscais");
            }
        }

        buscaNfs.Retorno = buscaNfs.Retorno
            .OrderByDescending(x => x.VigenciaAno)
            .ThenByDescending(x => x.VigenciaMes)
            .ToList();

        return buscaNfs;
    }
    
    public async Task<ApiGenericResult<IEnumerable<NotaFiscalResult>>> ListarNotasFiscaisPorVigenciaAsync(string? filtro, NotaFiscalStatusEnum? statusId, int? mes, int? ano, string? codDiretoria, string? documentoColaborador, string? codigoInternoColaborador, int cursor, int limite, int orgId, string? cpfGestor, string numeroNf = "")
    {
        var genericResult = new ApiGenericResult<IEnumerable<NotaFiscalResult>>();

        string? cpfParaRestricao = null;
        if (!cpfGestor.IsNullOrEmpty())
        {
            await ValidaAcessoGestor(cpfGestor, orgId);
            cpfParaRestricao = cpfGestor;
        }

        var diretoriasRestricao = new List<string>();
        if (cpfParaRestricao != null)
        {
            var restricoes = await restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfParaRestricao, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA, codDiretoria);
            if (restricoes != null) diretoriasRestricao.AddRange(restricoes);
        }
        try
        {
            var notasFiscais = (await notaFiscalRepository.ListarNotasFiscaisPorVigencia(filtro, statusId, mes, ano, diretoriasRestricao, documentoColaborador, codigoInternoColaborador, orgId, cursor, limite, numeroNf)).ToList();

            if (notasFiscais.Any())
            {
                var rubricas = (await notaFiscalRubricaRepository.ListarRubricasPorListaDeNotaFiscalId(
                    notasFiscais.Select(x => x.Id).ToList())).ToList();

                if (rubricas.Any())
                {
                    var rubricasPorNota = rubricas.GroupBy(r => r.NotaFiscalId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var nota in notasFiscais)
                    {
                        if (rubricasPorNota.TryGetValue(nota.Id, out var rubricasNota))
                        {
                            nota.Rubricas = rubricasNota;
                            nota.SumarioValorTotalDeRubricas = rubricasNota
                                .Where(x => x.Tipo == CalculoTipoConstant.VALOR)
                                .Sum(x => x.Natureza == "Crédito" ? x.Valor :
                                    x.Natureza == "Débito" ? -x.Valor : 0);
                        }
                    }
                }
            }

            genericResult.Retorno = notasFiscais;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Notas Fiscais");
        }
        return genericResult;
    }
    
    public async Task<ApiGenericResult<NotaFiscalResult>> InserirNotaFiscal(InserirNotaFiscalParam input, string codigoInternoColaborador, int orgId)
    {
        var apiResult  = new ApiGenericResult<NotaFiscalResult>();
        
        if (input.ListaDeIdsRubricasLiberacao.Count == 0)
        {
            throw new ArgumentException("Rubricas não selecionadas");
        }
        if (input.VigenciaMes == 0 || input.VigenciaAno == 0)
        {
            throw new ArgumentException("Vigência não selecionada");
        }
        if (input.NumeroNf.IsNullOrEmpty())
        {
            throw new ArgumentException("Numero da NF é obrigatória");
        }
        
        var listaDeRubricas =
            (await rubricaColaboradorLiberacaoNfRepository.ListarRubricasColaboradorLiberacaoNfPorListaDeIds(
                input.ListaDeIdsRubricasLiberacao)).DistinctBy(x => x.Id).ToList();
 
        var rubricasEmUso = listaDeRubricas
            .Where(r => r.EmUso)
            .Select(r => r.Descricao)
            .ToList();

        if (rubricasEmUso.Any())
        {
            var descricoes = string.Join(", ", rubricasEmUso);
            throw new ArgumentException(
                $"As seguintes rubricas selecionadas já estão em uso: {descricoes}");
        }
        
        var rubricasInvalidas = listaDeRubricas
            .Where(x => x.VigenciaMes != input.VigenciaMes || x.VigenciaAno != input.VigenciaAno)
            .Select(x => x.Descricao).ToList();

        if (rubricasInvalidas.Any())
        {
            var descricoes = string.Join(", ", rubricasInvalidas);
            throw new ArgumentException(
                $"As seguintes rubricas não são da vigência {input.VigenciaMes:D2}/{input.VigenciaAno}: {descricoes}");
        }
        
        try
        {
            dbConnectionUnitOfWork.BeginTransaction();
            
            decimal? valorAnalise = null;

            try
            {
                var token = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN");
                var analiseIa = await curriculoClient.AnalisarValorNotaFiscal(input.Base64Objeto, token);
                if (analiseIa.Valor != null)
                {
                    valorAnalise = analiseIa.Valor.Value.ParseDecimalUniversal();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao analisar NF: {ex.Message}");
            }

            var rubricasCredito = listaDeRubricas.Where(r => r.Natureza == "Crédito").Sum(x => x.Valor);
            var rubricasDebito = listaDeRubricas.Where(r => r.Natureza == "Débito").Sum(x => x.Valor);
            var soma = rubricasCredito - rubricasDebito;
            
            var idNotaFiscal = await notaFiscalRepository.InserirNotaFiscalAsync(input.VigenciaMes, input.VigenciaAno, codigoInternoColaborador, orgId, soma, input.NumeroNf, valorAnalise);
            var notaFiscal = await notaFiscalRepository.ObterNotaFiscalPorIdAsync(idNotaFiscal);
            
            var logParam = new NotaFiscalLogInput()
            {
                NotaFiscalId = idNotaFiscal,
                NotaFiscalStatusAnteriorId = notaFiscal.NotaFiscalStatusId,
                NotaFiscalStatusNovoId = notaFiscal.NotaFiscalStatusId,
                NumeroNf = notaFiscal.NumeroNf,
                DataEmissaoNotaFiscal = notaFiscal.DataEmissaoNotaFiscal,
                Observacao = notaFiscal.MotivoReprovacao,
                DataCriacao = DateTime.UtcNow,
                DataAlteracao = DateTime.UtcNow,
                CodigoInternoColaboradorAlteracao = Guid.Parse(codigoInternoColaborador),
                CodigoInternoColaboradorCriacao = Guid.Parse(codigoInternoColaborador)
            };
            
            await notaFiscalLogRepository.InserirLogAsync(logParam);

            foreach (var rubrica in listaDeRubricas)
            {
               await notaFiscalRubricaRepository.InserirNotaFiscalRubricaAsync(notaFiscal.Id, rubrica.RubricaColaboradorId.ToString(), rubrica.Valor, orgId);
            }
            
            var uploadArquivo = await UploadNotaFiscal(new UploadNotaFiscalParam
            {
                NotaFiscalId = idNotaFiscal,
                NumeroNf = notaFiscal.NumeroNf,
                Base64Objeto = input.Base64Objeto
            }, codigoInternoColaborador, false);
            
            apiResult.Retorno = uploadArquivo.Retorno;
            dbConnectionUnitOfWork.Commit();
        }
        catch (Exception ex)
        {
            dbConnectionUnitOfWork.SafeRollback();
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, "Nota Fiscal");
        }
        return apiResult;
    }

    public async Task<ApiGenericResult> AprovarNotasFiscais(NotaFiscalStatusUpdateParam param, string? cpfRequest, int orgId)
    {
        var result = new ApiGenericResult();
        await ModificarStatusDaNotaFiscalGestor(param, cpfRequest, NotaFiscalStatusEnum.NF_APROVADA, orgId);
        return result;
    }
    
    public async Task<ApiGenericResult> ReprovarNotasFiscais(NotaFiscalStatusUpdateParam param, string? cpfRequest, int orgId)
    {
        var result = new ApiGenericResult();
        await ModificarStatusDaNotaFiscalGestor(param, cpfRequest, NotaFiscalStatusEnum.NF_REPROVADA, orgId);
        return result;
    }

    private async Task ModificarStatusDaNotaFiscalGestor(NotaFiscalStatusUpdateParam param, string? cpfRequest, NotaFiscalStatusEnum novoStatus, int orgId, bool validaAcesso = true)
    {
        if (validaAcesso)
        {
            await ValidaAcessoGestor(cpfRequest, orgId);
        }
        dbConnectionUnitOfWork.BeginTransaction();
        try
        {
            var listaDeNfs = await notaFiscalRepository.ListarNotaFiscaisPorListaDeIdsAsync(param.Ids);
            var notaFiscalResults = listaDeNfs.ToArray();

            string motivo = null;
            HashSet<NotaFiscalStatusEnum> statusValidacao;

            switch (novoStatus)
            {
                case NotaFiscalStatusEnum.NF_APROVADA:
                    statusValidacao = _statusPermitidosParaAprovacaoDeNf;
                    break;
                case NotaFiscalStatusEnum.NF_REPROVADA:
                    statusValidacao = _statusPermitidosParaReprovacaoDeNf;
                    motivo = param.MotivoReprovacao;
                    break;
                case NotaFiscalStatusEnum.NF_PAGA:
                    statusValidacao = _statusPermitidosParaPagamentoDeNf;
                    break;
                default:
                    throw new ArgumentException($"Status {novoStatus} não é permitido para esta operação.");
            }

            if (notaFiscalResults.Any(nf => !statusValidacao.Contains(nf.NotaFiscalStatusId.GetValueOrDefault())))
            {
                throw new ArgumentException("Uma ou mais Notas Fiscais não estão em um status válido para alteração.");
            }

            await notaFiscalRepository.MudarStatusNotaFiscalPorListaDeIdsAsync(param.Ids, novoStatus,
                motivo, cpfRequest);

            foreach (var nf in notaFiscalResults)
            {
                var logParam = new NotaFiscalLogInput()
                {
                    NotaFiscalId = nf.Id,
                    NotaFiscalStatusAnteriorId = nf.NotaFiscalStatusId,
                    NotaFiscalStatusNovoId = novoStatus,
                    NumeroNf = nf.NumeroNf,
                    DataEmissaoNotaFiscal = nf.DataEmissaoNotaFiscal,
                    Observacao = motivo,
                    DataCriacao = DateTime.UtcNow,
                    DataAlteracao = DateTime.UtcNow,
                    CodigoInternoColaboradorAlteracao = Guid.Parse(cpfRequest),
                    CodigoInternoColaboradorCriacao = nf.CodigoInternoColaboradorCriacao == null ? null : Guid.Parse(nf.CodigoInternoColaboradorCriacao)
                };
                await notaFiscalLogRepository.InserirLogAsync(logParam);
            }
            dbConnectionUnitOfWork.Commit();
        }
        catch (Exception ex)
        {
            dbConnectionUnitOfWork.SafeRollback();
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, "Nota fiscais");
        }
    }

    public async Task<ApiGenericResult<IEnumerable<NotaFiscalStatusDTO>>> ListarNotaFiscalStatus()
    {
        var result = new ApiGenericResult<IEnumerable<NotaFiscalStatusDTO>>();
        result.Retorno = await notaFiscalRepository.ListarNotaFiscalStatus();
        return result;
    }

    private async Task<ApiGenericResult<NotaFiscalResult>> ObterNotaFiscalPorId(Guid id)
    {
        var result = new ApiGenericResult<NotaFiscalResult>();
        var nf = await notaFiscalRepository.ObterNotaFiscalPorIdAsync(id);
        var rubricas = await notaFiscalRubricaRepository.ListarRubricasPorNotaFiscalId(id);
        var notaFiscalRubricaResults = rubricas as NotaFiscalRubricaResult[] ?? rubricas.ToArray();
        
        nf.SumarioValorTotalDeRubricas = notaFiscalRubricaResults
            .Where(x => x.Tipo == CalculoTipoConstant.VALOR)
            .Sum(x => x.Natureza == "Crédito" ? x.Valor :
                x.Natureza == "Débito" ? -x.Valor : 0);
        nf.Rubricas = notaFiscalRubricaResults;
        
        return result;
    }

    public async Task<ApiGenericResult<NotaFiscalResult>> UploadNotaFiscal(UploadNotaFiscalParam input, string cpfRequest,bool useTransaction = true)
    {
        var result = new ApiGenericResult<NotaFiscalResult>();
        if (useTransaction)
        {
            dbConnectionUnitOfWork.BeginTransaction();
        }
        try
        {
            var buscaNotaFiscal = await notaFiscalRepository.ObterNotaFiscalPorIdAsync(input.NotaFiscalId);
            if (buscaNotaFiscal == null)
            {
                throw new ArgumentException("Nota Fiscal não encontrada.");
            }

            if (!_statusPermitidosParaManipulacaoDeNf.Contains(buscaNotaFiscal.NotaFiscalStatusId.GetValueOrDefault()))
            {
                throw new ArgumentException("Não é possivel editar a nota fiscal se ela não estiver em preparação.");
            }
            
            var status = NotaFiscalStatusEnum.NF_EM_ANALISE;
            var dataEmissao = DateTime.UtcNow;
            
            var dataFormatada = dataEmissao.ToString("yyyyMMdd_HHmmss"); 

            var path = $"nota_fiscal/{cpfRequest}-{dataFormatada}.{input.Base64Objeto.Tipo}";

            await uploadFileClient.UploadFile(path, input.Base64Objeto.Base64ToByteArray());
            
            await notaFiscalRepository.EditarNotaFiscalPorIdAsync(buscaNotaFiscal.Id, input.NumeroNf, dataEmissao, buscaNotaFiscal.Valor, path,  status,cpfRequest);
            
            var logParam = new NotaFiscalLogInput()
            {
                NotaFiscalId = buscaNotaFiscal.Id,
                NotaFiscalStatusAnteriorId = buscaNotaFiscal.NotaFiscalStatusId,
                NotaFiscalStatusNovoId = status,
                NumeroNf = buscaNotaFiscal.NumeroNf,
                DataEmissaoNotaFiscal =dataEmissao,
                Observacao = buscaNotaFiscal.MotivoReprovacao,
                DataCriacao = DateTime.UtcNow,
                DataAlteracao = DateTime.UtcNow,
                CodigoInternoColaboradorAlteracao = Guid.Parse(cpfRequest),
                CodigoInternoColaboradorCriacao = buscaNotaFiscal.CodigoInternoColaboradorCriacao == null ? null : Guid.Parse(buscaNotaFiscal.CodigoInternoColaboradorCriacao)
            };
            await notaFiscalLogRepository.InserirLogAsync(logParam);

            result.Retorno = (await ObterNotaFiscalPorId(input.NotaFiscalId)).Retorno;

            if (useTransaction)
            {
                dbConnectionUnitOfWork.Commit();
            }
        }
        catch (Exception ex)
        {
            if (useTransaction)
            {
                dbConnectionUnitOfWork.SafeRollback();
            }
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, "Upload Nota Fiscal");
        }
        return result;
    }

    public async Task<ApiGenericResult<NotaFiscalResult>> CancelarEnvioNf(Guid notaFiscalId, string cpfRequest)
    {
        var result = new ApiGenericResult<NotaFiscalResult>();
        dbConnectionUnitOfWork.BeginTransaction();
        try
        {
            var buscaNotaFiscal = await notaFiscalRepository.ObterNotaFiscalPorIdAsync(notaFiscalId);
            if (buscaNotaFiscal == null)
            {
                throw new ArgumentException("Nota Fiscal não encontrada.");
            }

            if (!_statusPermitidosParaCancelamentoEnvioNF.Contains(buscaNotaFiscal.NotaFiscalStatusId.GetValueOrDefault()))
            {
                throw new ArgumentException("Não é possível remover um documento em quanto a NF estiver diferente de em analise.");
            }
            
            var status = NotaFiscalStatusEnum.NF_CANCELADA;
            await notaFiscalRepository.EditarNotaFiscalPorIdAsync(buscaNotaFiscal.Id, buscaNotaFiscal.NumeroNf, buscaNotaFiscal.DataEmissaoNotaFiscal, buscaNotaFiscal.Valor, buscaNotaFiscal.UrlNotaFiscalDownload,  status,cpfRequest);
            
            var logParam = new NotaFiscalLogInput()
            {
                NotaFiscalId = buscaNotaFiscal.Id,
                NotaFiscalStatusAnteriorId = buscaNotaFiscal.NotaFiscalStatusId,
                NotaFiscalStatusNovoId = status,
                NumeroNf = null,
                DataEmissaoNotaFiscal = null,
                Observacao = buscaNotaFiscal.MotivoReprovacao,
                DataCriacao = DateTime.UtcNow,
                DataAlteracao = DateTime.UtcNow,
                CodigoInternoColaboradorAlteracao = Guid.Parse(cpfRequest),
                CodigoInternoColaboradorCriacao = buscaNotaFiscal.CodigoInternoColaboradorCriacao == null ? null : Guid.Parse(buscaNotaFiscal.CodigoInternoColaboradorCriacao)
            };
            await notaFiscalLogRepository.InserirLogAsync(logParam);
            
            await uploadFileClient.DeleteFile(buscaNotaFiscal.UrlNotaFiscalDownload);
            
            dbConnectionUnitOfWork.Commit();
        }
        catch (Exception ex)
        {
            dbConnectionUnitOfWork.SafeRollback();
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, "Cancelamento Nota Fiscal");
        }
        return result;
    }

    private async Task ValidaAcessoGestor(string? codigoInternoColaborador, int orgId)
    {
        var validaAcesso = await funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(codigoInternoColaborador, orgId, FuncionalidadeSistemaEnum.GESTAO_PRESTADOR);
        if (!validaAcesso)
        {
            throw new UnauthorizedAccessException("Acesso não autorizado para Gestão PJ");
        }
    }
    
    public async Task<ApiGenericResult<LiberarEmissaoDeNfsResult>> LiberarEmissaoDeNotasFiscaisPorVigencia(int orgId, int mes, int ano, string? codigoDiretoria, bool enviarEmail, string cpfRequest)
    {
        var result = new ApiGenericResult<LiberarEmissaoDeNfsResult>();
        
        dbConnectionUnitOfWork.BeginTransaction();
        try
        {
            var (listaErro, listaSucesso, rubricasInseridas) = await GerarRubricasContabeisPjFrequenciaMensalPorVigencia(orgId, mes, ano, cpfRequest, codigoDiretoria);

            var retorno = new LiberarEmissaoDeNfsResult();
            result.Retorno = retorno;
                
            result.Retorno.RubricaCcntabilErroLog.AddRange(listaErro); ;
            result.Retorno.RubricaContabilSucessoLog.AddRange(listaSucesso);
            
            result.Retorno.QuantidadeErros = listaErro.Count;
            result.Retorno.QuantidadeSucesso = listaSucesso.Count;

            if (enviarEmail)
            {
                var link =  await orgRepository.BuscaSubDominioOrg(orgId);
                var vigencia = $"{mes}/{ano}";
                var buscaTemplate =  templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.NOTIFICACAO_PJ_EMISSAO);
                if (buscaTemplate != null)
                {
                    var org = orgRepository.BuscarOrg(orgId);
                    foreach (var destinatario in rubricasInseridas.DistinctBy(x => x.CodigoInternoColaborador))
                    {
                        var buscaEmail = await usuarioColaboradorRepository.GetEmailColaboradorPorCodigoInterno(destinatario.CodigoInternoColaborador.ToString(), orgId);
                        
                        var template = buscaTemplate.Template;

                        var replace = template
                            .Replace("{COLABORADOR}", destinatario.NomeColaborador)
                            .Replace("{VIGENCIA}", vigencia)
                            .Replace("{PATH_TO_LINK}", link);
                        
                        envioEmail.EnviaEmailSemTemplate(destinatario.NomeColaborador, replace, $"Emissão de Nota Fiscal para {org.Descricao}", buscaEmail);
                    }
                }
            }
            
            dbConnectionUnitOfWork.Commit();
        }
        catch (Exception ex)
        {
            dbConnectionUnitOfWork.SafeRollback();
            ExceptionUtil.GerenciarRetornoExcecao(ex,  CRUDEnum.Update, "emissões de notas fiscais");
        }
        
        return result;
    }

    private async Task<(List<string>, List<string>, List<RubricaColaboradorResult> rubricasInseridas)> GerarRubricasContabeisPjFrequenciaMensalPorVigencia(int orgId, int mes, int ano, string cpfRequest, string? codigoDiretoria)
    {
        var listaLogsErros = new List<string>();
        var listaLogsSucessos = new List<string>();
        
        var listaDeRubricasColaborador = (await rubricaColaboradorRepository.ListarRubricasContabeisColaboradorPorVigenciaAsync(mes, ano, codigoDiretoria, orgId)).ToList();
        
        var listaExistentesParaLiberacaoNaVigencia = await rubricaColaboradorLiberacaoNfRepository.ListarRubricaColaboradorIdsEmLiberacaoPorVigenciaAsync(mes, ano, orgId);
        
        // Rúbricas do colaborador que JÁ EXISTEM em liberação
        var rubricasExistentes = listaDeRubricasColaborador
            .Where(x => listaExistentesParaLiberacaoNaVigencia.Contains(x.Id))
            .ToList();

        // Rúbricas do colaborador que NÃO EXISTEM em liberação (novas)
        var rubricasNaoExistentes = listaDeRubricasColaborador
            .Where(x => !listaExistentesParaLiberacaoNaVigencia.Contains(x.Id))
            .ToList();
        
       foreach (var rubrica in rubricasNaoExistentes)
       {
           try
           {
               await rubricaColaboradorLiberacaoNfRepository.InserirRubricaLiberacaoColaboradorAsync(orgId, rubrica.Id, mes, ano);
           }
           catch (Exception ex)
           {
               throw new ApplicationException($"Erro geral ao tentar criar rubrica contabil: {rubrica.Descricao} para: {rubrica.NomeColaborador}", ex);
           }
       }

       foreach (var rubrica in rubricasExistentes)
       {
           var mensagem = $"Rubrica: ({rubrica.Descricao}) já existente na NF do colaborador: {rubrica.NomeColaborador}";
           listaLogsErros.Add(mensagem);
       }

       return (
           listaLogsErros,
           listaLogsSucessos,
           rubricasNaoExistentes
       );
    }
    
    public async Task<ApiGenericResult> GerarPagamentoDeSolicitacoesPorIds(NotaFiscalStatusUpdateParam param, string? cpfRequest, int orgId, bool validaAcesso = true)
    {
        var result = new ApiGenericResult();
        await ModificarStatusDaNotaFiscalGestor(param, cpfRequest, NotaFiscalStatusEnum.NF_PAGA, orgId, validaAcesso);
        return result;
    }

    public async Task<ApiGenericResult<List<RubricaColaboradorLiberacaoNfDTO>>> ListarRubricasColaboradorParaLiberacaoDeNf(string codigoInternoColaborador, int? mes, int? ano, int orgId)
    {
        var apiResult = new ApiGenericResult<List<RubricaColaboradorLiberacaoNfDTO>>();
        var result = await rubricaColaboradorLiberacaoNfRepository.ListarRubricasColaboradorLiberacaoNfPorVigenciaAsync(mes, ano, orgId, codigoInternoColaborador);
        apiResult.Retorno = result;;
        return apiResult;
    }
}