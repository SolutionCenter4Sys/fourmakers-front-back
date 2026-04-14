using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BotFourmakers.Chat;

namespace BotFourmakers.Domain.Interfaces.Chat;

public interface IChatServices
{
    Task<ApiGenericResult<List<ChatDTO>>> ListarAsync(string codigoInternoColaborado, int orgId);
    Task<ApiGenericResult<ChatDTO>> InserirAsync(string codigoInternoColaborador, int orgId);
    Task<ApiGenericResult<ChatDTO>> ObterPorIdAsync(int chatId);
    Task<ApiGenericResult<ChatDTO>> RemoverAsync(int chatId, string codigoInternoColaborador);
}