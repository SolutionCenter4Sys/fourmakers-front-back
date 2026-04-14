using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador
{
    public interface IColaboradorModaisIgnoradosRepository
    {
        Task<IEnumerable<ColaboradorModaisIgnoradosDTO>> ListarPorColaboradorOrgAsync(string codigoInternoColaborador, int tbOrgId);
        Task<bool> InserirAsync(string codigoInternoColaborador, int tbOrgId, string tag);
    }
}
