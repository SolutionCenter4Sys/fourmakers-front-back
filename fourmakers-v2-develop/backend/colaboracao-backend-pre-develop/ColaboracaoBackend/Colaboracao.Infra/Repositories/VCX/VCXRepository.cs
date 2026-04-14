using Colaboracao.Core.Interfaces;
using Core.Domain.VCX;
using Dapper;
using DataTransferObject.Domain.MapaDeRelacionamento.VCX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.VCX;

public class VCXRepository : IVCXRepository
{
    private readonly IDBConnection _dapperConnection;

    public VCXRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    public async Task<IEnumerable<VCXDorDTO>> GetDoresByPosicaoIdAsync(Guid posicaoId)
    {
        var conn = _dapperConnection.GetConnection();

        const string sql = @"
            SELECT d.id AS Id, d.tb_organograma_posicao_id AS OrganogramaPosicaoId, d.titulo AS Titulo, d.descricao AS Descricao,
                   d.data_criacao AS DataCriacao, d.data_alteracao AS DataAlteracao,
                   d.tb_vcx_impactos_id AS VcxImpactosId, d.tb_vcx_urgencias_id AS VcxUrgenciasId,
                   imp.descricao AS VcxImpactosDescricao, urg.descricao AS VcxUrgenciasDescricao
            FROM tb_vcx_dores d
            LEFT JOIN tb_vcx_impactos imp ON imp.id = d.tb_vcx_impactos_id
            LEFT JOIN tb_vcx_urgencias urg ON urg.id = d.tb_vcx_urgencias_id
            WHERE d.tb_organograma_posicao_id = @posicaoId
            ORDER BY d.data_criacao DESC";

        return await conn.QueryAsync<VCXDorDTO>(sql, new { posicaoId });
    }

    public async Task<IEnumerable<VCXIniciativaDTO>> GetIniciativasByPosicaoIdAsync(Guid posicaoId, int orgId)
    {
        var conn = _dapperConnection.GetConnection();

        const string sql = @"
            SELECT i.id AS Id, i.tb_organograma_posicao_id AS OrganogramaPosicaoId, i.titulo AS Titulo, i.descricao AS Descricao,
                   i.data_criacao AS DataCriacao, i.data_alteracao AS DataAlteracao,
                   i.tb_vcx_status_id AS VcxStatusId, i.tb_vcx_temas_id AS VcxTemasId,
                   st.descricao AS VcxStatusDescricao, tem.descricao AS VcxTemasDescricao
            FROM tb_vcx_iniciativas i
            LEFT JOIN tb_vcx_status st ON st.id = i.tb_vcx_status_id
            LEFT JOIN tb_vcx_temas tem ON tem.id = i.tb_vcx_temas_id AND tem.tb_org_id = @orgId
            WHERE i.tb_organograma_posicao_id = @posicaoId
            ORDER BY i.data_criacao DESC";
        
        return await conn.QueryAsync<VCXIniciativaDTO>(sql, new { posicaoId, orgId });
    }

    public async Task<VCXPosicaoDoresIniciativasResultDTO> GetDoresAndIniciativasByPosicaoIdAsync(Guid posicaoId, int orgId)
    {
        var dores = (await GetDoresByPosicaoIdAsync(posicaoId)).ToList();
        var iniciativas = (await GetIniciativasByPosicaoIdAsync(posicaoId, orgId)).ToList();
        
        return new VCXPosicaoDoresIniciativasResultDTO { Dores = dores, Iniciativas = iniciativas };
    }

    public async Task<VCXHistoricoDoresIniciativasResultDTO> GetHistoricoDoresIniciativasByPosicaoIdAsync(Guid posicaoId)
    {
        var conn = _dapperConnection.GetConnection();

        const string sqlDores = @"
            SELECT id AS Id, tb_colaborador_codigo_interno_colaborador_alterador AS ColaboradorCodigoInternoColaboradorAlterador,
                   tb_organograma_posicao_id AS OrganogramaPosicaoId, acao AS Acao, objeto AS Objeto, alteracao AS Alteracao,
                   data_alteracao AS DataAlteracao
            FROM tb_vcx_dores_log
            WHERE tb_organograma_posicao_id = @posicaoId
            ORDER BY data_alteracao DESC";

        const string sqlIniciativas = @"
            SELECT id AS Id, tb_colaborador_codigo_interno_colaborador_alterador AS ColaboradorCodigoInternoColaboradorAlterador,
                   tb_organograma_posicao_id AS OrganogramaPosicaoId, acao AS Acao, objeto AS Objeto, alteracao AS Alteracao,
                   data_alteracao AS DataAlteracao
            FROM tb_vcx_iniciativas_log
            WHERE tb_organograma_posicao_id = @posicaoId
            ORDER BY data_alteracao DESC";

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var doresLogRaw = (await conn.QueryAsync<VCXLogDTO>(sqlDores, new { posicaoId })).ToList();
        var iniciativasLogRaw = (await conn.QueryAsync<VCXLogDTO>(sqlIniciativas, new { posicaoId })).ToList();

        var doresLog = doresLogRaw.Select(r => new VCXDorLogDTO
        {
            Id = r.Id,
            ColaboradorCodigoInternoColaboradorAlterador = r.ColaboradorCodigoInternoColaboradorAlterador,
            OrganogramaPosicaoId = r.OrganogramaPosicaoId,
            Acao = r.Acao,
            Objeto = DeserializeOrNull<VCXDorDTO>(r.Objeto, jsonOptions),
            Alteracao = DeserializeOrNull<VCXDorDTO>(r.Alteracao, jsonOptions),
            DataAlteracao = r.DataAlteracao
        }).ToList();

        var iniciativasLog = iniciativasLogRaw.Select(r => new VCXIniciativaLogDTO
        {
            Id = r.Id,
            ColaboradorCodigoInternoColaboradorAlterador = r.ColaboradorCodigoInternoColaboradorAlterador,
            OrganogramaPosicaoId = r.OrganogramaPosicaoId,
            Acao = r.Acao,
            Objeto = DeserializeOrNull<VCXIniciativaDTO>(r.Objeto, jsonOptions),
            Alteracao = DeserializeOrNull<VCXIniciativaDTO>(r.Alteracao, jsonOptions),
            DataAlteracao = r.DataAlteracao
        }).ToList();

        return new VCXHistoricoDoresIniciativasResultDTO { DoresLog = doresLog, IniciativasLog = iniciativasLog };
    }

    private static T? DeserializeOrNull<T>(string? json, JsonSerializerOptions options) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try { return JsonSerializer.Deserialize<T>(json, options); }
        catch { return null; }
    }

    public async Task<VCXDorDTO?> GetDorByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT d.id AS Id, d.tb_organograma_posicao_id AS OrganogramaPosicaoId, d.titulo AS Titulo, d.descricao AS Descricao,
                   d.data_criacao AS DataCriacao, d.data_alteracao AS DataAlteracao,
                   d.tb_vcx_impactos_id AS VcxImpactosId, d.tb_vcx_urgencias_id AS VcxUrgenciasId,
                   imp.descricao AS VcxImpactosDescricao, urg.descricao AS VcxUrgenciasDescricao
            FROM tb_vcx_dores d
            LEFT JOIN tb_vcx_impactos imp ON imp.id = d.tb_vcx_impactos_id
            LEFT JOIN tb_vcx_urgencias urg ON urg.id = d.tb_vcx_urgencias_id
            WHERE d.id = @id";
        var conn = _dapperConnection.GetConnection();
        return await conn.QuerySingleOrDefaultAsync<VCXDorDTO>(sql, new { id });
    }

    public async Task<Guid> InsertDorAsync(VCXDorInputDTO input)
    {
        var id = Guid.NewGuid();
        const string sql = @"
            INSERT INTO tb_vcx_dores (id, tb_organograma_posicao_id, titulo, descricao, data_criacao, data_alteracao, tb_vcx_impactos_id, tb_vcx_urgencias_id)
            VALUES (@id, @tb_organograma_posicao_id, @titulo, @descricao, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @tb_vcx_impactos_id, @tb_vcx_urgencias_id)";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new
        {
            id,
            tb_organograma_posicao_id = input.OrganogramaPosicaoId,
            titulo = input.Titulo,
            descricao = input.Descricao,
            tb_vcx_impactos_id = input.VcxImpactosId,
            tb_vcx_urgencias_id = input.VcxUrgenciasId
        });
        return id;
    }

    public async Task UpdateDorAsync(Guid id, VCXDorInputDTO input)
    {
        const string sql = @"
            UPDATE tb_vcx_dores SET titulo = @titulo, descricao = @descricao, data_alteracao = CURRENT_TIMESTAMP,
                   tb_vcx_impactos_id = @tb_vcx_impactos_id, tb_vcx_urgencias_id = @tb_vcx_urgencias_id
            WHERE id = @id";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new
        {
            id,
            titulo = input.Titulo,
            descricao = input.Descricao,
            tb_vcx_impactos_id = input.VcxImpactosId,
            tb_vcx_urgencias_id = input.VcxUrgenciasId
        });
    }

    public async Task DeleteDorAsync(Guid id)
    {
        const string sql = "DELETE FROM tb_vcx_dores WHERE id = @id";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new { id });
    }

    public async Task<VCXIniciativaDTO?> GetIniciativaByIdAsync(Guid id, int orgId)
    {
        const string sql = @"
            SELECT i.id AS Id, i.tb_organograma_posicao_id AS OrganogramaPosicaoId, i.titulo AS Titulo, i.descricao AS Descricao,
                   i.data_criacao AS DataCriacao, i.data_alteracao AS DataAlteracao,
                   i.tb_vcx_status_id AS VcxStatusId, i.tb_vcx_temas_id AS VcxTemasId,
                   st.descricao AS VcxStatusDescricao, tem.descricao AS VcxTemasDescricao
            FROM tb_vcx_iniciativas i
            LEFT JOIN tb_vcx_status st ON st.id = i.tb_vcx_status_id
            LEFT JOIN tb_vcx_temas tem ON tem.id = i.tb_vcx_temas_id AND tem.tb_org_id = @orgId
            WHERE i.id = @id";
        var conn = _dapperConnection.GetConnection();
        return await conn.QuerySingleOrDefaultAsync<VCXIniciativaDTO>(sql, new { id, orgId });
    }

    public async Task<Guid> InsertIniciativaAsync(VCXIniciativaInputDTO input)
    {
        var id = Guid.NewGuid();
        const string sql = @"
            INSERT INTO tb_vcx_iniciativas (id, tb_organograma_posicao_id, titulo, descricao, data_criacao, data_alteracao, tb_vcx_status_id, tb_vcx_temas_id)
            VALUES (@id, @tb_organograma_posicao_id, @titulo, @descricao, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @tb_vcx_status_id, @tb_vcx_temas_id)";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new
        {
            id,
            tb_organograma_posicao_id = input.OrganogramaPosicaoId,
            titulo = input.Titulo,
            descricao = input.Descricao,
            tb_vcx_status_id = input.VcxStatusId,
            tb_vcx_temas_id = input.VcxTemasId
        });
        return id;
    }

    public async Task UpdateIniciativaAsync(Guid id, VCXIniciativaInputDTO input)
    {
        const string sql = @"
            UPDATE tb_vcx_iniciativas SET titulo = @titulo, descricao = @descricao, data_alteracao = CURRENT_TIMESTAMP,
                   tb_vcx_status_id = @tb_vcx_status_id, tb_vcx_temas_id = @tb_vcx_temas_id
            WHERE id = @id";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new
        {
            id,
            titulo = input.Titulo,
            descricao = input.Descricao,
            tb_vcx_status_id = input.VcxStatusId,
            tb_vcx_temas_id = input.VcxTemasId
        });
    }

    public async Task DeleteIniciativaAsync(Guid id)
    {
        const string sql = "DELETE FROM tb_vcx_iniciativas WHERE id = @id";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new { id });
    }

    public async Task InserirDoresLogAsync(VCXLogDTO log)
    {
        var id = log.Id == Guid.Empty ? Guid.NewGuid() : log.Id;
        const string sql = @"
            INSERT INTO tb_vcx_dores_log (id, tb_colaborador_codigo_interno_colaborador_alterador, tb_organograma_posicao_id, acao, objeto, alteracao, data_alteracao)
            VALUES (@id, @tb_colaborador_codigo_interno_colaborador_alterador, @tb_organograma_posicao_id, @acao, @objeto, @alteracao, CURRENT_TIMESTAMP)";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new
        {
            id,
            tb_colaborador_codigo_interno_colaborador_alterador = log.ColaboradorCodigoInternoColaboradorAlterador,
            tb_organograma_posicao_id = log.OrganogramaPosicaoId,
            acao = log.Acao,
            objeto = log.Objeto,
            alteracao = log.Alteracao
        });
    }

    public async Task InserirIniciativasLogAsync(VCXLogDTO log)
    {
        var id = log.Id == Guid.Empty ? Guid.NewGuid() : log.Id;
        const string sql = @"
            INSERT INTO tb_vcx_iniciativas_log (id, tb_colaborador_codigo_interno_colaborador_alterador, tb_organograma_posicao_id, acao, objeto, alteracao, data_alteracao)
            VALUES (@id, @tb_colaborador_codigo_interno_colaborador_alterador, @tb_organograma_posicao_id, @acao, @objeto, @alteracao, CURRENT_TIMESTAMP)";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new
        {
            id,
            tb_colaborador_codigo_interno_colaborador_alterador = log.ColaboradorCodigoInternoColaboradorAlterador,
            tb_organograma_posicao_id = log.OrganogramaPosicaoId,
            acao = log.Acao,
            objeto = log.Objeto,
            alteracao = log.Alteracao
        });
    }

    public async Task<IEnumerable<VCXNotaBastidoresDTO>> GetNotasBastidoresByPosicaoIdAsync(Guid posicaoId, int orgId)
    {
        var conn = _dapperConnection.GetConnection();
        const string sql = @"
            SELECT n.id AS Id, n.tb_organograma_posicao_id AS OrganogramaPosicaoId, n.descricao AS Descricao,
                   n.data_criacao AS DataCriacao, n.data_alteracao AS DataAlteracao
            FROM tb_vcx_notas_bastidores n
            INNER JOIN tb_organograma_posicao op ON op.id = n.tb_organograma_posicao_id AND op.tb_org_id = @orgId
            WHERE n.tb_organograma_posicao_id = @posicaoId
            ORDER BY n.data_criacao DESC";
        return await conn.QueryAsync<VCXNotaBastidoresDTO>(sql, new { posicaoId, orgId });
    }

    public async Task<VCXNotaBastidoresDTO?> GetNotaBastidoresByIdAsync(Guid id, int orgId)
    {
        const string sql = @"
            SELECT n.id AS Id, n.tb_organograma_posicao_id AS OrganogramaPosicaoId, n.descricao AS Descricao,
                   n.data_criacao AS DataCriacao, n.data_alteracao AS DataAlteracao
            FROM tb_vcx_notas_bastidores n
            INNER JOIN tb_organograma_posicao op ON op.id = n.tb_organograma_posicao_id AND op.tb_org_id = @orgId
            WHERE n.id = @id";
        var conn = _dapperConnection.GetConnection();
        return await conn.QuerySingleOrDefaultAsync<VCXNotaBastidoresDTO>(sql, new { id, orgId });
    }

    public async Task<Guid> InsertNotaBastidoresAsync(VCXNotaBastidoresCriarInputDTO input)
    {
        var id = Guid.NewGuid();
        const string sql = @"
            INSERT INTO tb_vcx_notas_bastidores (id, tb_organograma_posicao_id, descricao, data_criacao, data_alteracao)
            VALUES (@id, @tb_organograma_posicao_id, @descricao, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new
        {
            id,
            tb_organograma_posicao_id = input.OrganogramaPosicaoId,
            descricao = input.Descricao
        });
        return id;
    }

    public async Task UpdateNotaBastidoresAsync(Guid id, VCXNotaBastidoresAtualizarInputDTO input)
    {
        const string sql = @"
            UPDATE tb_vcx_notas_bastidores SET descricao = @descricao, data_alteracao = CURRENT_TIMESTAMP
            WHERE id = @id";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new
        {
            id,
            descricao = input.Descricao
        });
    }

    public async Task DeleteNotaBastidoresAsync(Guid id)
    {
        const string sql = "DELETE FROM tb_vcx_notas_bastidores WHERE id = @id";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new { id });
    }

    public async Task InserirNotasBastidoresLogAsync(VCXLogDTO log)
    {
        var id = log.Id == Guid.Empty ? Guid.NewGuid() : log.Id;
        const string sql = @"
            INSERT INTO tb_vcx_notas_bastidores_log (id, tb_colaborador_codigo_interno_colaborador_alterador, tb_organograma_posicao_id, acao, objeto, alteracao, data_alteracao)
            VALUES (@id, @tb_colaborador_codigo_interno_colaborador_alterador, @tb_organograma_posicao_id, @acao, @objeto, @alteracao, CURRENT_TIMESTAMP)";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new
        {
            id,
            tb_colaborador_codigo_interno_colaborador_alterador = log.ColaboradorCodigoInternoColaboradorAlterador,
            tb_organograma_posicao_id = log.OrganogramaPosicaoId,
            acao = log.Acao,
            objeto = log.Objeto,
            alteracao = log.Alteracao
        });
    }

    public async Task<IEnumerable<VCXNotaBastidoresLogDTO>> GetHistoricoNotasBastidoresByPosicaoIdAsync(Guid posicaoId)
    {
        var conn = _dapperConnection.GetConnection();
        const string sql = @"
            SELECT id AS Id, tb_colaborador_codigo_interno_colaborador_alterador AS ColaboradorCodigoInternoColaboradorAlterador,
                   tb_organograma_posicao_id AS OrganogramaPosicaoId, acao AS Acao, objeto AS Objeto, alteracao AS Alteracao,
                   data_alteracao AS DataAlteracao
            FROM tb_vcx_notas_bastidores_log
            WHERE tb_organograma_posicao_id = @posicaoId
            ORDER BY data_alteracao DESC";
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var raw = (await conn.QueryAsync<VCXLogDTO>(sql, new { posicaoId })).ToList();
        return raw.Select(r => new VCXNotaBastidoresLogDTO
        {
            Id = r.Id,
            ColaboradorCodigoInternoColaboradorAlterador = r.ColaboradorCodigoInternoColaboradorAlterador,
            OrganogramaPosicaoId = r.OrganogramaPosicaoId,
            Acao = r.Acao,
            Objeto = DeserializeOrNull<VCXNotaBastidoresDTO>(r.Objeto, jsonOptions),
            Alteracao = DeserializeOrNull<VCXNotaBastidoresDTO>(r.Alteracao, jsonOptions),
            DataAlteracao = r.DataAlteracao
        });
    }

    public async Task<IEnumerable<VCXTemaDTO>> GetTemasByOrgIdAsync(int orgId, string? descricao = null)
    {
        var conn = _dapperConnection.GetConnection();

        var sql = @"
        SELECT id AS Id, descricao AS Descricao, tb_org_id AS OrgId
        FROM tb_vcx_temas
        WHERE tb_org_id = @orgId
        AND (@descricao IS NULL OR descricao LIKE @descricaoPattern)";

        return await conn.QueryAsync<VCXTemaDTO>(sql, new
        {
            orgId,
            descricao,
            descricaoPattern = descricao is null ? null : $"%{descricao.Trim()}%"
        });
    }

    public async Task<Guid> InsertTemaAsync(string descricao, int orgId)
    {
        var id = Guid.NewGuid();
        const string sql = @"
            INSERT INTO tb_vcx_temas (id, descricao, tb_org_id)
            VALUES (@id, @descricao, @tb_org_id)";
        var conn = _dapperConnection.GetConnection();
        await conn.ExecuteAsync(sql, new { id, descricao, tb_org_id = orgId });
        return id;
    }

    public async Task<IEnumerable<VCXImpactoDTO>> GetImpactosAsync()
    {
        const string sql = @"
            SELECT id AS Id, descricao AS Descricao
            FROM tb_vcx_impactos
            ORDER BY descricao";
        var conn = _dapperConnection.GetConnection();
        return await conn.QueryAsync<VCXImpactoDTO>(sql);
    }

    public async Task<IEnumerable<VCXUrgenciaDTO>> GetUrgenciasAsync()
    {
        const string sql = @"
            SELECT id AS Id, descricao AS Descricao
            FROM tb_vcx_urgencias
            ORDER BY descricao";
        var conn = _dapperConnection.GetConnection();
        return await conn.QueryAsync<VCXUrgenciaDTO>(sql);
    }

    public async Task<IEnumerable<VCXStatusDTO>> GetStatusAsync()
    {
        const string sql = @"
            SELECT id AS Id, descricao AS Descricao
            FROM tb_vcx_status
            ORDER BY descricao";
        var conn = _dapperConnection.GetConnection();
        return await conn.QueryAsync<VCXStatusDTO>(sql);
    }

    public async Task<VCXAgendasPorColaboradorClienteResultDTO> GetAgendasPorColaboradorClienteAsync(string codigoColaborador, string codigoCliente, int cursor, int limit)
    {
        try
        {
            const string sql = """
            SELECT
                tae.id AS Id,
                tae.graph_event_id AS GraphEventId,
                tae.tb_colaborador_codigo_interno_colaborador AS CodColaboradorCriador,
                tc.nome_completo AS NomeCompletoColaboradorCriador,
                tae.tb_cliente_org_codigo_cliente AS CodigoCliente,
                tae.tb_tipo_agenda_id AS TipoInteracao,
                tae.data_criacao AS DataCriacao,
                tae.data_atualizacao AS DataAtualizacao,
                tae.data_agendada AS DataAgendada,
                tae.data_inicio AS DataInicio,
                tae.data_fim AS DataFim,
                tae.status AS Status,
                tae.localizacao AS Localizacao,
                tae.link_reuniao AS LinkReuniao,
                (
                    SELECT COUNT(*) FROM tb_agenda_convidados ap WHERE ap.tb_agendas_comerciais_id = tae.id
                ) + (
                    SELECT COUNT(*) FROM tb_participantes_externo pe WHERE pe.tb_agendas_comerciais_id = tae.id
                ) AS QuantidadeParticipantes,
                tae.titulo AS Titulo,
                tae.descricao AS Descricao,
                tae.agenda_pai_id AS AgendaPaiId
            FROM tb_agendas_comerciais tae
            INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
            WHERE tae.tb_cliente_org_codigo_cliente = @CodigoCliente
            AND (
                  EXISTS (
                      SELECT 1 FROM tb_agenda_convidados ap
                      WHERE ap.tb_agendas_comerciais_id = tae.id
                        AND ap.codigo_colaborador_interno_externo = @CodigoColaborador
                        AND ap.tipo_codigo = 1 
                  )
                  OR EXISTS (
                      SELECT 1 FROM tb_agenda_convidados ap
                      INNER JOIN tb_gestor_externo ge ON ge.cod_gestor_externo = ap.codigo_colaborador_interno_externo
                      WHERE ap.tb_agendas_comerciais_id = tae.id
                        AND ap.tipo_codigo = 2 
                        AND ge.codigo_interno_colaborador = @CodigoColaborador
                  )
              )
            ORDER BY tae.data_agendada DESC
            LIMIT @Limite OFFSET @Cursor
            """;

            var conn = _dapperConnection.GetConnection();
            var lista = (await conn.QueryAsync<VCXAgendaItemDTO>(sql, new
            {
                CodigoColaborador = codigoColaborador,
                CodigoCliente = codigoCliente,
                Limite = limit,
                Cursor = cursor
            })).ToList();

            var agora = DateTime.Now;
            var agendasAntigas = lista.Where(a => a.DataAgendada.HasValue && a.DataAgendada.Value < agora).ToList();
            var agendasNovas = lista.Where(a => !a.DataAgendada.HasValue || a.DataAgendada.Value >= agora).ToList();

            return new VCXAgendasPorColaboradorClienteResultDTO
            {
                AgendasAntigas = agendasAntigas,
                AgendasNovas = agendasNovas
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao retornar agenda. ERRO: {ex.Message}.");
        }
    }

    public async Task<List<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>> PosicaoOrcamentoHistoricoListarPorPosicao(string posicaoId)
    {
        var conn = _dapperConnection.GetConnection();
        const string sql = @"
                SELECT CAST(historico_orcamento.id AS CHAR(36)) AS Id,
                       CAST(historico_orcamento.tb_organograma_posicao_id AS CHAR(36)) AS OrganogramaPosicaoId,
                       historico_orcamento.tb_org_id AS OrgId,
                       historico_orcamento.orcamento AS Orcamento,
                       historico_orcamento.data_inicio AS DataInicio,
                       historico_orcamento.data_fim AS DataFim,
                       historico_orcamento.codigo_interno_colaborador_alterador AS CodigoInternoColaboradorAlterador,
                       colab_alterador.nome_completo AS NomeColaboradorAlterador,
                       historico_orcamento.data_criacao AS DataCriacao
                FROM tb_organograma_posicao_orcamento historico_orcamento
                LEFT JOIN tb_colaborador colab_alterador
                    ON colab_alterador.codigo_interno_colaborador = historico_orcamento.codigo_interno_colaborador_alterador
                WHERE historico_orcamento.tb_organograma_posicao_id = @PosicaoId
                ORDER BY historico_orcamento.data_criacao DESC, historico_orcamento.id DESC;";
        return (await conn.QueryAsync<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>(sql, new { PosicaoId = posicaoId })).ToList();
    }

    public async Task<OrganogramaPosicaoOrcamentoHistoricoResponseDTO?> PosicaoOrcamentoHistoricoObterUltimoPorPosicao(string posicaoId)
    {
        var conn = _dapperConnection.GetConnection();
        const string sql = @"
                SELECT CAST(historico_orcamento.id AS CHAR(36)) AS Id,
                       CAST(historico_orcamento.tb_organograma_posicao_id AS CHAR(36)) AS OrganogramaPosicaoId,
                       historico_orcamento.tb_org_id AS OrgId,
                       historico_orcamento.orcamento AS Orcamento,
                       historico_orcamento.data_inicio AS DataInicio,
                       historico_orcamento.data_fim AS DataFim,
                       historico_orcamento.codigo_interno_colaborador_alterador AS CodigoInternoColaboradorAlterador,
                       colab_alterador.nome_completo AS NomeColaboradorAlterador,
                       historico_orcamento.data_criacao AS DataCriacao
                FROM tb_organograma_posicao_orcamento historico_orcamento
                LEFT JOIN tb_colaborador colab_alterador
                    ON colab_alterador.codigo_interno_colaborador = historico_orcamento.codigo_interno_colaborador_alterador
                WHERE historico_orcamento.tb_organograma_posicao_id = @PosicaoId
                ORDER BY historico_orcamento.data_criacao DESC, historico_orcamento.id DESC
                LIMIT 1;";
        return await conn.QuerySingleOrDefaultAsync<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>(sql, new { PosicaoId = posicaoId });
    }

    public async Task InserirHistoricoOrcamentoPosicao(string id, string posicaoId, int orgId, decimal orcamento, DateTime? dataInicio, DateTime? dataFim, string codigoInternoColaboradorAlterador)
    {
        var conn = _dapperConnection.GetConnection();
        const string qIns = @"
                INSERT INTO tb_organograma_posicao_orcamento
                    (id, tb_organograma_posicao_id, tb_org_id, orcamento, data_inicio, data_fim, codigo_interno_colaborador_alterador, data_criacao)
                VALUES
                    (@Id, @PosicaoId, @OrgId, @Orcamento, @DataInicio, @DataFim, @CodigoInternoColaboradorAlterador, NOW());";

        await conn.ExecuteAsync(qIns, new
        {
            Id = id,
            PosicaoId = posicaoId,
            OrgId = orgId,
            Orcamento = orcamento,
            DataInicio = dataInicio.HasValue ? dataInicio.Value.Date : (DateTime?)null,
            DataFim = dataFim.HasValue ? dataFim.Value.Date : (DateTime?)null,
            CodigoInternoColaboradorAlterador = codigoInternoColaboradorAlterador
        });
    }
}
