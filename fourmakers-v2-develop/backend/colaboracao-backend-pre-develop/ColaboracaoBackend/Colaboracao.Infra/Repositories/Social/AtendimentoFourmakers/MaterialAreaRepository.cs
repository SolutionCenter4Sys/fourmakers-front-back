using Colaboracao.Core.Interfaces;
using Core.Domain.Social.AtendimentoFourmakers;
using Dapper;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social.AtendimentoFourmakers
{
    public class MaterialAreaRepository : IMaterialAreaRepository
    {
        private readonly IDBConnection _dapperConnection;

        private const string SqlSelect = @"
            SELECT
                CAST(id AS CHAR(36)) AS Id,
                tb_org_id AS TbOrgId,
                nome AS Nome,
                slug AS Slug,
                ordem AS Ordem,
                data_criacao AS DataCriacao,
                data_alteracao AS DataAlteracao
            FROM tb_material_area";

        public MaterialAreaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<MaterialAreaResult>> ListarAsync(int orgId)
        {
            var sql = SqlSelect + " WHERE tb_org_id = @OrgId ORDER BY ordem ASC, nome ASC";
            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<MaterialAreaResult>(sql, new { OrgId = orgId });
        }

        public async Task<MaterialAreaResult> ObterPorIdAsync(string id, int orgId)
        {
            var sql = SqlSelect + " WHERE id = @Id AND tb_org_id = @OrgId";
            var connection = _dapperConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<MaterialAreaResult>(sql, new { Id = id, OrgId = orgId });
        }

        public async Task<string> InserirAsync(MaterialAreaInsertInput input, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                INSERT INTO tb_material_area (id, tb_org_id, nome, slug, ordem)
                VALUES (@NewId, @OrgId, @Nome, @Slug, @Ordem)";

            var newId = System.Guid.NewGuid().ToString();
            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, new
            {
                NewId = newId,
                OrgId = orgId,
                input.Nome,
                input.Slug,
                input.Ordem
            }, transaction);
            return newId;
        }

        public async Task<bool> AtualizarAsync(string id, MaterialAreaUpdateInput input, int orgId, IDbTransaction transaction = null)
        {
            var setClauses = new List<string>();
            if (input.Nome != null) setClauses.Add("nome = @Nome");
            if (input.Slug != null) setClauses.Add("slug = @Slug");
            if (input.Ordem.HasValue) setClauses.Add("ordem = @Ordem");

            if (setClauses.Count == 0) return false;

            var sql = $"UPDATE tb_material_area SET {string.Join(", ", setClauses)} WHERE id = @Id AND tb_org_id = @OrgId";

            var connection = _dapperConnection.GetConnection();
            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                OrgId = orgId,
                input.Nome,
                input.Slug,
                input.Ordem
            }, transaction);
            return rows > 0;
        }

        public async Task<bool> DeletarAsync(string id, int orgId, IDbTransaction transaction = null)
        {
            const string sql = "DELETE FROM tb_material_area WHERE id = @Id AND tb_org_id = @OrgId";
            var connection = _dapperConnection.GetConnection();
            var rows = await connection.ExecuteAsync(sql, new { Id = id, OrgId = orgId }, transaction);
            return rows > 0;
        }

        public async Task<bool> SlugExisteAsync(string slug, int orgId, string ignorarId = null)
        {
            var sql = "SELECT COUNT(1) FROM tb_material_area WHERE slug = @Slug AND tb_org_id = @OrgId";
            if (!string.IsNullOrEmpty(ignorarId))
                sql += " AND id != @IgnorarId";

            var connection = _dapperConnection.GetConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Slug = slug, OrgId = orgId, IgnorarId = ignorarId });
            return count > 0;
        }
    }
}
