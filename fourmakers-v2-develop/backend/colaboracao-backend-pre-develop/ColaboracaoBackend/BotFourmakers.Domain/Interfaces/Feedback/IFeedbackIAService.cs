using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BotFourmakers.Feedback;

namespace BotFourmakers.Domain.Interfaces.Feedback;

public interface IFeedbackIAService
{
    Task<ApiGenericResult<FeedbackDTO>> BuscarFeedbackPorIdAsync(int feedbackId);
    Task<ApiGenericResult<FeedbackDTO>> InserirFeedback(NovoFeedbackParam parametros, string codigoInternoColaborador);
}