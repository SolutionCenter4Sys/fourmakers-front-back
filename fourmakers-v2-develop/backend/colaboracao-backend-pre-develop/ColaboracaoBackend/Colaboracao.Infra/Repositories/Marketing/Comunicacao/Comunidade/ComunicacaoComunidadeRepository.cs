using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using ApiClient.Domain;
using Core.Domain.Marketing.Comunicacao.Comunidade;
using Dapper;
using DataTransferObject.Domain.Marketing.Comunicacao;
using DataTransferObject.Domain.Marketing.Comunicacao.Comunidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Marketing.Comunicacao.Comunidade
{
    public class ComunicacaoComunidadeRepository : IComunicacaoComunidadeRepository
    {
        private readonly IDBConnection _dapperConnection;

        /// <summary>
        /// Row para deserialização Dapper quando o banco retorna id como char(36) (string).
        /// </summary>
        private class ComunidadeResumoRow
        {
            public Guid Id { get; set; }
            public string Nome { get; set; }
            public string Descricao { get; set; }
            public string CapaUrl { get; set; }
            public string Tipo { get; set; }
            public bool PermitePostagemMembro { get; set; }
            public bool PermiteSair { get; set; }
            public string PublicacaoConfiguracaoPolitica { get; set; }
            public bool PublicacaoPermiteComentario { get; set; }
            public bool PublicacaoPermiteLikeHabilitado { get; set; }
            public int TotalPublicacoes { get; set; }
            public int TotalMembros { get; set; }
            public bool Participando { get; set; }
            public string CodigoInternoColaboradorCriacao { get; set; }
            public DateTime? DataCriacao { get; set; }
            public string CodigoInternoColaboradorUltimaAlteracao { get; set; }
            public DateTime? DataUltimaAlteracao { get; set; }
        }

        private class ColaboradorComunidadeRow
        {
            public string CodigoColaboradorInterno { get; set; }
            public string Email { get; set; }
            public string NomeCompleto { get; set; }
            public string CodDepartamento { get; set; }
            public string Departamento { get; set; }
            public string CodigoModeloContratacao { get; set; }
            public string ModeloContratacao { get; set; }
            public string CodDiretoria { get; set; }
            public string Diretoria { get; set; }
            public string ImagemPath { get; set; }
        }

        private class MembroRow
        {
            public Guid Id { get; set; }
            public string CodigoInternoColaborador { get; set; }
            public string NomeCompleto { get; set; }
            public string Email { get; set; }
            public string ImagemPath { get; set; }
            public string FormaParticipacaoUsuario { get; set; }
        }

        private class GrupoMembroRow
        {
            public Guid GrupoId { get; set; }
            public Guid Id { get; set; }
            public string CodigoInternoColaborador { get; set; }
            public string NomeCompleto { get; set; }
            public string Email { get; set; }
            public string ImagemPath { get; set; }
        }

        public ComunicacaoComunidadeRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<ComunidadesResumoResponseDTO> ObterListaComunidadesResumoAsync(int orgId, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();

            // Lista: públicas; privadas se o usuário está em grupo vinculado OU em tb_mkt_comunidade_usuario_participando (inclui quem saiu via nao_participando — ainda vê a comunidade).
            // TotalMembros: pública = cup; privada = união (grupos ∪ cup), excluindo nao_participando quando permite_sair = 1.
            // Participando: pública = cup; privada = (grupo OU cup) e, se permite_sair, fora de nao_participando.
            var sql = @"
                SELECT
                    com.id AS Id,
                    com.nome AS Nome,
                    com.descricao AS Descricao,
                    com.capa_url AS CapaUrl,
                    com.tipo AS Tipo,
                    com.permite_postagem_membro AS PermitePostagemMembro,
                    com.permite_sair AS PermiteSair,
                    com.publicacao_configuracao_politica AS PublicacaoConfiguracaoPolitica,
                    com.publicacao_permite_comentario AS PublicacaoPermiteComentario,
                    com.publicacao_permite_like_habilitado AS PublicacaoPermiteLikeHabilitado,
                    com.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                    com.data_criacao AS DataCriacao,
                    com.codigo_interno_colaborador_ultima_alteracao AS CodigoInternoColaboradorUltimaAlteracao,
                    com.data_ultima_alteracao AS DataUltimaAlteracao,
                    (SELECT COUNT(*) FROM tb_mkt_comunidade_publicacao cp WHERE cp.tb_mkt_comunidade_id = com.id) AS TotalPublicacoes,
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
                    END) AS TotalMembros,
                    ((com.tipo = 'publica' AND EXISTS (SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup2 WHERE cup2.tb_mkt_comunidade_id = com.id AND cup2.codigo_interno_colaborador = @CodigoInternoColaborador))
                     OR (com.tipo = 'privada' AND (
                            (EXISTS (
                                SELECT 1 FROM tb_mkt_comunidade_grupo cg
                                INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador
                                WHERE cg.tb_mkt_comunidade_id = com.id)
                             OR EXISTS (SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup3 WHERE cup3.tb_mkt_comunidade_id = com.id AND cup3.codigo_interno_colaborador = @CodigoInternoColaborador))
                            AND (com.permite_sair = 0 OR NOT EXISTS (SELECT 1 FROM tb_mkt_comunidade_privada_usuario_nao_participando np WHERE np.tb_mkt_comunidade_id = com.id AND np.codigo_interno_colaborador = @CodigoInternoColaborador))
                        ))) AS Participando
                FROM tb_mkt_comunidade com
                WHERE com.tb_org_id = @OrgId
                    AND com.ativo = 1
                    AND (
                        com.tipo = 'publica'
                        OR EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_grupo cg
                            INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador
                            WHERE cg.tb_mkt_comunidade_id = com.id
                        )
                        OR EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_usuario_participando cupl
                            WHERE cupl.tb_mkt_comunidade_id = com.id AND cupl.codigo_interno_colaborador = @CodigoInternoColaborador AND com.tipo = 'privada'
                        )
                    )
                ORDER BY com.nome";

            var rows = (await connection.QueryAsync<ComunidadeResumoRow>(sql, new { OrgId = orgId, CodigoInternoColaborador = codigoInternoColaborador })).ToList();
            var comunidades = rows.Select(r => new ComunidadeResumoDTO
            {
                Id = r.Id,
                Nome = r.Nome,
                Descricao = r.Descricao,
                CapaUrl = r.CapaUrl,
                Tipo = r.Tipo,
                PermitePostagemMembro = r.PermitePostagemMembro,
                PermiteSair = r.PermiteSair,
                PublicacaoConfiguracaoPolitica = r.PublicacaoConfiguracaoPolitica,
                PublicacaoPermiteComentario = r.PublicacaoPermiteComentario,
                PublicacaoPermiteLikeHabilitado = r.PublicacaoPermiteLikeHabilitado,
                TotalPublicacoes = r.TotalPublicacoes,
                TotalMembros = r.TotalMembros,
                Participando = r.Participando,
                CodigoInternoColaboradorCriacao = r.CodigoInternoColaboradorCriacao,
                DataCriacao = r.DataCriacao,
                CodigoInternoColaboradorUltimaAlteracao = r.CodigoInternoColaboradorUltimaAlteracao,
                DataUltimaAlteracao = r.DataUltimaAlteracao
            }).ToList();

            if (comunidades.Count > 0)
            {
                var ids = comunidades.Select(c => c.Id.ToString()).ToList();
                var sqlModeradores = @"
                    SELECT
                        cup.tb_mkt_comunidade_id AS ComunidadeId,
                        cup.id AS Id,
                        cup.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tc.nome_completo AS NomeCompleto,
                        tu.email AS Email,
                        ti.path AS ImagemPath
                    FROM tb_mkt_comunidade_usuario_participando cup
                    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = cup.codigo_interno_colaborador
                    LEFT JOIN tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador AND tu.tb_org_id = @OrgId
                    LEFT JOIN tb_imagem ti ON tc.imagem_id = ti.id
                    WHERE cup.tb_mkt_comunidade_id IN @Ids
                      AND cup.eh_moderador = 1
                    ORDER BY cup.tb_mkt_comunidade_id, tc.nome_completo";

                var moderadoresRows = (await connection.QueryAsync<(Guid ComunidadeId, Guid Id, string CodigoInternoColaborador, string NomeCompleto, string Email, string ImagemPath)>(
                    sqlModeradores, new { Ids = ids, OrgId = orgId })).ToList();

                var moderadoresPorComunidade = moderadoresRows
                    .GroupBy(r => r.ComunidadeId)
                    .ToDictionary(g => g.Key, g => g.Select(r => new ComunidadeMembroDTO
                    {
                        Id = r.Id,
                        CodigoInternoColaborador = r.CodigoInternoColaborador,
                        NomeCompleto = r.NomeCompleto,
                        Email = r.Email,
                        UrlFoto = !string.IsNullOrWhiteSpace(r.ImagemPath)
                            ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + r.ImagemPath
                            : null
                    }).ToList());

                foreach (var com in comunidades)
                {
                    var idStr = com.Id;
                    com.Moderadores = moderadoresPorComunidade.TryGetValue(idStr, out var mods) ? mods : new List<ComunidadeMembroDTO>();
                }

                await EnriquecerCriadorUltimoAlteradorAsync(connection, orgId, comunidades);
            }

            return new ComunidadesResumoResponseDTO
            {
                Comunidades = comunidades
            };
        }

        public async Task<ComunidadeDetalheDTO> ObterComunidadePorIdAsync(Guid id, int orgId, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var idStr = id.ToString();

            // Mesma regra de visibilidade da lista: ativo = 1 e (pública OU privada com grupo OU privada em usuario_participando).
            var sqlComunidade = @"
                SELECT
                    com.id AS Id,
                    com.nome AS Nome,
                    com.descricao AS Descricao,
                    com.capa_url AS CapaUrl,
                    com.tipo AS Tipo,
                    com.permite_postagem_membro AS PermitePostagemMembro,
                    com.permite_sair AS PermiteSair,
                    com.publicacao_configuracao_politica AS PublicacaoConfiguracaoPolitica,
                    com.publicacao_permite_comentario AS PublicacaoPermiteComentario,
                    com.publicacao_permite_like_habilitado AS PublicacaoPermiteLikeHabilitado,
                    com.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                    com.data_criacao AS DataCriacao,
                    com.codigo_interno_colaborador_ultima_alteracao AS CodigoInternoColaboradorUltimaAlteracao,
                    com.data_ultima_alteracao AS DataUltimaAlteracao,
                    (SELECT COUNT(*) FROM tb_mkt_comunidade_publicacao cp WHERE cp.tb_mkt_comunidade_id = com.id) AS TotalPublicacoes,
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
                    END) AS TotalMembros,
                    ((com.tipo = 'publica' AND EXISTS (SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup2 WHERE cup2.tb_mkt_comunidade_id = com.id AND cup2.codigo_interno_colaborador = @CodigoInternoColaborador))
                     OR (com.tipo = 'privada' AND (
                            (EXISTS (
                                SELECT 1 FROM tb_mkt_comunidade_grupo cg
                                INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador
                                WHERE cg.tb_mkt_comunidade_id = com.id)
                             OR EXISTS (SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup3 WHERE cup3.tb_mkt_comunidade_id = com.id AND cup3.codigo_interno_colaborador = @CodigoInternoColaborador))
                            AND (com.permite_sair = 0 OR NOT EXISTS (SELECT 1 FROM tb_mkt_comunidade_privada_usuario_nao_participando np WHERE np.tb_mkt_comunidade_id = com.id AND np.codigo_interno_colaborador = @CodigoInternoColaborador))
                        ))) AS Participando
                FROM tb_mkt_comunidade com
                WHERE com.id = @Id
                    AND com.tb_org_id = @OrgId
                    AND com.ativo = 1
                    AND (
                        com.tipo = 'publica'
                        OR EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_grupo cg
                            INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador
                            WHERE cg.tb_mkt_comunidade_id = com.id
                        )
                        OR EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_usuario_participando cupl
                            WHERE cupl.tb_mkt_comunidade_id = com.id AND cupl.codigo_interno_colaborador = @CodigoInternoColaborador AND com.tipo = 'privada'
                        )
                    )";

            var row = await connection.QuerySingleOrDefaultAsync<ComunidadeResumoRow>(sqlComunidade, new { Id = idStr, OrgId = orgId, CodigoInternoColaborador = codigoInternoColaborador });
            if (row == null)
                return null;

            var comunidade = new ComunidadeDetalheDTO
            {
                Id = row.Id,
                Nome = row.Nome,
                Descricao = row.Descricao,
                CapaUrl = row.CapaUrl,
                Tipo = row.Tipo,
                PermitePostagemMembro = row.PermitePostagemMembro,
                PermiteSair = row.PermiteSair,
                PublicacaoConfiguracaoPolitica = row.PublicacaoConfiguracaoPolitica,
                PublicacaoPermiteComentario = row.PublicacaoPermiteComentario,
                PublicacaoPermiteLikeHabilitado = row.PublicacaoPermiteLikeHabilitado,
                TotalPublicacoes = row.TotalPublicacoes,
                TotalMembros = row.TotalMembros,
                Participando = row.Participando,
                CodigoInternoColaboradorCriacao = row.CodigoInternoColaboradorCriacao,
                DataCriacao = row.DataCriacao,
                CodigoInternoColaboradorUltimaAlteracao = row.CodigoInternoColaboradorUltimaAlteracao,
                DataUltimaAlteracao = row.DataUltimaAlteracao
            };

            // Membros: apenas tb_mkt_comunidade_usuario_participando (individual). Privada: respeita nao_participando quando permite_sair.
            var sqlMembros = row.Tipo == "publica"
                ? $@"
                    SELECT
                        cup.id AS Id,
                        cup.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tc.nome_completo AS NomeCompleto,
                        tu.email AS Email,
                        ti.path AS ImagemPath,
                        '{ComunidadeFormaParticipacaoUsuario.UsuarioIndividual}' AS FormaParticipacaoUsuario
                    FROM tb_mkt_comunidade_usuario_participando cup
                    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = cup.codigo_interno_colaborador
                    LEFT JOIN tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador AND tu.tb_org_id = @OrgId
                    LEFT JOIN tb_imagem ti ON tc.imagem_id = ti.id
                    WHERE cup.tb_mkt_comunidade_id = @ComunidadeId
                    ORDER BY tc.nome_completo"
                : $@"
                    SELECT
                        cup.id AS Id,
                        cup.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tc.nome_completo AS NomeCompleto,
                        tu.email AS Email,
                        ti.path AS ImagemPath,
                        '{ComunidadeFormaParticipacaoUsuario.UsuarioIndividual}' AS FormaParticipacaoUsuario
                    FROM tb_mkt_comunidade_usuario_participando cup
                    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = cup.codigo_interno_colaborador
                    LEFT JOIN tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador AND tu.tb_org_id = @OrgId
                    LEFT JOIN tb_imagem ti ON tc.imagem_id = ti.id
                    WHERE cup.tb_mkt_comunidade_id = @ComunidadeId
                        AND (@PermiteSair = 0 OR NOT EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_privada_usuario_nao_participando np
                            WHERE np.tb_mkt_comunidade_id = @ComunidadeId AND np.codigo_interno_colaborador = cup.codigo_interno_colaborador))
                    ORDER BY tc.nome_completo";

            var membrosRows = (await connection.QueryAsync<MembroRow>(sqlMembros, new { ComunidadeId = idStr, OrgId = orgId, PermiteSair = row.PermiteSair ? 1 : 0 })).ToList();
            comunidade.Membros = membrosRows.Select(r => new ComunidadeMembroDTO
            {
                Id = r.Id,
                CodigoInternoColaborador = r.CodigoInternoColaborador,
                NomeCompleto = r.NomeCompleto,
                Email = r.Email,
                UrlFoto = !string.IsNullOrWhiteSpace(r.ImagemPath)
                    ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + r.ImagemPath
                    : null,
                FormaParticipacaoUsuario = r.FormaParticipacaoUsuario
            }).ToList();

            var sqlModeradores = @"
                SELECT
                    cup.id AS Id,
                    cup.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.nome_completo AS NomeCompleto,
                    tu.email AS Email,
                    ti.path AS ImagemPath
                FROM tb_mkt_comunidade_usuario_participando cup
                LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = cup.codigo_interno_colaborador
                LEFT JOIN tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador AND tu.tb_org_id = @OrgId
                LEFT JOIN tb_imagem ti ON tc.imagem_id = ti.id
                WHERE cup.tb_mkt_comunidade_id = @ComunidadeId
                  AND cup.eh_moderador = 1
                ORDER BY tc.nome_completo";

            var moderadoresRows = (await connection.QueryAsync<MembroRow>(sqlModeradores, new { ComunidadeId = idStr, OrgId = orgId })).ToList();
            comunidade.Moderadores = moderadoresRows.Select(r => new ComunidadeMembroDTO
            {
                Id = r.Id,
                CodigoInternoColaborador = r.CodigoInternoColaborador,
                NomeCompleto = r.NomeCompleto,
                Email = r.Email,
                UrlFoto = !string.IsNullOrWhiteSpace(r.ImagemPath)
                    ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + r.ImagemPath
                    : null
            }).ToList();

            var sqlGrupos = @"
                SELECT
                    g.id AS Id,
                    g.nome AS Nome,
                    g.descricao AS Descricao
                FROM tb_mkt_comunidade_grupo cg
                INNER JOIN tb_mkt_grupo g ON g.id = cg.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                WHERE cg.tb_mkt_comunidade_id = @ComunidadeId
                ORDER BY g.nome";

            comunidade.Grupos = (await connection.QueryAsync<ComunidadeGrupoResumoDTO>(sqlGrupos, new { ComunidadeId = idStr, OrgId = orgId })).ToList();
            foreach (var g in comunidade.Grupos)
                g.Membros = new List<ComunidadeMembroDTO>();

            if (comunidade.Grupos.Count > 0)
            {
                var sqlMembrosPorGrupo = $@"
                    SELECT
                        cg.tb_mkt_grupo_id AS GrupoId,
                        gu.id AS Id,
                        gu.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tc.nome_completo AS NomeCompleto,
                        tu.email AS Email,
                        ti.path AS ImagemPath
                    FROM tb_mkt_comunidade_grupo cg
                    INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id
                    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = gu.codigo_interno_colaborador
                    LEFT JOIN tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador AND tu.tb_org_id = @OrgId
                    LEFT JOIN tb_imagem ti ON tc.imagem_id = ti.id
                    WHERE cg.tb_mkt_comunidade_id = @ComunidadeId
                        AND (@PermiteSair = 0 OR NOT EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_privada_usuario_nao_participando np
                            WHERE np.tb_mkt_comunidade_id = @ComunidadeId AND np.codigo_interno_colaborador = gu.codigo_interno_colaborador))
                    ORDER BY cg.tb_mkt_grupo_id, tc.nome_completo";

                var membrosGrupoRows = (await connection.QueryAsync<GrupoMembroRow>(sqlMembrosPorGrupo, new { ComunidadeId = idStr, OrgId = orgId, PermiteSair = row.PermiteSair ? 1 : 0 })).ToList();
                var porGrupo = membrosGrupoRows
                    .GroupBy(r => r.GrupoId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(r => new ComunidadeMembroDTO
                        {
                            Id = r.Id,
                            CodigoInternoColaborador = r.CodigoInternoColaborador,
                            NomeCompleto = r.NomeCompleto,
                            Email = r.Email,
                            UrlFoto = !string.IsNullOrWhiteSpace(r.ImagemPath)
                                ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + r.ImagemPath
                                : null,
                            FormaParticipacaoUsuario = ComunidadeFormaParticipacaoUsuario.UsuarioPeloGrupo
                        }).ToList());

                foreach (var g in comunidade.Grupos)
                {
                    if (porGrupo.TryGetValue(g.Id, out var lista))
                        g.Membros = lista;
                }
            }

            await EnriquecerCriadorUltimoAlteradorAsync(connection, orgId, new List<ComunidadeResumoDTO> { comunidade });

            return comunidade;
        }

        public async Task EnriquecerColaboradoresResumoComunidadeAsync(ComunidadeResumoDTO comunidade, int orgId)
        {
            if (comunidade == null)
                return;
            var connection = _dapperConnection.GetConnection();
            await EnriquecerCriadorUltimoAlteradorAsync(connection, orgId, new List<ComunidadeResumoDTO> { comunidade });
        }

        private static async Task EnriquecerCriadorUltimoAlteradorAsync(System.Data.IDbConnection connection, int orgId, IReadOnlyList<ComunidadeResumoDTO> comunidades)
        {
            if (comunidades == null || comunidades.Count == 0)
                return;

            var codigos = comunidades
                .SelectMany(c => new[] { c.CodigoInternoColaboradorCriacao, c.CodigoInternoColaboradorUltimaAlteracao })
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var map = await ObterMapColaboradoresResumoPorCodigosAsync(connection, orgId, codigos);
            foreach (var c in comunidades)
            {
                if (!string.IsNullOrWhiteSpace(c.CodigoInternoColaboradorCriacao) && map.TryGetValue(c.CodigoInternoColaboradorCriacao, out var criador))
                    c.Criador = criador;
                if (!string.IsNullOrWhiteSpace(c.CodigoInternoColaboradorUltimaAlteracao) && map.TryGetValue(c.CodigoInternoColaboradorUltimaAlteracao, out var ultimo))
                    c.UltimoAlterador = ultimo;
            }
        }

        private static async Task<Dictionary<string, ColaboradorResumoDTO>> ObterMapColaboradoresResumoPorCodigosAsync(System.Data.IDbConnection connection, int orgId, List<string> codigosInternos)
        {
            if (codigosInternos == null || codigosInternos.Count == 0)
                return new Dictionary<string, ColaboradorResumoDTO>(StringComparer.OrdinalIgnoreCase);

            const string sql = @"
                SELECT
                    tc.codigo_interno_colaborador AS CodigoColaboradorInterno,
                    tu.email AS Email,
                    tc.nome_completo AS NomeCompleto,
                    tco.cod_departamento AS CodDepartamento,
                    d.departamento AS Departamento,
                    tco.codigo_modelo_contratacao AS CodigoModeloContratacao,
                    COALESCE(tmco.descricao, tco.modelo_contratacao) AS ModeloContratacao,
                    tco.cod_diretoria AS CodDiretoria,
                    tco.diretoria AS Diretoria,
                    ti.path AS ImagemPath
                FROM tb_colaborador tc
                LEFT JOIN tb_colaborador_org tco
                    ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    AND tco.tb_org_id = @OrgId
                    AND tco.ativo = 1
                    AND tco.cod_diretoria <> 'BANCO TALENTOS'
                LEFT JOIN tb_usuario tu
                    ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador
                    AND tu.tb_org_id = @OrgId
                LEFT JOIN tb_departamento_org d
                    ON tco.cod_departamento = d.cod_departamento
                    AND d.tb_org_id = @OrgId
                LEFT JOIN tb_modelo_contratacao_org tmco
                    ON tmco.codigo_modelo_contratacao = tco.codigo_modelo_contratacao AND tmco.tb_org_id = @OrgId
                LEFT JOIN tb_imagem ti ON tc.imagem_id = ti.id
                WHERE tc.codigo_interno_colaborador IN @Codigos";

            var rows = (await connection.QueryAsync<ColaboradorComunidadeRow>(sql, new { OrgId = orgId, Codigos = codigosInternos })).ToList();

            var comparer = StringComparer.OrdinalIgnoreCase;
            var map = new Dictionary<string, ColaboradorResumoDTO>(comparer);
            foreach (var r in rows)
            {
                if (string.IsNullOrWhiteSpace(r.CodigoColaboradorInterno))
                    continue;
                map[r.CodigoColaboradorInterno] = MapearColaboradorComunidadeRow(r);
            }

            return map;
        }

        private static ColaboradorResumoDTO MapearColaboradorComunidadeRow(ColaboradorComunidadeRow r)
        {
            return new ColaboradorResumoDTO
            {
                CodigoColaboradorInterno = r.CodigoColaboradorInterno,
                Email = r.Email,
                NomeCompleto = r.NomeCompleto,
                CodDepartamento = r.CodDepartamento,
                Departamento = r.Departamento,
                CodigoModeloContratacao = r.CodigoModeloContratacao,
                ModeloContratacao = r.ModeloContratacao,
                CodDiretoria = r.CodDiretoria,
                Diretoria = r.Diretoria,
                UrlFoto = !string.IsNullOrWhiteSpace(r.ImagemPath)
                    ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + r.ImagemPath
                    : null
            };
        }

        public async Task<bool> ColaboradorEModeradorDaComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var idStr = comunidadeId.ToString();

            const string sql = @"
                SELECT COUNT(*)
                FROM tb_mkt_comunidade_usuario_participando cup
                INNER JOIN tb_mkt_comunidade c ON c.id = cup.tb_mkt_comunidade_id AND c.tb_org_id = @OrgId
                WHERE cup.tb_mkt_comunidade_id = @ComunidadeId
                  AND cup.codigo_interno_colaborador = @CodigoInternoColaborador
                  AND cup.eh_moderador = 1";

            var count = await connection.ExecuteScalarAsync<int>(sql, new
            {
                ComunidadeId = idStr,
                OrgId = orgId,
                CodigoInternoColaborador = codigoInternoColaborador
            });

            return count > 0;
        }

        public async Task<string> ObterCodigoInternoColaboradorCriacaoComunidadeAsync(Guid comunidadeId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = @"
                SELECT codigo_interno_colaborador_criacao
                FROM tb_mkt_comunidade
                WHERE id = @Id AND tb_org_id = @OrgId";

            return await connection.QuerySingleOrDefaultAsync<string>(sql, new
            {
                Id = comunidadeId.ToString(),
                OrgId = orgId
            });
        }

        public async Task<List<string>> ObterCodigosParticipantesComunidadeAsync(Guid comunidadeId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = @"
                SELECT cup.codigo_interno_colaborador
                FROM tb_mkt_comunidade_usuario_participando cup
                INNER JOIN tb_mkt_comunidade c ON c.id = cup.tb_mkt_comunidade_id
                WHERE cup.tb_mkt_comunidade_id = @ComunidadeId
                  AND c.tb_org_id = @OrgId";

            var codigos = await connection.QueryAsync<string>(sql, new
            {
                ComunidadeId = comunidadeId.ToString(),
                OrgId = orgId
            });

            return codigos
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }

        public async Task<string> InserirComunidadeAsync(string codigoInternoColaboradorCriacao, int orgId, InserirComunidadeRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();
            var comunidadeId = Guid.NewGuid().ToString();
            var agora = DateTime.Now;

            var sqlComunidade = @"
                INSERT INTO tb_mkt_comunidade
                (
                    id,
                    nome,
                    descricao,
                    capa_url,
                    tipo,
                    permite_postagem_membro,
                    permite_sair,
                    publicacao_configuracao_politica,
                    publicacao_permite_comentario,
                    publicacao_permite_like_habilitado,
                    ativo,
                    tb_org_id,
                    data_criacao,
                    codigo_interno_colaborador_criacao,
                    codigo_interno_colaborador_ultima_alteracao,
                    data_ultima_alteracao
                )
                VALUES
                (
                    @ComunidadeId,
                    @Nome,
                    @Descricao,
                    @CapaUrl,
                    @Tipo,
                    @PermitePostagemMembro,
                    @PermiteSair,
                    @PublicacaoConfiguracaoPolitica,
                    @PublicacaoPermiteComentario,
                    @PublicacaoPermiteLikeHabilitado,
                    1,
                    @OrgId,
                    @DataCriacao,
                    @CodigoInternoColaboradorCriacao,
                    @CodigoInternoColaboradorUltimaAlteracao,
                    @DataUltimaAlteracao
                )";

            await connection.ExecuteAsync(sqlComunidade, new
            {
                ComunidadeId = comunidadeId,
                request.Nome,
                request.Descricao,
                CapaUrl = request.CapaUrl ?? "", // capa opcional
                request.Tipo,
                request.PermitePostagemMembro,
                request.PermiteSair,
                request.PublicacaoConfiguracaoPolitica,
                request.PublicacaoPermiteComentario,
                request.PublicacaoPermiteLikeHabilitado,
                OrgId = orgId,
                DataCriacao = agora,
                CodigoInternoColaboradorCriacao = codigoInternoColaboradorCriacao,
                CodigoInternoColaboradorUltimaAlteracao = codigoInternoColaboradorCriacao,
                DataUltimaAlteracao = agora
            });

            var moderadores = request.CodigosInternoColaboradoresModeradores?.Count > 0
                ? request.CodigosInternoColaboradoresModeradores
                : new List<string> { codigoInternoColaboradorCriacao };
            var codigosModeradores = ConjuntoCodigosModeradores(moderadores);

            if (request.GruposComunidade?.Count > 0)
            {
                await InserirGruposComunidadeAsync(connection, comunidadeId, request.GruposComunidade);
            }

            if (request.CodigoInternoColaboradoresParticipantes?.Count > 0)
                await InserirParticipantesComunidadeAsync(connection, comunidadeId, request.CodigoInternoColaboradoresParticipantes, codigosModeradores);

            return comunidadeId;
        }

        public async Task<bool> AtualizarComunidadeAsync(Guid id, int orgId, AtualizarComunidadeRequestDTO request, string codigoInternoColaboradorUltimaAlteracao, DateTime dataUltimaAlteracao, bool atualizarCapaUrl)
        {
            var connection = _dapperConnection.GetConnection();
            var idStr = id.ToString();

            var sqlUpdate = @"
                UPDATE tb_mkt_comunidade SET
                    nome = @Nome,
                    descricao = @Descricao,
                    capa_url = CASE WHEN @AtualizarCapaUrl = 1 THEN @CapaUrl ELSE capa_url END,
                    tipo = @Tipo,
                    permite_postagem_membro = @PermitePostagemMembro,
                    permite_sair = @PermiteSair,
                    publicacao_configuracao_politica = @PublicacaoConfiguracaoPolitica,
                    publicacao_permite_comentario = @PublicacaoPermiteComentario,
                    publicacao_permite_like_habilitado = @PublicacaoPermiteLikeHabilitado,
                    ativo = @Ativo,
                    codigo_interno_colaborador_ultima_alteracao = @CodigoInternoColaboradorUltimaAlteracao,
                    data_ultima_alteracao = @DataUltimaAlteracao
                WHERE id = @Id AND tb_org_id = @OrgId";
            var rows = await connection.ExecuteAsync(sqlUpdate, new
            {
                Id = idStr,
                OrgId = orgId,
                request.Nome,
                request.Descricao,
                AtualizarCapaUrl = atualizarCapaUrl ? 1 : 0,
                CapaUrl = request.CapaUrl ?? "",
                request.Tipo,
                request.PermitePostagemMembro,
                request.PermiteSair,
                request.PublicacaoConfiguracaoPolitica,
                request.PublicacaoPermiteComentario,
                request.PublicacaoPermiteLikeHabilitado,
                Ativo = request.Ativo ? 1 : 0,
                CodigoInternoColaboradorUltimaAlteracao = codigoInternoColaboradorUltimaAlteracao,
                DataUltimaAlteracao = dataUltimaAlteracao
            });
            if (rows == 0)
                return false;

            // Arquivar (ativo = 0): apenas desativa a comunidade; preserva grupos e participantes no banco.
            if (!request.Ativo)
                return true;

            if (request.GruposComunidade != null)
            {
                await connection.ExecuteAsync("DELETE FROM tb_mkt_comunidade_grupo WHERE tb_mkt_comunidade_id = @ComunidadeId", new { ComunidadeId = idStr });
                if (request.GruposComunidade.Count > 0)
                    await InserirGruposComunidadeAsync(connection, idStr, request.GruposComunidade);
            }

            if (request.CodigoInternoColaboradoresParticipantes != null)
            {
                await connection.ExecuteAsync("DELETE FROM tb_mkt_comunidade_usuario_participando WHERE tb_mkt_comunidade_id = @ComunidadeId", new { ComunidadeId = idStr });
                if (request.CodigoInternoColaboradoresParticipantes.Count > 0)
                {
                    var codigosModeradores = ConjuntoCodigosModeradores(request.CodigosInternoColaboradoresModeradores);
                    await InserirParticipantesComunidadeAsync(connection, idStr, request.CodigoInternoColaboradoresParticipantes, codigosModeradores);
                }
            }

            return true;
        }

        private static HashSet<string> ConjuntoCodigosModeradores(IEnumerable<string> moderadores)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            if (moderadores == null)
                return set;
            foreach (var m in moderadores)
            {
                if (string.IsNullOrWhiteSpace(m))
                    continue;
                set.Add(m.Trim());
            }
            return set;
        }

        private async Task InserirGruposComunidadeAsync(System.Data.IDbConnection connection, string comunidadeId, List<string> grupoIds)
        {
            var sql = @"
                INSERT INTO tb_mkt_comunidade_grupo (id, tb_mkt_comunidade_id, tb_mkt_grupo_id)
                VALUES (@Id, @ComunidadeId, @GrupoId)";

            foreach (var grupoId in grupoIds)
            {
                await connection.ExecuteAsync(sql, new
                {
                    Id = Guid.NewGuid().ToString(),
                    ComunidadeId = comunidadeId,
                    GrupoId = grupoId
                });
            }
        }

        private async Task InserirParticipantesComunidadeAsync(System.Data.IDbConnection connection, string comunidadeId, List<string> participantes, HashSet<string> codigosModeradores)
        {
            codigosModeradores ??= new HashSet<string>(StringComparer.Ordinal);
            var sql = @"
                INSERT INTO tb_mkt_comunidade_usuario_participando (id, tb_mkt_comunidade_id, codigo_interno_colaborador, eh_moderador)
                VALUES (@Id, @ComunidadeId, @CodigoInternoColaborador, @EhModerador)";

            foreach (var codigo in participantes)
            {
                if (string.IsNullOrWhiteSpace(codigo))
                    continue;
                var c = codigo.Trim();
                await connection.ExecuteAsync(sql, new
                {
                    Id = Guid.NewGuid().ToString(),
                    ComunidadeId = comunidadeId,
                    CodigoInternoColaborador = c,
                    EhModerador = codigosModeradores.Contains(c) ? 1 : 0
                });
            }
        }

        public async Task<bool> ParticiparComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var idStr = comunidadeId.ToString();

            var sqlComunidade = "SELECT tipo, permite_sair FROM tb_mkt_comunidade WHERE id = @ComunidadeId AND tb_org_id = @OrgId AND ativo = 1";
            var comInfo = await connection.QuerySingleOrDefaultAsync<(string Tipo, int PermiteSair)>(sqlComunidade, new { ComunidadeId = idStr, OrgId = orgId });
            if (comInfo.Tipo == null)
                return false;

            if (comInfo.Tipo == "publica")
            {
                var sqlJaParticipa = "SELECT 1 FROM tb_mkt_comunidade_usuario_participando WHERE tb_mkt_comunidade_id = @ComunidadeId AND codigo_interno_colaborador = @CodigoInternoColaborador LIMIT 1";
                var jaParticipa = await connection.ExecuteScalarAsync<int?>(sqlJaParticipa, new { ComunidadeId = idStr, CodigoInternoColaborador = codigoInternoColaborador });
                if (jaParticipa == 1)
                    return true;
                var sqlInsert = @"
                    INSERT INTO tb_mkt_comunidade_usuario_participando (id, tb_mkt_comunidade_id, codigo_interno_colaborador, eh_moderador)
                    VALUES (@Id, @ComunidadeId, @CodigoInternoColaborador, 0)";
                await connection.ExecuteAsync(sqlInsert, new { Id = Guid.NewGuid().ToString(), ComunidadeId = idStr, CodigoInternoColaborador = codigoInternoColaborador });
                return true;
            }

            var sqlEstaNoGrupo = @"
                SELECT 1 FROM tb_mkt_comunidade_grupo cg
                INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador
                WHERE cg.tb_mkt_comunidade_id = @ComunidadeId LIMIT 1";
            var estaNoGrupo = await connection.ExecuteScalarAsync<int?>(sqlEstaNoGrupo, new { ComunidadeId = idStr, CodigoInternoColaborador = codigoInternoColaborador });
            var sqlEstaNoCup = @"
                SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup
                WHERE cup.tb_mkt_comunidade_id = @ComunidadeId AND cup.codigo_interno_colaborador = @CodigoInternoColaborador LIMIT 1";
            var estaNoCup = await connection.ExecuteScalarAsync<int?>(sqlEstaNoCup, new { ComunidadeId = idStr, CodigoInternoColaborador = codigoInternoColaborador });
            if (estaNoGrupo != 1 && estaNoCup != 1)
                return false;

            if (comInfo.PermiteSair == 1)
            {
                await connection.ExecuteAsync(
                    "DELETE FROM tb_mkt_comunidade_privada_usuario_nao_participando WHERE tb_mkt_comunidade_id = @ComunidadeId AND codigo_interno_colaborador = @CodigoInternoColaborador",
                    new { ComunidadeId = idStr, CodigoInternoColaborador = codigoInternoColaborador });
            }
            return true;
        }

        public async Task<bool> SairComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var idStr = comunidadeId.ToString();

            var sqlComunidade = "SELECT tipo, permite_sair FROM tb_mkt_comunidade WHERE id = @ComunidadeId AND tb_org_id = @OrgId";
            var comInfo = await connection.QuerySingleOrDefaultAsync<(string Tipo, int PermiteSair)>(sqlComunidade, new { ComunidadeId = idStr, OrgId = orgId });
            if (comInfo.Tipo == null || comInfo.PermiteSair != 1)
                return false;

            if (comInfo.Tipo == "publica")
            {
                var rows = await connection.ExecuteAsync(
                    "DELETE FROM tb_mkt_comunidade_usuario_participando WHERE tb_mkt_comunidade_id = @ComunidadeId AND codigo_interno_colaborador = @CodigoInternoColaborador",
                    new { ComunidadeId = idStr, CodigoInternoColaborador = codigoInternoColaborador });
                return rows > 0;
            }

            var sqlEstaNoGrupo = @"
                SELECT 1 FROM tb_mkt_comunidade_grupo cg
                INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id AND gu.codigo_interno_colaborador = @CodigoInternoColaborador
                WHERE cg.tb_mkt_comunidade_id = @ComunidadeId LIMIT 1";
            var estaNoGrupo = await connection.ExecuteScalarAsync<int?>(sqlEstaNoGrupo, new { ComunidadeId = idStr, CodigoInternoColaborador = codigoInternoColaborador });
            var sqlEstaNoCup = @"
                SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup
                WHERE cup.tb_mkt_comunidade_id = @ComunidadeId AND cup.codigo_interno_colaborador = @CodigoInternoColaborador LIMIT 1";
            var estaNoCup = await connection.ExecuteScalarAsync<int?>(sqlEstaNoCup, new { ComunidadeId = idStr, CodigoInternoColaborador = codigoInternoColaborador });
            if (estaNoGrupo != 1 && estaNoCup != 1)
                return false;

            var fezAlgo = false;
            if (estaNoGrupo == 1)
            {
                var sqlInsertNaoParticipando = @"
                    INSERT IGNORE INTO tb_mkt_comunidade_privada_usuario_nao_participando (id, tb_mkt_comunidade_id, codigo_interno_colaborador, data_saida)
                    VALUES (@Id, @ComunidadeId, @CodigoInternoColaborador, @DataSaida)";
                var rowsNp = await connection.ExecuteAsync(sqlInsertNaoParticipando, new
                {
                    Id = Guid.NewGuid().ToString(),
                    ComunidadeId = idStr,
                    CodigoInternoColaborador = codigoInternoColaborador,
                    DataSaida = DateTime.Now
                });
                fezAlgo = rowsNp > 0;
            }
            if (estaNoCup == 1)
            {
                var rowsCup = await connection.ExecuteAsync(
                    "DELETE FROM tb_mkt_comunidade_usuario_participando WHERE tb_mkt_comunidade_id = @ComunidadeId AND codigo_interno_colaborador = @CodigoInternoColaborador",
                    new { ComunidadeId = idStr, CodigoInternoColaborador = codigoInternoColaborador });
                fezAlgo = fezAlgo || rowsCup > 0;
            }
            return fezAlgo;
        }
    }
}
