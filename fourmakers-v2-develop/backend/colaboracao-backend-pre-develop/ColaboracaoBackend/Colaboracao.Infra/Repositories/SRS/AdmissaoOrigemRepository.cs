using System;
using System.Text.Json;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS.AdmissaoOrigem;

namespace Colaboracao.Infra.Repositories.SRS;

public class AdmissaoOrigemRepository : IAdmissaoOrigemRepository
{
    private readonly IDBConnection _dapperConnection;

    public AdmissaoOrigemRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private const string SelectBase = @"
        SELECT
            o.id                    AS Id,
            o.tb_admissao_id        AS AdmissaoId,
            o.origem_tipo           AS OrigemTipo,
            o.tb_vaga_id            AS TbVagaId,
            o.tb_candidato_vaga_id  AS TbCandidatoVagaId,
            o.data_criacao          AS DataCriacao
        FROM tb_admissao_origem o";

    public async Task<Guid> InserirAsync(Guid admissaoId, AdmissaoOrigemInput input, string alteradorCpf)
    {
        var connection = _dapperConnection.GetConnection();

        // Lê estado anterior para logar corretamente REPLACE vs CREATE
        var anterior = await ObterPorAdmissaoAsync(admissaoId);
        var acao = anterior != null ? "REPLACE" : "CREATE";

        if (anterior != null)
        {
            const string deleteSql = "DELETE FROM tb_admissao_origem WHERE tb_admissao_id = @AdmissaoId";
            await connection.ExecuteAsync(deleteSql, new { AdmissaoId = admissaoId.ToString() });
        }

        var id = Guid.NewGuid();
        const string insertSql = @"
            INSERT INTO tb_admissao_origem
                (id, tb_admissao_id, origem_tipo, tb_vaga_id, tb_candidato_vaga_id)
            VALUES
                (@Id, @AdmissaoId, @OrigemTipo, @TbVagaId, @TbCandidatoVagaId)";

        await connection.ExecuteAsync(insertSql, new
        {
            Id = id.ToString(),
            AdmissaoId = admissaoId.ToString(),
            OrigemTipo = input.OrigemTipo.ToString(),
            TbVagaId = input.TbVagaId?.ToString(),
            input.TbCandidatoVagaId
        });

        var novo = await ObterPorAdmissaoAsync(admissaoId);
        await InserirLogAsync(connection, id, admissaoId, acao, alteradorCpf, anterior, novo);

        return id;
    }

    public async Task<AdmissaoOrigemResult> ObterPorAdmissaoAsync(Guid admissaoId)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE o.tb_admissao_id = @AdmissaoId";
        return await connection.QueryFirstOrDefaultAsync<AdmissaoOrigemResult>(
            sql, new { AdmissaoId = admissaoId.ToString() });
    }

    private static async Task InserirLogAsync(
        System.Data.IDbConnection connection,
        Guid origemId,
        Guid admissaoId,
        string acao,
        string alteradorCpf,
        AdmissaoOrigemResult objeto,
        AdmissaoOrigemResult alteracoes)
    {
        var logId = Guid.NewGuid().ToString();
        var objetoJson = objeto != null ? JsonSerializer.Serialize(objeto, JsonOptions) : "{}";
        var alteracoesJson = alteracoes != null ? JsonSerializer.Serialize(alteracoes, JsonOptions) : "{}";

        const string logSql = @"
            INSERT INTO tb_admissao_origem_log
                (id, tb_admissao_origem_id, tb_admissao_id, acao,
                 tb_colaborador_codigo_interno_colaborador_alterador, objeto, alteracoes)
            VALUES
                (@Id, @OrigemId, @AdmissaoId, @Acao, @AlteradorCodigo, @Objeto, @Alteracoes)";

        await connection.ExecuteAsync(logSql, new
        {
            Id = logId,
            OrigemId = origemId.ToString(),
            AdmissaoId = admissaoId.ToString(),
            Acao = acao,
            AlteradorCodigo = alteradorCpf ?? string.Empty,
            Objeto = objetoJson,
            Alteracoes = alteracoesJson
        });
    }
}
