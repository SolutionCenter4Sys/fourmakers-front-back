using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Financeiro.Domain.Interfaces.Reembolso.Verba;

public interface IVerbaTipoService
{
    Task<ApiGenericResult<List<VerbaTipoDTO>>> ListarAsync(int orgId);
    Task<ApiGenericResult<VerbaTipoDTO?>> BuscarPorIdAsync(int id);
}