using Colaboracao.Helper.Enum;
using Core.Domain.Reembolso.Verba;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;
using Financeiro.Domain.Interfaces.Reembolso.Verba;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Verba;

[LogDomainClass]
public class VerbaTipoService : IVerbaTipoService
{
    private readonly IVerbaTipoRepository _verbaTipoRepository;

    public VerbaTipoService(IVerbaTipoRepository verbaTipoRepository)
    {
        _verbaTipoRepository = verbaTipoRepository;
    }

    public async Task<ApiGenericResult<List<VerbaTipoDTO>>> ListarAsync(int orgId)
    {
        var apiGenericResult = new ApiGenericResult<List<VerbaTipoDTO>>();
        try
        {
           apiGenericResult.Retorno =  await _verbaTipoRepository.ListarAsync(orgId);
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Tipo Verba");
        }
        return apiGenericResult;
    }
    
    public async Task<ApiGenericResult<VerbaTipoDTO?>> BuscarPorIdAsync(int id)
    {
        var apiGenericResult = new ApiGenericResult<VerbaTipoDTO?>();
        try
        {
            apiGenericResult.Retorno =  await _verbaTipoRepository.BuscarPorIdAsync(id);
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Tipo Verba");
        }
        return apiGenericResult;
    }
}