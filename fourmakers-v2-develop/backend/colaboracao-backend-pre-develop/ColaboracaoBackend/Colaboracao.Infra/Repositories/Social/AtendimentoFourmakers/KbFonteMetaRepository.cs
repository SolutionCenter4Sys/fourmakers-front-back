using Colaboracao.Core.Interfaces;
using Core.Domain.Social.AtendimentoFourmakers;
using Dapper;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social.AtendimentoFourmakers
{
    public class KbFonteMetaRepository : IKbFonteMetaRepository
    {
        private readonly IDBConnection _dapperConnection;

        public KbFonteMetaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task UpsertAsync(string fonteId, string areaId, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                INSERT INTO tb_kb_fonte_meta (fonte_id, tb_org_id, tb_material_area_id)
                VALUES (@FonteId, @OrgId, @AreaId)
                ON DUPLICATE KEY UPDATE
                    tb_material_area_id = @AreaId";

            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, new { OrgId = orgId, FonteId = fonteId, AreaId = areaId }, transaction);
        }

        public async Task DeletarAsync(string fonteId, int orgId, IDbTransaction transaction = null)
        {
            const string sql = "DELETE FROM tb_kb_fonte_meta WHERE fonte_id = @FonteId AND tb_org_id = @OrgId";
            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, new { FonteId = fonteId, OrgId = orgId }, transaction);
        }

        public async Task<int> ContarPorAreaAsync(string areaId, int orgId)
        {
            const string sql = @"
                SELECT COUNT(1) FROM tb_kb_fonte_meta
                WHERE tb_material_area_id = @AreaId AND tb_org_id = @OrgId";

            var connection = _dapperConnection.GetConnection();
            return await connection.ExecuteScalarAsync<int>(sql, new { AreaId = areaId, OrgId = orgId });
        }

        public async Task<int> ContarFontesAsync(int orgId)
        {
            const string sql = "SELECT COUNT(DISTINCT fonte_id) FROM tb_kb_chunk WHERE tb_org_id = @OrgId";
            var connection = _dapperConnection.GetConnection();
            return await connection.ExecuteScalarAsync<int>(sql, new { OrgId = orgId });
        }

        public async Task<IEnumerable<KbFonteResult>> ListarAsync(int orgId)
        {
            const string sql = @"
                SELECT
                    c.fonte_id AS FonteId,
                    MAX(c.titulo) AS Titulo,
                    MAX(c.tipo_fonte) AS TipoFonte,
                    COUNT(c.id) AS ChunkCount,
                    CAST(MAX(fm.tb_material_area_id) AS CHAR(36)) AS AreaId,
                    MAX(ma.nome) AS AreaNome,
                    MIN(c.data_criacao) AS DataCriacao,
                    MAX(c.data_criacao) AS DataAlteracao
                FROM tb_kb_chunk c
                LEFT JOIN tb_kb_fonte_meta fm ON fm.fonte_id = c.fonte_id AND fm.tb_org_id = c.tb_org_id
                LEFT JOIN tb_material_area ma ON ma.id = fm.tb_material_area_id AND ma.tb_org_id = c.tb_org_id
                WHERE c.tb_org_id = @OrgId AND c.fonte_id IS NOT NULL
                GROUP BY c.fonte_id
                ORDER BY DataAlteracao DESC";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<KbFonteResult>(sql, new { OrgId = orgId });
        }

        public async Task<KbFonteResult> ObterPorFonteIdAsync(string fonteId, int orgId)
        {
            const string sql = @"
                SELECT
                    c.fonte_id AS FonteId,
                    MAX(c.titulo) AS Titulo,
                    MAX(c.tipo_fonte) AS TipoFonte,
                    COUNT(c.id) AS ChunkCount,
                    CAST(MAX(fm.tb_material_area_id) AS CHAR(36)) AS AreaId,
                    MAX(ma.nome) AS AreaNome,
                    MIN(c.data_criacao) AS DataCriacao,
                    MAX(c.data_criacao) AS DataAlteracao
                FROM tb_kb_chunk c
                LEFT JOIN tb_kb_fonte_meta fm ON fm.fonte_id = c.fonte_id AND fm.tb_org_id = c.tb_org_id
                LEFT JOIN tb_material_area ma ON ma.id = fm.tb_material_area_id AND ma.tb_org_id = c.tb_org_id
                WHERE c.tb_org_id = @OrgId AND c.fonte_id = @FonteId
                GROUP BY c.fonte_id";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<KbFonteResult>(sql, new { FonteId = fonteId, OrgId = orgId });
        }
    }
}
