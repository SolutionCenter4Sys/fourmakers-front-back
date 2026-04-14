using ApiClient.Domain.Interfaces;
using BotFourmakers.Domain.Interfaces.Feedback;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.Domain.BotFourmakers.Feedback;
using Core.Domain.BotFourmakers.Questao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BotFourmakers.Feedback;
using DataTransferObject.Domain.BotFourmakers.Questao;
using Logs.Infra.Attributes;

namespace BotFourmakers.Domain.Impl.Feedback;

[LogDomainClass]
public class FeedbackIAService : IFeedbackIAService
{
    private readonly IFeedbackAIRepository _feedbackAiRepository;
    private readonly IQuestaoRepository _questaoRepository;
    private readonly IChatIAClient _chatIaClient;
    private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

    public FeedbackIAService(IFeedbackAIRepository feedbackAiRepository, IQuestaoRepository questaoRepository, IChatIAClient chatIaClient, IDBConnectionUnitOfWork dbConnectionUnitOfWork)
    {
        _feedbackAiRepository = feedbackAiRepository;
        _questaoRepository = questaoRepository;
        _chatIaClient = chatIaClient;
        _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
    }

    public async Task<ApiGenericResult<FeedbackDTO>> InserirFeedback(NovoFeedbackParam parametros, string codigoInternoColaborador)
    {
        var result = new ApiGenericResult<FeedbackDTO>();
        _dbConnectionUnitOfWork.BeginTransaction();
        try
        {
            var buscaResposta = await _questaoRepository.ObterPorIdAsync(parametros.QuestaoId);
            if (buscaResposta == null)
            {
                throw new ArgumentException("Resposta não encontrada");
            }

            if (buscaResposta.Tipo == (int)TipoUsuarioQuestaoEnum.Usuario)
            {
                throw new ArgumentException("Nao é possível realizar feedback para resposta de usuario.");
            }

            if (buscaResposta.ReferenciaRespostaId == null)
            {
                throw new ArgumentException("Nao é possível realizar feedback para perguntas sem resposta.");
            }

            var jaExisteFeedback = await _feedbackAiRepository.BuscarFeedbackPorQuestaoIdAsync(buscaResposta.Id);

            if (jaExisteFeedback != null)
            {
                throw new ArgumentException("Já existe um feedback para essa resposta");
            }
            
            var buscaQuestaoReferencia = await _questaoRepository.ObterPorIdAsync(buscaResposta.ReferenciaRespostaId.Value);
            
            result.Retorno = await _feedbackAiRepository.InserirFeedback(parametros.QuestaoId, codigoInternoColaborador,
                buscaQuestaoReferencia.Mensagem, buscaResposta.Mensagem, parametros.Feedback, parametros.Comentario);
            await _chatIaClient.PostFeedbackAsync(buscaQuestaoReferencia.Mensagem, buscaResposta.Mensagem, buscaResposta.Query, buscaResposta.QueryHabilidades, parametros.Feedback);
            _dbConnectionUnitOfWork.Commit();
        }
        catch (Exception ex)
        {
            _dbConnectionUnitOfWork.Rollback();
           ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, "Feedback");
        }

        return result;
    }

    public async Task<ApiGenericResult<FeedbackDTO>> BuscarFeedbackPorIdAsync(int feedbackId)
    {
        var result = new ApiGenericResult<FeedbackDTO>();
        try
        {
            result.Retorno = await _feedbackAiRepository.BuscarFeedbackPorIdAsync(feedbackId);
        }
        catch (Exception ex)
        {
            ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Feedback");
        }

        return result;
    }
}