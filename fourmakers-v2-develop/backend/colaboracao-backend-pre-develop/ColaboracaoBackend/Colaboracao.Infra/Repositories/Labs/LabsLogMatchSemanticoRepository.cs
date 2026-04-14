using Colaboracao.Core.Interfaces;
using Core.Domain.Labs;
using Dapper;
using System;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Labs
{
    /// <summary>
    /// Implementação do repositório de log do Match Semântico (best_candidates/hyde).
    /// </summary>
    public class LabsLogMatchSemanticoRepository : ILabsLogMatchSemanticoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public LabsLogMatchSemanticoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<Guid> InserirAsync(int tbOrgId, string? codigoInternoColaborador, string? objetoRequest, string? objetoResponse)
        {
            var connection = _dapperConnection.GetConnection();
            var id = Guid.NewGuid();

            const string sql = @"INSERT INTO tb_labs_log_match_semantico
                                      (id, tb_org_id, data_criacao, codigo_interno_colaborador, objeto_request, objeto_response)
                                  VALUES
                                      (@id, @tbOrgId, NOW(), @codigoInternoColaborador, @objetoRequest, @objetoResponse);";

            await connection.ExecuteAsync(sql, new { id = id.ToString(), tbOrgId, codigoInternoColaborador, objetoRequest, objetoResponse });
            return id;
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = "SELECT 1 FROM tb_labs_log_match_semantico WHERE id = @id LIMIT 1;";
            var result = await connection.ExecuteScalarAsync<int?>(sql, new { id = id.ToString() });
            return result.HasValue && result.Value == 1;
        }

        public async Task<int> ContarPorOrgAsync(int tbOrgId)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = "SELECT COUNT(*) FROM tb_labs_log_match_semantico WHERE tb_org_id = @tbOrgId;";
            var n = await connection.ExecuteScalarAsync<long>(sql, new { tbOrgId });
            return (int)n;
        }
    }
}
