using Competencia.Domain.Enums;
using DataTransferObject.Domain.Competencia;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services
{
    public interface IGestaoDeCompetenciaService
    {
        Task<AdicionarCompetenciaDTO> AlterarCategoriaDaHabilidade(int id, TipoCompetenciaSRSEnum atual, TipoCompetenciaSRSEnum destino, string cpf, string tokenUsuario, int orgId);
    }
}