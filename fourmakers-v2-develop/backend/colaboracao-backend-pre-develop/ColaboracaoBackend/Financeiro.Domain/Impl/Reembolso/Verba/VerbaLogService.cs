using Colaboracao.Helper.Enum;
using Core.Domain.Reembolso.Verba;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;
using Financeiro.Domain.Interfaces.Reembolso.Verba;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Verba;

[LogDomainClass]
public class VerbaLogService : IVerbaLogService
{
    private IVerbaLogRepository _verbaLogRepository;

    public VerbaLogService(IVerbaLogRepository verbaLogRepository)
    {
        _verbaLogRepository = verbaLogRepository;
    }

    public async Task<ApiGenericResult<List<VerbaLogDTO>>> ListarLogsAsync(int orgId)
    {
        var apiResult = new ApiGenericResult<List<VerbaLogDTO>>();
        try
        {
            apiResult.Retorno = await _verbaLogRepository.ListarLogsAsync(orgId);
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Listar Logs");
        }
        
        return apiResult;
    }

    public async Task InserirLogAsync(string regra, string acao, string valorAnterior, string novoValor, string codigoInternoColaborador, int orgId)
    {
        try
        {
            await _verbaLogRepository.InserirLogAsync(regra, acao, valorAnterior, novoValor, codigoInternoColaborador, orgId);
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Inserir Logs");
        }
    }
}