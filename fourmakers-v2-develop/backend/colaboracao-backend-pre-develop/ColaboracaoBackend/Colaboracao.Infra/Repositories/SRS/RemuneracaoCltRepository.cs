using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS.RemuneracaoClt;

namespace Colaboracao.Infra.Repositories.SRS;

public class RemuneracaoCltRepository : IRemuneracaoCltRepository
{
    private readonly IDBConnection _dapperConnection;

    public RemuneracaoCltRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private const string SelectBase = @"
        SELECT 
            r.id AS Id,
            r.tb_admissao_cargo_id AS AdmissaoCargoId,
            c.descricao AS CargoDescricao,
            r.faixa1_inicio AS Faixa1Inicio,
            r.faixa1_final AS Faixa1Final,
            r.faixa2_inicio AS Faixa2Inicio,
            r.faixa2_final AS Faixa2Final,
            r.faixa3_inicio AS Faixa3Inicio,
            r.faixa3_final AS Faixa3Final,
            r.faixa4_inicio AS Faixa4Inicio,
            r.faixa4_final AS Faixa4Final,
            r.tb_cbo_id AS CboId,
            cb.codigo AS CboCodigo,
            cb.titulo AS CboTitulo,
            r.piso AS Piso,
            r.ativo AS Ativo,
            r.data_criacao AS DataCriacao,
            r.data_alteracao AS DataAlteracao
        FROM tb_admissao_remuneracao_clt r
        INNER JOIN tb_admissao_cargo c ON c.id = r.tb_admissao_cargo_id
        LEFT JOIN tb_admissao_cbo cb ON cb.id = r.tb_cbo_id";

    public async Task<IEnumerable<RemuneracaoCltResult>> ListarAsync(bool somenteAtivos = true, int? cursor = null, int? limite = null, Guid? admissaoCargoId = null)
    {
        var connection = _dapperConnection.GetConnection();
        var where = new List<string> { "1=1" };
        if (somenteAtivos)
            where.Add("r.ativo = 1");
        if (admissaoCargoId.HasValue && admissaoCargoId.Value != Guid.Empty)
            where.Add("r.tb_admissao_cargo_id = @AdmissaoCargoId");

        var sql = SelectBase + " WHERE " + string.Join(" AND ", where) + " ORDER BY c.descricao";

        var dynamicParam = new DynamicParameters();
        if (admissaoCargoId.HasValue && admissaoCargoId.Value != Guid.Empty)
            dynamicParam.Add("AdmissaoCargoId", admissaoCargoId.Value.ToString());
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

        return await connection.QueryAsync<RemuneracaoCltResult>(sql, dynamicParam);
    }

    public async Task<RemuneracaoCltResult> ObterPorIdAsync(Guid id)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE r.id = @Id";
        return await connection.QueryFirstOrDefaultAsync<RemuneracaoCltResult>(sql, new { Id = id.ToString() });
    }

    public async Task<RemuneracaoCltResult> ObterPorAdmissaoCargoIdAsync(Guid admissaoCargoId)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE r.tb_admissao_cargo_id = @AdmissaoCargoId";
        return await connection.QueryFirstOrDefaultAsync<RemuneracaoCltResult>(sql, new { AdmissaoCargoId = admissaoCargoId.ToString() });
    }

    public async Task<Guid> InserirAsync(RemuneracaoCltInput input, string alteradorCpf)
    {
        var id = Guid.NewGuid();
        var connection = _dapperConnection.GetConnection();
        var sql = @"
            INSERT INTO tb_admissao_remuneracao_clt (
                id, tb_admissao_cargo_id, faixa1_inicio, faixa1_final, faixa2_inicio, faixa2_final,
                faixa3_inicio, faixa3_final, faixa4_inicio, faixa4_final, tb_cbo_id, piso, ativo)
            VALUES (
                @Id, @AdmissaoCargoId, @Faixa1Inicio, @Faixa1Final, @Faixa2Inicio, @Faixa2Final,
                @Faixa3Inicio, @Faixa3Final, @Faixa4Inicio, @Faixa4Final, @CboId, @Piso, @Ativo)";
        await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            AdmissaoCargoId = input.AdmissaoCargoId.ToString(),
            Faixa1Inicio = input.Faixa1Inicio,
            Faixa1Final = input.Faixa1Final,
            Faixa2Inicio = input.Faixa2Inicio,
            Faixa2Final = input.Faixa2Final,
            Faixa3Inicio = input.Faixa3Inicio,
            Faixa3Final = input.Faixa3Final,
            Faixa4Inicio = input.Faixa4Inicio,
            Faixa4Final = input.Faixa4Final,
            CboId = input.CboId.HasValue ? input.CboId.Value.ToString() : (string)null,
            Piso = input.Piso,
            Ativo = input.Ativo ? 1 : 0
        });

        var criado = await ObterPorIdAsync(id);
        await InserirLogAsync(connection, id, "CREATE", alteradorCpf, criado, null);

        return id;
    }

    public async Task<bool> AtualizarAsync(Guid id, RemuneracaoCltInput input, string alteradorCpf)
    {
        var connection = _dapperConnection.GetConnection();
        var estadoAnterior = await ObterPorIdAsync(id);
        if (estadoAnterior == null) return false;

        var sql = @"
            UPDATE tb_admissao_remuneracao_clt SET
                tb_admissao_cargo_id = @AdmissaoCargoId,
                faixa1_inicio = @Faixa1Inicio, faixa1_final = @Faixa1Final,
                faixa2_inicio = @Faixa2Inicio, faixa2_final = @Faixa2Final,
                faixa3_inicio = @Faixa3Inicio, faixa3_final = @Faixa3Final,
                faixa4_inicio = @Faixa4Inicio, faixa4_final = @Faixa4Final,
                tb_cbo_id = @CboId, piso = @Piso, ativo = @Ativo
            WHERE id = @Id";
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            AdmissaoCargoId = input.AdmissaoCargoId.ToString(),
            Faixa1Inicio = input.Faixa1Inicio,
            Faixa1Final = input.Faixa1Final,
            Faixa2Inicio = input.Faixa2Inicio,
            Faixa2Final = input.Faixa2Final,
            Faixa3Inicio = input.Faixa3Inicio,
            Faixa3Final = input.Faixa3Final,
            Faixa4Inicio = input.Faixa4Inicio,
            Faixa4Final = input.Faixa4Final,
            CboId = input.CboId.HasValue ? input.CboId.Value.ToString() : (string)null,
            Piso = input.Piso,
            Ativo = input.Ativo ? 1 : 0
        });
        if (rows == 0) return false;

        var estadoNovo = await ObterPorIdAsync(id);
        await InserirLogAsync(connection, id, "UPDATE", alteradorCpf, estadoAnterior, estadoNovo);

        return true;
    }

    private async Task InserirLogAsync(IDbConnection connection, Guid tbRemuneracaoCltId, string acao, string alteradorCpf, RemuneracaoCltResult objeto, RemuneracaoCltResult alteracoes)
    {
        var logId = Guid.NewGuid().ToString();
        var objetoJson = objeto != null ? JsonSerializer.Serialize(objeto, JsonOptions) : "{}";
        var alteracoesJson = alteracoes != null ? JsonSerializer.Serialize(alteracoes, JsonOptions) : "{}";

        var logSql = @"
            INSERT INTO tb_admissao_remuneracao_clt_log (id, tb_admissao_remuneracao_clt_id, acao, alterador_cpf, objeto, alteracoes)
            VALUES (@Id, @TbAdmissaoRemuneracaoCltId, @Acao, @AlteradorCpf, @Objeto, @Alteracoes)";

        await connection.ExecuteAsync(logSql, new
        {
            Id = logId,
            TbAdmissaoRemuneracaoCltId = tbRemuneracaoCltId.ToString(),
            Acao = acao,
            AlteradorCpf = alteradorCpf ?? "",
            Objeto = objetoJson,
            Alteracoes = alteracoesJson
        });
    }
}
