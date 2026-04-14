using Colaboracao.Core.Interfaces;
using Core.Domain.Social.AtendimentoFourmakers;
using Dapper;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social.AtendimentoFourmakers
{
    public class AssistenteChatLogRepository : IAssistenteChatLogRepository
    {
        private readonly IDBConnection _dapperConnection;

        public AssistenteChatLogRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<string> InserirAsync(AssistenteChatLogInsertInput input, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                INSERT INTO tb_assistente_chat_log
                    (id, tb_org_id, codigo_interno_colaborador, mensagem_usuario, mensagem_assistente,
                     fontes, qtd_chunks, similaridade_max, exibir_chamado)
                VALUES
                    (@NewId, @OrgId, @CodigoInternoColaborador, @MensagemUsuario, @MensagemAssistente,
                     @Fontes, @QtdChunks, @SimilaridadeMax, @ExibirChamado)";

            var newId = System.Guid.NewGuid().ToString();
            var codColab = string.IsNullOrWhiteSpace(input.CodigoInternoColaborador) ? null : input.CodigoInternoColaborador;
            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, new
            {
                NewId = newId,
                OrgId = orgId,
                CodigoInternoColaborador = codColab,
                input.MensagemUsuario,
                input.MensagemAssistente,
                input.Fontes,
                input.QtdChunks,
                input.SimilaridadeMax,
                input.ExibirChamado
            }, transaction);
            return newId;
        }

        public async Task<bool> AtualizarFeedbackAsync(string id, string codigoInternoColaborador, int orgId, string feedback, IDbTransaction transaction = null)
        {
            const string sql = @"
                UPDATE tb_assistente_chat_log
                SET feedback = @Feedback, feedback_em = NOW()
                WHERE id = @Id
                  AND codigo_interno_colaborador = @CodigoInternoColaborador
                  AND tb_org_id = @OrgId
                  AND feedback IS NULL";

            var connection = _dapperConnection.GetConnection();
            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                CodigoInternoColaborador = codigoInternoColaborador,
                OrgId = orgId,
                Feedback = feedback
            }, transaction);
            return rows > 0;
        }

        public async Task<AssistenteChatLogResult> ObterPorIdAsync(string id, int orgId)
        {
            const string sql = @"
                SELECT
                    CAST(id AS CHAR(36)) AS Id,
                    tb_org_id AS TbOrgId,
                    CAST(codigo_interno_colaborador AS CHAR(36)) AS CodigoInternoColaborador,
                    mensagem_usuario AS MensagemUsuario,
                    mensagem_assistente AS MensagemAssistente,
                    fontes AS Fontes,
                    qtd_chunks AS QtdChunks,
                    similaridade_max AS SimilaridadeMax,
                    exibir_chamado AS ExibirChamado,
                    feedback AS Feedback,
                    feedback_em AS FeedbackEm,
                    status_curadoria AS StatusCuradoria,
                    nota_curador AS NotaCurador,
                    data_criacao AS DataCriacao
                FROM tb_assistente_chat_log
                WHERE id = @Id AND tb_org_id = @OrgId";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<AssistenteChatLogResult>(sql, new { Id = id, OrgId = orgId });
        }

        public async Task<bool> AtualizarCuradoriaAsync(string id, string statusCuradoria, string notaCurador, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                UPDATE tb_assistente_chat_log
                SET status_curadoria = @StatusCuradoria, nota_curador = @NotaCurador
                WHERE id = @Id AND tb_org_id = @OrgId";

            var connection = _dapperConnection.GetConnection();
            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                StatusCuradoria = statusCuradoria,
                NotaCurador = notaCurador,
                OrgId = orgId
            }, transaction);
            return rows > 0;
        }

        public async Task<IEnumerable<AssistenteChatLogResult>> ListarAsync(int orgId, int limite = 200, int offset = 0, string statusCuradoria = null, string feedback = null)
        {
            var sql = @"
                SELECT
                    CAST(id AS CHAR(36)) AS Id,
                    tb_org_id AS TbOrgId,
                    CAST(codigo_interno_colaborador AS CHAR(36)) AS CodigoInternoColaborador,
                    mensagem_usuario AS MensagemUsuario,
                    mensagem_assistente AS MensagemAssistente,
                    fontes AS Fontes,
                    qtd_chunks AS QtdChunks,
                    similaridade_max AS SimilaridadeMax,
                    exibir_chamado AS ExibirChamado,
                    feedback AS Feedback,
                    feedback_em AS FeedbackEm,
                    status_curadoria AS StatusCuradoria,
                    nota_curador AS NotaCurador,
                    data_criacao AS DataCriacao
                FROM tb_assistente_chat_log
                WHERE tb_org_id = @OrgId";

            if (!string.IsNullOrEmpty(statusCuradoria))
                sql += " AND status_curadoria = @StatusCuradoria";

            if (!string.IsNullOrEmpty(feedback))
                sql += " AND feedback = @Feedback";

            sql += " ORDER BY data_criacao DESC LIMIT @Limite OFFSET @Offset";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<AssistenteChatLogResult>(sql, new
            {
                OrgId = orgId,
                Limite = limite,
                Offset = offset,
                StatusCuradoria = statusCuradoria,
                Feedback = feedback
            });
        }

        public async Task<FeedbackStatsResult> ObterFeedbackStatsAsync(int orgId)
        {
            const string sql = @"
                SELECT
                    COUNT(1) AS TotalLogs,
                    SUM(CASE WHEN feedback = 'up' THEN 1 ELSE 0 END) AS ThumbsUp,
                    SUM(CASE WHEN feedback = 'down' THEN 1 ELSE 0 END) AS ThumbsDown
                FROM tb_assistente_chat_log
                WHERE tb_org_id = @OrgId";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<FeedbackStatsResult>(sql, new { OrgId = orgId });
        }

        public async Task<MetricasMensalResult> ObterMetricasMensalAsync(int orgId)
        {
            const string sql = @"
                SELECT
                    COUNT(1) AS PerguntasMes,
                    COUNT(DISTINCT codigo_interno_colaborador) AS UsuariosUnicosMes,
                    AVG(similaridade_max) AS SimilaridadeMedia,
                    SUM(CASE WHEN exibir_chamado = 1 THEN 1 ELSE 0 END) AS ChamadosSugeridos
                FROM tb_assistente_chat_log
                WHERE tb_org_id = @OrgId
                  AND data_criacao >= DATE_FORMAT(NOW(), '%Y-%m-01')";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<MetricasMensalResult>(sql, new { OrgId = orgId });
        }
    }
}
