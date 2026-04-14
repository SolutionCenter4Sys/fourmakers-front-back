using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.BotFourmakers.Feedback;
using Dapper;
using DataTransferObject.Domain.BotFourmakers.Feedback;

namespace Colaboracao.Infra.Repositories.BotFourmakers.Feedback;

public class FeedbackAIRepository : IFeedbackAIRepository
{
    private readonly IDBConnection _dapperConnection;
    private const string DEFAULT_SQL = @"
        SELECT
            id AS Id,
            comentario AS Comentario,
            feedback AS Feedback
        FROM tb_chat_bot_feedback tcbf
    ";

    public FeedbackAIRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    public async Task<FeedbackDTO> InserirFeedback(int questaoId, string codigoInternoColaborador, string questao, string resposta, bool feedback, string comentario)
    {
        var connection = _dapperConnection.GetConnection();

        var query = @"
            INSERT INTO tb_chat_bot_feedback(questao_id, questao, resposta, comentario, feedback, codigo_interno_colaborador)
            VALUES (@QuestaoId, @Questao, @Resposta, @Comentario, @Feedback, @CodigoInternoColaborador);
            
            SELECT LAST_INSERT_ID();
        ";

        var parametros = new
        {
            QuestaoId = questaoId,
            Questao = questao,
            Resposta = resposta,
            Comentario = comentario,
            Feedback = feedback,
            CodigoInternoColaborador = codigoInternoColaborador
        };

        var insert = await connection.ExecuteScalarAsync<int>(query, parametros);
        return await BuscarFeedbackPorIdAsync(insert);
    }

    public async Task<FeedbackDTO> BuscarFeedbackPorIdAsync(int feedbackId)
    {
        var connection = _dapperConnection.GetConnection();
        var query = DEFAULT_SQL;

        query += @"
            WHERE tcbf.id = @FeedbackId;
        ";
        var parametros = new
        {
            FeedbackId = feedbackId
        };
        
        var result = await connection.QueryFirstOrDefaultAsync<FeedbackDTO>(query, parametros);
        
        return result;
    }
    
    public async Task<FeedbackDTO> BuscarFeedbackPorQuestaoIdAsync(int questaoId)
    {
        var connection = _dapperConnection.GetConnection();
        var query = DEFAULT_SQL;

        query += @"
            WHERE tcbf.questao_id = @QuestaoId;
        ";
        var parametros = new
        {
            QuestaoId = questaoId
        };
        
        var result = await connection.QueryFirstOrDefaultAsync<FeedbackDTO>(query, parametros);
        
        return result;
    }
}