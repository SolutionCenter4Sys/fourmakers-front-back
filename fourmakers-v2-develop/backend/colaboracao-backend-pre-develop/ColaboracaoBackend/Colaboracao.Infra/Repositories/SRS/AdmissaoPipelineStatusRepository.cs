using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS.AdmissaoPipeline;

namespace Colaboracao.Infra.Repositories.SRS;

public class AdmissaoPipelineStatusRepository : IAdmissaoPipelineStatusRepository
{
    private readonly IDBConnection _dapperConnection;

    public AdmissaoPipelineStatusRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    public async Task<IEnumerable<AdmissaoPipelineStatusResult>> ListarPorPipelineAsync(int orgId, Guid pipelineId, bool somenteAtivos = true)
    {
        var connection = _dapperConnection.GetConnection();
        var where = new List<string>
        {
            "p.tb_org_id = @OrgId",
            "ps.tb_admissao_pipeline_id = @PipelineId",
            "s.tb_org_id = @OrgId"
        };
        if (somenteAtivos)
            where.Add("ps.ativo = 1");

        var sql = @"
            SELECT 
                ps.id AS Id,
                ps.tb_admissao_pipeline_id AS AdmissaoPipelineId,
                ps.tb_admissao_status_id AS AdmissaoStatusId,
                s.descricao AS StatusDescricao,
                s.codigo AS StatusCodigo,
                ps.ordem AS Ordem,
                ps.obrigatorio AS Obrigatorio,
                ps.permite_retroceder AS PermiteRetroceder,
                ps.status_inicial AS StatusInicial,
                ps.status_final AS StatusFinal,
                ps.ativo AS Ativo,
                ps.data_criacao AS DataCriacao,
                ps.data_alteracao AS DataAlteracao
            FROM tb_admissao_pipeline_status ps
            INNER JOIN tb_admissao_pipeline p ON p.id = ps.tb_admissao_pipeline_id
            INNER JOIN tb_admissao_status s ON s.id = ps.tb_admissao_status_id
            WHERE " + string.Join(" AND ", where) + @"
            ORDER BY ps.ordem";

        return await connection.QueryAsync<AdmissaoPipelineStatusResult>(sql, new { OrgId = orgId, PipelineId = pipelineId.ToString() });
    }

    public async Task SubstituirPorPipelineAsync(int orgId, Guid pipelineId, IReadOnlyList<AdmissaoPipelineStatusItemInput> itens)
    {
        var connection = _dapperConnection.GetConnection();
        using var tx = connection.BeginTransaction();
        try
        {
            var pipelineOk = await connection.ExecuteScalarAsync<long>(
                @"SELECT COUNT(1) FROM tb_admissao_pipeline WHERE id = @Id AND tb_org_id = @OrgId",
                new { Id = pipelineId.ToString(), OrgId = orgId },
                tx);
            if (pipelineOk == 0)
            {
                tx.Rollback();
                throw new InvalidOperationException("Pipeline não encontrado para a organização.");
            }

            await connection.ExecuteAsync(
                @"DELETE FROM tb_admissao_pipeline_status WHERE tb_admissao_pipeline_id = @PipelineId",
                new { PipelineId = pipelineId.ToString() },
                tx);

            if (itens == null || itens.Count == 0)
            {
                tx.Commit();
                return;
            }

            const string insertSql = @"
                INSERT INTO tb_admissao_pipeline_status (
                    id, tb_admissao_pipeline_id, tb_admissao_status_id, ordem,
                    obrigatorio, permite_retroceder, status_inicial, status_final, ativo)
                VALUES (
                    @Id, @AdmissaoPipelineId, @AdmissaoStatusId, @Ordem,
                    @Obrigatorio, @PermiteRetroceder, @StatusInicial, @StatusFinal, @Ativo)";

            foreach (var item in itens)
            {
                object permiteRetroceder = item.PermiteRetroceder.HasValue
                    ? (item.PermiteRetroceder.Value ? 1 : 0)
                    : (object)DBNull.Value;

                await connection.ExecuteAsync(insertSql, new
                {
                    Id = Guid.NewGuid().ToString(),
                    AdmissaoPipelineId = pipelineId.ToString(),
                    AdmissaoStatusId = item.AdmissaoStatusId.ToString(),
                    Ordem = item.Ordem,
                    Obrigatorio = item.Obrigatorio ? 1 : 0,
                    PermiteRetroceder = permiteRetroceder,
                    StatusInicial = item.StatusInicial ? 1 : 0,
                    StatusFinal = item.StatusFinal ? 1 : 0,
                    Ativo = item.Ativo ? 1 : 0
                }, tx);
            }

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
