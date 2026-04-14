using DataTransferObject.Domain.Labs.MatchSemantico;
using System.Threading.Tasks;

namespace Labs.Domain.Interfaces
{
    /// <summary>
    /// Serviço de domínio para Match Semântico (best candidates hyde).
    /// </summary>
    public interface IMatchSemanticoService
    {
        Task<MatchSemanticoHydeResult> BuscarMelhoresCandidatosAsync(MatchSemanticoHydeRequest request, int orgId, string? codigoInternoColaborador);
    }
}
