using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS.AdmissaoStatus;

namespace Colaboracao.Infra.Repositories.SRS;

public class AdmissaoStatusRepository : IAdmissaoStatusRepository
{
    private readonly IDBConnection _dapperConnection;

    public AdmissaoStatusRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    private const string SelectBase = @"
        SELECT 
            s.id AS Id,
            s.tb_org_id AS TbOrgId,
            s.descricao AS Descricao,
            s.codigo AS Codigo,
            s.ordem AS Ordem,
            s.ativo AS Ativo,
            s.data_criacao AS DataCriacao,
            s.data_alteracao AS DataAlteracao
        FROM tb_admissao_status s";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<IEnumerable<AdmissaoStatusResult>> ListarAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null)
    {
        var connection = _dapperConnection.GetConnection();
        var where = new List<string> { "s.tb_org_id = @OrgId" };
        if (somenteAtivos)
            where.Add("s.ativo = 1");
        if (!string.IsNullOrWhiteSpace(busca))
            where.Add("s.descricao LIKE @Busca");

        var sql = SelectBase + " WHERE " + string.Join(" AND ", where) + " ORDER BY s.ordem, s.descricao";

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

        return await connection.QueryAsync<AdmissaoStatusResult>(sql, dynamicParam);
    }

    public async Task<AdmissaoStatusResult> ObterPorIdAsync(Guid id, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE s.id = @Id AND s.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<AdmissaoStatusResult>(sql, new { Id = id.ToString(), OrgId = orgId });
    }

    public async Task<AdmissaoStatusResult> ObterPorDescricaoAsync(string descricao, int orgId)
    {
        if (string.IsNullOrWhiteSpace(descricao)) return null;
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE s.descricao = @Descricao AND s.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<AdmissaoStatusResult>(sql, new { Descricao = descricao.Trim(), OrgId = orgId });
    }

    public async Task<AdmissaoStatusResult> ObterPorCodigoNaOrgAsync(int codigo, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE s.codigo = @Codigo AND s.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<AdmissaoStatusResult>(sql, new { Codigo = codigo, OrgId = orgId });
    }

    public async Task<Guid> InserirAsync(AdmissaoStatusInput input, string alteradorCpf, int orgId)
    {
        var id = Guid.NewGuid();
        var connection = _dapperConnection.GetConnection();

        const string sql = @"
            INSERT INTO tb_admissao_status (id, tb_org_id, descricao, codigo, ordem, ativo)
            VALUES (@Id, @OrgId, @Descricao, @Codigo, @Ordem, @Ativo)";
        await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            Descricao = input.Descricao?.Trim(),
            Codigo = input.Codigo,
            Ordem = input.Ordem,
            Ativo = input.Ativo ? 1 : 0
        });

        var criado = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "CREATE", alteradorCpf, criado, null);

        return id;
    }

    public async Task<bool> AtualizarAsync(Guid id, AdmissaoStatusInput input, string alteradorCpf, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var estadoAnterior = await ObterPorIdAsync(id, orgId);
        if (estadoAnterior == null) return false;

        const string sql = @"
            UPDATE tb_admissao_status
            SET descricao = @Descricao, codigo = @Codigo, ordem = @Ordem, ativo = @Ativo
            WHERE id = @Id AND tb_org_id = @OrgId";
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            Descricao = input.Descricao?.Trim(),
            Codigo = input.Codigo,
            Ordem = input.Ordem,
            Ativo = input.Ativo ? 1 : 0
        });
        if (rows == 0) return false;

        var estadoNovo = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "UPDATE", alteradorCpf, estadoAnterior, estadoNovo);

        return true;
    }

    public async Task<bool> ExcluirLogicamenteAsync(Guid id, string alteradorCpf, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var estadoAnterior = await ObterPorIdAsync(id, orgId);
        if (estadoAnterior == null) return false;

        const string sql = @"
            UPDATE tb_admissao_status
            SET ativo = 0
            WHERE id = @Id AND tb_org_id = @OrgId AND ativo = 1";
        var rows = await connection.ExecuteAsync(sql, new { Id = id.ToString(), OrgId = orgId });
        if (rows == 0) return false;

        var estadoNovo = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "DELETE", alteradorCpf, estadoAnterior, estadoNovo);

        return true;
    }

    private static async Task InserirLogAsync(IDbConnection connection, Guid tbAdmissaoStatusId, string acao, string alteradorCpf, AdmissaoStatusResult objeto, AdmissaoStatusResult alteracoes)
    {
        var logId = Guid.NewGuid().ToString();
        var objetoJson = objeto != null ? JsonSerializer.Serialize(objeto, JsonOptions) : "{}";
        var alteracoesJson = alteracoes != null ? JsonSerializer.Serialize(alteracoes, JsonOptions) : "{}";

        const string logSql = @"
            INSERT INTO tb_admissao_status_log (id, tb_admissao_status_id, acao, alterador_cpf, objeto, alteracoes)
            VALUES (@Id, @TbAdmissaoStatusId, @Acao, @AlteradorCpf, @Objeto, @Alteracoes)";

        await connection.ExecuteAsync(logSql, new
        {
            Id = logId,
            TbAdmissaoStatusId = tbAdmissaoStatusId.ToString(),
            Acao = acao,
            AlteradorCpf = alteradorCpf ?? "",
            Objeto = objetoJson,
            Alteracoes = alteracoesJson
        });
    }
}
