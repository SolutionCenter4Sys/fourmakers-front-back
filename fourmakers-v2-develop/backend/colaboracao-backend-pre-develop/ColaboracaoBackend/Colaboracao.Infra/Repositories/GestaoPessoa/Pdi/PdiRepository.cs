using Colaboracao.Core.Interfaces;
using Core.Domain.GestaoPessoa.Pdi;
using Dapper;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.GestaoPessoa.Pdi
{
    public class PdiRepository : IPdiRepository
    {
        private readonly IDBConnection _connection;

        /// <summary>Nome do titular e um gestor (MIN) por subordinado na org; reutilizado nas listagens de métricas.</summary>
        private const string SqlLeftJoinTitularENomeEGestorParaPdi = @"
                LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_interno_colaborador
                LEFT JOIN (
                    SELECT codigo_interno_colaborador_subordinado, MIN(codigo_interno_colaborador_gestor) AS codigo_interno_colaborador_gestor
                    FROM vw_gestores_colaboradores_org
                    WHERE tb_org_id = @OrgId
                    GROUP BY codigo_interno_colaborador_subordinado
                ) vgest ON vgest.codigo_interno_colaborador_subordinado = p.codigo_interno_colaborador
                LEFT JOIN tb_colaborador tg ON tg.codigo_interno_colaborador = vgest.codigo_interno_colaborador_gestor";

        public PdiRepository(IDBConnection connection)
        {
            _connection = connection;
        }

        /// <summary>Converte valor vindo do MySQL (string, byte[] BINARY(16), Guid) para Guid.</summary>
        private static Guid ObjectToGuid(object value)
        {
            if (value == null || value is DBNull) return Guid.Empty;
            if (value is Guid g) return g;
            if (value is string s && Guid.TryParse(s, out var parsed)) return parsed;
            if (value is byte[] bytes && bytes.Length == 16) return new Guid(bytes);
            return Guid.Empty;
        }

        /// <summary>Converte valor vindo do MySQL (string, byte[] BINARY(16), Guid) para string (UUID).</summary>
        private static string ObjectToIdString(object value)
        {
            if (value == null || value is DBNull) return null;
            if (value is string s) return s;
            if (value is Guid g) return g.ToString();
            if (value is byte[] bytes && bytes.Length == 16) return new Guid(bytes).ToString();
            return value?.ToString();
        }

        /// <summary>Converte valor vindo do MySQL para string (evita IConvertible quando o driver retorna byte[] ou outro tipo).</summary>
        private static string ObjectToString(object value)
        {
            if (value == null || value is DBNull) return null;
            if (value is string s) return s;
            if (value is byte[] bytes) return bytes.Length == 16 ? new Guid(bytes).ToString() : System.Text.Encoding.UTF8.GetString(bytes);
            return value.ToString();
        }

        /// <summary>Converte valor vindo do MySQL para DateTime (não nulo).</summary>
        private static DateTime ObjectToDateTime(object value)
        {
            var d = ParseDateTime(value);
            return d ?? default;
        }

        public async Task<IEnumerable<PdiResumoDTO>> ListarMeusPdisAsync(string codigoInternoColaborador, int orgId)
        {
            var conn = _connection.GetConnection();

            var pdiRows = (await conn.QueryAsync<PdiRow>(@"
                SELECT p.id AS IdRaw, p.codigo_interno_colaborador AS ColaboradorId, tc.nome_completo AS NomeCompletoColaborador,
                    p.titulo AS Titulo, p.descricao AS Descricao, p.status AS Status, p.data_criacao AS DataCriacao, p.data_atualizacao AS DataAtualizacao,
                    p.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao, p.codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao, p.dead_line AS DeadLine
                FROM tb_pdi p
                LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_interno_colaborador
                WHERE p.codigo_interno_colaborador = @CodigoInternoColaborador
                ORDER BY p.data_criacao DESC",
                new { CodigoInternoColaborador = codigoInternoColaborador })).ToList();

            if (pdiRows.Count == 0)
                return Array.Empty<PdiResumoDTO>();

            var pdiIds = pdiRows.Select(p => ObjectToGuid(p.IdRaw)).ToList();
            var skills = await ObterSkillsPorPdiIdsAsync(conn, pdiIds);
            var actionPlans = await ObterActionPlansPorPdiIdsAsync(conn, pdiIds);

            var result = new List<PdiResumoDTO>();
            foreach (var p in pdiRows)
            {
                var pIdStr = ObjectToIdString(p.IdRaw);
                var plans = actionPlans.Where(a => ObjectToIdString(a.PdiId) == pIdStr).ToList();
                var total = plans.Count;
                var concluidos = plans.Count(a => ParseDateTime(a.ConcluidoEmRaw).HasValue);
                var progress = total > 0 ? (double)concluidos / total : 0d;
                var status = progress >= 1.0 ? "COMPLETED" : ObjectToString(p.Status);

                result.Add(new PdiResumoDTO
                {
                    Id = ObjectToGuid(p.IdRaw),
                    ColaboradorId = ObjectToString(p.ColaboradorId),
                    NomeColaborador = ObjectToString(p.NomeCompletoColaborador),
                    Titulo = ObjectToString(p.Titulo),
                    Descricao = ObjectToString(p.Descricao),
                    Status = status,
                    DataCriacao = ObjectToDateTime(p.DataCriacao),
                    DataAtualizacao = ParseDateTime(p.DataAtualizacao),
                    CodigoInternoColaboradorCriacao = ObjectToString(p.CodigoInternoColaboradorCriacao),
                    CodigoInternoColaboradorAlteracao = ObjectToString(p.CodigoInternoColaboradorAlteracao),
                    Progress = progress,
                    DeadLine = ObjectToDateTime(p.DeadLine),
                    Skills = skills.Where(s => ObjectToIdString(s.PdiId) == pIdStr).Select(s => new PdiSkillDTO { Id = ObjectToGuid(s.IdRaw), NomeSkill = s.NomeSkill, CodigoSkill = s.CodigoSkill }).ToList(),
                    ActionPlans = plans.Select(a => new PdiActionPlanDTO { Id = ObjectToGuid(a.IdRaw), Description = a.Description, Deadline = ParseDateTime(a.DeadlineRaw), ConcluidoEm = ParseDateTime(a.ConcluidoEmRaw), CodigoInternoColaboradorCriacao = ObjectToString(a.CodigoInternoColaboradorCriacao), CodigoInternoColaboradorAlteracao = ObjectToString(a.CodigoInternoColaboradorAlteracao) }).ToList()
                });
            }
            return result;
        }

        public async Task<IEnumerable<PdiResumoDTO>> ListarPdisPorColaboradorIdAsync(string colaboradorId, int orgId)
        {
            return await ListarMeusPdisAsync(colaboradorId, orgId);
        }

        public async Task<PdiResumoDTO> ObterPdiCompletoPorIdAsync(Guid pdiId, string codigoInternoColaborador, int orgId)
        {
            var conn = _connection.GetConnection();
            var pdiIdStr = pdiId.ToString();
            var p = await conn.QueryFirstOrDefaultAsync<PdiRow>(@"
                SELECT p.id AS IdRaw, p.codigo_interno_colaborador AS ColaboradorId, tc.nome_completo AS NomeCompletoColaborador,
                    p.titulo AS Titulo, p.descricao AS Descricao, p.status AS Status, p.data_criacao AS DataCriacao, p.data_atualizacao AS DataAtualizacao,
                    p.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao, p.codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao,
                    p.dead_line AS DeadLine
                FROM tb_pdi p
                LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_interno_colaborador
                WHERE p.id = @PdiId AND p.codigo_interno_colaborador = @CodigoInternoColaborador",
                new { PdiId = pdiIdStr, CodigoInternoColaborador = codigoInternoColaborador });
            if (p == null) return null;

            var skills = (await conn.QueryAsync<PdiSkillRow>(@"
                SELECT id AS IdRaw, pdi_id AS PdiId, nome_skill AS NomeSkill, codigo_skill AS CodigoSkill
                FROM tb_pdi_skill WHERE pdi_id = @PdiId", new { PdiId = pdiIdStr })).ToList();
            var plans = (await conn.QueryAsync<ActionPlanRow>(@"
                SELECT id AS IdRaw, pdi_id AS PdiId, description AS Description, deadline AS DeadlineRaw, concluido_em AS ConcluidoEmRaw,
                    codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao, codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao
                FROM tb_pdi_plano_acao WHERE pdi_id = @PdiId", new { PdiId = pdiIdStr })).ToList();

            var total = plans.Count;
            var concluidos = plans.Count(a => ParseDateTime(a.ConcluidoEmRaw).HasValue);
            var progress = total > 0 ? (double)concluidos / total : 0d;
            var status = progress >= 1.0 ? "COMPLETED" : ObjectToString(p.Status);

            return new PdiResumoDTO
            {
                Id = ObjectToGuid(p.IdRaw),
                ColaboradorId = ObjectToString(p.ColaboradorId),
                NomeColaborador = ObjectToString(p.NomeCompletoColaborador),
                Titulo = ObjectToString(p.Titulo),
                Descricao = ObjectToString(p.Descricao),
                Status = status,
                DataCriacao = ObjectToDateTime(p.DataCriacao),
                DataAtualizacao = ParseDateTime(p.DataAtualizacao),
                CodigoInternoColaboradorCriacao = ObjectToString(p.CodigoInternoColaboradorCriacao),
                CodigoInternoColaboradorAlteracao = ObjectToString(p.CodigoInternoColaboradorAlteracao),
                Progress = progress,
                DeadLine = ParseDateTime(p.DeadLine),
                Skills = skills.Select(s => new PdiSkillDTO { Id = ObjectToGuid(s.IdRaw), NomeSkill = s.NomeSkill, CodigoSkill = s.CodigoSkill }).ToList(),
                ActionPlans = plans.Select(a => new PdiActionPlanDTO { Id = ObjectToGuid(a.IdRaw), Description = a.Description, Deadline = ParseDateTime(a.DeadlineRaw), ConcluidoEm = ParseDateTime(a.ConcluidoEmRaw), CodigoInternoColaboradorCriacao = ObjectToString(a.CodigoInternoColaboradorCriacao), CodigoInternoColaboradorAlteracao = ObjectToString(a.CodigoInternoColaboradorAlteracao) }).ToList()
            };
        }

        public async Task<Guid> InserirPdiAsync(string codigoInternoColaborador, int orgId, PdiCriarRequestDTO request, string codigoInternoCriacao = null, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _connection.GetConnection();
            var pdiId = Guid.NewGuid();
            const string statusInicial = "NOT_STARTED";
            var criacao = string.IsNullOrEmpty(codigoInternoCriacao) ? codigoInternoColaborador : codigoInternoCriacao;

            var deadLinePdi = request.DeadLine;
            if (!deadLinePdi.HasValue && request.ActionPlans != null && request.ActionPlans.Count > 0)
            {
                var deadlines = request.ActionPlans
                    .Where(a => a?.Deadline.HasValue == true)
                    .Select(a => a.Deadline.Value)
                    .ToList();
                if (deadlines.Count > 0)
                    deadLinePdi = deadlines.Max();
            }

            await conn.ExecuteAsync(@"
                INSERT INTO tb_pdi (id, codigo_interno_colaborador, titulo, descricao, data_criacao, status, codigo_interno_colaborador_criacao, dead_line, tb_org_id)
                VALUES (@Id, @ColaboradorId, @Titulo, @Descricao, NOW(), @Status, @Criacao, @DeadLine, @OrgId)",
                new { Id = pdiId, ColaboradorId = codigoInternoColaborador, request.Titulo, request.Descricao, Status = statusInicial, Criacao = criacao, DeadLine = deadLinePdi, OrgId = orgId }, transaction);

            foreach (var s in request.Skills ?? new List<PdiSkillInputDTO>())
            {
                var skillId = Guid.NewGuid();
                await conn.ExecuteAsync(@"
                    INSERT INTO tb_pdi_skill (id, pdi_id, nome_skill, codigo_skill)
                    VALUES (@Id, @PdiId, @NomeSkill, @CodigoSkill)",
                    new { Id = skillId, PdiId = pdiId, s.NomeSkill, s.CodigoSkill }, transaction);
            }

            foreach (var ap in request.ActionPlans ?? new List<PdiActionPlanInputDTO>())
            {
                var apId = Guid.NewGuid();
                await conn.ExecuteAsync(@"
                    INSERT INTO tb_pdi_plano_acao (id, pdi_id, description, deadline, created_at, codigo_interno_colaborador_criacao)
                    VALUES (@Id, @PdiId, @Description, @Deadline, NOW(), @CodigoCriacao)",
                    new { Id = apId, PdiId = pdiId, ap.Description, ap.Deadline, CodigoCriacao = codigoInternoColaborador }, transaction);
            }

            return pdiId;
        }

        public async Task<(string ColaboradorId, string CriadoPor, string Status)> ObterCriadorPdiAsync(Guid pdiId)
        {
            var conn = _connection.GetConnection();
            var pdiIdStr = pdiId.ToString();
            var row = await conn.QueryFirstOrDefaultAsync<CriadorPdiRow>(@"
                SELECT codigo_interno_colaborador AS ColaboradorId, codigo_interno_colaborador_criacao AS CriadoPor, status AS Status
                FROM tb_pdi WHERE id = @PdiId", new { PdiId = pdiIdStr });
            if (row == null) return (null, null, null);
            return (ObjectToString(row.ColaboradorId), ObjectToString(row.CriadoPor), ObjectToString(row.Status));
        }

        /// <summary>Normaliza status do PDI para o valor em inglês persistido no banco. Sempre gravar em inglês.</summary>
        private static string NormalizarStatusPdi(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return null;
            var s = status.Trim();
            if (string.Equals(s, "NOT_STARTED", StringComparison.OrdinalIgnoreCase)) return "NOT_STARTED";
            if (string.Equals(s, "IN_ANALYSIS", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "em_analise", StringComparison.OrdinalIgnoreCase)) return "IN_ANALYSIS";
            if (string.Equals(s, "IN_PROGRESS", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "em_andamento", StringComparison.OrdinalIgnoreCase)) return "IN_PROGRESS";
            if (string.Equals(s, "COMPLETED", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "finalizado", StringComparison.OrdinalIgnoreCase)) return "COMPLETED";
            if (string.Equals(s, "CANCELLED", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "cancelado", StringComparison.OrdinalIgnoreCase)) return "CANCELLED";
            return s; // valor já em inglês ou desconhecido
        }

        public async Task<bool> AtualizarStatusPdiAsync(Guid pdiId, string status, string codigoInternoAlteracao, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _connection.GetConnection();
            var statusNorm = NormalizarStatusPdi(status) ?? status;
            var rows = await conn.ExecuteAsync(@"
                UPDATE tb_pdi SET status = @Status, data_atualizacao = NOW(), codigo_interno_colaborador_alteracao = @CodigoAlteracao
                WHERE id = @PdiId", new { PdiId = pdiId, Status = statusNorm, CodigoAlteracao = codigoInternoAlteracao }, transaction);
            return rows > 0;
        }

        public async Task<bool> AtualizarPdiAsync(Guid pdiId, string codigoInternoColaborador, PdiAtualizarRequestDTO request, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _connection.GetConnection();
            var statusNorm = request?.Status != null ? NormalizarStatusPdi(request.Status) : null;
            var rows = await conn.ExecuteAsync(@"
                UPDATE tb_pdi SET titulo = @Titulo, descricao = @Descricao,
                    status = COALESCE(@Status, status), data_atualizacao = NOW(), codigo_interno_colaborador_alteracao = @ColaboradorId
                WHERE id = @PdiId AND codigo_interno_colaborador = @ColaboradorId",
                new { request.Titulo, request.Descricao, Status = statusNorm, PdiId = pdiId, ColaboradorId = codigoInternoColaborador }, transaction);
            return rows > 0;
        }

        public async Task<Guid> InserirActionPlanAsync(Guid pdiId, string codigoInternoColaborador, PdiActionPlanInputDTO request, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _connection.GetConnection();
            var id = Guid.NewGuid();
            await conn.ExecuteAsync(@"
                INSERT INTO tb_pdi_plano_acao (id, pdi_id, description, deadline, created_at, codigo_interno_colaborador_criacao)
                VALUES (@Id, @PdiId, @Description, @Deadline, NOW(), @CodigoCriacao)",
                new { Id = id, PdiId = pdiId, request.Description, request.Deadline, CodigoCriacao = codigoInternoColaborador }, transaction);
            return id;
        }

        public async Task<bool> AtualizarActionPlanAsync(Guid pdiId, Guid actionPlanId, string codigoInternoColaborador, PdiActionPlanInputDTO request, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _connection.GetConnection();
            var rows = await conn.ExecuteAsync(@"
                UPDATE tb_pdi_plano_acao SET description = @Description, deadline = @Deadline, codigo_interno_colaborador_alteracao = @ColaboradorId
                WHERE id = @ActionPlanId AND pdi_id = @PdiId
                AND EXISTS (SELECT 1 FROM tb_pdi WHERE id = @PdiId AND codigo_interno_colaborador = @ColaboradorId)",
                new { request.Description, request.Deadline, ColaboradorId = codigoInternoColaborador, ActionPlanId = actionPlanId, PdiId = pdiId }, transaction);
            return rows > 0;
        }

        public async Task<bool> ExcluirActionPlanAsync(Guid pdiId, Guid actionPlanId, string codigoInternoColaborador, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _connection.GetConnection();
            var rows = await conn.ExecuteAsync(@"
                DELETE FROM tb_pdi_plano_acao
                WHERE id = @ActionPlanId AND pdi_id = @PdiId
                AND EXISTS (SELECT 1 FROM tb_pdi WHERE id = @PdiId AND codigo_interno_colaborador = @ColaboradorId)",
                new { ActionPlanId = actionPlanId, PdiId = pdiId, ColaboradorId = codigoInternoColaborador }, transaction);
            return rows > 0;
        }

        public async Task<bool> ConcluirActionPlanAsync(Guid pdiId, Guid actionPlanId, string codigoInternoColaborador, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _connection.GetConnection();
            var rows = await conn.ExecuteAsync(@"
                UPDATE tb_pdi_plano_acao SET concluido_em = NOW(), codigo_interno_colaborador_alteracao = @ColaboradorId
                WHERE id = @ActionPlanId AND pdi_id = @PdiId
                AND EXISTS (SELECT 1 FROM tb_pdi WHERE id = @PdiId AND codigo_interno_colaborador = @ColaboradorId)",
                new { ActionPlanId = actionPlanId, PdiId = pdiId, ColaboradorId = codigoInternoColaborador }, transaction);
            if (rows == 0) return false;

            var total = await ContarActionPlansAsync(pdiId);
            var concluidos = await ContarActionPlansConcluidosAsync(pdiId);
            if (total > 0 && concluidos >= total)
                await conn.ExecuteAsync("UPDATE tb_pdi SET status = 'COMPLETED', data_atualizacao = NOW(), codigo_interno_colaborador_alteracao = @ColaboradorId WHERE id = @PdiId", new { PdiId = pdiId, ColaboradorId = codigoInternoColaborador }, transaction);
            return true;
        }

        public async Task<Guid> InserirEvidenciaAsync(Guid pdiId, string docName, string docPath, string docMime, long? docSize, string tipo, byte[] docBytes, string link, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _connection.GetConnection();
            var id = Guid.NewGuid();
            await conn.ExecuteAsync(@"
                INSERT INTO tb_pdi_ativos (id, pdi_id, created_at, doc_name, doc_path, doc_mime, doc_size, tipo, doc, link)
                VALUES (@Id, @PdiId, NOW(), @DocName, @DocPath, @DocMime, @DocSize, @Tipo, @Doc, @Link)",
                new { Id = id, PdiId = pdiId, DocName = docName, DocPath = docPath, DocMime = docMime, DocSize = docSize, Tipo = tipo ?? "EVIDENCIA", Doc = docBytes, Link = link }, transaction);
            return id;
        }

        public async Task<IEnumerable<PdiEvidenciaDTO>> ListarEvidenciasAsync(Guid pdiId, string codigoInternoColaborador)
        {
            var conn = _connection.GetConnection();
            return await conn.QueryAsync<PdiEvidenciaDTO>(@"
                SELECT a.id AS Id, a.pdi_id AS PdiId, a.doc_name AS DocName, a.doc_path AS DocPath,
                    a.doc_mime AS DocMime, a.doc_size AS DocSize, a.tipo AS Tipo, a.created_at AS CreatedAt,
                    a.link AS Link
                FROM tb_pdi_ativos a
                INNER JOIN tb_pdi p ON p.id = a.pdi_id AND p.codigo_interno_colaborador = @ColaboradorId
                WHERE a.pdi_id = @PdiId",
                new { PdiId = pdiId, ColaboradorId = codigoInternoColaborador });
        }

        public async Task<(byte[] Doc, string DocName, string DocMime, string Link)> ObterEvidenciaBytesAsync(Guid pdiId, Guid evidenciaId, string codigoInternoColaborador)
        {
            var conn = _connection.GetConnection();
            var row = await conn.QueryFirstOrDefaultAsync<EvidenciaBytesRow>(@"
                SELECT a.doc AS Doc, a.doc_name AS DocName, a.doc_mime AS DocMime, a.link AS Link
                FROM tb_pdi_ativos a
                INNER JOIN tb_pdi p ON p.id = a.pdi_id AND p.codigo_interno_colaborador = @ColaboradorId
                WHERE a.id = @EvidenciaId AND a.pdi_id = @PdiId",
                new { PdiId = pdiId, EvidenciaId = evidenciaId, ColaboradorId = codigoInternoColaborador });
            if (row == null)
                return (null, null, null, null);
            if (row.Doc == null && string.IsNullOrWhiteSpace(row.Link))
                return (null, null, null, null);
            return (row.Doc, row.DocName, row.DocMime, row.Link);
        }

        public async Task<IEnumerable<PdiResumoTimeDTO>> ListarPdisDoTimeAsync(IEnumerable<string> codigosInternosSubordinados, int orgId)
        {
            var list = codigosInternosSubordinados?.ToList() ?? new List<string>();
            if (list.Count == 0) return Array.Empty<PdiResumoTimeDTO>();

            var conn = _connection.GetConnection();
            var pdis = (await conn.QueryAsync<PdiRow>(@"
                SELECT p.id AS IdRaw, p.codigo_interno_colaborador AS ColaboradorId, tc.nome_completo AS NomeCompletoColaborador,
                    p.titulo AS Titulo, p.status AS Status, p.data_criacao AS DataCriacao, p.data_atualizacao AS DataAtualizacao
                FROM tb_pdi p
                LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_interno_colaborador
                WHERE p.codigo_interno_colaborador IN @Ids
                ORDER BY p.data_criacao DESC", new { Ids = list })).ToList();

            var result = new List<PdiResumoTimeDTO>();
            foreach (var p in pdis)
            {
                var pdiGuid = ObjectToGuid(p.IdRaw);
                var total = await ContarActionPlansAsync(pdiGuid);
                var concluidos = await ContarActionPlansConcluidosAsync(pdiGuid);
                var progress = total > 0 ? (double)concluidos / total : 0d;
                result.Add(new PdiResumoTimeDTO
                {
                    ColaboradorId = ObjectToString(p.ColaboradorId),
                    NomeColaborador = ObjectToString(p.NomeCompletoColaborador),
                    PdiId = pdiGuid,
                    Titulo = ObjectToString(p.Titulo),
                    Status = ObjectToString(p.Status),
                    DataCriacao = ObjectToDateTime(p.DataCriacao),
                    DataAtualizacao = ParseDateTime(p.DataAtualizacao),
                    Progress = progress
                });
            }
            return result;
        }

        public async Task<(IEnumerable<PdiResumoTimeDTO> Items, int TotalCount)> ListarPdisDoTimeAsync(IEnumerable<string> codigosInternosSubordinados, int orgId, int pagina, int tamanhoPagina)
        {
            var list = codigosInternosSubordinados?.ToList() ?? new List<string>();
            if (list.Count == 0) return (Array.Empty<PdiResumoTimeDTO>(), 0);

            var conn = _connection.GetConnection();
            var totalCount = await conn.ExecuteScalarAsync<int>(@"
                SELECT COUNT(1) FROM tb_pdi WHERE codigo_interno_colaborador IN @Ids", new { Ids = list });

            if (totalCount == 0) return (Array.Empty<PdiResumoTimeDTO>(), 0);

            var offset = (pagina - 1) * tamanhoPagina;
            var pdis = (await conn.QueryAsync<PdiRow>(@"
                SELECT p.id AS IdRaw, p.codigo_interno_colaborador AS ColaboradorId, tc.nome_completo AS NomeCompletoColaborador,
                    p.titulo AS Titulo, p.status AS Status, p.data_criacao AS DataCriacao, p.data_atualizacao AS DataAtualizacao
                FROM tb_pdi p
                LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_interno_colaborador
                WHERE p.codigo_interno_colaborador IN @Ids
                ORDER BY p.data_criacao DESC
                LIMIT @Limit OFFSET @Offset", new { Ids = list, Limit = tamanhoPagina, Offset = offset })).ToList();

            var result = new List<PdiResumoTimeDTO>();
            foreach (var p in pdis)
            {
                var pdiGuid = ObjectToGuid(p.IdRaw);
                var total = await ContarActionPlansAsync(pdiGuid);
                var concluidos = await ContarActionPlansConcluidosAsync(pdiGuid);
                var progress = total > 0 ? (double)concluidos / total : 0d;
                result.Add(new PdiResumoTimeDTO
                {
                    ColaboradorId = ObjectToString(p.ColaboradorId),
                    NomeColaborador = ObjectToString(p.NomeCompletoColaborador),
                    PdiId = pdiGuid,
                    Titulo = ObjectToString(p.Titulo),
                    Status = ObjectToString(p.Status),
                    DataCriacao = ObjectToDateTime(p.DataCriacao),
                    DataAtualizacao = ParseDateTime(p.DataAtualizacao),
                    Progress = progress
                });
            }
            return (result, totalCount);
        }

        public async Task<(IEnumerable<PdiResumoRhDTO> Items, int TotalCount)> ListarPdisRhPorOrgAsync(
            int orgId, IReadOnlyList<string> codigosInternosColaborador, int pagina, int tamanhoPagina)
        {
            var conn = _connection.GetConnection();
            const string selectRh = @"
                SELECT p.id AS IdRaw, p.codigo_interno_colaborador AS ColaboradorId, tc.nome_completo AS NomeCompletoColaborador,
                    p.titulo AS Titulo, p.status AS Status, p.data_criacao AS DataCriacao, p.data_atualizacao AS DataAtualizacao,
                    (SELECT co.diretoria FROM tb_colaborador_org co
                     WHERE co.codigo_interno_colaborador = p.codigo_interno_colaborador AND co.tb_org_id = @OrgId AND co.ativo = 1
                     ORDER BY co.data_alteracao DESC LIMIT 1) AS DiretoriaDesc";
            const string baseFrom = @"
                FROM tb_pdi p
                LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = p.codigo_interno_colaborador";

            string whereOrg = @" WHERE p.tb_org_id = @OrgId ";

            if (codigosInternosColaborador != null)
            {
                var ids = codigosInternosColaborador.ToList();
                if (ids.Count == 0)
                    return (Array.Empty<PdiResumoRhDTO>(), 0);

                var where = whereOrg + " AND p.codigo_interno_colaborador IN @Ids ";
                var countParams = new { OrgId = orgId, Ids = ids };
                var listParams = new { OrgId = orgId, Ids = ids, Limit = tamanhoPagina, Offset = (pagina - 1) * tamanhoPagina };

                var totalCount = await conn.ExecuteScalarAsync<int>($"SELECT COUNT(1) {baseFrom} {where}", countParams);
                if (totalCount == 0)
                    return (Array.Empty<PdiResumoRhDTO>(), 0);

                var pdis = (await conn.QueryAsync<PdiRhRow>($@"
                {selectRh}
                {baseFrom}
                {where}
                ORDER BY p.data_criacao DESC
                LIMIT @Limit OFFSET @Offset", listParams)).ToList();

                return await MaterializarRhAsync(orgId, pdis, totalCount);
            }

            var countParamsAll = new { OrgId = orgId };
            var listParamsAll = new { OrgId = orgId, Limit = tamanhoPagina, Offset = (pagina - 1) * tamanhoPagina };

            var total = await conn.ExecuteScalarAsync<int>($"SELECT COUNT(1) {baseFrom} {whereOrg}", countParamsAll);
            if (total == 0)
                return (Array.Empty<PdiResumoRhDTO>(), 0);

            var rows = (await conn.QueryAsync<PdiRhRow>($@"
                {selectRh}
                {baseFrom}
                {whereOrg}
                ORDER BY p.data_criacao DESC
                LIMIT @Limit OFFSET @Offset", listParamsAll)).ToList();

            return await MaterializarRhAsync(orgId, rows, total);
        }

        public async Task<PdiMetricasBigNumbersDTO> ObterContagensPdisRhAsync(int orgId, IReadOnlyList<string> codigosInternosColaborador)
        {
            var conn = _connection.GetConnection();
            const string baseFrom = @" FROM tb_pdi p ";
            string where = @" WHERE p.tb_org_id = @OrgId ";
            object param;
            if (codigosInternosColaborador != null)
            {
                var ids = codigosInternosColaborador.ToList();
                if (ids.Count == 0)
                    return new PdiMetricasBigNumbersDTO();
                where += " AND p.codigo_interno_colaborador IN @Ids ";
                param = new { OrgId = orgId, Ids = ids };
            }
            else
                param = new { OrgId = orgId };

            var rows = (await conn.QueryAsync<StatusCountRow>($"SELECT p.status AS Status, COUNT(1) AS Total {baseFrom} {where} GROUP BY p.status", param)).ToList();
            var result = new PdiMetricasBigNumbersDTO();
            foreach (var r in rows)
            {
                var status = (ObjectToString(r.Status) ?? "").Trim().ToUpperInvariant();
                var total = r.Total;
                if (status == "NOT_STARTED") result.NaoIniciado += total;
                else if (status == "IN_ANALYSIS") result.EmAnalise += total;
                else if (status == "IN_PROGRESS") result.EmAndamento += total;
                else if (status == "COMPLETED") result.Finalizados += total;
                else if (status == "CANCELLED") result.Cancelados += total;
                else result.EmAndamento += total;
            }
            return result;
        }

        private async Task<(IEnumerable<PdiResumoRhDTO> Items, int TotalCount)> MaterializarRhAsync(int orgId, List<PdiRhRow> pdis, int totalCount)
        {
            var dist = pdis
                .Select(p => ObjectToString(p.ColaboradorId))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var ci in dist)
                cache[ci] = await ObterNomeSuperiorImediatoAsync(orgId, ci);

            var conn = _connection.GetConnection();
            var pdiIdStrs = pdis.Select(p => ObjectToIdString(p.IdRaw)).Where(x => x != null).ToList();
            var stats = pdiIdStrs.Count == 0
                ? new List<ActionPlanStatsRow>()
                : (await conn.QueryAsync<ActionPlanStatsRow>(@"
                SELECT pdi_id AS PdiId, COUNT(1) AS Total, SUM(IF(concluido_em IS NOT NULL, 1, 0)) AS Concluidos, MAX(deadline) AS PrevisaoRaw
                FROM tb_pdi_plano_acao WHERE pdi_id IN @PdiIds GROUP BY pdi_id", new { PdiIds = pdiIdStrs })).ToList();

            var result = new List<PdiResumoRhDTO>();
            foreach (var p in pdis)
            {
                var pdiGuid = ObjectToGuid(p.IdRaw);
                var pdiIdStr = ObjectToIdString(p.IdRaw);
                var st = stats.FirstOrDefault(s => ObjectToIdString(s.PdiId) == pdiIdStr);
                var total = st?.Total ?? 0;
                var concluidos = st?.Concluidos ?? 0;
                var progress = total > 0 ? (double)concluidos / total : 0d;
                var previsao = st != null ? ParseDateTime(st.PrevisaoRaw) : null;
                var ci = ObjectToString(p.ColaboradorId);
                cache.TryGetValue(ci ?? "", out var nomeGestorImediato);
                result.Add(new PdiResumoRhDTO
                {
                    ColaboradorId = ci,
                    NomeColaborador = ObjectToString(p.NomeCompletoColaborador),
                    NomeColaboradorSuperiorImediato = nomeGestorImediato,
                    DescricaoDiretoria = ObjectToString(p.DiretoriaDesc),
                    PdiId = pdiGuid,
                    Titulo = ObjectToString(p.Titulo),
                    Status = ObjectToString(p.Status),
                    DataCriacao = ObjectToDateTime(p.DataCriacao),
                    DataAtualizacao = ParseDateTime(p.DataAtualizacao),
                    Progress = progress,
                    Previsao = previsao
                });
            }
            return (result, totalCount);
        }

        /// <summary>Gestor imediato (primeiro nível em <c>tb_colaborador_hierarquia</c>); se houver mais de um vínculo direto, retorna o primeiro nome resolvido.</summary>
        private async Task<string> ObterNomeSuperiorImediatoAsync(int orgId, string codigoInternoColaborador)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                return null;

            var conn = _connection.GetConnection();
            const string sqlExtColab = @"
                SELECT TRIM(cod_colaborador_externo) FROM tb_colaborador_org
                WHERE tb_org_id = @OrgId AND TRIM(codigo_interno_colaborador) = @Ci AND ativo = 1
                ORDER BY data_alteracao DESC LIMIT 1";
            var extColab = await conn.QueryFirstOrDefaultAsync<string>(sqlExtColab, new { OrgId = orgId, Ci = codigoInternoColaborador.Trim() });
            if (string.IsNullOrWhiteSpace(extColab))
                return null;

            const string sqlSup = @"
                SELECT TRIM(cod_colaborador_superior) AS Sup
                FROM tb_colaborador_hierarquia
                WHERE tb_org_id = @OrgId AND TRIM(cod_colaborador_externo) = @SubExt";
            const string sqlNome = @"
                SELECT TRIM(tc.nome_completo) FROM tb_colaborador_org co
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = co.codigo_interno_colaborador
                WHERE co.tb_org_id = @OrgId AND co.ativo = 1 AND TRIM(co.cod_colaborador_externo) = @Ext
                ORDER BY co.data_alteracao DESC LIMIT 1";

            foreach (var s in await conn.QueryAsync<string>(sqlSup, new { OrgId = orgId, SubExt = extColab.Trim() }))
            {
                var supExt = (s ?? "").Trim();
                if (supExt.Length == 0)
                    continue;
                var nome = await conn.QueryFirstOrDefaultAsync<string>(sqlNome, new { OrgId = orgId, Ext = supExt });
                if (!string.IsNullOrWhiteSpace(nome))
                    return nome.Trim();
            }

            return null;
        }

        private async Task<(IEnumerable<PdiResumoTimeDTO> Items, int TotalCount)> MaterializarResumoTimeAsync(
            List<PdiRow> pdis, int totalCount)
        {
            var result = new List<PdiResumoTimeDTO>();
            foreach (var p in pdis)
            {
                var pdiGuid = ObjectToGuid(p.IdRaw);
                var total = await ContarActionPlansAsync(pdiGuid);
                var concluidos = await ContarActionPlansConcluidosAsync(pdiGuid);
                var progress = total > 0 ? (double)concluidos / total : 0d;
                result.Add(new PdiResumoTimeDTO
                {
                    ColaboradorId = ObjectToString(p.ColaboradorId),
                    NomeColaborador = ObjectToString(p.NomeCompletoColaborador),
                    PdiId = pdiGuid,
                    Titulo = ObjectToString(p.Titulo),
                    Status = ObjectToString(p.Status),
                    DataCriacao = ObjectToDateTime(p.DataCriacao),
                    DataAtualizacao = ParseDateTime(p.DataAtualizacao),
                    Progress = progress
                });
            }
            return (result, totalCount);
        }

        public async Task<PdiCompletoTimeDTO> ObterPdiCompletoDoTimeAsync(Guid pdiId, int orgId)
        {
            var conn = _connection.GetConnection();
            var pdiIdStr = pdiId.ToString();
            var p = await conn.QueryFirstOrDefaultAsync<PdiRow>(@"
                SELECT id AS IdRaw, codigo_interno_colaborador AS ColaboradorId, titulo AS Titulo, descricao AS Descricao, status AS Status, dead_line AS DeadLine, tb_org_id AS OrgId,                   data_criacao AS DataCriacao, data_atualizacao AS DataAtualizacao,
                    codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao, codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao
                FROM tb_pdi WHERE id = @PdiId", new { PdiId = pdiIdStr });
            if (p == null) return null;

            var skills = (await conn.QueryAsync<PdiSkillRow>("SELECT id AS IdRaw, pdi_id AS PdiId, nome_skill AS NomeSkill, codigo_skill AS CodigoSkill FROM tb_pdi_skill WHERE pdi_id = @PdiId", new { PdiId = pdiIdStr })).ToList();
            var plans = (await conn.QueryAsync<ActionPlanRow>(@"
                SELECT id AS IdRaw, pdi_id AS PdiId, description AS Description, deadline AS DeadlineRaw, concluido_em AS ConcluidoEmRaw,
                    codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao, codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao
                FROM tb_pdi_plano_acao WHERE pdi_id = @PdiId", new { PdiId = pdiIdStr })).ToList();
            var evidencias = (await conn.QueryAsync<PdiEvidenciaResumoRow>("SELECT id AS IdRaw, doc_name AS DocName, doc_path AS DocPath, tipo AS Tipo, link AS Link FROM tb_pdi_ativos WHERE pdi_id = @PdiId", new { PdiId = pdiIdStr })).ToList();

            var total = plans.Count;
            var concluidos = plans.Count(a => ParseDateTime(a.ConcluidoEmRaw).HasValue);
            var progress = total > 0 ? (double)concluidos / total : 0d;

            return new PdiCompletoTimeDTO
            {
                Id = ObjectToGuid(p.IdRaw),
                ColaboradorId = ObjectToString(p.ColaboradorId),
                Titulo = ObjectToString(p.Titulo),
                Descricao = ObjectToString(p.Descricao),
                Status = ObjectToString(p.Status),
                DataCriacao = ObjectToDateTime(p.DataCriacao),
                DataAtualizacao = ParseDateTime(p.DataAtualizacao),
                CodigoInternoColaboradorCriacao = ObjectToString(p.CodigoInternoColaboradorCriacao),
                CodigoInternoColaboradorAlteracao = ObjectToString(p.CodigoInternoColaboradorAlteracao),
                Progress = progress,
                DeadLine = ObjectToDateTime(p.DeadLine),
                Skills = skills.Select(s => new PdiSkillDTO { Id = ObjectToGuid(s.IdRaw), NomeSkill = s.NomeSkill, CodigoSkill = s.CodigoSkill }).ToList(),
                ActionPlans = plans.Select(a => new PdiActionPlanDTO { Id = ObjectToGuid(a.IdRaw), Description = a.Description, Deadline = ParseDateTime(a.DeadlineRaw), ConcluidoEm = ParseDateTime(a.ConcluidoEmRaw), CodigoInternoColaboradorCriacao = ObjectToString(a.CodigoInternoColaboradorCriacao), CodigoInternoColaboradorAlteracao = ObjectToString(a.CodigoInternoColaboradorAlteracao) }).ToList(),
                Evidencias = evidencias.Select(e => new PdiEvidenciaResumoDTO { Id = ObjectToGuid(e.IdRaw), DocName = e.DocName, DocPath = e.DocPath, Tipo = e.Tipo, Link = e.Link }).ToList()
            };
        }

        public async Task<DateTime?> ObterDeadlinePdiAsync(Guid pdiId)
        {
            var conn = _connection.GetConnection();
            var row = await conn.QueryFirstOrDefaultAsync<DeadlineRow>("SELECT dead_line AS DeadLineRaw FROM tb_pdi WHERE id = @PdiId", new { PdiId = pdiId });
            return row == null ? null : ParseDateTime(row.DeadLineRaw);
        }

        public async Task<bool> PertenceAoColaboradorAsync(Guid pdiId, string codigoInternoColaborador)
        {
            var conn = _connection.GetConnection();
            var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM tb_pdi WHERE id = @PdiId AND codigo_interno_colaborador = @ColaboradorId",
                new { PdiId = pdiId, ColaboradorId = codigoInternoColaborador });
            return count > 0;
        }

        public async Task<int> ContarActionPlansAsync(Guid pdiId)
        {
            var conn = _connection.GetConnection();
            return await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM tb_pdi_plano_acao WHERE pdi_id = @PdiId", new { PdiId = pdiId });
        }

        public async Task<int> ContarActionPlansConcluidosAsync(Guid pdiId)
        {
            var conn = _connection.GetConnection();
            return await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM tb_pdi_plano_acao WHERE pdi_id = @PdiId AND concluido_em IS NOT NULL", new { PdiId = pdiId });
        }

        /// <summary>Status usados no banco para métricas (big numbers e listas).</summary>
        private static readonly string StatusNotStarted = "NOT_STARTED";
        private static readonly string StatusEmAnalise = "IN_ANALYSIS";
        private static readonly string StatusEmAndamento = "IN_PROGRESS";
        private static readonly string StatusFinalizado = "COMPLETED";
        private static readonly string StatusCancelado = "CANCELLED";

        public async Task<PdiMetricasBigNumbersDTO> ObterContagensPorStatusAsync(IEnumerable<string> codigosColaborador, int orgId)
        {
            var list = codigosColaborador?.ToList() ?? new List<string>();
            if (list.Count == 0)
                return new PdiMetricasBigNumbersDTO();

            var conn = _connection.GetConnection();
            var rows = (await conn.QueryAsync<StatusCountRow>(@"
                SELECT status AS Status, COUNT(1) AS Total
                FROM tb_pdi
                WHERE codigo_interno_colaborador IN @Ids AND (tb_org_id IS NULL OR tb_org_id = @OrgId)
                GROUP BY status", new { Ids = list, OrgId = orgId })).ToList();

            var result = new PdiMetricasBigNumbersDTO();
            foreach (var r in rows)
            {
                var status = (ObjectToString(r.Status) ?? "").Trim().ToUpperInvariant();
                var total = r.Total;
                if (status == "NOT_STARTED") result.NaoIniciado += total;
                else if (status == "IN_ANALYSIS") result.EmAnalise += total;
                else if (status == "IN_PROGRESS") result.EmAndamento += total;
                else if (status == "COMPLETED") result.Finalizados += total;
                else if (status == "CANCELLED") result.Cancelados += total;
                else result.EmAndamento += total; // fallback para valores legados
            }
            return result;
        }

        public async Task<IEnumerable<PdiMetricaItemDTO>> ListarPdisComPrevisaoPorStatusesAsync(IEnumerable<string> codigosColaborador, int orgId, IEnumerable<string> statuses)
        {
            var codigos = codigosColaborador?.ToList() ?? new List<string>();
            var statusList = statuses?.ToList() ?? new List<string>();
            if (codigos.Count == 0 || statusList.Count == 0)
                return Array.Empty<PdiMetricaItemDTO>();

            var conn = _connection.GetConnection();
            var pdis = (await conn.QueryAsync<PdiRow>($@"
                SELECT p.id AS IdRaw, p.codigo_interno_colaborador AS ColaboradorId,
                       tc.nome_completo AS NomeCompletoColaborador,
                       vgest.codigo_interno_colaborador_gestor AS GestorIdRaw,
                       tg.nome_completo AS NomeGestorRaw,
                       p.titulo AS Titulo, p.status AS Status, p.data_criacao AS DataCriacao
                FROM tb_pdi p
                {SqlLeftJoinTitularENomeEGestorParaPdi}
                WHERE p.codigo_interno_colaborador IN @Ids AND p.status IN @Statuses AND (p.tb_org_id IS NULL OR p.tb_org_id = @OrgId)
                ORDER BY p.data_criacao DESC", new { Ids = codigos, Statuses = statusList, OrgId = orgId })).ToList();

            if (pdis.Count == 0) return Array.Empty<PdiMetricaItemDTO>();

            var pdiIdStrs = pdis.Select(p => ObjectToIdString(p.IdRaw)).Where(x => x != null).ToList();
            var stats = (await conn.QueryAsync<ActionPlanStatsRow>(@"
                SELECT pdi_id AS PdiId, COUNT(1) AS Total, SUM(IF(concluido_em IS NOT NULL, 1, 0)) AS Concluidos, MAX(deadline) AS PrevisaoRaw
                FROM tb_pdi_plano_acao WHERE pdi_id IN @PdiIds GROUP BY pdi_id", new { PdiIds = pdiIdStrs })).ToList();

            var result = new List<PdiMetricaItemDTO>();
            foreach (var p in pdis)
            {
                var pdiIdStr = ObjectToIdString(p.IdRaw);
                var st = stats.FirstOrDefault(s => ObjectToIdString(s.PdiId) == pdiIdStr);
                result.Add(MapearPdiMetricaItemDTO(p, st));
            }
            return result;
        }

        public async Task<PdiMetricasBigNumbersDTO> ObterContagensPorStatusPorOrgAsync(int orgId)
        {
            var conn = _connection.GetConnection();
            var rows = (await conn.QueryAsync<StatusCountRow>(@"
                SELECT status AS Status, COUNT(1) AS Total
                FROM tb_pdi
                WHERE tb_org_id = @OrgId
                GROUP BY status", new { OrgId = orgId })).ToList();

            var result = new PdiMetricasBigNumbersDTO();
            foreach (var r in rows)
            {
                var status = (ObjectToString(r.Status) ?? "").Trim().ToUpperInvariant();
                var total = r.Total;
                if (status == "NOT_STARTED") result.NaoIniciado += total;
                else if (status == "IN_ANALYSIS") result.EmAnalise += total;
                else if (status == "IN_PROGRESS") result.EmAndamento += total;
                else if (status == "COMPLETED") result.Finalizados += total;
                else if (status == "CANCELLED") result.Cancelados += total;
                else result.EmAndamento += total;
            }
            return result;
        }

        public async Task<IEnumerable<PdiMetricaItemDTO>> ListarPdisComPrevisaoPorStatusesPorOrgAsync(int orgId, IEnumerable<string> statuses)
        {
            var statusList = statuses?.ToList() ?? new List<string>();
            if (statusList.Count == 0)
                return Array.Empty<PdiMetricaItemDTO>();

            var conn = _connection.GetConnection();
            var pdis = (await conn.QueryAsync<PdiRow>($@"
                SELECT p.id AS IdRaw, p.codigo_interno_colaborador AS ColaboradorId,
                       tc.nome_completo AS NomeCompletoColaborador,
                       vgest.codigo_interno_colaborador_gestor AS GestorIdRaw,
                       tg.nome_completo AS NomeGestorRaw,
                       p.titulo AS Titulo, p.status AS Status, p.data_criacao AS DataCriacao
                FROM tb_pdi p
                {SqlLeftJoinTitularENomeEGestorParaPdi}
                WHERE p.tb_org_id = @OrgId AND p.status IN @Statuses
                ORDER BY p.data_criacao DESC", new { OrgId = orgId, Statuses = statusList })).ToList();

            if (pdis.Count == 0) return Array.Empty<PdiMetricaItemDTO>();

            var pdiIdStrs = pdis.Select(p => ObjectToIdString(p.IdRaw)).Where(x => x != null).ToList();
            var stats = (await conn.QueryAsync<ActionPlanStatsRow>(@"
                SELECT pdi_id AS PdiId, COUNT(1) AS Total, SUM(IF(concluido_em IS NOT NULL, 1, 0)) AS Concluidos, MAX(deadline) AS PrevisaoRaw
                FROM tb_pdi_plano_acao WHERE pdi_id IN @PdiIds GROUP BY pdi_id", new { PdiIds = pdiIdStrs })).ToList();

            var result = new List<PdiMetricaItemDTO>();
            foreach (var p in pdis)
            {
                var pdiIdStr = ObjectToIdString(p.IdRaw);
                var st = stats.FirstOrDefault(s => ObjectToIdString(s.PdiId) == pdiIdStr);
                result.Add(MapearPdiMetricaItemDTO(p, st));
            }
            return result;
        }

        public async Task<IEnumerable<PdiGestorItemDTO>> ListarGestoresOrgAsync(int orgId)
        {
            return await ListarGestoresPorDiretoriasAsync(orgId, null);
        }

        public async Task<IEnumerable<PdiGestorItemDTO>> ListarGestoresPorDiretoriasAsync(int orgId, IReadOnlyList<string> codDiretorias)
        {
            var conn = _connection.GetConnection();
            List<PdiGestorItemDTO> lista;
            if (codDiretorias == null || codDiretorias.Count == 0)
            {
                var rows = await conn.QueryAsync<PdiGestorRow>(@"
                    SELECT DISTINCT codigo_interno_colaborador_gestor AS Cpf, nome_completo_gestor AS Nome
                    FROM vw_gestores_colaboradores_org
                    WHERE tb_org_id = @OrgId AND codigo_interno_colaborador_gestor IS NOT NULL
                    ORDER BY nome_completo_gestor", new { OrgId = orgId });
                lista = MapGestores(rows).ToList();
            }
            else
            {
                var rowsF = await conn.QueryAsync<PdiGestorRow>(@"
                    SELECT DISTINCT g.codigo_interno_colaborador_gestor AS Cpf, g.nome_completo_gestor AS Nome
                    FROM vw_gestores_colaboradores_org g
                    INNER JOIN tb_colaborador_org co ON co.codigo_interno_colaborador = g.codigo_interno_colaborador_subordinado AND co.tb_org_id = g.tb_org_id
                    WHERE g.tb_org_id = @OrgId AND g.codigo_interno_colaborador_gestor IS NOT NULL
                      AND co.ativo = 1 AND co.cod_diretoria IN @Dirs
                    ORDER BY g.nome_completo_gestor", new { OrgId = orgId, Dirs = codDiretorias.ToList() });
                lista = MapGestores(rowsF).ToList();
            }

            await EnriquecerGestoresComUnidadesAsync(conn, orgId, lista);
            return lista;
        }

        public async Task<(IEnumerable<PdiGestorItemDTO> Itens, int TotalItens)> ListarGestoresPorDiretoriasPaginadoAsync(
            int orgId, IReadOnlyList<string> codDiretorias, int skip, int take)
        {
            var conn = _connection.GetConnection();
            if (codDiretorias == null || codDiretorias.Count == 0)
            {
                const string baseFrom = @"
                    FROM vw_gestores_colaboradores_org
                    WHERE tb_org_id = @OrgId AND codigo_interno_colaborador_gestor IS NOT NULL";
                var countSql = "SELECT COUNT(DISTINCT codigo_interno_colaborador_gestor) " + baseFrom;
                var total = (int)await conn.ExecuteScalarAsync<long>(countSql, new { OrgId = orgId });
                var rows = await conn.QueryAsync<PdiGestorRow>($@"
                    SELECT DISTINCT codigo_interno_colaborador_gestor AS Cpf, nome_completo_gestor AS Nome
                    {baseFrom}
                    ORDER BY nome_completo_gestor
                    LIMIT @Take OFFSET @Skip", new { OrgId = orgId, Take = take, Skip = skip });
                var page = MapGestores(rows).ToList();
                await EnriquecerGestoresComUnidadesAsync(conn, orgId, page);
                return (page, total);
            }

            var dirs = codDiretorias.ToList();
            const string baseFromDir = @"
                FROM vw_gestores_colaboradores_org g
                INNER JOIN tb_colaborador_org co ON co.codigo_interno_colaborador = g.codigo_interno_colaborador_subordinado AND co.tb_org_id = g.tb_org_id
                WHERE g.tb_org_id = @OrgId AND g.codigo_interno_colaborador_gestor IS NOT NULL
                  AND co.ativo = 1 AND co.cod_diretoria IN @Dirs";
            var countSqlDir = "SELECT COUNT(DISTINCT g.codigo_interno_colaborador_gestor) " + baseFromDir;
            var totalDir = (int)await conn.ExecuteScalarAsync<long>(countSqlDir, new { OrgId = orgId, Dirs = dirs });
            var rowsF = await conn.QueryAsync<PdiGestorRow>($@"
                SELECT DISTINCT g.codigo_interno_colaborador_gestor AS Cpf, g.nome_completo_gestor AS Nome
                {baseFromDir}
                ORDER BY g.nome_completo_gestor
                LIMIT @Take OFFSET @Skip", new { OrgId = orgId, Dirs = dirs, Take = take, Skip = skip });
            var pageDir = MapGestores(rowsF).ToList();
            await EnriquecerGestoresComUnidadesAsync(conn, orgId, pageDir);
            return (pageDir, totalDir);
        }

        private static IEnumerable<PdiGestorItemDTO> MapGestores(IEnumerable<PdiGestorRow> rows)
        {
            return rows.Select(r => new PdiGestorItemDTO
            {
                Cpf = ObjectToString(r?.Cpf),
                Nome = ObjectToString(r?.Nome),
                Unidades = new List<PdiGestorUnidadeDTO>()
            }).Where(x => !string.IsNullOrEmpty(x.Cpf)).ToList();
        }

        public async Task<PdiMetricasBigNumbersDTO> ObterContagensMetricasFiltradasAsync(PdiMetricasQueryDTO q)
        {
            var conn = _connection.GetConnection();
            var (sql, param) = MontarSqlMetricasBase(q, forGroupByStatus: true);
            var rows = (await conn.QueryAsync<StatusCountRow>(sql, param)).ToList();
            var result = new PdiMetricasBigNumbersDTO();
            foreach (var r in rows)
            {
                var status = (ObjectToString(r.Status) ?? "").Trim().ToUpperInvariant();
                var total = r.Total;
                if (status == "NOT_STARTED") result.NaoIniciado += total;
                else if (status == "IN_ANALYSIS") result.EmAnalise += total;
                else if (status == "IN_PROGRESS") result.EmAndamento += total;
                else if (status == "COMPLETED") result.Finalizados += total;
                else if (status == "CANCELLED") result.Cancelados += total;
                else result.EmAndamento += total;
            }
            return result;
        }

        public async Task<int> ContarPdisMetricasFiltradasAsync(PdiMetricasQueryDTO q)
        {
            var conn = _connection.GetConnection();
            var (joinWhere, p) = MontarMetricasListagemJoinWhere(q, incluirFiltroStatus: true);
            var sql = "SELECT COUNT(1) " + joinWhere;
            var total = await conn.ExecuteScalarAsync<long>(sql, p);
            return (int)total;
        }

        public async Task<IEnumerable<PdiMetricaItemDTO>> ListarPdisMetricasFiltradasAsync(PdiMetricasQueryDTO q)
        {
            var conn = _connection.GetConnection();
            var (sql, param) = MontarSqlMetricasBase(q, forGroupByStatus: false);
            var pdis = (await conn.QueryAsync<PdiRow>(sql, param)).ToList();
            if (pdis.Count == 0) return Array.Empty<PdiMetricaItemDTO>();

            var pdiIdStrs = pdis.Select(p => ObjectToIdString(p.IdRaw)).Where(x => x != null).ToList();
            var stats = (await conn.QueryAsync<ActionPlanStatsRow>(@"
                SELECT pdi_id AS PdiId, COUNT(1) AS Total, SUM(IF(concluido_em IS NOT NULL, 1, 0)) AS Concluidos, MAX(deadline) AS PrevisaoRaw
                FROM tb_pdi_plano_acao WHERE pdi_id IN @PdiIds GROUP BY pdi_id", new { PdiIds = pdiIdStrs })).ToList();

            var result = new List<PdiMetricaItemDTO>();
            foreach (var p in pdis)
            {
                var pdiIdStr = ObjectToIdString(p.IdRaw);
                var st = stats.FirstOrDefault(s => ObjectToIdString(s.PdiId) == pdiIdStr);
                result.Add(MapearPdiMetricaItemDTO(p, st));
            }
            return result;
        }

        public async Task<IEnumerable<PdiMetricaExportLinhaDTO>> ListarPdisMetricasExportPorIdsAsync(IEnumerable<Guid> pdiIds, PdiMetricasQueryDTO escopo)
        {
            var ids = pdiIds?.Distinct().ToList() ?? new List<Guid>();
            if (ids.Count == 0)
                return Array.Empty<PdiMetricaExportLinhaDTO>();

            var conn = _connection.GetConnection();
            var (baseSql, baseParam) = MontarSqlMetricasExportWhere(escopo);
            var sql = $@"
                SELECT p.id AS IdRaw, p.titulo AS Titulo, p.codigo_interno_colaborador AS ColaboradorId, p.status AS Status,
                       p.data_criacao AS DataCriacaoRaw, p.dead_line AS DeadLineRaw
                FROM tb_pdi p
                INNER JOIN tb_colaborador_org co ON co.codigo_interno_colaborador = p.codigo_interno_colaborador AND co.tb_org_id = @OrgId
                {baseSql}
                  AND p.id IN @PdiIds";

            var param = new DynamicParameters(baseParam);
            param.Add("PdiIds", ids.Select(x => x.ToString()).ToList());

            var rows = await conn.QueryAsync<PdiExportRow>(sql, param);
            return rows.Select(r => new PdiMetricaExportLinhaDTO
            {
                PdiId = ObjectToGuid(r.IdRaw),
                Titulo = ObjectToString(r.Titulo),
                ColaboradorId = ObjectToString(r.ColaboradorId),
                Status = ObjectToString(r.Status),
                DataCriacao = ParseDateTime(r.DataCriacaoRaw),
                DeadLine = ParseDateTime(r.DeadLineRaw)
            }).Where(x => x.PdiId != Guid.Empty).ToList();
        }

        private static string InserirJoinsTitularEGestorAntesDoWhere(string joinWhere)
        {
            const string token = " WHERE ";
            var i = joinWhere.IndexOf(token, StringComparison.Ordinal);
            if (i < 0) return joinWhere;
            return joinWhere.Insert(i, SqlLeftJoinTitularENomeEGestorParaPdi);
        }

        private static PdiMetricaItemDTO MapearPdiMetricaItemDTO(PdiRow p, ActionPlanStatsRow st)
        {
            var total = st?.Total ?? 0;
            var concluidos = st?.Concluidos ?? 0;
            var progress = total > 0 ? (double)concluidos / total : 0d;
            var previsao = st != null ? ParseDateTime(st.PrevisaoRaw) : null;
            return new PdiMetricaItemDTO
            {
                PdiId = ObjectToGuid(p.IdRaw),
                GestorId = ObjectToString(p.GestorIdRaw),
                NomeGestor = ObjectToString(p.NomeGestorRaw),
                ColaboradorId = ObjectToString(p.ColaboradorId),
                NomeCompleto = ObjectToString(p.NomeCompletoColaborador),
                Titulo = ObjectToString(p.Titulo),
                Status = ObjectToString(p.Status),
                Progress = progress,
                Previsao = previsao
            };
        }

        private static async Task EnriquecerGestoresComUnidadesAsync(IDbConnection conn, int orgId, List<PdiGestorItemDTO> gestores)
        {
            if (gestores == null || gestores.Count == 0) return;
            var ids = gestores.Where(g => !string.IsNullOrEmpty(g.Cpf)).Select(g => g.Cpf).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (ids.Count == 0) return;

            const string sql = @"
                SELECT g.codigo_interno_colaborador_gestor AS GestorCpf,
                       co.cod_diretoria AS CodDiretoria,
                       MAX(co.diretoria) AS DescricaoUnidade
                FROM vw_gestores_colaboradores_org g
                INNER JOIN tb_colaborador_org co ON co.codigo_interno_colaborador = g.codigo_interno_colaborador_subordinado AND co.tb_org_id = g.tb_org_id
                WHERE g.tb_org_id = @OrgId AND g.codigo_interno_colaborador_gestor IN @Gestores AND co.ativo = 1
                  AND co.cod_diretoria IS NOT NULL AND co.cod_diretoria <> ''
                GROUP BY g.codigo_interno_colaborador_gestor, co.cod_diretoria
                ORDER BY g.codigo_interno_colaborador_gestor, co.cod_diretoria";

            var rows = (await conn.QueryAsync<GestorUnidadeRow>(sql, new { OrgId = orgId, Gestores = ids })).ToList();
            var porGestor = rows
                .GroupBy(r => ObjectToString(r.GestorCpf) ?? "", StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Select(r => new PdiGestorUnidadeDTO
                {
                    CodDiretoria = ObjectToString(r.CodDiretoria),
                    Descricao = ObjectToString(r.DescricaoUnidade)
                }).ToList(), StringComparer.OrdinalIgnoreCase);

            foreach (var g in gestores)
            {
                if (string.IsNullOrEmpty(g.Cpf)) continue;
                if (porGestor.TryGetValue(g.Cpf, out var unidades))
                    g.Unidades = unidades;
            }
        }

        private static (string SqlFragment, DynamicParameters Param) MontarSqlMetricasExportWhere(PdiMetricasQueryDTO q)
        {
            var p = new DynamicParameters();
            p.Add("OrgId", q.OrgId);
            var sb = new StringBuilder();
            sb.Append(" WHERE p.tb_org_id = @OrgId AND co.ativo = 1 ");
            if (q.CodDiretorias != null && q.CodDiretorias.Count > 0)
            {
                sb.Append(" AND co.cod_diretoria IN @Dirs ");
                p.Add("Dirs", q.CodDiretorias.ToList());
            }
            if (q.CodigosColaborador != null && q.CodigosColaborador.Count > 0)
            {
                sb.Append(" AND p.codigo_interno_colaborador IN @Cpfs ");
                p.Add("Cpfs", q.CodigosColaborador.ToList());
            }
            if (q.DataCriacaoMin.HasValue)
            {
                sb.Append(" AND p.data_criacao >= @DataIni ");
                p.Add("DataIni", q.DataCriacaoMin.Value);
            }
            if (q.DataDeadlineMax.HasValue)
            {
                sb.Append(" AND p.dead_line IS NOT NULL AND p.dead_line <= @DataFim ");
                p.Add("DataFim", q.DataDeadlineMax.Value);
            }
            return (sb.ToString(), p);
        }

        /// <summary>FROM/JOIN/WHERE comum à listagem e à contagem (status opcional — desligado no GROUP BY por status).</summary>
        private static (string JoinWhere, DynamicParameters Param) MontarMetricasListagemJoinWhere(PdiMetricasQueryDTO q, bool incluirFiltroStatus)
        {
            var p = new DynamicParameters();
            p.Add("OrgId", q.OrgId);
            var sb = new StringBuilder();
            sb.Append(@"
                FROM tb_pdi p
                INNER JOIN tb_colaborador_org co ON co.codigo_interno_colaborador = p.codigo_interno_colaborador AND co.tb_org_id = @OrgId
                WHERE p.tb_org_id = @OrgId AND co.ativo = 1 ");

            if (q.CodDiretorias != null && q.CodDiretorias.Count > 0)
            {
                sb.Append(" AND co.cod_diretoria IN @Dirs ");
                p.Add("Dirs", q.CodDiretorias.ToList());
            }
            if (q.CodigosColaborador != null && q.CodigosColaborador.Count > 0)
            {
                sb.Append(" AND p.codigo_interno_colaborador IN @Cpfs ");
                p.Add("Cpfs", q.CodigosColaborador.ToList());
            }
            if (q.DataCriacaoMin.HasValue)
            {
                sb.Append(" AND p.data_criacao >= @DataIni ");
                p.Add("DataIni", q.DataCriacaoMin.Value);
            }
            if (q.DataDeadlineMax.HasValue)
            {
                sb.Append(" AND p.dead_line IS NOT NULL AND p.dead_line <= @DataFim ");
                p.Add("DataFim", q.DataDeadlineMax.Value);
            }
            if (incluirFiltroStatus)
            {
                var st = q.StatusesListagem;
                if (st != null && st.Count > 0)
                {
                    sb.Append(" AND p.status IN @Statuses ");
                    p.Add("Statuses", st.ToList());
                }
            }

            return (sb.ToString(), p);
        }

        private static (string Sql, DynamicParameters Param) MontarSqlMetricasBase(PdiMetricasQueryDTO q, bool forGroupByStatus)
        {
            var sb = new StringBuilder();
            if (forGroupByStatus)
            {
                sb.Append("SELECT p.status AS Status, COUNT(1) AS Total ");
                var (jw, p) = MontarMetricasListagemJoinWhere(q, incluirFiltroStatus: false);
                sb.Append(jw);
                sb.Append(" GROUP BY p.status ");
                return (sb.ToString(), p);
            }

            sb.Append(@"SELECT p.id AS IdRaw, p.codigo_interno_colaborador AS ColaboradorId,
                tc.nome_completo AS NomeCompletoColaborador,
                vgest.codigo_interno_colaborador_gestor AS GestorIdRaw,
                tg.nome_completo AS NomeGestorRaw,
                p.titulo AS Titulo, p.status AS Status, p.data_criacao AS DataCriacao ");
            var (joinWhere, param) = MontarMetricasListagemJoinWhere(q, incluirFiltroStatus: true);
            sb.Append(InserirJoinsTitularEGestorAntesDoWhere(joinWhere));
            sb.Append(" ORDER BY p.data_criacao DESC ");
            if (q.Take.HasValue)
            {
                param.Add("Take", q.Take.Value);
                param.Add("Skip", q.Skip ?? 0);
                sb.Append(" LIMIT @Take OFFSET @Skip ");
            }

            return (sb.ToString(), param);
        }

        private class PdiExportRow
        {
            public object IdRaw { get; set; }
            public object Titulo { get; set; }
            public object ColaboradorId { get; set; }
            public object Status { get; set; }
            public object DataCriacaoRaw { get; set; }
            public object DeadLineRaw { get; set; }
        }

        private class PdiGestorRow
        {
            public object Cpf { get; set; }
            public object Nome { get; set; }
        }

        private class GestorUnidadeRow
        {
            public object GestorCpf { get; set; }
            public object CodDiretoria { get; set; }
            public object DescricaoUnidade { get; set; }
        }

        private class StatusCountRow
        {
            public object Status { get; set; }
            public int Total { get; set; }
        }

        private class ActionPlanStatsRow
        {
            public object PdiId { get; set; }
            public int Total { get; set; }
            public int Concluidos { get; set; }
            public object PrevisaoRaw { get; set; }
        }

        private static async Task<List<PdiSkillRow>> ObterSkillsPorPdiIdsAsync(MySql.Data.MySqlClient.MySqlConnection conn, List<Guid> pdiIds)
        {
            if (pdiIds == null || pdiIds.Count == 0) return new List<PdiSkillRow>();
            var pdiIdStrs = pdiIds.Select(x => x.ToString()).ToList();
            return (await conn.QueryAsync<PdiSkillRow>(@"
                SELECT id AS IdRaw, pdi_id AS PdiId, nome_skill AS NomeSkill, codigo_skill AS CodigoSkill
                FROM tb_pdi_skill WHERE pdi_id IN @PdiIds", new { PdiIds = pdiIdStrs })).ToList();
        }

        private static async Task<List<ActionPlanRow>> ObterActionPlansPorPdiIdsAsync(MySql.Data.MySqlClient.MySqlConnection conn, List<Guid> pdiIds)
        {
            if (pdiIds == null || pdiIds.Count == 0) return new List<ActionPlanRow>();
            var pdiIdStrs = pdiIds.Select(x => x.ToString()).ToList();
            return (await conn.QueryAsync<ActionPlanRow>(@"
                SELECT id AS IdRaw, pdi_id AS PdiId, description AS Description, deadline AS DeadlineRaw, concluido_em AS ConcluidoEmRaw,
                    codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao, codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao
                FROM tb_pdi_plano_acao WHERE pdi_id IN @PdiIds", new { PdiIds = pdiIdStrs })).ToList();
        }

        private class CriadorPdiRow
        {
            public object ColaboradorId { get; set; }
            public object CriadoPor { get; set; }
            public object Status { get; set; }
        }

        private class DeadlineRow
        {
            public object DeadLineRaw { get; set; }
        }

        private class PdiRhRow
        {
            public object IdRaw { get; set; }
            public object ColaboradorId { get; set; }
            public object NomeCompletoColaborador { get; set; }
            public object Titulo { get; set; }
            public object Status { get; set; }
            public object DataCriacao { get; set; }
            public object DataAtualizacao { get; set; }
            public object DiretoriaDesc { get; set; }
        }

        private class PdiRow
        {
            public object IdRaw { get; set; }
            public object ColaboradorId { get; set; }
            public object NomeCompletoColaborador { get; set; }
            public object GestorIdRaw { get; set; }
            public object NomeGestorRaw { get; set; }
            public object Titulo { get; set; }
            public object Descricao { get; set; }
            public object Status { get; set; }
            public object DataCriacao { get; set; }
            public object DataAtualizacao { get; set; }
            public object DeadLine { get; set; }
            public object OrgId { get; set; }
            public object CodigoInternoColaboradorCriacao { get; set; }
            public object CodigoInternoColaboradorAlteracao { get; set; }
        }

        private class PdiSkillRow
        {
            public object IdRaw { get; set; }
            public object PdiId { get; set; }
            public string NomeSkill { get; set; }
            public string CodigoSkill { get; set; }
        }

        private class ActionPlanRow
        {
            public object IdRaw { get; set; }
            public object PdiId { get; set; }
            public string Description { get; set; }
            public object DeadlineRaw { get; set; }
            public object ConcluidoEmRaw { get; set; }
            public object CodigoInternoColaboradorCriacao { get; set; }
            public object CodigoInternoColaboradorAlteracao { get; set; }
        }

        private static DateTime? ParseDateTime(object value)
        {
            if (value == null || value is DBNull) return null;
            if (value is DateTime dt) return dt;
            if (value is DateTimeOffset dto) return dto.UtcDateTime;
            if (value is string s && DateTime.TryParse(s, out var parsed)) return parsed;
            return null;
        }

        private class PdiEvidenciaResumoRow
        {
            public object IdRaw { get; set; }
            public string DocName { get; set; }
            public string DocPath { get; set; }
            public string Tipo { get; set; }
            public string Link { get; set; }
        }

        private class EvidenciaBytesRow
        {
            public byte[] Doc { get; set; }
            public string DocName { get; set; }
            public string DocMime { get; set; }
            public string Link { get; set; }
        }
    }
}
