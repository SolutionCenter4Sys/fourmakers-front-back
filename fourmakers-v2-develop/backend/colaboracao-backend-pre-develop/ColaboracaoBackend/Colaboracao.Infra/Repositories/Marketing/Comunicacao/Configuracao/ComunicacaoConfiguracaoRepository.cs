using Colaboracao.Core.Interfaces;
using Core.Domain.Marketing.Comunicacao.Configuracao;
using Dapper;
using DataTransferObject.Domain.Marketing.Comunicacao.Configuracao;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Marketing.Comunicacao.Configuracao
{
    public class ComunicacaoConfiguracaoRepository : IComunicacaoConfiguracaoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ComunicacaoConfiguracaoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<ConfiguracaoNotificacaoDTO> InserirOuAtualizarConfiguracaoAsync(string codigoInternoColaborador, int orgId, ConfiguracaoNotificacaoDTO request)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                INSERT INTO tb_mkt_colaborador_configuracao
                (
                    codigo_interno_colaborador,
                    tb_org_id,
                    notifica_web,
                    notifica_teams,
                    notifica_email,
                    notifica_plataforma
                )
                VALUES
                (
                    @CodigoInternoColaborador,
                    @OrgId,
                    @NotificaWeb,
                    @NotificaTeams,
                    @NotificaEmail,
                    @NotificaPlataforma
                )
                ON DUPLICATE KEY UPDATE
                    notifica_web = @NotificaWeb,
                    notifica_teams = @NotificaTeams,
                    notifica_email = @NotificaEmail,
                    notifica_plataforma = @NotificaPlataforma";

            await connection.ExecuteAsync(sql, new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                OrgId = orgId,
                request.NotificaWeb,
                request.NotificaTeams,
                request.NotificaEmail,
                request.NotificaPlataforma
            });

            return await ObterConfiguracaoAsync(codigoInternoColaborador, orgId);
        }

        public async Task<ConfiguracaoNotificacaoDTO> ObterConfiguracaoAsync(string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    notifica_web AS NotificaWeb,
                    notifica_teams AS NotificaTeams,
                    notifica_email AS NotificaEmail,
                    notifica_plataforma AS NotificaPlataforma
                FROM tb_mkt_colaborador_configuracao
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                    AND tb_org_id = @OrgId";

            var config = (await connection.QueryAsync<ConfiguracaoNotificacaoDTO>(sql, new
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                OrgId = orgId
            })).FirstOrDefault();

            return config;
        }

        public async Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresComNotificaPlataformaAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    COALESCE(col.nome_completo, c.codigo_interno_colaborador) AS Nome
                FROM tb_mkt_colaborador_configuracao c
                LEFT JOIN tb_colaborador col ON col.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE c.tb_org_id = @OrgId
                    AND c.notifica_plataforma = 1";
            var rows = await connection.QueryAsync<ColaboradorNotificaPlataformaDTO>(sql, new { OrgId = orgId });
            return rows.ToList();
        }

        public async Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresComNotificaPlataformaPorGruposAsync(int orgId, IReadOnlyList<string> grupoIds)
        {
            if (grupoIds == null || grupoIds.Count == 0)
                return new List<ColaboradorNotificaPlataformaDTO>();

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT DISTINCT
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    COALESCE(col.nome_completo, c.codigo_interno_colaborador) AS Nome
                FROM tb_mkt_colaborador_configuracao c
                INNER JOIN tb_mkt_grupo_usuario gu ON gu.codigo_interno_colaborador = c.codigo_interno_colaborador
                INNER JOIN tb_mkt_grupo g ON g.id = gu.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                LEFT JOIN tb_colaborador col ON col.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE c.tb_org_id = @OrgId
                    AND c.notifica_plataforma = 1
                    AND gu.tb_mkt_grupo_id IN @GrupoIds";
            var rows = await connection.QueryAsync<ColaboradorNotificaPlataformaDTO>(sql, new { OrgId = orgId, GrupoIds = grupoIds });
            return rows.ToList();
        }

        public async Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComNotificaEmailAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    u.email AS Email,
                    COALESCE(col.nome_completo, c.codigo_interno_colaborador) AS Nome
                FROM tb_mkt_colaborador_configuracao c
                INNER JOIN tb_usuario u ON u.codigo_interno_colaborador = c.codigo_interno_colaborador AND u.tb_org_id = c.tb_org_id
                LEFT JOIN tb_colaborador col ON col.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE c.tb_org_id = @OrgId
                    AND c.notifica_email = 1
                    AND TRIM(COALESCE(u.email, '')) <> ''";
            var rows = await connection.QueryAsync<ColaboradorNotificaEmailDTO>(sql, new { OrgId = orgId });
            return rows.ToList();
        }

        public async Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComNotificaEmailPorGruposAsync(int orgId, IReadOnlyList<string> grupoIds)
        {
            if (grupoIds == null || grupoIds.Count == 0)
                return new List<ColaboradorNotificaEmailDTO>();

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT DISTINCT
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    u.email AS Email,
                    COALESCE(col.nome_completo, c.codigo_interno_colaborador) AS Nome
                FROM tb_mkt_colaborador_configuracao c
                INNER JOIN tb_usuario u ON u.codigo_interno_colaborador = c.codigo_interno_colaborador AND u.tb_org_id = c.tb_org_id
                INNER JOIN tb_mkt_grupo_usuario gu ON gu.codigo_interno_colaborador = c.codigo_interno_colaborador
                INNER JOIN tb_mkt_grupo g ON g.id = gu.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                LEFT JOIN tb_colaborador col ON col.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE c.tb_org_id = @OrgId
                    AND c.notifica_email = 1
                    AND TRIM(COALESCE(u.email, '')) <> ''
                    AND gu.tb_mkt_grupo_id IN @GrupoIds";
            var rows = await connection.QueryAsync<ColaboradorNotificaEmailDTO>(sql, new { OrgId = orgId, GrupoIds = grupoIds });
            return rows.ToList();
        }

        public async Task<bool> ObterEnviarEmailPublicacaoOficialSempreAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var v = await connection.QueryFirstOrDefaultAsync<int?>(@"
                SELECT enviar_email_publicacao_oficial_sempre
                FROM tb_mkt_org_config
                WHERE tb_org_id = @OrgId", new { OrgId = orgId });
            return v == 1;
        }


        public async Task<bool> ObterEnviarNotificacaoPublicacaoOficialSempreAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var v = await connection.QueryFirstOrDefaultAsync<int?>(@"
                SELECT enviar_notificacao_publicacao_oficial_sempre
                FROM tb_mkt_org_config
                WHERE tb_org_id = @OrgId", new { OrgId = orgId });
            return v == 1;
        }

        public async Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComEmailCadastradoAtivosNaOrgAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT
                    tu.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tu.email as Email,
                    COALESCE(tc.nome_completo, tu.codigo_interno_colaborador) AS Nome
                FROM 
                	tb_usuario tu
                JOIN
                	tb_colaborador_org tco ON tu.codigo_interno_colaborador = tco.codigo_interno_colaborador
                	AND tu.tb_org_id = tco.tb_org_id
                JOIN
                	tb_colaborador tc ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador
                WHERE
                	tu.tb_org_id = @OrgId
                    AND tu.ativo = 1 AND tco.ativo = 1
                    AND COALESCE(tu.sistemico, 0) = 0
                    AND tu.codigo_interno_colaborador IS NOT NULL
                    AND tco.cod_diretoria <> 'BANCO TALENTOS'";
            var rows = await connection.QueryAsync<ColaboradorNotificaEmailDTO>(sql, new { OrgId = orgId });
            return rows.ToList();
        }

        public async Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComEmailCadastradoAtivosNaOrgPorGruposAsync(int orgId, IReadOnlyList<string> grupoIds)
        {
            if (grupoIds == null || grupoIds.Count == 0)
                return new List<ColaboradorNotificaEmailDTO>();

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT DISTINCT
                    u.codigo_interno_colaborador AS CodigoInternoColaborador,
                    u.email AS Email,
                    COALESCE(col.nome_completo, u.codigo_interno_colaborador) AS Nome
                FROM tb_usuario u
                INNER JOIN tb_mkt_grupo_usuario gu ON gu.codigo_interno_colaborador = u.codigo_interno_colaborador
                INNER JOIN tb_mkt_grupo g ON g.id = gu.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                LEFT JOIN tb_colaborador col ON col.codigo_interno_colaborador = u.codigo_interno_colaborador
                WHERE u.tb_org_id = @OrgId
                    AND u.ativo = 1
                    AND COALESCE(u.sistemico, 0) = 0
                    AND u.codigo_interno_colaborador IS NOT NULL
                    AND TRIM(COALESCE(u.email, '')) <> ''
                    AND gu.tb_mkt_grupo_id IN @GrupoIds";
            var rows = await connection.QueryAsync<ColaboradorNotificaEmailDTO>(sql, new { OrgId = orgId, GrupoIds = grupoIds });
            return rows.ToList();
        }

        public async Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresAtivosNaOrgParaNotificacaoAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT
                    tu.codigo_interno_colaborador AS CodigoInternoColaborador,
                    COALESCE(tc.nome_completo, tu.codigo_interno_colaborador) AS Nome
                FROM 
                	tb_usuario tu
                JOIN
                	tb_colaborador_org tco ON tu.codigo_interno_colaborador = tco.codigo_interno_colaborador
                	AND tu.tb_org_id = tco.tb_org_id
                JOIN
                	tb_colaborador tc ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador
                WHERE
                	tu.tb_org_id = @OrgId
                    AND tu.ativo = 1 AND tco.ativo = 1
                    AND COALESCE(tu.sistemico, 0) = 0
                    AND tu.codigo_interno_colaborador IS NOT NULL
                    AND tco.cod_diretoria <> 'BANCO TALENTOS'";
            var rows = await connection.QueryAsync<ColaboradorNotificaPlataformaDTO>(sql, new { OrgId = orgId });
            return rows.ToList();
        }

        public async Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresAtivosNaOrgParaNotificacaoPorGruposAsync(int orgId, IReadOnlyList<string> grupoIds)
        {
            if (grupoIds == null || grupoIds.Count == 0)
                return new List<ColaboradorNotificaPlataformaDTO>();

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT DISTINCT
                    u.codigo_interno_colaborador AS CodigoInternoColaborador,
                    COALESCE(col.nome_completo, u.codigo_interno_colaborador) AS Nome
                FROM tb_usuario u
                INNER JOIN tb_mkt_grupo_usuario gu ON gu.codigo_interno_colaborador = u.codigo_interno_colaborador
                INNER JOIN tb_mkt_grupo g ON g.id = gu.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                LEFT JOIN tb_colaborador col ON col.codigo_interno_colaborador = u.codigo_interno_colaborador
                WHERE u.tb_org_id = @OrgId
                    AND u.ativo = 1
                    AND COALESCE(u.sistemico, 0) = 0
                    AND u.codigo_interno_colaborador IS NOT NULL
                    AND gu.tb_mkt_grupo_id IN @GrupoIds";
            var rows = await connection.QueryAsync<ColaboradorNotificaPlataformaDTO>(sql, new { OrgId = orgId, GrupoIds = grupoIds });
            return rows.ToList();
        }

        public async Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresComNotificaPlataformaPorComunidadeAsync(int orgId, string comunidadeId)
        {
            if (string.IsNullOrWhiteSpace(comunidadeId))
                return new List<ColaboradorNotificaPlataformaDTO>();

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT DISTINCT
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    COALESCE(col.nome_completo, c.codigo_interno_colaborador) AS Nome
                FROM tb_mkt_colaborador_configuracao c
                INNER JOIN tb_mkt_comunidade com ON com.id = @ComunidadeId AND com.tb_org_id = @OrgId AND com.ativo = 1
                LEFT JOIN tb_colaborador col ON col.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE c.tb_org_id = @OrgId
                    AND c.notifica_plataforma = 1
                    AND NOT EXISTS (
                        SELECT 1 FROM tb_mkt_comunidade_privada_usuario_nao_participando np
                        WHERE np.tb_mkt_comunidade_id = @ComunidadeId
                            AND np.codigo_interno_colaborador = c.codigo_interno_colaborador
                    )
                    AND (
                        EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_grupo cg
                            INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id
                                AND gu.codigo_interno_colaborador = c.codigo_interno_colaborador
                            INNER JOIN tb_mkt_grupo g ON g.id = cg.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                            WHERE cg.tb_mkt_comunidade_id = @ComunidadeId
                        )
                        OR EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup
                            WHERE cup.tb_mkt_comunidade_id = @ComunidadeId
                                AND cup.codigo_interno_colaborador = c.codigo_interno_colaborador
                        )
                    )";
            var rows = await connection.QueryAsync<ColaboradorNotificaPlataformaDTO>(sql, new { OrgId = orgId, ComunidadeId = comunidadeId });
            return rows.ToList();
        }

        public async Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComNotificaEmailPorComunidadeAsync(int orgId, string comunidadeId)
        {
            if (string.IsNullOrWhiteSpace(comunidadeId))
                return new List<ColaboradorNotificaEmailDTO>();

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT DISTINCT
                    c.codigo_interno_colaborador AS CodigoInternoColaborador,
                    u.email AS Email,
                    COALESCE(col.nome_completo, c.codigo_interno_colaborador) AS Nome
                FROM tb_mkt_colaborador_configuracao c
                INNER JOIN tb_usuario u ON u.codigo_interno_colaborador = c.codigo_interno_colaborador AND u.tb_org_id = c.tb_org_id
                INNER JOIN tb_mkt_comunidade com ON com.id = @ComunidadeId AND com.tb_org_id = @OrgId AND com.ativo = 1
                LEFT JOIN tb_colaborador col ON col.codigo_interno_colaborador = c.codigo_interno_colaborador
                WHERE c.tb_org_id = @OrgId
                    AND c.notifica_email = 1
                    AND TRIM(COALESCE(u.email, '')) <> ''
                    AND NOT EXISTS (
                        SELECT 1 FROM tb_mkt_comunidade_privada_usuario_nao_participando np
                        WHERE np.tb_mkt_comunidade_id = @ComunidadeId
                            AND np.codigo_interno_colaborador = c.codigo_interno_colaborador
                    )
                    AND (
                        EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_grupo cg
                            INNER JOIN tb_mkt_grupo_usuario gu ON gu.tb_mkt_grupo_id = cg.tb_mkt_grupo_id
                                AND gu.codigo_interno_colaborador = c.codigo_interno_colaborador
                            INNER JOIN tb_mkt_grupo g ON g.id = cg.tb_mkt_grupo_id AND g.tb_org_id = @OrgId
                            WHERE cg.tb_mkt_comunidade_id = @ComunidadeId
                        )
                        OR EXISTS (
                            SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup
                            WHERE cup.tb_mkt_comunidade_id = @ComunidadeId
                                AND cup.codigo_interno_colaborador = c.codigo_interno_colaborador
                        )
                    )";
            var rows = await connection.QueryAsync<ColaboradorNotificaEmailDTO>(sql, new { OrgId = orgId, ComunidadeId = comunidadeId });
            return rows.ToList();
        }
    }
}
