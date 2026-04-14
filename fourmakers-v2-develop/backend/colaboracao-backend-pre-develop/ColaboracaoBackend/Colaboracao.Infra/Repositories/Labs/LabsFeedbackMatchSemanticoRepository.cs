using Colaboracao.Core.Interfaces;
using Core.Domain.Labs;
using Dapper;
using System;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Labs
{
    /// <summary>
    /// Implementação do repositório de feedback Match Semântico (tb_labs_feedback_match_semantico).
    /// </summary>
    public class LabsFeedbackMatchSemanticoRepository : ILabsFeedbackMatchSemanticoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public LabsFeedbackMatchSemanticoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task InserirAsync(Guid id, Guid idLogRankCandidatesIds, Guid idLogMatchSemantico, bool matchSemanticoMelhor, int tbOrgId, string? codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            const string sql = @"INSERT INTO tb_labs_feedback_match_semantico
                                      (id, id_log_rank_candidates_ids, id_log_match_semantico, match_semantico_melhor, tb_org_id, codigo_interno_colaborador, data_criacao)
                                  VALUES
                                      (@id, @idLogRankCandidatesIds, @idLogMatchSemantico, @matchSemanticoMelhor, @tbOrgId, @codigoInternoColaborador, NOW());";

            await connection.ExecuteAsync(sql, new
            {
                id = id.ToString(),
                idLogRankCandidatesIds = idLogRankCandidatesIds.ToString(),
                idLogMatchSemantico = idLogMatchSemantico.ToString(),
                matchSemanticoMelhor = matchSemanticoMelhor ? 1 : 0,
                tbOrgId,
                codigoInternoColaborador
            });
        }

        public async Task<(int PreferiuMetodoAntigo, int PreferiuMetodoNovo)> ObterContagensPreferenciaPorOrgAsync(int tbOrgId)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = @"
                SELECT
                    COALESCE(SUM(CASE WHEN match_semantico_melhor = 0 THEN 1 ELSE 0 END), 0) AS PreferiuMetodoAntigo,
                    COALESCE(SUM(CASE WHEN match_semantico_melhor = 1 THEN 1 ELSE 0 END), 0) AS PreferiuMetodoNovo
                FROM tb_labs_feedback_match_semantico
                WHERE tb_org_id = @tbOrgId;";

            var row = await connection.QuerySingleAsync<FeedbackPreferenciaRow>(sql, new { tbOrgId });
            return ((int)row.PreferiuMetodoAntigo, (int)row.PreferiuMetodoNovo);
        }

        private sealed class FeedbackPreferenciaRow
        {
            public long PreferiuMetodoAntigo { get; set; }
            public long PreferiuMetodoNovo { get; set; }
        }
    }
}
