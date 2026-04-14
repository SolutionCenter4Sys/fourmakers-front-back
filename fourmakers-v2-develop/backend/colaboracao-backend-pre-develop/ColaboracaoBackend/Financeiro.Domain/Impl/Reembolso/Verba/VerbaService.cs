using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.Domain.Reembolso.Verba;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Reembolso.Parametro;
using Financeiro.Domain.Interfaces.Reembolso.Validadores;
using Financeiro.Domain.Interfaces.Reembolso.Verba;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Verba;

[LogDomainClass]
public class VerbaService : IVerbaService
{
    private readonly IVerbaRepository _verbaRepository;
    private readonly IParametroReembolsoService _parametroReembolsoService;
    private readonly IVerbaLogService _verbaLogService;
    private readonly IDBConnectionUnitOfWork _unitOfWork;
    private readonly IVerbaValidadorService _verbaValidadorService;
    private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

    public VerbaService(IVerbaRepository verbaRepository, IParametroReembolsoService parametroReembolsoService, IVerbaLogService verbaLogService, IDBConnectionUnitOfWork unitOfWork, IVerbaValidadorService verbaValidadorService, IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
    {
        _verbaRepository = verbaRepository;
        _parametroReembolsoService = parametroReembolsoService;
        _verbaLogService = verbaLogService;
        _unitOfWork = unitOfWork;
        _verbaValidadorService = verbaValidadorService;
        _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
    }

    public async Task<ApiGenericResult<VerbaEParametroDTO>> EditarAsync(VerbaEParametroDTO input, int orgId, string codigoInternoColaborador)
    {
        var apiResult = new ApiGenericResult<VerbaEParametroDTO>();
        _unitOfWork.BeginTransaction();
        try
        {
            await ValidaAcesso(orgId, codigoInternoColaborador);
            var parametro = await _parametroReembolsoService.EditarAsync(input.ParametroReembolso, orgId, codigoInternoColaborador);

            var listaDeVerbas = await _verbaRepository.ListarAsync(orgId);

            var listaDeIds = listaDeVerbas.Select(x => x.Id).ToList();
            
            var verbasParaEditar = input.Verbas
                .Where(x => listaDeIds.Contains(x.Id))
                .ToList();
            
            var verbasParaInserir = input.Verbas
                .Where(x => !listaDeIds.Contains(x.Id))
                .ToList();

            var listaDeVerbasRetorno = new List<VerbaDTO>();

            foreach (var verba in verbasParaInserir)
            {
                var insert = await InserirAsync(verba, orgId, codigoInternoColaborador);
                listaDeVerbasRetorno.Add(insert.Retorno);
            }
            
            foreach (var verba in verbasParaEditar)
            {
                await _verbaValidadorService.ValidaVerba(verba, CRUDEnum.Update);
                var update = await _verbaRepository.EditarAsync(verba.Id, verba.Categoria, verba.TipoCusto, verba.Unidade, verba.Valor, verba.CustoCliente, verba.Ativo, codigoInternoColaborador);
                listaDeVerbasRetorno.Add(update);
                
                var verbaAnterior = listaDeVerbas.FirstOrDefault(x => x.Id == verba.Id);
                if (verbaAnterior != null)
                {
                    var verbaLog = await _verbaValidadorService.GerenciarLogs(verba, verbaAnterior, CRUDEnum.Update);
                    foreach (var log in verbaLog)
                    {
                        await _verbaLogService.InserirLogAsync(log.Regra, log.Acao, log.ValorAnterior, log.NovoValor, codigoInternoColaborador, orgId);
                    }
                }
            }

            apiResult.Retorno = new()
            {
                Verbas = listaDeVerbasRetorno,
                ParametroReembolso = parametro.Retorno
            };
            _unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, "Verba");
        }
        return apiResult;
    }

    private async Task<ApiGenericResult<VerbaDTO>> InserirAsync(VerbaDTO input, int orgId, string codigoInternoColaborador)
    {
        var apiResult = new ApiGenericResult<VerbaDTO>();
        try
        {
            await _verbaValidadorService.ValidaVerba(input, CRUDEnum.Create);
            var inserir = await _verbaRepository.InserirAsync(input.Categoria, input.TipoCusto, input.Unidade,
            input.Valor, input.CustoCliente, orgId, input.Ativo, codigoInternoColaborador);
            await _verbaLogService.InserirLogAsync(input.Categoria, CRUDEnum.Create.ObterAcao(), "Vazio", "Nova Verba", codigoInternoColaborador, orgId);
            apiResult.Retorno = inserir;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Verba");
        }

        return apiResult;
    }

    public async Task<ApiGenericResult<VerbaEParametroDTO>> ObterVerbasEParametro(int orgId, string cpf)
    {
        var apiResult = new ApiGenericResult<VerbaEParametroDTO>();
        try
        {
            await ValidaAcesso(orgId, cpf);
            var parametro = await _parametroReembolsoService.ObterPorOrgIdAsync(orgId);
            var verbas = await _verbaRepository.ListarAsync(orgId);

            apiResult.Retorno = new()
            {
                Verbas = verbas,
                ParametroReembolso = parametro.Retorno
            };
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Verbas e Parametro");
            throw;
        }
        return apiResult;
    }
    
    public async Task<ApiGenericResult<List<VerbaSimplificadoDTO>>> ListarSimplificadoAsync(int orgId)
    {
        var apiResult = new ApiGenericResult<List<VerbaSimplificadoDTO>>();
        try
        {
            apiResult.Retorno = await _verbaRepository.ListarSimplificadoAsync(orgId);
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Verbas");
            throw;
        }
        return apiResult;
    }

    private async Task ValidaAcesso(int orgId, string cpf)
    {
        var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.PARAMETRIZACAO_REEMBOLSO).Result;
        if (!isValid)
        {
            throw new ArgumentException("Você não possui acesso a parametrização de reembolso");
        }
    }
    
    public async Task<ApiGenericResult<VerbaEParametroValidacaoDTO>> ObterVerbasEParametroValidacao(int orgId, string cpf)
    {
        var apiResult = new ApiGenericResult<VerbaEParametroValidacaoDTO>();
        try
        {
            var parametro = await _parametroReembolsoService.ObterPorOrgIdAsync(orgId);
            var verbas = await _verbaRepository.ListarSimplificadoAsync(orgId);

            apiResult.Retorno = new()
            {
                ParametroReembolso = parametro.Retorno,
                Verbas = verbas
            };
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Verbas e Parametro");
            throw;
        }
        return apiResult;
    }
    
    public async Task<ApiGenericResult<List<VerbaSimplificadoDTO>>> ListarSimplificadoComExcecaoAsync(int orgId, string cpfRequset, string codigoProjeto, string codigoCliente)
    {
        var apiResult = new ApiGenericResult<List<VerbaSimplificadoDTO>>();
        try
        {
            var verbas = await _verbaRepository.ListarSimplificadoAsync(orgId);
            foreach (var verba in verbas)
            {
                var excecao = await _verbaRepository.ObterExcecaoDaVerbaPorColaboradorProjetoECliente(verba.Id, orgId, codigoCliente, codigoProjeto, cpfRequset);
                if (excecao.HasValue)
                {
                    verba.Valor = excecao.Value;
                }
            }

            apiResult.Retorno = verbas;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Verbas");
            throw;
        }
        return apiResult;
    }
}