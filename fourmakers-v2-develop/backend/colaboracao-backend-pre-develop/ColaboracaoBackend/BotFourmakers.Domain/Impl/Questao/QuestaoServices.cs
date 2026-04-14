using ApiClient.Domain.Interfaces;
using BotFourmakers.Domain.Interfaces;
using BotFourmakers.Domain.Interfaces.Questao;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.Domain.BotFourmakers;
using Core.Domain.BotFourmakers.Chat;
using Core.Domain.BotFourmakers.Questao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BotFourmakers.Chat;
using DataTransferObject.Domain.BotFourmakers.Questao;

namespace BotFourmakers.Domain.Impl.Questao;

public class QuestaoServices : IQuestaoServices
{
    private readonly IQuestaoRepository _questaoRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IDBConnectionUnitOfWork _unitOfWork;
    private readonly IChatIAClient _chatIaClient;

    public QuestaoServices(IQuestaoRepository questaoRepository, IChatRepository chatRepository, IDBConnectionUnitOfWork unitOfWork, IChatIAClient chatIaClient)
    {
        _questaoRepository = questaoRepository;
        _chatRepository = chatRepository;
        _unitOfWork = unitOfWork;
        _chatIaClient = chatIaClient;
    }

    public async Task<ApiGenericResult<List<QuestaoDTO>>> InserirAsync(string questao, int? chatId, string codigoInternoColaborador, int orgId)
    {
        var result = new ApiGenericResult<List<QuestaoDTO>>();
        _unitOfWork.BeginTransaction();
        try
        {
            var listaDeMensagens = new List<QuestaoDTO>();
            int chatIdAux;
            
            if (!chatId.HasValue)
            {
                var novoChat = await _chatRepository.InserirAsync(codigoInternoColaborador, orgId);
                chatIdAux = novoChat.Id;
            }
            else
            {
                chatIdAux = chatId.Value;
                await VerificaSeExisteChat(chatIdAux);
            }
            var novaQuestao = await _questaoRepository.InserirAsync(questao, null,null,(int)TipoUsuarioQuestaoEnum.Usuario, chatIdAux);
            var questaoBot = await _chatIaClient.PostQuestionAsync(questao, orgId);
            
            var novaResposta = await _questaoRepository.InserirAsync(questaoBot.Response, questaoBot.QueryMapaAlocacao, questaoBot.QueryHabilidade, (int)TipoUsuarioQuestaoEnum.Assistente, chatIdAux, novaQuestao.Id);
            
            if (chatId != chatIdAux)
            {
                var chat = new ChatDTO()
                {
                    Id = chatIdAux,
                    Mensagem = questao,
                };
                
                novaQuestao.Chat = chat;
                novaResposta.Chat = chat;
            }
            
            listaDeMensagens.Add(novaQuestao);
            listaDeMensagens.Add(novaResposta);
            
            result.Retorno = listaDeMensagens;
            
            _unitOfWork.Commit();
            return result;
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, nameof(InserirAsync));
        }
        return result;
    }

    public async Task<ApiGenericResult<List<QuestaoDTO>>> ListarAsync(int chatId)
    {
        var result = new ApiGenericResult<List<QuestaoDTO>>();
        try
        {
            await VerificaSeExisteChat(chatId);
            result.Retorno = await _questaoRepository.ListarAsync(chatId);
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarAsync));
        }
        return result;
    }

    private async Task VerificaSeExisteChat(int chatId)
    {
        if (chatId == 0)
        {
            throw new ArgumentException("ChatId Invalido");
        }
        
        var buscarChat = await _chatRepository.ObterPorIdAsync(chatId);
        if (buscarChat == null)
        {
            throw new ArgumentException("Chat não encontrado");
        }
    }
}