using DataTransferObject.Domain.Labs.MatchSemantico;
using System.Threading.Tasks;

namespace Labs.Domain.Interfaces
{
    /// <summary>
    /// Cliente para a API Match Semântico (GCP).
    /// </summary>
    public interface IMatchSemanticoClient
    {
        Task<MatchSemanticoHydeResult> BuscarMelhoresCandidatosAsync(MatchSemanticoHydeRequest request);
    }
}
