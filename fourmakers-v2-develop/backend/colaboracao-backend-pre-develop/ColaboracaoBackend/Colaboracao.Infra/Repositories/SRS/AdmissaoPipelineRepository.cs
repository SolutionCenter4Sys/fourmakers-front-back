using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS.AdmissaoPipeline;

namespace Colaboracao.Infra.Repositories.SRS;

public class AdmissaoPipelineRepository : IAdmissaoPipelineRepository
{
    private readonly IDBConnection _dapperConnection;

    public AdmissaoPipelineRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    private const string SelectBase = @"
        SELECT 
            p.id AS Id,
            p.tb_org_id AS OrgId,
            p.nome AS Nome,
            p.descricao AS Descricao,
            p.ativo AS Ativo,
            p.versao AS Versao,
            p.data_criacao AS DataCriacao,
            p.data_alteracao AS DataAlteracao
        FROM tb_admissao_pipeline p";

    private const string SelectStatusComJoin = @"
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
            INNER JOIN tb_admissao_status s ON s.id = ps.tb_admissao_status_id";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<IEnumerable<AdmissaoPipelineResult>> ListarAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null)
    {
        var connection = _dapperConnection.GetConnection();
        var where = new List<string> { "p.tb_org_id = @OrgId" };
        if (somenteAtivos)
            where.Add("p.ativo = 1");
        if (!string.IsNullOrWhiteSpace(busca))
            where.Add("p.nome LIKE @Busca");

        var sql = SelectBase + " WHERE " + string.Join(" AND ", where) + " ORDER BY p.nome";

        var dynamicParam = new DynamicParameters();
        dynamicParam.Add("OrgId", orgId);
        if (!string.IsNullOrWhiteSpace(busca))
            dynamicParam.Add("Busca", $"%{busca.Trim()}%");
        if (limite.HasValue && limite.Value > 0)
        {
            sql += " LIMIT @Limite";
            dynamicParam.Add("Limite", limite.Value);
            if (cursor.HasValue && cursor.Value > 0)
            {
                sql += " OFFSET @Cursor";
                dynamicParam.Add("Cursor", cursor.Value);
            }
        }

        var pipelines = (await connection.QueryAsync<AdmissaoPipelineResult>(sql, dynamicParam)).ToList();
        if (pipelines.Count == 0)
            return pipelines;

        var statusSql = SelectStatusComJoin + @"
            WHERE p.tb_org_id = @OrgId
              AND s.tb_org_id = @OrgId
              AND ps.tb_admissao_pipeline_id IN @PipelineIds
            ORDER BY ps.tb_admissao_pipeline_id, ps.ordem";

        var pipelineIds = pipelines.Select(p => p.Id.ToString()).ToList();
        var statuses = await connection.QueryAsync<AdmissaoPipelineStatusResult>(statusSql, new { OrgId = orgId, PipelineIds = pipelineIds });
        var porPipeline = statuses.GroupBy(s => s.AdmissaoPipelineId).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var p in pipelines)
            p.StatusItens = porPipeline.TryGetValue(p.Id, out var itens) ? itens : new List<AdmissaoPipelineStatusResult>();

        return pipelines;
    }

    public async Task<IEnumerable<AdmissaoPipelineSummaryResult>> ListarSummaryAsync(int orgId, bool somenteAtivos = true)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = @"
            SELECT p.id AS Id, p.nome AS Nome
            FROM tb_admissao_pipeline p
            WHERE p.tb_org_id = @OrgId";
        if (somenteAtivos)
            sql += " AND p.ativo = 1";
        sql += " ORDER BY p.nome";
        return await connection.QueryAsync<AdmissaoPipelineSummaryResult>(sql, new { OrgId = orgId });
    }

    public async Task<AdmissaoPipelineResult> ObterPorIdAsync(Guid id, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE p.id = @Id AND p.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<AdmissaoPipelineResult>(sql, new { Id = id.ToString(), OrgId = orgId });
    }

    public async Task<AdmissaoPipelineResult> ObterPorNomeAsync(string nome, int orgId)
    {
        if (string.IsNullOrWhiteSpace(nome)) return null;
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE p.nome = @Nome AND p.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<AdmissaoPipelineResult>(sql, new { Nome = nome.Trim(), OrgId = orgId });
    }

    public async Task<Guid> InserirAsync(AdmissaoPipelineInput input, string alteradorCpf, int orgId)
    {
        var id = Guid.NewGuid();
        var connection = _dapperConnection.GetConnection();

        var sql = @"
            INSERT INTO tb_admissao_pipeline (id, tb_org_id, nome, descricao, ativo, versao)
            VALUES (@Id, @OrgId, @Nome, @Descricao, @Ativo, @Versao)";
        await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            Nome = input.Nome?.Trim(),
            Descricao = string.IsNullOrWhiteSpace(input.Descricao) ? null : input.Descricao.Trim(),
            Ativo = input.Ativo ? 1 : 0,
            Versao = input.Versao < 1 ? 1 : input.Versao
        });

        var criado = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "CREATE", alteradorCpf, criado, null);

        return id;
    }

    public async Task<bool> AtualizarAsync(Guid id, AdmissaoPipelineAtualizarInput input, string alteradorCpf, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var estadoAnterior = await ObterPorIdAsync(id, orgId);
        if (estadoAnterior == null) return false;

        var sql = @"
            UPDATE tb_admissao_pipeline
            SET nome = @Nome, descricao = @Descricao, ativo = @Ativo, versao = @Versao
            WHERE id = @Id AND tb_org_id = @OrgId";
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            Nome = input.Nome?.Trim(),
            Descricao = string.IsNullOrWhiteSpace(input.Descricao) ? null : input.Descricao.Trim(),
            Ativo = input.Ativo ? 1 : 0,
            Versao = input.Versao < 1 ? 1 : input.Versao
        });
        if (rows == 0) return false;

        var estadoNovo = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "UPDATE", alteradorCpf, estadoAnterior, estadoNovo);

        return true;
    }

    public async Task<bool> InativarAsync(Guid id, string alteradorCpf, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var estadoAnterior = await ObterPorIdAsync(id, orgId);
        if (estadoAnterior == null) return false;
        if (!estadoAnterior.Ativo) return true;

        var sql = @"
            UPDATE tb_admissao_pipeline
            SET ativo = 0
            WHERE id = @Id AND tb_org_id = @OrgId";
        var rows = await connection.ExecuteAsync(sql, new { Id = id.ToString(), OrgId = orgId });
        if (rows == 0) return false;

        var estadoNovo = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "UPDATE", alteradorCpf, estadoAnterior, estadoNovo);

        return true;
    }

    private async Task InserirLogAsync(IDbConnection connection, Guid admissaoPipelineId, string acao, string alteradorCpf, AdmissaoPipelineResult objeto, AdmissaoPipelineResult alteracoes)
    {
        var logId = Guid.NewGuid().ToString();
        var objetoJson = objeto != null ? JsonSerializer.Serialize(objeto, JsonOptions) : "{}";
        var alteracoesJson = alteracoes != null ? JsonSerializer.Serialize(alteracoes, JsonOptions) : "{}";

        var logSql = @"
            INSERT INTO tb_admissao_pipeline_log (id, tb_admissao_pipeline_id, acao, alterador_cpf, objeto, alteracoes)
            VALUES (@Id, @AdmissaoPipelineId, @Acao, @AlteradorCpf, @Objeto, @Alteracoes)";

        await connection.ExecuteAsync(logSql, new
        {
            Id = logId,
            AdmissaoPipelineId = admissaoPipelineId.ToString(),
            Acao = acao,
            AlteradorCpf = alteradorCpf ?? "",
            Objeto = objetoJson,
            Alteracoes = alteracoesJson
        });
    }
}
