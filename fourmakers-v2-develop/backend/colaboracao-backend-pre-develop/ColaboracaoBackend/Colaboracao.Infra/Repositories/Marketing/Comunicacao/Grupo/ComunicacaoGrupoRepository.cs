using Colaboracao.Core.Interfaces;
using Core.Domain.Marketing.Comunicacao.Grupo;
using Dapper;
using DataTransferObject.Domain.Marketing.Comunicacao;
using DataTransferObject.Domain.Marketing.Comunicacao.Grupo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Marketing.Comunicacao.Grupo
{
    public class ComunicacaoGrupoRepository : IComunicacaoGrupoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ComunicacaoGrupoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private const string SqlColaboradoresDisponiveisBaseFromWhere = @"
                FROM
                    tb_colaborador tc
                JOIN
                    tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND tco.tb_org_id = @OrgId
                JOIN
                    tb_usuario tu ON tco.codigo_interno_colaborador = tu.codigo_interno_colaborador AND tu.tb_org_id = @OrgId
                LEFT JOIN
                    tb_modelo_contratacao_org tmco ON tmco.codigo_modelo_contratacao = tco.codigo_modelo_contratacao AND tmco.tb_org_id = @OrgId
                WHERE
                    tc.ativo = 1 and tco.ativo = 1 and tu.ativo = 1 and tco.cod_diretoria <> 'BANCO TALENTOS'";

        public async Task<List<ColaboradorDisponivelModeloContratacaoResumoDTO>> ObterSugestoesModeloContratacaoColaboradoresDisponiveisAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            return await CarregarSugestoesModeloContratacaoColaboradoresDisponiveisAsync(connection, orgId);
        }

        public async Task<List<ColaboradorDisponivelDiretoriaResumoDTO>> ObterSugestoesDiretoriaColaboradoresDisponiveisAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            return await CarregarSugestoesDiretoriaColaboradoresDisponiveisAsync(connection, orgId);
        }

        private static async Task<List<ColaboradorDisponivelModeloContratacaoResumoDTO>> CarregarSugestoesModeloContratacaoColaboradoresDisponiveisAsync(IDbConnection connection, int orgId)
        {
            var sqlModelosReferencia = @"
                SELECT
                    tco.codigo_modelo_contratacao AS CodigoModeloContratacao,
                    MAX(COALESCE(tmco.descricao, tco.modelo_contratacao)) AS Descricao,
                    COUNT(*) AS QuantidadeColaboradores" + SqlColaboradoresDisponiveisBaseFromWhere + @"
                GROUP BY tco.codigo_modelo_contratacao";

            return (await connection.QueryAsync<ColaboradorDisponivelModeloContratacaoResumoDTO>(sqlModelosReferencia, new { OrgId = orgId }))
                .OrderBy(x => x.Descricao ?? x.CodigoModeloContratacao ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static async Task<List<ColaboradorDisponivelDiretoriaResumoDTO>> CarregarSugestoesDiretoriaColaboradoresDisponiveisAsync(IDbConnection connection, int orgId)
        {
            var sqlDiretoriasReferencia = @"
                SELECT
                    tco.cod_diretoria AS CodDiretoria,
                    MAX(tco.diretoria) AS Diretoria,
                    COUNT(*) AS QuantidadeColaboradores" + SqlColaboradoresDisponiveisBaseFromWhere + @"
                GROUP BY tco.cod_diretoria";

            return (await connection.QueryAsync<ColaboradorDisponivelDiretoriaResumoDTO>(sqlDiretoriasReferencia, new { OrgId = orgId }))
                .OrderBy(x => x.Diretoria ?? x.CodDiretoria ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<ColaboradoresResponseDTO> ListarColaboradoresDisponiveisParaAdicionarAsync(string filtro, int orgId, List<string> codigosModeloContratacao = null, List<string> codigosDiretoria = null)
        {
            var connection = _dapperConnection.GetConnection();

            var codigosModelo = NormalizarListaFiltro(codigosModeloContratacao);
            var codigosDir = NormalizarListaFiltro(codigosDiretoria);

            var modelosReferencia = await CarregarSugestoesModeloContratacaoColaboradoresDisponiveisAsync(connection, orgId);
            var diretoriasReferencia = await CarregarSugestoesDiretoriaColaboradoresDisponiveisAsync(connection, orgId);

            var sql = @"
                SELECT
                    tc.codigo_interno_colaborador AS CodigoColaboradorInterno,
                    tu.email AS Email,
                    tc.nome_completo AS NomeCompleto,
                    tco.cod_departamento AS CodDepartamento,
                    tco.departamento AS Departamento,
                    tco.codigo_modelo_contratacao AS CodigoModeloContratacao,
                    COALESCE(tmco.descricao, tco.modelo_contratacao) AS ModeloContratacao,
                    tco.cod_diretoria AS CodDiretoria,
                    tco.diretoria AS Diretoria" + SqlColaboradoresDisponiveisBaseFromWhere;

            if (codigosModelo.Count > 0)
                sql += " AND tco.codigo_modelo_contratacao IN @CodigosModeloContratacao";

            if (codigosDir.Count > 0)
                sql += " AND tco.cod_diretoria IN @CodigosDiretoria";

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                sql += @"
                    AND (tc.nome_completo LIKE @Filtro OR tu.email LIKE @Filtro OR tco.departamento LIKE @Filtro)";
            }

            sql += " ORDER BY tc.nome_completo";

            var colaboradores = (await connection.QueryAsync<ColaboradorResumoDTO>(sql, new
            {
                OrgId = orgId,
                Filtro = string.IsNullOrWhiteSpace(filtro) ? null : $"%{filtro.Trim()}%",
                CodigosModeloContratacao = codigosModelo,
                CodigosDiretoria = codigosDir
            })).ToList();

            return new ColaboradoresResponseDTO
            {
                Colaboradores = colaboradores,
                ModelosContratacaoResumo = modelosReferencia,
                DiretoriasResumo = diretoriasReferencia
            };
        }

        private static List<string> NormalizarListaFiltro(List<string> valores)
        {
            if (valores == null || valores.Count == 0)
                return new List<string>();
            return valores
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }

        public async Task<List<GrupoResumoDTO>> ListarGrupoResumoAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    g.id AS Id,
                    g.nome AS Nome,
                    g.descricao AS Descricao,
                    g.permite_criar_publicacao_oficial AS PermiteCriarPublicacaoOficial,
                    g.publicacao_oficial_requer_aprovacao AS PublicacaoOficialRequerAprovacao,
                    g.aprova_publicacao_oficial AS AprovaPublicacaoOficial,
                    g.permite_criar_comunidade AS PermiteCriarComunidade,
                    g.permite_acessar_analytics AS PermiteAcessarAnalytics,
                    'Ativo' AS Status,
                    (SELECT COUNT(*) FROM tb_mkt_grupo_usuario gu WHERE gu.tb_mkt_grupo_id = g.id) AS QuantidadeParticipantes
                FROM tb_mkt_grupo g
                WHERE g.tb_org_id = @OrgId
                ORDER BY g.nome";

            var grupos = (await connection.QueryAsync<GrupoResumoDTO>(sql, new { OrgId = orgId })).ToList();
            foreach (var grupo in grupos)
                grupo.IniciaisMembros = await ObterIniciaisMembrosAsync(grupo.Id.ToString());

            return grupos;
        }

        public async Task<GrupoDetalheDTO> ObterGrupoAsync(string grupoId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sqlGrupo = @"
                SELECT
                    g.id AS Id,
                    g.nome AS Nome,
                    g.descricao AS Descricao,
                    g.permite_criar_publicacao_oficial AS PermiteCriarPublicacaoOficial,
                    g.publicacao_oficial_requer_aprovacao AS PublicacaoOficialRequerAprovacao,
                    g.aprova_publicacao_oficial AS AprovaPublicacaoOficial,
                    g.permite_criar_comunidade AS PermiteCriarComunidade,
                    g.permite_acessar_analytics AS PermiteAcessarAnalytics
                FROM tb_mkt_grupo g
                WHERE g.id = @GrupoId
                    AND g.tb_org_id = @OrgId";

            var grupo = (await connection.QueryAsync<GrupoDetalheDTO>(sqlGrupo, new { GrupoId = grupoId, OrgId = orgId })).FirstOrDefault();

            if (grupo == null)
                return null;

            var sqlMembros = @"
                SELECT
                    tc.codigo_interno_colaborador AS CodigoColaboradorInterno,
                    tu.email AS Email,
                    tc.nome_completo AS NomeCompleto,
                    tco.cod_departamento AS CodDepartamento,
                    tco.departamento AS Departamento
                FROM tb_mkt_grupo_usuario gu
                JOIN tb_colaborador tc
                    ON gu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                JOIN tb_colaborador_org tco
                    ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    AND tco.tb_org_id = @OrgId
                JOIN tb_usuario tu
                    ON tco.codigo_interno_colaborador = tu.codigo_interno_colaborador
                    AND tu.tb_org_id = @OrgId
                WHERE gu.tb_mkt_grupo_id = @GrupoId
                    AND tc.ativo = 1 AND tco.ativo = 1 AND tu.ativo = 1 AND tco.cod_diretoria <> 'BANCO TALENTOS'
                ORDER BY tc.nome_completo";

            grupo.ColaboradoresParticipantes = (await connection.QueryAsync<ColaboradorResumoDTO>(sqlMembros, new
            {
                GrupoId = grupoId,
                OrgId = orgId
            })).ToList();

            return grupo;
        }

        public async Task<string> InserirGrupoAsync(int orgId, InserirGrupoRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();
            var grupoId = Guid.NewGuid().ToString();

            var sqlGrupo = @"
                INSERT INTO tb_mkt_grupo
                (
                    id,
                    nome,
                    descricao,
                    tb_org_id,
                    permite_criar_publicacao_oficial,
                    publicacao_oficial_requer_aprovacao,
                    aprova_publicacao_oficial,
                    permite_criar_comunidade,
                    permite_acessar_analytics
                )
                VALUES
                (
                    @GrupoId,
                    @Nome,
                    @Descricao,
                    @OrgId,
                    @PermiteCriarPublicacaoOficial,
                    @PublicacaoOficialRequerAprovacao,
                    @AprovaPublicacaoOficial,
                    @PermiteCriarComunidade,
                    @PermiteAcessarAnalytics
                )";

            await connection.ExecuteAsync(sqlGrupo, new
            {
                GrupoId = grupoId,
                request.Nome,
                request.Descricao,
                OrgId = orgId,
                request.PermiteCriarPublicacaoOficial,
                request.PublicacaoOficialRequerAprovacao,
                request.AprovaPublicacaoOficial,
                request.PermiteCriarComunidade,
                request.PermiteAcessarAnalytics
            });

            if (request.CodigoInternoColaboradoresParticipantes?.Count > 0)
            {
                await InserirMembrosGrupoAsync(connection, grupoId, request.CodigoInternoColaboradoresParticipantes);
            }

            return grupoId;
        }

        public async Task<bool> AtualizarGrupoAsync(string grupoId, int orgId, AtualizarGrupoRequestDTO request)
        {
            var connection = _dapperConnection.GetConnection();

            var sqlGrupo = @"
                UPDATE
                    tb_mkt_grupo
                SET 
                    nome = @Nome,
                    descricao = @Descricao,
                    permite_criar_publicacao_oficial = @PermiteCriarPublicacaoOficial,
                    publicacao_oficial_requer_aprovacao = @PublicacaoOficialRequerAprovacao,
                    aprova_publicacao_oficial = @AprovaPublicacaoOficial,
                    permite_criar_comunidade = @PermiteCriarComunidade,
                    permite_acessar_analytics = @PermiteAcessarAnalytics
                WHERE
                    id = @GrupoId
                    AND tb_org_id = @OrgId";

            var rowsAffected = await connection.ExecuteAsync(sqlGrupo, new
            {
                GrupoId = grupoId,
                OrgId = orgId,
                request.Nome,
                request.Descricao,
                request.PermiteCriarPublicacaoOficial,
                request.PublicacaoOficialRequerAprovacao,
                request.AprovaPublicacaoOficial,
                request.PermiteCriarComunidade,
                request.PermiteAcessarAnalytics
            });

            if (rowsAffected == 0) return false;

            var sqlDeleteMembros = @"
                DELETE FROM tb_mkt_grupo_usuario
                WHERE tb_mkt_grupo_id = @GrupoId";

            await connection.ExecuteAsync(sqlDeleteMembros, new { GrupoId = grupoId });

            if (request.CodigoInternoColaboradoresParticipantes?.Count > 0)
            {
                await InserirMembrosGrupoAsync(connection, grupoId, request.CodigoInternoColaboradoresParticipantes);
            }

            return true;
        }

        public async Task<bool> DeletarGrupoAsync(string grupoId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sqlDeleteRelPublicacao = @"
                DELETE FROM tb_mkt_publicaco_grupo
                WHERE tb_mkt_grupo_id = @GrupoId";

            await connection.ExecuteAsync(sqlDeleteRelPublicacao, new { GrupoId = grupoId });

            var sqlDeleteMembros = @"
                DELETE FROM tb_mkt_grupo_usuario
                WHERE tb_mkt_grupo_id = @GrupoId";

            await connection.ExecuteAsync(sqlDeleteMembros, new { GrupoId = grupoId });

            var sql = @"
                DELETE FROM tb_mkt_grupo
                WHERE id = @GrupoId
                    AND tb_org_id = @OrgId";

            var rowsAffected = await connection.ExecuteAsync(sql, new { GrupoId = grupoId, OrgId = orgId });

            return rowsAffected > 0;
        }

        public async Task<bool> UsuarioPossuiPermissaoCriarComunidadeAsync(string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT 1
                FROM tb_mkt_grupo_usuario gu
                INNER JOIN tb_mkt_grupo g ON g.id = gu.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                WHERE gu.codigo_interno_colaborador = @CodigoInternoColaborador
                  AND g.permite_criar_comunidade = 1
                LIMIT 1";
            var existe = await connection.ExecuteScalarAsync<int?>(sql, new { CodigoInternoColaborador = codigoInternoColaborador, OrgId = orgId });
            return existe.HasValue && existe.Value == 1;
        }

        public async Task<bool> UsuarioPodeCriarPublicacaoOficialAsync(string codigoInternoColaborador, int orgId)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                return false;

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT 1
                FROM tb_mkt_grupo_usuario gu
                INNER JOIN tb_mkt_grupo g ON g.id = gu.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                WHERE gu.codigo_interno_colaborador = @CodigoInternoColaborador
                  AND g.permite_criar_publicacao_oficial = 1
                LIMIT 1";
            var existe = await connection.ExecuteScalarAsync<int?>(sql, new { CodigoInternoColaborador = codigoInternoColaborador, OrgId = orgId });
            return existe.HasValue && existe.Value == 1;
        }

        public async Task<bool> UsuarioPodeAprovarPublicacaoOficialAsync(string codigoInternoColaborador, int orgId)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                return false;

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT 1
                FROM tb_mkt_grupo_usuario gu
                INNER JOIN tb_mkt_grupo g ON g.id = gu.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                WHERE gu.codigo_interno_colaborador = @CodigoInternoColaborador
                  AND g.aprova_publicacao_oficial = 1
                LIMIT 1";
            var existe = await connection.ExecuteScalarAsync<int?>(sql, new { CodigoInternoColaborador = codigoInternoColaborador, OrgId = orgId });
            return existe.HasValue && existe.Value == 1;
        }

        public async Task<bool> ObterSomentePublicacaoOficialNoFeedOrgAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var v = await connection.QueryFirstOrDefaultAsync<int?>(@"
                SELECT somente_publicacao_oficial_no_feed
                FROM tb_mkt_org_config
                WHERE tb_org_id = @OrgId", new { OrgId = orgId });
            return v == 1;
        }

        public async Task<bool> ObterOcultarCriadorComunidadeOrgAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var v = await connection.QueryFirstOrDefaultAsync<int?>(@"
                SELECT ocultar_criador_comunidade
                FROM tb_mkt_org_config
                WHERE tb_org_id = @OrgId", new { OrgId = orgId });
            return v == 1;
        }

        public async Task<PermissoesGrupoUsuarioResponseDTO> ObterPermissoesGruposUsuarioAsync(string codigoInternoColaborador, int orgId, bool somentePublicacaoOficialNoFeed, bool ocultarCriadorComunidade)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                return new PermissoesGrupoUsuarioResponseDTO
                {
                    SomentePublicacaoOficialNoFeed = somentePublicacaoOficialNoFeed,
                    OcultarCriadorComunidade = ocultarCriadorComunidade
                };

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT
                    COALESCE(MAX(g.permite_criar_publicacao_oficial), 0) AS PermiteCriarPublicacaoOficial,
                    COALESCE(MAX(g.publicacao_oficial_requer_aprovacao), 0) AS PublicacaoOficialRequerAprovacao,
                    COALESCE(MAX(g.aprova_publicacao_oficial), 0) AS AprovaPublicacaoOficial,
                    COALESCE(MAX(g.permite_criar_comunidade), 0) AS PermiteCriarComunidade,
                    COALESCE(MAX(g.permite_acessar_analytics), 0) AS PermiteAcessarAnalytics
                FROM tb_mkt_grupo_usuario gu
                INNER JOIN tb_mkt_grupo g ON g.id = gu.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                WHERE gu.codigo_interno_colaborador = @CodigoInternoColaborador";

            var row = await connection.QuerySingleOrDefaultAsync<(int PermiteCriarPublicacaoOficial, int PublicacaoOficialRequerAprovacao, int AprovaPublicacaoOficial, int PermiteCriarComunidade, int PermiteAcessarAnalytics)?>(sql, new { CodigoInternoColaborador = codigoInternoColaborador, OrgId = orgId });
            if (row == null)
                return new PermissoesGrupoUsuarioResponseDTO
                {
                    SomentePublicacaoOficialNoFeed = somentePublicacaoOficialNoFeed,
                    OcultarCriadorComunidade = ocultarCriadorComunidade
                };

            return new PermissoesGrupoUsuarioResponseDTO
            {
                PermiteCriarPublicacaoOficial = row.Value.PermiteCriarPublicacaoOficial != 0,
                PublicacaoOficialRequerAprovacao = row.Value.PublicacaoOficialRequerAprovacao != 0,
                AprovaPublicacaoOficial = row.Value.AprovaPublicacaoOficial != 0,
                PermiteCriarComunidade = row.Value.PermiteCriarComunidade != 0,
                PermiteAcessarAnalytics = row.Value.PermiteAcessarAnalytics != 0,
                SomentePublicacaoOficialNoFeed = somentePublicacaoOficialNoFeed,
                OcultarCriadorComunidade = ocultarCriadorComunidade
            };
        }

        private async Task InserirMembrosGrupoAsync(System.Data.IDbConnection connection, string grupoId, List<string> codigosInternos)
        {
            var sqlMembro = @"
                INSERT INTO tb_mkt_grupo_usuario (id, tb_mkt_grupo_id, codigo_interno_colaborador, data_criacao)
                VALUES (@Id, @GrupoId, @CodigoInternoColaborador, @DataCriacao)";

            foreach (var codigo in codigosInternos)
            {
                await connection.ExecuteAsync(sqlMembro, new
                {
                    Id = Guid.NewGuid().ToString(),
                    GrupoId = grupoId,
                    CodigoInternoColaborador = codigo,
                    DataCriacao = DateTime.Now
                });
            }
        }

        private async Task<List<string>> ObterIniciaisMembrosAsync(string grupoId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    CONCAT(
                        UPPER(LEFT(SUBSTRING_INDEX(c.nome_completo, ' ', 1), 1)),
                        UPPER(
                            IF(
                                LOCATE(' ', c.nome_completo) > 0,
                                LEFT(SUBSTRING_INDEX(SUBSTRING_INDEX(c.nome_completo, ' ', 2), ' ', -1), 1),
                                'X'
                            )
                        )
                    ) AS Iniciais
                FROM
                    tb_mkt_grupo_usuario gu
                JOIN
                    tb_colaborador c 
                        ON gu.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE
                    gu.tb_mkt_grupo_id = @GrupoId
                LIMIT 5;
                           ";

            var iniciais = await connection.QueryAsync<string>(sql, new
            {
                GrupoId = grupoId
            });

            return iniciais.ToList();
        }
    }
}
