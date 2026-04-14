using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Analytics;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.Analytics
{
    public interface IComunicacaoAnalyticsService
    {
        Task<ApiGenericResult<AnalyticsResumoResponseDTO>> ObterResumoAsync(int orgId, AnalyticsResumoRequestDTO request);
        Task<ApiGenericResult<AnalyticsResumoResponseDTO>> ObterResumoComunicadosOficiaisAsync(int orgId, AnalyticsResumoRequestDTO request);
        Task<ApiGenericResult<PostsPorComunidadeResponseDTO>> ObterPostsPorComunidadeAsync(int orgId, PostsPorComunidadeRequestDTO request);
    }
}
