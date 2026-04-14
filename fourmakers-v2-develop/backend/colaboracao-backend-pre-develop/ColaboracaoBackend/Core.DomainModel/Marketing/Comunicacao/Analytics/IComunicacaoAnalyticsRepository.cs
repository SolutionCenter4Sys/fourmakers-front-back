using DataTransferObject.Domain.Marketing.Comunicacao.Analytics;
using System.Threading.Tasks;

namespace Core.Domain.Marketing.Comunicacao.Analytics
{
    public interface IComunicacaoAnalyticsRepository
    {
        Task<AnalyticsResumoResponseDTO> ObterResumoAsync(int orgId, AnalyticsResumoRequestDTO request);
        Task<AnalyticsResumoResponseDTO> ObterResumoComunicadosOficiaisAsync(int orgId, AnalyticsResumoRequestDTO request);
        Task<PostsPorComunidadeResponseDTO> ObterPostsPorComunidadeAsync(int orgId, PostsPorComunidadeRequestDTO request);
    }
}
