using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Parametro;

namespace Financeiro.Domain.Interfaces.Reembolso.Parametro;

public interface IParametroReembolsoService
{
    Task<ApiGenericResult<ParametroReembolsoDTO>> EditarAsync(ParametroReembolsoDTO input, int orgId, string codigoInternoColaborador);
    Task<ApiGenericResult<ParametroReembolsoDTO?>> ObterPorOrgIdAsync(int orgId);
}