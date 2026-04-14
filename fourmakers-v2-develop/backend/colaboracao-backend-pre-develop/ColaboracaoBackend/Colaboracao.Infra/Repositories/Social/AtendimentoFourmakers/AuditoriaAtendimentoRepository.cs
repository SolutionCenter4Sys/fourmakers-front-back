using Colaboracao.Core.Interfaces;
using Core.Domain.Social.AtendimentoFourmakers;
using Dapper;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social.AtendimentoFourmakers
{
    public class AuditoriaAtendimentoRepository : IAuditoriaAtendimentoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public AuditoriaAtendimentoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task InserirAsync(AuditoriaInsertInput input, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                INSERT INTO tb_auditoria
                    (id, tb_org_id, codigo_interno_colaborador, actor_label, acao, detalhe, payload, tipo_entidade, entidade_id)
                VALUES
                    (UUID(), @OrgId, @CodigoInternoColaborador, @ActorLabel, @Acao, @Detalhe, @Payload, @TipoEntidade, @EntidadeId)";

            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, new
            {
                OrgId = orgId,
                input.CodigoInternoColaborador,
                input.ActorLabel,
                input.Acao,
                input.Detalhe,
                input.Payload,
                input.TipoEntidade,
                input.EntidadeId
            }, transaction);
        }

        public async Task<(IEnumerable<AuditoriaItemResult> itens, bool temMais)> ListarAsync(
            int orgId, int limite, int offset,
            DateTime? dataInicio = null, DateTime? dataFim = null,
            string categoria = null, string busca = null)
        {
            var sql = @"
                SELECT
                    CAST(id AS CHAR(36)) AS Id,
                    data_criacao AS DataCriacao,
                    actor_label AS ActorLabel,
                    acao AS Acao,
                    detalhe AS Detalhe,
                    SUBSTRING_INDEX(acao, '.', 1) AS Categoria,
                    tipo_entidade AS TipoEntidade,
                    entidade_id AS EntidadeId
                FROM tb_auditoria
                WHERE tb_org_id = @OrgId";

            var parameters = new DynamicParameters();
            parameters.Add("OrgId", orgId);

            if (dataInicio.HasValue)
            {
                sql += " AND data_criacao >= @DataInicio";
                parameters.Add("DataInicio", dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                sql += " AND data_criacao <= @DataFim";
                parameters.Add("DataFim", dataFim.Value.Date.AddDays(1));
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                sql += " AND acao LIKE CONCAT(@Categoria, '.%')";
                parameters.Add("Categoria", categoria);
            }

            if (!string.IsNullOrEmpty(busca))
            {
                var buscaEscaped = busca.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
                sql += " AND (detalhe LIKE @Busca OR acao LIKE @Busca OR actor_label LIKE @Busca)";
                parameters.Add("Busca", $"%{buscaEscaped}%");
            }

            sql += " ORDER BY data_criacao DESC LIMIT @Limite OFFSET @Offset";
            parameters.Add("Limite", limite + 1);
            parameters.Add("Offset", offset);

            var connection = _dapperConnection.GetConnection();
            var itens = (await connection.QueryAsync<AuditoriaItemResult>(sql, parameters)).ToList();

            var temMais = itens.Count > limite;
            if (temMais)
                itens.RemoveAt(itens.Count - 1);

            return (itens, temMais);
        }

        public async Task<IEnumerable<AuditoriaItemResult>> ListarParaCsvAsync(int orgId,
            DateTime? dataInicio = null, DateTime? dataFim = null,
            string categoria = null, string busca = null,
            int maxLinhas = 2500)
        {
            var sql = @"
                SELECT
                    CAST(id AS CHAR(36)) AS Id,
                    data_criacao AS DataCriacao,
                    actor_label AS ActorLabel,
                    acao AS Acao,
                    detalhe AS Detalhe,
                    SUBSTRING_INDEX(acao, '.', 1) AS Categoria,
                    tipo_entidade AS TipoEntidade,
                    entidade_id AS EntidadeId
                FROM tb_auditoria
                WHERE tb_org_id = @OrgId";

            var parameters = new DynamicParameters();
            parameters.Add("OrgId", orgId);

            if (dataInicio.HasValue)
            {
                sql += " AND data_criacao >= @DataInicio";
                parameters.Add("DataInicio", dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                sql += " AND data_criacao <= @DataFim";
                parameters.Add("DataFim", dataFim.Value.Date.AddDays(1));
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                sql += " AND acao LIKE CONCAT(@Categoria, '.%')";
                parameters.Add("Categoria", categoria);
            }

            if (!string.IsNullOrEmpty(busca))
            {
                var buscaEscaped = busca.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
                sql += " AND (detalhe LIKE @Busca OR acao LIKE @Busca OR actor_label LIKE @Busca)";
                parameters.Add("Busca", $"%{buscaEscaped}%");
            }

            sql += " ORDER BY data_criacao DESC LIMIT @MaxLinhas";
            parameters.Add("MaxLinhas", maxLinhas);

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<AuditoriaItemResult>(sql, parameters);
        }
    }
}
