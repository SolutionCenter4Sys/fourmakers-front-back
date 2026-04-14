using DataTransferObject.Domain.Diretoria;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IColaboradorDepartamentoService
    {
        Task<List<DepartamentoColaboradorDTO>> ListarDepartamentosDosColaboradoresAsync(int orgId, string codigoDiretoria, string cpfRequest);
    }
}