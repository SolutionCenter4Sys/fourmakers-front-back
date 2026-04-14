using System.Threading.Tasks;
using DataTransferObject.Domain.BotFourmakers.Feedback;

namespace Core.Domain.BotFourmakers.Feedback;

public interface IFeedbackAIRepository
{
    Task<FeedbackDTO> InserirFeedback(int questaoId, string codigoInternoColaborador, string questao, string resposta, bool feedback, string comentario);
    Task<FeedbackDTO> BuscarFeedbackPorIdAsync(int feedbackId);
    Task<FeedbackDTO> BuscarFeedbackPorQuestaoIdAsync(int questaoId);
}