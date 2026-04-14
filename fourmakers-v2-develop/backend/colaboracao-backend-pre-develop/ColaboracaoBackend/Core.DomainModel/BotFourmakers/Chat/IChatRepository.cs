using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.BotFourmakers.Chat;

namespace Core.Domain.BotFourmakers.Chat;

public interface IChatRepository
{
    Task<List<ChatDTO>> ListarAsync(string codigoInternoColaborador, int orgId);
    Task<ChatDTO> InserirAsync(string codigoInternoColaborador, int orgId);
    Task<ChatDTO> ObterPorIdAsync(int chatId);
    Task RemoverAsync(int chatId);
}