using BotFourmakers.Domain.Interfaces;
using BotFourmakers.Domain.Interfaces.Chat;
using Colaboracao.Helper.Enum;
using Core.Domain.BotFourmakers;
using Core.Domain.BotFourmakers.Chat;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BotFourmakers.Chat;

namespace BotFourmakers.Domain.Impl.Chat;

public class ChatServices : IChatServices
{
    private readonly IChatRepository _chatRepository;

    public ChatServices(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<ApiGenericResult<List<ChatDTO>>> ListarAsync(string codigoInternoColaborado, int orgId)
    {
        var result = new ApiGenericResult<List<ChatDTO>>();
        try
        {
            result.Retorno = await _chatRepository.ListarAsync(codigoInternoColaborado, orgId);
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, nameof(ListarAsync));
        }
        return result;
    }

    public async Task<ApiGenericResult<ChatDTO>> InserirAsync(string codigoInternoColaborador, int orgId)
    {
        var result = new ApiGenericResult<ChatDTO>();
        try
        {
            result.Retorno = await _chatRepository.InserirAsync(codigoInternoColaborador, orgId);
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, nameof(InserirAsync));
        }
        return result;
    }

    public async Task<ApiGenericResult<ChatDTO>> ObterPorIdAsync(int chatId)
    {
        var result = new ApiGenericResult<ChatDTO>();
        try
        {
            result.Retorno = await _chatRepository.ObterPorIdAsync(chatId);
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ObterPorIdAsync));
        }
        return result;
    }

    public async Task<ApiGenericResult<ChatDTO>> RemoverAsync(int chatId, string codigoInternoColaborador)
    {
        var result = new ApiGenericResult<ChatDTO>();
        try
        {
            var chat = await _chatRepository.ObterPorIdAsync(chatId);
            

            if (chat == null || chat.CodigoInternoColaborador != codigoInternoColaborador)
            {
                throw new ArgumentException("Chat não encontrado ou não pertence ao colaborador");
            }

            await _chatRepository.RemoverAsync(chatId);
            
            result.Retorno = chat;
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ObterPorIdAsync));
        }
        return result;
    }
}