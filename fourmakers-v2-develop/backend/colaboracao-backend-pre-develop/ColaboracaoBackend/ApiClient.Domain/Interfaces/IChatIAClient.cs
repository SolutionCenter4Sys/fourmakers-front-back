using System.Threading.Tasks;
using DataTransferObject.Domain.BotFourmakers;
using DataTransferObject.Domain.BotFourmakers.ChatIA;

namespace ApiClient.Domain.Interfaces;

public interface IChatIAClient
{
    Task<ChatIAQuestionReponseDTO> PostQuestionAsync(string question, int orgId);
    Task<string> PostFeedbackAsync(string question, string response, string query, string queryHabilidades, bool feedback);
}