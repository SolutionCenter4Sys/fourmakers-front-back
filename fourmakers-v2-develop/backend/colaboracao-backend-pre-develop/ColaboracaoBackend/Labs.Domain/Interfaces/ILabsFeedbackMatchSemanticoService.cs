using DataTransferObject.Domain.Labs;
using System.Threading.Tasks;

namespace Labs.Domain.Interfaces
{
    /// <summary>
    /// Serviço para registrar feedback do usuário sobre qual resultado de match preferiu.
    /// </summary>
    public interface ILabsFeedbackMatchSemanticoService
    {
        Task RegistrarFeedbackAsync(FeedbackMatchSemanticoRequest request, int orgId, string? codigoInternoColaborador);

        /// <summary>
        /// Agrega logs de geração e feedbacks (RankCandidatesIds vs Match Semântico) para a organização.
        /// </summary>
        Task<MatchSemanticoMetricasDto> ObterMetricasAsync(int orgId);
    }
}
