using DataTransferObject.Domain.Projeto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Projeto
{
    public interface IStatusProjetoRepository
    {
        Task<List<StatusProjetosDTO>> ListarStatusProjeto(int orgId);
    }
}