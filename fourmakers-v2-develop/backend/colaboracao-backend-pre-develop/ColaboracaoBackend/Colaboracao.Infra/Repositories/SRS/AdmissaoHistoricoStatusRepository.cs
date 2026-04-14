using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;

namespace Colaboracao.Infra.Repositories.SRS;

public class AdmissaoHistoricoStatusRepository : IAdmissaoHistoricoStatusRepository
{
    private readonly IDBConnection _dapperConnection;

    public AdmissaoHistoricoStatusRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    private const string SelectBase = @"
        SELECT
            h.id                                            AS Id,
            h.tb_org_id                                     AS TbOrgId,
            h.tb_admissao_id                                AS AdmissaoId,
            h.tb_admissao_status_origem_id                  AS AdmissaoStatusOrigemId,
            h.tb_admissao_status_destino_id                 AS AdmissaoStatusDestinoId,
            h.tb_colaborador_codigo_interno_colaborador     AS CodigoInternoColaborador,
            h.data_movimentacao                             AS DataMovimentacao,
            h.observacao                                    AS Observacao,
            h.data_criacao                                  AS DataCriacao,
            h.data_atualizacao                              AS DataAtualizacao
        FROM tb_admissao_historico_status h";

    public async Task<IEnumerable<AdmissaoHistoricoStatusResult>> ListarPorAdmissaoAsync(Guid admissaoId, int orgId, int? cursor = null, int? limite = null)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE h.tb_admissao_id = @AdmissaoId AND h.tb_org_id = @OrgId ORDER BY h.data_movimentacao DESC";

        var dynamicParam = new DynamicParameters();
        dynamicParam.Add("AdmissaoId", admissaoId.ToString());
        dynamicParam.Add("OrgId", orgId);

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

        return await connection.QueryAsync<AdmissaoHistoricoStatusResult>(sql, dynamicParam);
    }

    public async Task<AdmissaoHistoricoStatusResult> ObterPorIdAsync(Guid id, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE h.id = @Id AND h.tb_org_id = @OrgId";
        return await connection.QueryFirstOrDefaultAsync<AdmissaoHistoricoStatusResult>(sql, new { Id = id.ToString(), OrgId = orgId });
    }

    public async Task<Guid> InserirAsync(AdmissaoHistoricoStatusInput input, Guid admissaoId, Guid? statusOrigemId, string codigoInternoColaborador, int orgId)
    {
        var id = Guid.NewGuid();
        var connection = _dapperConnection.GetConnection();

        const string sql = @"
            INSERT INTO tb_admissao_historico_status
                (id, tb_org_id, tb_admissao_id, tb_admissao_status_origem_id,
                 tb_admissao_status_destino_id, tb_colaborador_codigo_interno_colaborador, observacao)
            VALUES
                (@Id, @OrgId, @AdmissaoId, @StatusOrigemId,
                 @StatusDestinoId, @CodigoInternoColaborador, @Observacao)";

        await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            AdmissaoId = admissaoId.ToString(),
            StatusOrigemId = statusOrigemId.HasValue ? statusOrigemId.Value.ToString() : (string)null,
            StatusDestinoId = input.AdmissaoStatusDestinoId.ToString(),
            CodigoInternoColaborador = codigoInternoColaborador,
            Observacao = input.Observacao?.Trim()
        });

        return id;
    }
}
