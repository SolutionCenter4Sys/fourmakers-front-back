using Colaboracao.Core.Interfaces;
using Core.Domain.Social.AtendimentoFourmakers;
using Dapper;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social.AtendimentoFourmakers
{
    public class ChamadoRepository : IChamadoRepository
    {
        private readonly IDBConnection _dapperConnection;

        private const string SqlSelect = @"
            SELECT
                CAST(c.id AS CHAR(36)) AS Id,
                c.tb_org_id AS TbOrgId,
                CAST(c.codigo_interno_colaborador AS CHAR(36)) AS CodigoInternoColaborador,
                c.cooperado_label AS CooperadoLabel,
                c.assunto AS Assunto,
                c.descricao AS Descricao,
                c.status AS Status,
                c.prioridade AS Prioridade,
                c.notas_resolucao AS NotasResolucao,
                c.resposta_publica AS RespostaPublica,
                c.data_criacao AS DataCriacao,
                c.data_alteracao AS DataAlteracao
            FROM tb_chamado c";

        public ChamadoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<ChamadoResult> ObterPorIdAsync(string id, int orgId)
        {
            var sql = SqlSelect + " WHERE c.id = @Id AND c.tb_org_id = @OrgId";
            var connection = _dapperConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<ChamadoResult>(sql, new { Id = id, OrgId = orgId });
        }

        public async Task<IEnumerable<ChamadoResult>> ListarAsync(int orgId, string codigoInternoColaborador = null,
            string status = null, int? limite = null, int? offset = null)
        {
            var sql = SqlSelect + " WHERE c.tb_org_id = @OrgId";

            if (!string.IsNullOrEmpty(codigoInternoColaborador))
                sql += " AND c.codigo_interno_colaborador = @CodigoInternoColaborador";

            if (!string.IsNullOrEmpty(status))
                sql += " AND c.status = @Status";

            sql += " ORDER BY c.data_criacao DESC";

            if (limite.HasValue)
                sql += " LIMIT @Limite";

            if (offset.HasValue)
                sql += " OFFSET @Offset";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<ChamadoResult>(sql, new
            {
                OrgId = orgId,
                CodigoInternoColaborador = codigoInternoColaborador,
                Status = status,
                Limite = limite ?? 50,
                Offset = offset ?? 0
            });
        }

        public async Task<string> InserirAsync(ChamadoInsertInput input, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                INSERT INTO tb_chamado (id, tb_org_id, codigo_interno_colaborador, cooperado_label, assunto, descricao, status, prioridade)
                VALUES (@NewId, @OrgId, @CodigoInternoColaborador, @CooperadoLabel, @Assunto, @Descricao, @Status, @Prioridade)";

            var newId = System.Guid.NewGuid().ToString();
            var codColab = string.IsNullOrWhiteSpace(input.CodigoInternoColaborador) ? null : input.CodigoInternoColaborador;
            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, new
            {
                NewId = newId,
                OrgId = orgId,
                CodigoInternoColaborador = codColab,
                input.CooperadoLabel,
                input.Assunto,
                input.Descricao,
                input.Status,
                input.Prioridade
            }, transaction);
            return newId;
        }

        public async Task<bool> AtualizarAsync(string id, ChamadoUpdateInput input, int orgId, IDbTransaction transaction = null)
        {
            var setClauses = new List<string>();
            if (input.Status != null) setClauses.Add("status = @Status");
            if (input.Prioridade != null) setClauses.Add("prioridade = @Prioridade");
            if (input.NotasResolucao != null) setClauses.Add("notas_resolucao = @NotasResolucao");
            if (input.RespostaPublica != null) setClauses.Add("resposta_publica = @RespostaPublica");

            if (setClauses.Count == 0) return false;

            var sql = $"UPDATE tb_chamado SET {string.Join(", ", setClauses)} WHERE id = @Id AND tb_org_id = @OrgId";

            var connection = _dapperConnection.GetConnection();
            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                OrgId = orgId,
                input.Status,
                input.Prioridade,
                input.NotasResolucao,
                input.RespostaPublica
            }, transaction);
            return rows > 0;
        }

        public async Task<IEnumerable<DistribuicaoStatusItem>> ObterDistribuicaoStatusAsync(int orgId)
        {
            const string sql = @"
                SELECT
                    status AS Nome,
                    COUNT(1) AS Valor,
                    CASE status
                        WHEN 'aberto' THEN '#3b82f6'
                        WHEN 'em_andamento' THEN '#f59e0b'
                        WHEN 'aguardando' THEN '#8b5cf6'
                        WHEN 'resolvido' THEN '#22c55e'
                        WHEN 'cancelado' THEN '#ef4444'
                        ELSE '#6b7280'
                    END AS Cor
                FROM tb_chamado
                WHERE tb_org_id = @OrgId
                GROUP BY status
                ORDER BY Valor DESC";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<DistribuicaoStatusItem>(sql, new { OrgId = orgId });
        }
    }
}
