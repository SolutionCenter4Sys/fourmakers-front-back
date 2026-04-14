using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Core.Domain.Reembolso.Verba;

public interface IVerbaTipoRepository
{
    Task<List<VerbaTipoDTO>> ListarAsync(int orgId);
    Task<VerbaTipoDTO?> BuscarPorIdAsync(int id);
}