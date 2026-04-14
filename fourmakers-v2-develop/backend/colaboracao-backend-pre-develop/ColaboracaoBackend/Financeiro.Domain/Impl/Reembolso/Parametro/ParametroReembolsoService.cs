using Colaboracao.Helper.Enum;
using Core.Domain.Reembolso.Parametro;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Parametro;
using Financeiro.Domain.Interfaces.Reembolso.Parametro;
using Financeiro.Domain.Interfaces.Reembolso.Validadores;
using Financeiro.Domain.Interfaces.Reembolso.Verba;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Parametro;

[LogDomainClass]
public class ParametroReembolsoService : IParametroReembolsoService
{
    private readonly IParametroReembolsoRepository _parametroReembolsoRepository;
    private readonly IVerbaLogService _verbaLogService;
    private readonly IParametroReembolsoValidadorService _parametroReembolsoValidadorService;

    public ParametroReembolsoService(IParametroReembolsoRepository parametroReembolsoRepository, IVerbaLogService verbaLogService, IParametroReembolsoValidadorService parametroReembolsoValidadorService)
    {
        _parametroReembolsoRepository = parametroReembolsoRepository;
        _verbaLogService = verbaLogService;
        _parametroReembolsoValidadorService = parametroReembolsoValidadorService;
    }

    public async Task<ApiGenericResult<ParametroReembolsoDTO>> EditarAsync(ParametroReembolsoDTO input, int orgId, string codigoInternoColaborador)
    {
        var apiResult = new ApiGenericResult<ParametroReembolsoDTO>();
        try
        {
            var buscarPorOrg = await _parametroReembolsoRepository.BuscarPorOrgIdAsync(orgId);
            if (buscarPorOrg != null)
            {
                _parametroReembolsoValidadorService.ValidaParametroReembolso(input, CRUDEnum.Update);
                var update = await _parametroReembolsoRepository.EditarAsync(buscarPorOrg.Id, input.LimiteEnvio, input.DiaPagamento, input.ValidadeComprovanteDias, input.LimiteEnvioAlternativo, input.DiaPagamentoAlternativo, codigoInternoColaborador, input.PermitirAprovarMinhasSolicitacoes);
                var updatesLogs = _parametroReembolsoValidadorService.GerenciarLogs(buscarPorOrg, update, CRUDEnum.Update);
                foreach (var log in updatesLogs)
                {
                    await _verbaLogService.InserirLogAsync(log.Regra, log.Acao, log.ValorAnterior, log.NovoValor, codigoInternoColaborador, orgId);
                }
                apiResult.Retorno = update;
            }
            else
            {
                var insert = await InserirAsync(input, orgId, codigoInternoColaborador);
                apiResult.Retorno = insert.Retorno;
            }
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update,"Parametro");
        }
        return apiResult;
    }

    private async Task<ApiGenericResult<ParametroReembolsoDTO>> InserirAsync(ParametroReembolsoDTO input, int orgId, string codigoInternoColaborador)
    {
        var apiResult = new ApiGenericResult<ParametroReembolsoDTO>();
        try
        {
            _parametroReembolsoValidadorService.ValidaParametroReembolso(input, CRUDEnum.Create);
            var insert = await _parametroReembolsoRepository.InserirParametroAsync(input.LimiteEnvio, input.DiaPagamento, input.ValidadeComprovanteDias, orgId, input.LimiteEnvioAlternativo, input.DiaPagamentoAlternativo, codigoInternoColaborador, input.PermitirAprovarMinhasSolicitacoes);
            await _verbaLogService.InserirLogAsync("Parametro Reembolso", CRUDEnum.Create.ObterAcao(), "Vazio", "Novo Parametro", codigoInternoColaborador, orgId);
            apiResult.Retorno = insert;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create,"Parametro");
        }
        return apiResult;
    }

    public async Task<ApiGenericResult<ParametroReembolsoDTO?>> ObterPorOrgIdAsync(int orgId)
    {
        var apiResult = new ApiGenericResult<ParametroReembolsoDTO?>();
        try
        {
            var parametro = await _parametroReembolsoRepository.BuscarPorOrgIdAsync(orgId);
            apiResult.Retorno = parametro;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Verbas e Parametro");
            throw;
        }
        return apiResult;
    }
}