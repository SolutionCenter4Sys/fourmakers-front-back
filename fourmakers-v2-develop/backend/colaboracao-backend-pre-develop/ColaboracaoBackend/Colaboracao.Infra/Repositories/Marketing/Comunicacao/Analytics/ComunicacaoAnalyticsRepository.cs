using Colaboracao.Core.Interfaces;
using Core.Domain.Marketing.Comunicacao.Analytics;
using Dapper;
using DataTransferObject.Domain.Marketing.Comunicacao.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Marketing.Comunicacao.Analytics
{
    public class ComunicacaoAnalyticsRepository : IComunicacaoAnalyticsRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ComunicacaoAnalyticsRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private class AnalyticsRow
        {
            public Guid Id { get; set; }
            public string Titulo { get; set; }
            public string PublicacaoTipo { get; set; }
            public bool RequerConfirmacaoLeitura { get; set; }
            public DateTime? DataReferencia { get; set; }
            public Guid? ComunidadeId { get; set; }
            public string ComunidadeNome { get; set; }
            public string Grupos { get; set; }
            public int Likes { get; set; }
            public int Comentarios { get; set; }
            public int Aceites { get; set; }
            public int Visualizacoes { get; set; }
            public int Alcance { get; set; }
            public int TemVideo { get; set; }
            public int TemImagem { get; set; }
            public int TemDocumento { get; set; }
        }

        private class PostsPorComunidadeRow
        {
            public Guid ComunidadeId { get; set; }
            public string ComunidadeNome { get; set; }
            public int Membros { get; set; }
            public int Posts { get; set; }
            public int Likes { get; set; }
            public int Comentarios { get; set; }
            public int Aceites { get; set; }
            public int Visualizacoes { get; set; }
        }

        public async Task<AnalyticsResumoResponseDTO> ObterResumoAsync(int orgId, AnalyticsResumoRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("OrgId", orgId);

            var whereClause = BuildWhereClause(request, parameters);
            var joinsClause = GetJoinsClause();
            var baseSql = $"{joinsClause} {whereClause}";

            var bigNumbersSql = $@"
                SELECT
                    SUM(CASE WHEN base.ComunidadeId IS NOT NULL THEN 1 ELSE 0 END) AS PostsComunidades,
                    SUM(CASE WHEN base.ComunidadeId IS NOT NULL THEN COALESCE(i.QuantidadeCurtida, 0) + COALESCE(c.QuantidadeComentarios, 0) ELSE 0 END) AS EngajamentoPosts,
                    SUM(CASE WHEN base.ComunidadeId IS NOT NULL THEN COALESCE(i.QuantidadeCurtida, 0) ELSE 0 END) AS LikesPosts,
                    SUM(CASE WHEN base.ComunidadeId IS NOT NULL THEN COALESCE(c.QuantidadeComentarios, 0) ELSE 0 END) AS ComentariosPosts,
                    SUM(CASE WHEN base.ComunidadeId IS NULL THEN 1 ELSE 0 END) AS ComunicadosPublicados,
                    SUM(CASE WHEN base.ComunidadeId IS NULL AND base.Tipo = 'informativo' THEN 1 ELSE 0 END) AS ComunicadosInformativos,
                    SUM(CASE WHEN base.ComunidadeId IS NULL AND base.Tipo = 'documento' THEN 1 ELSE 0 END) AS ComunicadosDocumentos,
                    SUM(CASE WHEN base.ComunidadeId IS NULL THEN COALESCE(i.QuantidadeConfirmacoesLeitura, 0) ELSE 0 END) AS AceitesComunicados,
                    SUM(CASE WHEN base.ComunidadeId IS NULL AND base.RequerConfirmacaoLeitura = 1 THEN 1 ELSE 0 END) AS ComunicadosObrigatorios,
                    SUM(CASE WHEN base.ComunidadeId IS NULL AND base.RequerConfirmacaoLeitura = 0 THEN 1 ELSE 0 END) AS ComunicadosOpcionais
                FROM (
                    SELECT DISTINCT
                        p.id,
                        p.tipo AS Tipo,
                        p.requer_confirmacao_leitura AS RequerConfirmacaoLeitura,
                        cp.tb_mkt_comunidade_id AS ComunidadeId
                    {baseSql}
                ) base
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        SUM(CASE WHEN visualizado = 1 THEN 1 ELSE 0 END) AS QuantidadeVisualizacao,
                        COUNT(*) AS Alcance,
                        SUM(CASE WHEN curtida_emoji IS NOT NULL AND TRIM(curtida_emoji) != '' THEN 1 ELSE 0 END) AS QuantidadeCurtida,
                        SUM(CASE WHEN confirmou_leitura = 1 THEN 1 ELSE 0 END) AS QuantidadeConfirmacoesLeitura
                    FROM tb_mkt_publicacao_colaborador_interacao
                    GROUP BY tb_mkt_publicacao_id
                ) i ON i.tb_mkt_publicacao_id = base.id
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        COUNT(*) AS QuantidadeComentarios
                    FROM tb_mkt_publicacao_comentario
                    GROUP BY tb_mkt_publicacao_id
                ) c ON c.tb_mkt_publicacao_id = base.id";

            var bigNumbers = await connection.QuerySingleOrDefaultAsync<AnalyticsBigNumbersDTO>(bigNumbersSql, parameters)
                ?? new AnalyticsBigNumbersDTO();

            var totalSql = $@"
                SELECT COUNT(*) FROM (
                    SELECT DISTINCT p.id
                    {baseSql}
                ) t";
            var total = await connection.ExecuteScalarAsync<int>(totalSql, parameters);

            var pagina = request?.Pagina > 0 ? request.Pagina : 1;
            var tamanhoPagina = request?.TamanhoPagina > 0 ? request.TamanhoPagina : 20;
            var offset = (pagina - 1) * tamanhoPagina;
            parameters.Add("Limit", tamanhoPagina);
            parameters.Add("Offset", offset);

            var listSql = $@"
                SELECT
                    p.id AS Id,
                    p.titulo AS Titulo,
                    p.tipo AS PublicacaoTipo,
                    p.requer_confirmacao_leitura AS RequerConfirmacaoLeitura,
                    COALESCE(p.data_publicacao, p.data_criacao) AS DataReferencia,
                    cp.tb_mkt_comunidade_id AS ComunidadeId,
                    com.nome AS ComunidadeNome,
                    GROUP_CONCAT(DISTINCT g.nome ORDER BY g.nome SEPARATOR ', ') AS Grupos,
                    COALESCE(i.QuantidadeCurtida, 0) AS Likes,
                    COALESCE(c.QuantidadeComentarios, 0) AS Comentarios,
                    COALESCE(i.QuantidadeConfirmacoesLeitura, 0) AS Aceites,
                    COALESCE(i.QuantidadeVisualizacao, 0) AS Visualizacoes,
                    COALESCE(i.Alcance, 0) AS Alcance,
                    MAX(CASE WHEN a.tipo = 'video' THEN 1 ELSE 0 END) AS TemVideo,
                    MAX(CASE WHEN a.tipo = 'imagem' THEN 1 ELSE 0 END) AS TemImagem,
                    MAX(CASE WHEN a.tipo = 'documento' THEN 1 ELSE 0 END) AS TemDocumento
                {joinsClause}
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        SUM(CASE WHEN visualizado = 1 THEN 1 ELSE 0 END) AS QuantidadeVisualizacao,
                        COUNT(*) AS Alcance,
                        SUM(CASE WHEN curtida_emoji IS NOT NULL AND TRIM(curtida_emoji) != '' THEN 1 ELSE 0 END) AS QuantidadeCurtida,
                        SUM(CASE WHEN confirmou_leitura = 1 THEN 1 ELSE 0 END) AS QuantidadeConfirmacoesLeitura
                    FROM tb_mkt_publicacao_colaborador_interacao
                    GROUP BY tb_mkt_publicacao_id
                ) i ON i.tb_mkt_publicacao_id = p.id
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        COUNT(*) AS QuantidadeComentarios
                    FROM tb_mkt_publicacao_comentario
                    GROUP BY tb_mkt_publicacao_id
                ) c ON c.tb_mkt_publicacao_id = p.id
                LEFT JOIN tb_mkt_publicacao_anexo a ON a.tb_mkt_publicacao_id = p.id
                {whereClause}
                GROUP BY p.id
                ORDER BY COALESCE(p.data_publicacao, p.data_criacao) DESC
                LIMIT @Limit OFFSET @Offset";

            var rows = (await connection.QueryAsync<AnalyticsRow>(listSql, parameters)).ToList();
            var itens = rows.Select(MapItem).ToList();

            return new AnalyticsResumoResponseDTO
            {
                BigNumbers = bigNumbers,
                Itens = itens,
                TotalItens = total
            };
        }

        public async Task<AnalyticsResumoResponseDTO> ObterResumoComunicadosOficiaisAsync(int orgId, AnalyticsResumoRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("OrgId", orgId);

            var whereClause = BuildWhereClauseComunicadosOficiais(request, parameters);
            var joinsClause = GetJoinsClause();
            var baseSql = $"{joinsClause} {whereClause}";

            var bigNumbersSql = $@"
                SELECT
                    SUM(CASE WHEN base.ComunidadeId IS NOT NULL THEN 1 ELSE 0 END) AS PostsComunidades,
                    SUM(CASE WHEN base.ComunidadeId IS NOT NULL THEN COALESCE(i.QuantidadeCurtida, 0) + COALESCE(c.QuantidadeComentarios, 0) ELSE 0 END) AS EngajamentoPosts,
                    SUM(CASE WHEN base.ComunidadeId IS NOT NULL THEN COALESCE(i.QuantidadeCurtida, 0) ELSE 0 END) AS LikesPosts,
                    SUM(CASE WHEN base.ComunidadeId IS NOT NULL THEN COALESCE(c.QuantidadeComentarios, 0) ELSE 0 END) AS ComentariosPosts,
                    SUM(CASE WHEN base.ComunidadeId IS NULL THEN 1 ELSE 0 END) AS ComunicadosPublicados,
                    SUM(CASE WHEN base.ComunidadeId IS NULL AND base.Tipo = 'informativo' THEN 1 ELSE 0 END) AS ComunicadosInformativos,
                    SUM(CASE WHEN base.ComunidadeId IS NULL AND base.Tipo = 'documento' THEN 1 ELSE 0 END) AS ComunicadosDocumentos,
                    SUM(CASE WHEN base.ComunidadeId IS NULL THEN COALESCE(i.QuantidadeConfirmacoesLeitura, 0) ELSE 0 END) AS AceitesComunicados,
                    SUM(CASE WHEN base.ComunidadeId IS NULL AND base.RequerConfirmacaoLeitura = 1 THEN 1 ELSE 0 END) AS ComunicadosObrigatorios,
                    SUM(CASE WHEN base.ComunidadeId IS NULL AND base.RequerConfirmacaoLeitura = 0 THEN 1 ELSE 0 END) AS ComunicadosOpcionais,
                    SUM(COALESCE(i.QuantidadeVisualizacao, 0)) AS TotalVisualizacoesComunicados,
                    SUM(COALESCE(i.QuantidadeCurtida, 0)) AS LikesComunicados,
                    SUM(COALESCE(c.QuantidadeComentarios, 0)) AS ComentariosComunicados
                FROM (
                    SELECT DISTINCT
                        p.id,
                        p.tipo AS Tipo,
                        p.requer_confirmacao_leitura AS RequerConfirmacaoLeitura,
                        cp.tb_mkt_comunidade_id AS ComunidadeId
                    {baseSql}
                ) base
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        SUM(CASE WHEN visualizado = 1 THEN 1 ELSE 0 END) AS QuantidadeVisualizacao,
                        COUNT(*) AS Alcance,
                        SUM(CASE WHEN curtida_emoji IS NOT NULL AND TRIM(curtida_emoji) != '' THEN 1 ELSE 0 END) AS QuantidadeCurtida,
                        SUM(CASE WHEN confirmou_leitura = 1 THEN 1 ELSE 0 END) AS QuantidadeConfirmacoesLeitura
                    FROM tb_mkt_publicacao_colaborador_interacao
                    GROUP BY tb_mkt_publicacao_id
                ) i ON i.tb_mkt_publicacao_id = base.id
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        COUNT(*) AS QuantidadeComentarios
                    FROM tb_mkt_publicacao_comentario
                    GROUP BY tb_mkt_publicacao_id
                ) c ON c.tb_mkt_publicacao_id = base.id";

            var bigNumbers = await connection.QuerySingleOrDefaultAsync<AnalyticsBigNumbersDTO>(bigNumbersSql, parameters)
                ?? new AnalyticsBigNumbersDTO();

            bigNumbers.TaxaEngajamentoComunicados = bigNumbers.TotalVisualizacoesComunicados > 0
                ? Math.Round((bigNumbers.AceitesComunicados + bigNumbers.LikesComunicados + bigNumbers.ComentariosComunicados) / (decimal)bigNumbers.TotalVisualizacoesComunicados * 100m, 0)
                : 0m;

            var totalSql = $@"
                SELECT COUNT(*) FROM (
                    SELECT DISTINCT p.id
                    {baseSql}
                ) t";
            var total = await connection.ExecuteScalarAsync<int>(totalSql, parameters);

            request ??= new AnalyticsResumoRequestDTO();
            var pagina = request.Pagina > 0 ? request.Pagina : 1;
            var tamanhoPagina = request.TamanhoPagina > 0 ? request.TamanhoPagina : 20;
            if (tamanhoPagina > 200) tamanhoPagina = 200;
            var offset = (pagina - 1) * tamanhoPagina;
            parameters.Add("Limit", tamanhoPagina);
            parameters.Add("Offset", offset);

            var listSql = $@"
                SELECT
                    p.id AS Id,
                    p.titulo AS Titulo,
                    p.tipo AS PublicacaoTipo,
                    p.requer_confirmacao_leitura AS RequerConfirmacaoLeitura,
                    COALESCE(p.data_publicacao, p.data_criacao) AS DataReferencia,
                    cp.tb_mkt_comunidade_id AS ComunidadeId,
                    com.nome AS ComunidadeNome,
                    GROUP_CONCAT(DISTINCT g.nome ORDER BY g.nome SEPARATOR ', ') AS Grupos,
                    COALESCE(i.QuantidadeCurtida, 0) AS Likes,
                    COALESCE(c.QuantidadeComentarios, 0) AS Comentarios,
                    COALESCE(i.QuantidadeConfirmacoesLeitura, 0) AS Aceites,
                    COALESCE(i.QuantidadeVisualizacao, 0) AS Visualizacoes,
                    COALESCE(i.Alcance, 0) AS Alcance,
                    MAX(CASE WHEN a.tipo = 'video' THEN 1 ELSE 0 END) AS TemVideo,
                    MAX(CASE WHEN a.tipo = 'imagem' THEN 1 ELSE 0 END) AS TemImagem,
                    MAX(CASE WHEN a.tipo = 'documento' THEN 1 ELSE 0 END) AS TemDocumento
                {joinsClause}
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        SUM(CASE WHEN visualizado = 1 THEN 1 ELSE 0 END) AS QuantidadeVisualizacao,
                        COUNT(*) AS Alcance,
                        SUM(CASE WHEN curtida_emoji IS NOT NULL AND TRIM(curtida_emoji) != '' THEN 1 ELSE 0 END) AS QuantidadeCurtida,
                        SUM(CASE WHEN confirmou_leitura = 1 THEN 1 ELSE 0 END) AS QuantidadeConfirmacoesLeitura
                    FROM tb_mkt_publicacao_colaborador_interacao
                    GROUP BY tb_mkt_publicacao_id
                ) i ON i.tb_mkt_publicacao_id = p.id
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        COUNT(*) AS QuantidadeComentarios
                    FROM tb_mkt_publicacao_comentario
                    GROUP BY tb_mkt_publicacao_id
                ) c ON c.tb_mkt_publicacao_id = p.id
                LEFT JOIN tb_mkt_publicacao_anexo a ON a.tb_mkt_publicacao_id = p.id
                {whereClause}
                GROUP BY p.id
                ORDER BY COALESCE(p.data_publicacao, p.data_criacao) DESC
                LIMIT @Limit OFFSET @Offset";

            var rows = (await connection.QueryAsync<AnalyticsRow>(listSql, parameters)).ToList();
            var itens = rows.Select(MapItemComunicadoOficial).ToList();

            return new AnalyticsResumoResponseDTO
            {
                BigNumbers = bigNumbers,
                Itens = itens,
                TotalItens = total
            };
        }

        public async Task<PostsPorComunidadeResponseDTO> ObterPostsPorComunidadeAsync(int orgId, PostsPorComunidadeRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("OrgId", orgId);

            var whereClause = BuildPostsPorComunidadeWhereClause(request, parameters);

            var sql = $@"
                SELECT
                    com.id AS ComunidadeId,
                    com.nome AS ComunidadeNome,
                    (CASE
                        WHEN com.tipo = 'publica' THEN (SELECT COUNT(*) FROM tb_mkt_comunidade_usuario_participando cup WHERE cup.tb_mkt_comunidade_id = com.id)
                        ELSE (SELECT COUNT(*) FROM (
                            SELECT u.c FROM (
                                SELECT DISTINCT gu.codigo_interno_colaborador AS c
                                FROM tb_mkt_comunidade_grupo cg
                                INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id
                                WHERE cg.tb_mkt_comunidade_id = com.id
                                UNION
                                SELECT DISTINCT cup_u.codigo_interno_colaborador AS c
                                FROM tb_mkt_comunidade_usuario_participando cup_u
                                WHERE cup_u.tb_mkt_comunidade_id = com.id
                            ) u
                            WHERE com.permite_sair = 0
                               OR NOT EXISTS (
                                   SELECT 1 FROM tb_mkt_comunidade_privada_usuario_nao_participando np
                                   WHERE np.tb_mkt_comunidade_id = com.id AND np.codigo_interno_colaborador = u.c
                               )
                        ) cnt)
                    END) AS Membros,
                    COUNT(DISTINCT p.id) AS Posts,
                    SUM(COALESCE(i.QuantidadeCurtida, 0)) AS Likes,
                    SUM(COALESCE(c.QuantidadeComentarios, 0)) AS Comentarios,
                    SUM(COALESCE(i.QuantidadeConfirmacoesLeitura, 0)) AS Aceites,
                    SUM(COALESCE(i.QuantidadeVisualizacao, 0)) AS Visualizacoes
                FROM tb_mkt_comunidade com
                INNER JOIN tb_mkt_comunidade_publicacao cp ON cp.tb_mkt_comunidade_id = com.id
                INNER JOIN tb_mkt_publicacao p ON p.id = cp.tb_mkt_publicacao_id
                    AND (p.publicacao_status = 'ativa' OR p.aprovacao_status IN ('pendente', 'aguardando_aprovacao'))
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        SUM(CASE WHEN visualizado = 1 THEN 1 ELSE 0 END) AS QuantidadeVisualizacao,
                        SUM(CASE WHEN curtida_emoji IS NOT NULL AND TRIM(curtida_emoji) != '' THEN 1 ELSE 0 END) AS QuantidadeCurtida,
                        SUM(CASE WHEN confirmou_leitura = 1 THEN 1 ELSE 0 END) AS QuantidadeConfirmacoesLeitura
                    FROM tb_mkt_publicacao_colaborador_interacao
                    GROUP BY tb_mkt_publicacao_id
                ) i ON i.tb_mkt_publicacao_id = p.id
                LEFT JOIN (
                    SELECT
                        tb_mkt_publicacao_id,
                        COUNT(*) AS QuantidadeComentarios
                    FROM tb_mkt_publicacao_comentario
                    GROUP BY tb_mkt_publicacao_id
                ) c ON c.tb_mkt_publicacao_id = p.id
                {whereClause}
                GROUP BY com.id, com.nome, com.tipo, com.permite_sair
                ORDER BY com.nome";

            var rows = (await connection.QueryAsync<PostsPorComunidadeRow>(sql, parameters)).ToList();
            var itens = rows.Select(MapPostsPorComunidadeItem).ToList();

            return new PostsPorComunidadeResponseDTO { Itens = itens };
        }

        private static string BuildPostsPorComunidadeWhereClause(PostsPorComunidadeRequestDTO request, DynamicParameters parameters)
        {
            var filtros = new List<string>
            {
                "com.tb_org_id = @OrgId",
                "com.ativo = 1"
            };

            if (request?.DataInicio != null)
            {
                filtros.Add("COALESCE(p.data_publicacao, p.data_criacao) >= @DataInicio");
                parameters.Add("DataInicio", request.DataInicio);
            }

            if (request?.DataFim != null)
            {
                filtros.Add("COALESCE(p.data_publicacao, p.data_criacao) <= @DataFim");
                parameters.Add("DataFim", request.DataFim);
            }

            return "WHERE " + string.Join(" AND ", filtros);
        }

        private static PostsPorComunidadeItemDTO MapPostsPorComunidadeItem(PostsPorComunidadeRow row)
        {
            var interacoes = row.Likes + row.Comentarios + row.Aceites;
            var engajamentoPercentual = row.Visualizacoes > 0
                ? Math.Round((decimal)interacoes / row.Visualizacoes * 100m, 0)
                : 0m;

            return new PostsPorComunidadeItemDTO
            {
                ComunidadeId = row.ComunidadeId,
                ComunidadeNome = row.ComunidadeNome ?? string.Empty,
                Membros = row.Membros,
                Posts = row.Posts,
                Likes = row.Likes,
                Comentarios = row.Comentarios,
                Aceites = row.Aceites,
                Visualizacoes = row.Visualizacoes,
                EngajamentoPercentual = engajamentoPercentual
            };
        }

        private static string GetJoinsClause()
            => @"
                FROM tb_mkt_publicacao p
                LEFT JOIN tb_mkt_comunidade_publicacao cp ON cp.tb_mkt_publicacao_id = p.id
                LEFT JOIN tb_mkt_comunidade com ON com.id = cp.tb_mkt_comunidade_id
                LEFT JOIN tb_mkt_publicaco_grupo pg ON pg.tb_mkt_publicacao_id = p.id
                LEFT JOIN tb_mkt_grupo g ON g.id = pg.tb_mkt_grupo_id";

        private static string BuildWhereClause(AnalyticsResumoRequestDTO request, DynamicParameters parameters)
        {
            var filtros = new List<string>
            {
                "p.tb_org_id = @OrgId",
                "(p.publicacao_status = 'ativa' OR p.aprovacao_status IN ('pendente', 'aguardando_aprovacao'))"
            };

            if (request?.DataInicio != null)
            {
                filtros.Add("COALESCE(p.data_publicacao, p.data_criacao) >= @DataInicio");
                parameters.Add("DataInicio", request.DataInicio);
            }

            if (request?.DataFim != null)
            {
                filtros.Add("COALESCE(p.data_publicacao, p.data_criacao) <= @DataFim");
                parameters.Add("DataFim", request.DataFim);
            }

            if (!string.IsNullOrWhiteSpace(request?.Texto))
            {
                filtros.Add("(p.titulo LIKE @Texto OR com.nome LIKE @Texto OR g.nome LIKE @Texto)");
                parameters.Add("Texto", $"%{request.Texto.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(request?.TipoConteudo))
            {
                if (string.Equals(request.TipoConteudo, "post", StringComparison.OrdinalIgnoreCase))
                    filtros.Add("cp.tb_mkt_publicacao_id IS NOT NULL");
                else if (string.Equals(request.TipoConteudo, "comunicado", StringComparison.OrdinalIgnoreCase))
                    filtros.Add("cp.tb_mkt_publicacao_id IS NULL");
            }

            return "WHERE " + string.Join(" AND ", filtros);
        }

        private static string BuildWhereClauseComunicadosOficiais(AnalyticsResumoRequestDTO request, DynamicParameters parameters)
        {
            var filtros = new List<string>
            {
                "p.tb_org_id = @OrgId",
                "(p.publicacao_status = 'ativa' OR p.aprovacao_status IN ('pendente', 'aguardando_aprovacao'))",
                "cp.tb_mkt_publicacao_id IS NULL"
            };

            if (request?.DataInicio != null)
            {
                filtros.Add("COALESCE(p.data_publicacao, p.data_criacao) >= @DataInicio");
                parameters.Add("DataInicio", request.DataInicio);
            }

            if (request?.DataFim != null)
            {
                filtros.Add("COALESCE(p.data_publicacao, p.data_criacao) <= @DataFim");
                parameters.Add("DataFim", request.DataFim);
            }

            if (!string.IsNullOrWhiteSpace(request?.Texto))
            {
                filtros.Add("(p.titulo LIKE @Texto OR com.nome LIKE @Texto OR g.nome LIKE @Texto)");
                parameters.Add("Texto", $"%{request.Texto.Trim()}%");
            }

            return "WHERE " + string.Join(" AND ", filtros);
        }

        private static AnalyticsItemDTO MapItem(AnalyticsRow row)
        {
            var isPost = row.ComunidadeId.HasValue;
            var tipo = isPost ? $"Post ({ResolverSubTipoPost(row)})" : $"Comunicado ({row.PublicacaoTipo})";

            var onde = isPost
                ? (string.IsNullOrWhiteSpace(row.ComunidadeNome) ? "Comunidade" : row.ComunidadeNome)
                : (string.IsNullOrWhiteSpace(row.Grupos) ? "Todos" : row.Grupos);

            var interacoes = row.Likes + row.Comentarios + row.Aceites;
            var engajamentoPercentual = row.Visualizacoes > 0
                ? Math.Round((decimal)interacoes / row.Visualizacoes * 100m, 0)
                : 0m;

            return new AnalyticsItemDTO
            {
                PublicacaoId = row.Id,
                Tipo = tipo,
                Titulo = row.Titulo,
                Onde = onde,
                ObrigatorioLeitura = row.RequerConfirmacaoLeitura,
                Data = row.DataReferencia,
                Likes = row.Likes,
                Comentarios = row.Comentarios,
                Aceites = row.Aceites,
                Visualizacoes = row.Visualizacoes,
                Alcance = row.Alcance,
                EngajamentoPercentual = engajamentoPercentual
            };
        }

        private static string ResolverSubTipoPost(AnalyticsRow row)
        {
            if (row.TemVideo > 0) return "video";
            if (row.TemImagem > 0) return "imagem";
            if (row.TemDocumento > 0) return "documento";
            return "texto";
        }

        private static AnalyticsItemDTO MapItemComunicadoOficial(AnalyticsRow row)
        {
            var tipo = string.IsNullOrEmpty(row.PublicacaoTipo)
                ? string.Empty
                : char.ToUpperInvariant(row.PublicacaoTipo[0]) + row.PublicacaoTipo.Substring(1).ToLowerInvariant();
            var onde = string.IsNullOrWhiteSpace(row.Grupos) ? "Todos" : row.Grupos;
            var interacoes = row.Likes + row.Comentarios + row.Aceites;
            var engajamentoPercentual = row.Visualizacoes > 0
                ? Math.Round((decimal)interacoes / row.Visualizacoes * 100m, 0)
                : 0m;

            return new AnalyticsItemDTO
            {
                PublicacaoId = row.Id,
                Tipo = tipo,
                Titulo = row.Titulo,
                Onde = onde,
                ObrigatorioLeitura = row.RequerConfirmacaoLeitura,
                Data = row.DataReferencia,
                Likes = row.Likes,
                Comentarios = row.Comentarios,
                Aceites = row.Aceites,
                Visualizacoes = row.Visualizacoes,
                Alcance = row.Alcance,
                EngajamentoPercentual = engajamentoPercentual
            };
        }
    }
}
