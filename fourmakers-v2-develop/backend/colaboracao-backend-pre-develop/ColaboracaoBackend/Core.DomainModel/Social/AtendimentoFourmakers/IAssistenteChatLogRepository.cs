using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Core.Domain.Social.AtendimentoFourmakers
{
    public interface IAssistenteChatLogRepository
    {
        Task<string> InserirAsync(AssistenteChatLogInsertInput input, int orgId, IDbTransaction transaction = null);
        Task<bool> AtualizarFeedbackAsync(string id, string codigoInternoColaborador, int orgId, string feedback, IDbTransaction transaction = null);
        Task<AssistenteChatLogResult> ObterPorIdAsync(string id, int orgId);
        Task<bool> AtualizarCuradoriaAsync(string id, string statusCuradoria, string notaCurador, int orgId, IDbTransaction transaction = null);
        Task<IEnumerable<AssistenteChatLogResult>> ListarAsync(int orgId, int limite = 200, int offset = 0, string statusCuradoria = null, string feedback = null);
        Task<FeedbackStatsResult> ObterFeedbackStatsAsync(int orgId);
        Task<MetricasMensalResult> ObterMetricasMensalAsync(int orgId);
    }
}
