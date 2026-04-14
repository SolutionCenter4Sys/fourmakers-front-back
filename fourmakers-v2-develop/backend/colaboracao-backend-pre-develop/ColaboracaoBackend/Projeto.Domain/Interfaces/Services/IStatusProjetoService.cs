using DataTransferObject.Domain.Projeto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IStatusProjetoService
    {
        Task<List<StatusProjetosDTO>> ListarStatus(int orgId);
    }
}