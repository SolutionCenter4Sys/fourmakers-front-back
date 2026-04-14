using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS.Cbo;

namespace Colaboracao.Infra.Repositories.SRS;

public class CboRepository : ICboRepository
{
    private readonly IDBConnection _dapperConnection;

    public CboRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    private const string SelectBase = @"
        SELECT 
            c.id AS Id,
            c.tb_org_id AS TbOrgId,
            c.codigo AS Codigo,
            c.titulo AS Titulo,
            c.descricao AS Descricao,
            c.ativo AS Ativo,
            c.data_criacao AS DataCriacao,
            c.data_alteracao AS DataAlteracao
        FROM tb_admissao_cbo c";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<IEnumerable<CboResult>> ListarAsync(int orgId, bool somenteAtivos = false, int? cursor = null, int? limite = null, string busca = null)
    {
        var connection = _dapperConnection.GetConnection();
        var where = new List<string> { "c.tb_org_id = @OrgId" };
        if (somenteAtivos)
            where.Add("c.ativo = 1");
        if (!string.IsNullOrWhiteSpace(busca))
            where.Add("(c.codigo LIKE @Busca OR c.titulo LIKE @Busca)");

        var sql = SelectBase + " WHERE " + string.Join(" AND ", where) + " ORDER BY c.codigo";

        var dynamicParam = new DynamicParameters();
        dynamicParam.Add("OrgId", orgId);
        if (!string.IsNullOrWhiteSpace(busca))
            dynamicParam.Add("Busca", $"%{busca}%");
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

        return await connection.QueryAsync<CboResult>(sql, dynamicParam);
    }

    public async Task<CboResult> ObterPorIdAsync(Guid id, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE c.id = @Id AND c.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<CboResult>(sql, new { Id = id.ToString(), OrgId = orgId });
    }

    public async Task<CboResult> ObterPorCodigoAsync(string codigo, int orgId)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return null;
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE c.codigo = @Codigo AND c.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<CboResult>(sql, new { Codigo = codigo.Trim(), OrgId = orgId });
    }

    public async Task<Guid> InserirAsync(CboInput input, string alteradorCpf, int orgId)
    {
        var id = Guid.NewGuid();
        var connection = _dapperConnection.GetConnection();

        var sql = @"
            INSERT INTO tb_admissao_cbo (id, tb_org_id, codigo, titulo, descricao, ativo)
            VALUES (@Id, @OrgId, @Codigo, @Titulo, @Descricao, @Ativo)";
        await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            Codigo = input.Codigo?.Trim(),
            Titulo = input.Titulo?.Trim(),
            Descricao = string.IsNullOrWhiteSpace(input.Descricao) ? null : input.Descricao.Trim(),
            Ativo = input.Ativo ? 1 : 0
        });

        var criado = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "CREATE", alteradorCpf, criado, null);

        return id;
    }

    public async Task<bool> AtualizarAsync(Guid id, CboInput input, string alteradorCpf, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var estadoAnterior = await ObterPorIdAsync(id, orgId);
        if (estadoAnterior == null) return false;

        var sql = @"
            UPDATE tb_admissao_cbo
            SET codigo = @Codigo, titulo = @Titulo, descricao = @Descricao, ativo = @Ativo
            WHERE id = @Id AND tb_org_id = @OrgId";
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            Codigo = input.Codigo?.Trim(),
            Titulo = input.Titulo?.Trim(),
            Descricao = string.IsNullOrWhiteSpace(input.Descricao) ? null : input.Descricao.Trim(),
            Ativo = input.Ativo ? 1 : 0
        });
        if (rows == 0) return false;

        var estadoNovo = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "UPDATE", alteradorCpf, estadoAnterior, estadoNovo);

        return true;
    }

    private async Task InserirLogAsync(IDbConnection connection, Guid tbCboId, string acao, string alteradorCpf, CboResult objeto, CboResult alteracoes)
    {
        var logId = Guid.NewGuid().ToString();
        var objetoJson = objeto != null ? JsonSerializer.Serialize(objeto, JsonOptions) : "{}";
        var alteracoesJson = alteracoes != null ? JsonSerializer.Serialize(alteracoes, JsonOptions) : "{}";

        var logSql = @"
            INSERT INTO tb_admissao_cbo_log (id, tb_admissao_cbo_id, acao, alterador_cpf, objeto, alteracoes)
            VALUES (@Id, @TbAdmissaoCboId, @Acao, @AlteradorCpf, @Objeto, @Alteracoes)";

        await connection.ExecuteAsync(logSql, new
        {
            Id = logId,
            TbAdmissaoCboId = tbCboId.ToString(),
            Acao = acao,
            AlteradorCpf = alteradorCpf ?? "",
            Objeto = objetoJson,
            Alteracoes = alteracoesJson
        });
    }
}
