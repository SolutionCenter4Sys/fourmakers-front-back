using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.BotFourmakers.Questao;

namespace Core.Domain.BotFourmakers.Questao;

public interface IQuestaoRepository
{ 
    Task<QuestaoDTO> InserirAsync(string mensagem, string? botQuery, string? QueryHabilidades, int tipo, int chatId, int? questaoId = null);
    Task<QuestaoDTO> ObterPorIdAsync(int questaoId);
    Task<List<QuestaoDTO>> ListarAsync(int chatId);
}