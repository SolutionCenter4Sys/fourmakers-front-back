using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BotFourmakers.Questao;

namespace BotFourmakers.Domain.Interfaces.Questao;

public interface IQuestaoServices
{
    Task<ApiGenericResult<List<QuestaoDTO>>> InserirAsync(string questao, int? chatId, string codigoInternoColaborador, int orgId);
    Task<ApiGenericResult<List<QuestaoDTO>>> ListarAsync(int chatId);
}