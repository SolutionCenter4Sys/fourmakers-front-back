using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.BotFourmakers.Questao;
using Dapper;
using DataTransferObject.Domain.BotFourmakers.Feedback;
using DataTransferObject.Domain.BotFourmakers.Questao;

namespace Colaboracao.Infra.Repositories.BotFourmakers.Questao;

public class QuestaoRepository : IQuestaoRepository
{
    private readonly IDBConnection _dapperConnection;

    private const string SELECT_DEFAULT = @"
        SELECT 
            tqcb.id AS Id,
            tqcb.chat_id AS ChatId,
            tqcb.mensagem AS Mensagem,
            tqcb.query AS Query,
            tqcb.data_criacao AS CriadoEm,
            tqcb.tipo AS Tipo,
            tqcb.referencia_resposta_id AS ReferenciaRespostaId,
            tqcb.query_habilidades AS QueryHabilidades,
            tcbf.id AS FeedbackId,
            tcbf.comentario AS Comentario,
            tcbf.feedback AS Feedback
        FROM tb_questao_chat_bot tqcb
        LEFT JOIN tb_chat_bot_feedback tcbf ON tcbf.questao_id = tqcb.id
    ";

    public QuestaoRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    public async Task<QuestaoDTO> InserirAsync(string mensagem, string? botQuery, string? queryHabilidades, int tipo, int chatId, int? questaoId = null)
    {
        var connection = _dapperConnection.GetConnection();

        var query = @"
            INSERT INTO tb_questao_chat_bot(mensagem, query, query_habilidades, tipo, chat_id, referencia_resposta_id)
            VALUES (@Mensagem, @Query, @QueryHabilidades, @Tipo, @ChatId, @ReferenciaRespostaId);
            
            SELECT LAST_INSERT_ID();
        ";

        var parametros = new
        {
            Mensagem = mensagem,
            Query = botQuery,
            ChatId = chatId,
            QueryHabilidades = queryHabilidades,
            Tipo = tipo,
            ReferenciaRespostaId = questaoId
        };

        var insert = await connection.ExecuteScalarAsync<int>(query, parametros);
        return await ObterPorIdAsync(insert);
    }

    public async Task<QuestaoDTO> ObterPorIdAsync(int questaoId)
    {
        var connection = _dapperConnection.GetConnection();

        var query = SELECT_DEFAULT;
        query += " WHERE tqcb.id = @QuestaoId;";

        var parametros = new
        {
            QuestaoId = questaoId,
        };

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(query, parametros);

        if (result == null) return null;

        var questao = new QuestaoDTO
        {
            Id = result.Id,
            ChatId = result.ChatId,
            Mensagem = result.Mensagem,
            Query = result.Query,
            CriadoEm = result.CriadoEm,
            Tipo = result.Tipo,
            ReferenciaRespostaId = result.ReferenciaRespostaId,
            Feedback = result.FeedbackId != null ? new FeedbackDTO
            {
                Id = result.FeedbackId,
                Comentario = result.Comentario,
                Feedback = result.Feedback
            } : null
        };

        return questao;
    }
    public async Task<List<QuestaoDTO>> ListarAsync(int chatId)
    {
        var connection = _dapperConnection.GetConnection();

        var query = SELECT_DEFAULT;
        query += " WHERE tqcb.chat_id = @ChatId;";

        var parametros = new
        {
            ChatId = chatId,
        };

        var result = await connection.QueryAsync<dynamic>(query, parametros);

        var lista = result.GroupBy(x => new
        {
            x.Id,
            x.ChatId,
            x.Mensagem,
            x.Query,
            x.CriadoEm,
            x.FeedbackId,
            x.Feedback,
            x.Comentario,
            x.Tipo,
            x.ReferenciaRespostaId
        }).Select(group => new QuestaoDTO()
        {
            Id = group.Key.Id,
            ChatId = group.Key.ChatId,
            Mensagem = group.Key.Mensagem,
            Query = group.Key.Query,
            CriadoEm = group.Key.CriadoEm,
            Tipo = group.Key.Tipo,
            ReferenciaRespostaId = group.Key.ReferenciaRespostaId,
            Feedback = group.Key.FeedbackId != null ? new FeedbackDTO()
            {
                Id = group.Key.FeedbackId,
                Comentario = group.Key.Comentario,
                Feedback = group.Key.Feedback
            }: null
        }).ToList();
        
        return lista;
    }
}