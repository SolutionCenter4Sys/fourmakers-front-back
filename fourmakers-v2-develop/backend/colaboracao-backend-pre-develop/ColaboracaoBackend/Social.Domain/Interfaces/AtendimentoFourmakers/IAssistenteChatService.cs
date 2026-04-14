using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces.AtendimentoFourmakers
{
    public interface IAssistenteChatService
    {
        Task<ApiGenericResult<ConversarResult>> ConversarAsync(ConversarInput input, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult<FeedbackChatResult>> RegistrarFeedbackAsync(FeedbackChatInput input, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult<List<AssistenteChatLogResult>>> ListarLogsAsync(FiltroLogsInput filtro, int orgId);
        Task<ApiGenericResult<FeedbackStatsResult>> ObterFeedbackStatsAsync(int orgId);
        Task<ApiGenericResult<MetricasMensalResult>> ObterMetricasMensalAsync(int orgId);
    }
}
