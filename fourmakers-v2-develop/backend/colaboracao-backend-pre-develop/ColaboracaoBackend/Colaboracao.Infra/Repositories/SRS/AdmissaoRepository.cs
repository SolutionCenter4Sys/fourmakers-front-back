using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS.Admissao;
using DataTransferObject.Domain.SRS.AdmissaoOrigem;

namespace Colaboracao.Infra.Repositories.SRS;

public class AdmissaoRepository : IAdmissaoRepository
{
    private readonly IDBConnection _dapperConnection;

    public AdmissaoRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    private const string SelectBase = @"
        SELECT
            a.id                                            AS Id,
            a.tb_org_id                                     AS TbOrgId,
            a.tb_admissao_pipeline_id                       AS AdmissaoPipelineId,
            a.tb_admissao_status_id                         AS AdmissaoStatusId,
            a.tb_colaborador_codigo_interno_colaborador     AS CodigoInternoColaborador,
            a.data_inicio                                   AS DataInicio,
            a.data_fim                                      AS DataFim,
            a.observacao                                    AS Observacao,
            a.ativo                                         AS Ativo,
            a.data_criacao                                  AS DataCriacao,
            a.data_atualizacao                              AS DataAtualizacao,
            -- colaborador alvo
            c.nome_completo                                 AS NomeColaborador,
            -- vaga: VAGA→tb_vaga direto | CANDIDATURA→tb_vaga via tb_candidato_vaga
            COALESCE(tv_direta.id, tv_candidatura.id)       AS VagaId,
            COALESCE(tv_direta.titulo, tv_candidatura.titulo) AS VagaTitulo,
            -- cliente: tb_vaga→tb_gestor_externo_perfil→tb_cliente_org
            cl.id                                           AS ClienteId,
            cl.nome_cliente                                 AS NomeCliente,
            -- origem (LEFT JOIN — nulo quando módulo de recrutamento não está presente)
            o.id                                            AS OrigemId,
            o.origem_tipo                                   AS OrigemTipo,
            o.tb_vaga_id                                    AS OrigemTbVagaId,
            o.tb_candidato_vaga_id                          AS OrigemTbCandidatoVagaId,
            o.data_criacao                                  AS OrigemDataCriacao
        FROM tb_admissao a
        LEFT JOIN tb_colaborador    c              ON c.codigo_interno_colaborador = a.tb_colaborador_codigo_interno_colaborador
        LEFT JOIN tb_admissao_origem o             ON o.tb_admissao_id = a.id
        -- origem tipo VAGA: tb_vaga_id aponta direto para tb_vaga.id
        LEFT JOIN tb_vaga           tv_direta      ON tv_direta.id = o.tb_vaga_id
        -- origem tipo CANDIDATURA: tb_candidato_vaga_id → tb_candidato_vaga → tb_vaga
        LEFT JOIN tb_candidato_vaga cv             ON cv.id = o.tb_candidato_vaga_id
        LEFT JOIN tb_vaga           tv_candidatura ON tv_candidatura.id = cv.tb_vaga_id
        -- cliente: tb_vaga.tb_gestor_cod → tb_gestor_externo.cod_gestor_externo → tb_cliente_org
        LEFT JOIN tb_gestor_externo ge             ON ge.cod_gestor_externo = COALESCE(tv_direta.tb_gestor_cod, tv_candidatura.tb_gestor_cod)
                                                  AND ge.tb_org_id = a.tb_org_id
        LEFT JOIN tb_cliente_org    cl             ON cl.codigo_cliente = ge.codigo_cliente";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<IEnumerable<AdmissaoResult>> ListarAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, Guid? pipelineId = null, Guid? statusId = null)
    {
        var connection = _dapperConnection.GetConnection();
        var where = new List<string> { "a.tb_org_id = @OrgId" };

        if (somenteAtivos)
            where.Add("a.ativo = 1");
        if (pipelineId.HasValue)
            where.Add("a.tb_admissao_pipeline_id = @PipelineId");
        if (statusId.HasValue)
            where.Add("a.tb_admissao_status_id = @StatusId");

        var sql = SelectBase + " WHERE " + string.Join(" AND ", where) + " ORDER BY a.data_inicio DESC";

        var dynamicParam = new DynamicParameters();
        dynamicParam.Add("OrgId", orgId);
        if (pipelineId.HasValue) dynamicParam.Add("PipelineId", pipelineId.Value.ToString());
        if (statusId.HasValue) dynamicParam.Add("StatusId", statusId.Value.ToString());

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

        return await connection.QueryAsync<AdmissaoResult, AdmissaoOrigemResult, AdmissaoResult>(
            sql,
            MapAdmissaoComOrigem,
            dynamicParam,
            splitOn: "OrigemId");
    }

    public async Task<AdmissaoResult> ObterPorIdAsync(Guid id, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var sql = SelectBase + " WHERE a.id = @Id AND a.tb_org_id = @OrgId";
        var rows = await connection.QueryAsync<AdmissaoResult, AdmissaoOrigemResult, AdmissaoResult>(
            sql,
            MapAdmissaoComOrigem,
            new { Id = id.ToString(), OrgId = orgId },
            splitOn: "OrigemId");
        return rows.FirstOrDefault();
    }

    private static AdmissaoResult MapAdmissaoComOrigem(AdmissaoResult admissao, AdmissaoOrigemResult origem)
    {
        // Dapper retorna objeto com Id = Guid.Empty quando o LEFT JOIN não encontra linha
        if (origem != null && origem.Id != Guid.Empty)
            admissao.Origem = origem;
        return admissao;
    }

    public async Task<Guid> InserirAsync(AdmissaoInput input, string alteradorCpf, int orgId)
    {
        var id = Guid.NewGuid();
        var connection = _dapperConnection.GetConnection();

        const string sql = @"
            INSERT INTO tb_admissao
                (id, tb_org_id, tb_admissao_pipeline_id, tb_admissao_status_id,
                 tb_colaborador_codigo_interno_colaborador, data_inicio, data_fim, observacao, ativo)
            VALUES
                (@Id, @OrgId, @PipelineId, @StatusId,
                 @CodigoInternoColaborador, @DataInicio, @DataFim, @Observacao, @Ativo)";

        await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            PipelineId = input.AdmissaoPipelineId.ToString(),
            StatusId = input.AdmissaoStatusId.ToString(),
            CodigoInternoColaborador = input.CodigoInternoColaborador?.Trim(),
            input.DataInicio,
            input.DataFim,
            Observacao = input.Observacao?.Trim(),
            Ativo = input.Ativo ? 1 : 0
        });

        var criado = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "CREATE", alteradorCpf, criado, null);

        return id;
    }

    public async Task<bool> AtualizarAsync(Guid id, AdmissaoUpdateInput input, string alteradorCpf, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var estadoAnterior = await ObterPorIdAsync(id, orgId);
        if (estadoAnterior == null) return false;

        const string sql = @"
            UPDATE tb_admissao
            SET
                tb_admissao_pipeline_id                   = @PipelineId,
                tb_colaborador_codigo_interno_colaborador = @CodigoInternoColaborador,
                observacao                                = @Observacao,
                ativo                                     = @Ativo
            WHERE id = @Id AND tb_org_id = @OrgId";

        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            PipelineId = input.AdmissaoPipelineId.ToString(),
            CodigoInternoColaborador = input.CodigoInternoColaborador?.Trim(),
            Observacao = input.Observacao?.Trim(),
            Ativo = input.Ativo ? 1 : 0
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

        const string sql = @"
            UPDATE tb_admissao
            SET ativo = 0
            WHERE id = @Id AND tb_org_id = @OrgId AND ativo = 1";

        var rows = await connection.ExecuteAsync(sql, new { Id = id.ToString(), OrgId = orgId });
        if (rows == 0) return false;

        var estadoNovo = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "DELETE", alteradorCpf, estadoAnterior, estadoNovo);

        return true;
    }

    public async Task<bool> AtualizarStatusAsync(Guid id, Guid novoStatusId, string alteradorCpf, int orgId)
    {
        var connection = _dapperConnection.GetConnection();
        var estadoAnterior = await ObterPorIdAsync(id, orgId);
        if (estadoAnterior == null) return false;

        const string sql = @"
            UPDATE tb_admissao
            SET tb_admissao_status_id = @NovoStatusId
            WHERE id = @Id AND tb_org_id = @OrgId AND ativo = 1";

        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id.ToString(),
            OrgId = orgId,
            NovoStatusId = novoStatusId.ToString()
        });
        if (rows == 0) return false;

        var estadoNovo = await ObterPorIdAsync(id, orgId);
        await InserirLogAsync(connection, id, "STATUS_UPDATE", alteradorCpf, estadoAnterior, estadoNovo);

        return true;
    }

    private static async Task InserirLogAsync(IDbConnection connection, Guid tbAdmissaoId, string acao, string alteradorCpf, AdmissaoResult objeto, AdmissaoResult alteracoes)
    {
        var logId = Guid.NewGuid().ToString();
        var objetoJson = objeto != null ? JsonSerializer.Serialize(objeto, JsonOptions) : "{}";
        var alteracoesJson = alteracoes != null ? JsonSerializer.Serialize(alteracoes, JsonOptions) : "{}";

        const string logSql = @"
            INSERT INTO tb_admissao_log (id, tb_admissao_id, acao, tb_colaborador_codigo_interno_colaborador_alterador, objeto, alteracoes)
            VALUES (@Id, @TbAdmissaoId, @Acao, @AlteradorCodigo, @Objeto, @Alteracoes)";

        await connection.ExecuteAsync(logSql, new
        {
            Id = logId,
            TbAdmissaoId = tbAdmissaoId.ToString(),
            Acao = acao,
            AlteradorCodigo = alteradorCpf ?? "",
            Objeto = objetoJson,
            Alteracoes = alteracoesJson
        });
    }
}
