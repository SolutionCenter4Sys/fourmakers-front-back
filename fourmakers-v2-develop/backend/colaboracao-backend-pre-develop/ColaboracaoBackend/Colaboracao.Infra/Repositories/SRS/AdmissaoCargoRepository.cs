using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS.AdmissaoCargo;

namespace Colaboracao.Infra.Repositories.SRS;

public class AdmissaoCargoRepository : IAdmissaoCargoRepository
{
    private readonly IDBConnection _dapperConnection;

    public AdmissaoCargoRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    private const string SelectBase = @"
        SELECT 
            c.id AS Id,
            c.tb_org_id AS TbOrgId,
            c.descricao AS Descricao,
            c.tb_cbo_id AS CboId,
            c.ativo AS Ativo,
            c.data_criacao AS DataCriacao,
            c.data_alteracao AS DataAlteracao
        FROM tb_admissao_cargo c";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<IEnumerable<AdmissaoCargoResult>> ListarAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null)
    {
        var connection = _dapperConnection.GetConnection();
        var where = new List<string> { "c.tb_org_id = @OrgId" };
        if (somenteAtivos)
            where.Add("c.ativo = 1");
        if (!string.IsNullOrWhiteSpace(busca))
            where.Add("c.descricao LIKE @Busca");

        var sql = SelectBase + " WHERE " + string.Join(" AND ", where) + " ORDER BY c.descricao";

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

        return await connection.QueryAsync<AdmissaoCargoResult>(sql, dynamicParam);
    }

    public async Task<IEnumerable<AdmissaoCargoResult>> ListarComRemuneracaoCadastradaAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null)
    {
        var connection = _dapperConnection.GetConnection();
        var where = new List<string> { "c.tb_org_id = @OrgId" };
        if (somenteAtivos)
            where.Add("c.ativo = 1");
        if (!string.IsNullOrWhiteSpace(busca))
            where.Add("c.descricao LIKE @Busca");

        var sql = SelectBase
                  + @" INNER JOIN tb_admissao_remuneracao_clt r ON r.tb_admissao_cargo_id = c.id
 WHERE " + string.Join(" AND ", where) + " ORDER BY c.descricao";

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

        return await connection.QueryAsync<AdmissaoCargoResult>(sql, dynamicParam);
    }

    public async Task<AdmissaoCargoResult> ObterPorIdAsync(Guid id, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE c.id = @Id AND c.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<AdmissaoCargoResult>(sql, new { Id = id.ToString(), OrgId = orgId });
    }

    public async Task<AdmissaoCargoResult> ObterPorDescricaoAsync(string descricao, int orgId)
    {
        if (string.IsNullOrWhiteSpace(descricao)) return null;
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE c.descricao = @Descricao AND c.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<AdmissaoCargoResult>(sql, new { Descricao = descricao.Trim(), OrgId = orgId });
    }

    public async Task<bool> ExisteAtivoPorIdEOrgAsync(Guid id, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        const string sql = @"SELECT 1 FROM tb_admissao_cargo c WHERE c.id = @Id AND c.tb_org_id = @OrgId AND c.ativo = 1 LIMIT 1";
        var found = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Id = id.ToString(), OrgId = orgId });
        return found.HasValue;
    }

    public async Task<Guid> InserirAsync(AdmissaoCargoInput input, string alteradorCpf, int orgId)
    {
        var id = Guid.NewGuid();
        var connection = _dapperConnection.GetConnection();

        var sql = @"
            INSERT INTO tb_admissao_cargo (id, tb_org_id, descricao, tb_cbo_id, ativo)
            VALUES (@Id, @OrgId, @Descricao, @CboId, @Ativo)";
        await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            Descricao = input.Descricao?.Trim(),
            CboId = input.CboId.ToString(),
            Ativo = input.Ativo ? 1 : 0
        });

        var criado = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "CREATE", alteradorCpf, criado, null);

        return id;
    }

    public async Task<bool> AtualizarAsync(Guid id, AdmissaoCargoInput input, string alteradorCpf, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var estadoAnterior = await ObterPorIdAsync(id, orgId);
        if (estadoAnterior == null) return false;

        var sql = @"
            UPDATE tb_admissao_cargo
            SET descricao = @Descricao, tb_cbo_id = @CboId, ativo = @Ativo
            WHERE id = @Id AND tb_org_id = @OrgId";
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            Descricao = input.Descricao?.Trim(),
            CboId = input.CboId.ToString(),
            Ativo = input.Ativo ? 1 : 0
        });
        if (rows == 0) return false;

        var estadoNovo = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "UPDATE", alteradorCpf, estadoAnterior, estadoNovo);

        return true;
    }

    private async Task InserirLogAsync(IDbConnection connection, Guid tbAdmissaoCargoId, string acao, string alteradorCpf, AdmissaoCargoResult objeto, AdmissaoCargoResult alteracoes)
    {
        var logId = Guid.NewGuid().ToString();
        var objetoJson = objeto != null ? JsonSerializer.Serialize(objeto, JsonOptions) : "{}";
        var alteracoesJson = alteracoes != null ? JsonSerializer.Serialize(alteracoes, JsonOptions) : "{}";

        var logSql = @"
            INSERT INTO tb_admissao_cargo_log (id, tb_admissao_cargo_id, acao, alterador_cpf, objeto, alteracoes)
            VALUES (@Id, @TbAdmissaoCargoId, @Acao, @AlteradorCpf, @Objeto, @Alteracoes)";

        await connection.ExecuteAsync(logSql, new
        {
            Id = logId,
            TbAdmissaoCargoId = tbAdmissaoCargoId.ToString(),
            Acao = acao,
            AlteradorCpf = alteradorCpf ?? "",
            Objeto = objetoJson,
            Alteracoes = alteracoesJson
        });
    }
}
