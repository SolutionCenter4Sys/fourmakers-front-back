using Colaboracao.Core.Interfaces;
using Core.Domain.Labs;
using Dapper;
using System;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Labs
{
    /// <summary>
    /// Implementação do repositório de log do ScoreSingleCandidate (tb_labs_log_score_single_candidates).
    /// </summary>
    public class LabsLogScoreSingleCandidatesRepository : ILabsLogScoreSingleCandidatesRepository
    {
        private readonly IDBConnection _dapperConnection;

        public LabsLogScoreSingleCandidatesRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<Guid> InserirAsync(int tbOrgId, string? idVaga, string? codigoInternoColaborador, string? objetoRequest, string? objetoResponse)
        {
            var connection = _dapperConnection.GetConnection();
            var id = Guid.NewGuid();

            const string sql = @"INSERT INTO tb_labs_log_score_single_candidates
                                      (id, tb_org_id, data_criacao, id_vaga, codigo_interno_colaborador, objeto_request, objeto_response)
                                  VALUES
                                      (@id, @tbOrgId, NOW(), @idVaga, @codigoInternoColaborador, @objetoRequest, @objetoResponse);";

            await connection.ExecuteAsync(sql, new { id = id.ToString(), tbOrgId, idVaga, codigoInternoColaborador, objetoRequest, objetoResponse });
            return id;
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = "SELECT 1 FROM tb_labs_log_score_single_candidates WHERE id = @id LIMIT 1;";
            var result = await connection.ExecuteScalarAsync<int?>(sql, new { id = id.ToString() });
            return result.HasValue && result.Value == 1;
        }
    }
}
