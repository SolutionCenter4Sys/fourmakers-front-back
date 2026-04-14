using Colaboracao.Core.Interfaces;
using Core.Domain.Competencia;
using Dapper;
using DataTransferObject.Domain.Competencia.MapaCompetencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Competencia.MapaCompetencia
{
    public class MapaCompetenciaRepository : IMapaCompetenciaRepository
    {
        private readonly IConnectionStringCore _connectionString;
        private string MAIN_QUERY = "MAIN_QUERY", SEPARATOR = "{{SEPARATOR}}";

        public MapaCompetenciaRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<ColaboradorSkillDetalheDTO>> ListaColaboradoresSkillsDetalhes(int orgId, string separador = ",")
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    var competenciaArray = new[] { "competencia", "softskill", "metodologia", "dominionegocio", "idioma" };
                    var competenciaAliasArray = new[] { "Hardskills", "Softskills", "Metodologias", "DominiosNegocio", "Idiomas" };

                    var createTemp = "";
                    var dropTempTables = "";
                    var idx = 0;

                    // Exibe o array para ver o resultado
                    foreach (var competencia in competenciaArray)
                    {
                        createTemp += @$"
                                        -- Tabela temporária para {competencia}
                                        DROP TEMPORARY TABLE IF EXISTS temp_{competencia};
                                        CREATE TEMPORARY TABLE temp_{competencia} AS
                                        SELECT
                                            tcc.codigo_interno_colaborador,
                                            GROUP_CONCAT(CONCAT(tc.descricao, ' (', CASE WHEN tn.descricao IS NULL THEN ""Não definido"" ELSE tn.descricao END, ')') SEPARATOR '{SEPARATOR} ') AS {competenciaAliasArray[idx]}
                                        FROM
                                            tb_colaborador_{competencia} tcc
                                        JOIN
                                            tb_{competencia} tc ON tc.id = tcc.{competencia}_id
                                        LEFT JOIN
                                            tb_nivel tn ON tcc.tb_nivel_id = tn.id
                                        JOIN
                                            tb_colaborador_org tco ON tco.codigo_interno_colaborador = tcc.codigo_interno_colaborador
                                        WHERE
                                            tc.ativo = 1 AND tcc.ativo = 1 AND tco.ativo = 1 AND tco.tb_org_id = @OrgId
                                        GROUP BY
                                            tco.codigo_interno_colaborador;
                                        ";

                        dropTempTables += $@"DROP TEMPORARY TABLE IF EXISTS temp_{competencia};
                                            ";

                        idx++;
                    }

                    string query = @$"
                                            SET SESSION TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

                                            {createTemp}

                                            -- Tabela temporária para alocações
                                            DROP TEMPORARY TABLE IF EXISTS temp_alocacoes;
                                            CREATE TEMPORARY TABLE temp_alocacoes AS
                                            SELECT
                                                tcpa.codigo_interno_colaborador,
                                                GROUP_CONCAT(DISTINCT tclo.nome_cliente ORDER BY tclo.nome_cliente SEPARATOR '{SEPARATOR} ') AS Clientes
                                            FROM
                                                tb_colaborador_periodo_alocacao tcpa
                                                join
                                                    tb_projeto_org tpo on tcpa.codigo_projeto = tpo.cod_projeto and tcpa.tb_org_id = tpo.tb_org_id
                                                left join
		                                            tb_cliente_org tclo ON tpo.cod_cliente = tclo.codigo_cliente AND tpo.tb_org_id = tclo.tb_org_id
                                                join
                                                    tb_colaborador_org tco ON tco.codigo_interno_colaborador = tcpa.codigo_interno_colaborador and tcpa.tb_org_id = tpo.tb_org_id
                                            WHERE
                                                tcpa.data_fim >= CURDATE() and tcpa.ativo = 1 and tco.ativo = 1 and tcpa.tb_org_id = @OrgId
                                            GROUP BY
                                                tcpa.codigo_interno_colaborador;

                                            -- Tabela temporária para formação
                                            DROP TEMPORARY TABLE IF EXISTS temp_formacao;
                                            CREATE TEMPORARY TABLE temp_formacao AS
                                            SELECT
                                                tc.codigo_interno_colaborador,
                                                GROUP_CONCAT(DISTINCT CONCAT(te.descricao, ' (', ttd.descricao, ')') SEPARATOR '{SEPARATOR} ') AS Formacoes
                                            FROM
                                                tb_colaborador tc
                                            LEFT JOIN
                                                tb_escolaridade te ON tc.codigo_interno_colaborador = te.codigo_interno_colaborador
                                            LEFT JOIN
                                                tb_formacao tf ON te.tb_formacao_id = tf.id
                                            LEFT JOIN
                                                tb_tipo_diploma ttd ON te.tipo_diploma_id = ttd.id
											WHERE
												te.ativo = 1 OR tf.descricao is null
                                            GROUP BY
                                                tc.codigo_interno_colaborador;

                                            -- Tabela temporaria para cidadanias
                                            DROP TEMPORARY TABLE IF EXISTS temp_cidadania;
                                            CREATE TEMPORARY TABLE temp_cidadania AS
                                            SELECT
                                                tcc.codigo_interno_colaborador,
                                                GROUP_CONCAT(DISTINCT CONCAT(tcn.descricao, ' (', tcns.descricao, ')') SEPARATOR '{SEPARATOR} ') AS Cidadanias
                                            FROM
                                                tb_cidadania_colaborador tcc
                                            JOIN
                                                tb_cidadania tcn ON tcn.id = tcc.tb_cidadania_id
                                            JOIN
                                                tb_cidadania_status tcns ON  tcns.id = tcc.tb_cidadania_status_id
                                            WHERE
                                                tcc.ativo = 1
                                            GROUP BY
                                                tcc.codigo_interno_colaborador;

                                            -- Tabela temporaria para vistos

                                            DROP TEMPORARY TABLE IF EXISTS temp_visto;
                                            CREATE TEMPORARY TABLE temp_visto AS
                                            SELECT
                                                tcc.codigo_interno_colaborador,
                                                GROUP_CONCAT(DISTINCT CONCAT(tcn.Descricao , ' (', tcc.validade, ')') SEPARATOR '{SEPARATOR} ') AS Vistos
                                            FROM
                                                tb_colaborador_visto tcc
                                            JOIN
                                                tb_pais tcn ON tcn.id = tcc.tb_pais_id
                                            GROUP BY
                                                tcc.codigo_interno_colaborador;

                                            -- Tabela temporaria Passaportes
                                            DROP TEMPORARY TABLE IF EXISTS temp_passaporte;
                                            CREATE TEMPORARY TABLE temp_passaporte AS
                                            SELECT
                                                tcp.codigo_interno_colaborador,
                                                GROUP_CONCAT(DISTINCT CONCAT(tn.Descricao ) SEPARATOR '{SEPARATOR} ') AS Passaportes
                                            FROM tb_colaborador_passaporte tcp
                                            JOIN tb_nacionalidade tn ON tn.id = tcp.tb_nacionalidade_id
                                            GROUP BY
                                                tcp.codigo_interno_colaborador;

                                            -- Consulta principal

                                            SELECT
                                                tco.cod_colaborador_externo AS CodColaborador,
                                                tc.nome_completo AS NomeColaborador,
                                                tu.email AS EmailColaborador,
                                                tco.cod_diretoria AS CodUnidade,
                                                tco.diretoria AS Unidade,
                                                vcg.cod_gerente AS CodColaboradorGestor,
                                                vcg.nome_completo_gerente AS NomeColaboradorGestor,
                                                ths.Hardskills,
                                                tss.Softskills,
                                                tm.Metodologias,
                                                tdn.DominiosNegocio,
                                                ti.Idiomas,
                                                tf.Formacoes,
                                                tco.data_admissao Admissao,
                                                DATEDIFF(CURDATE(), data_admissao) TempoDeCasaEmDias,
                                                tco.codigo_cargo as CodigoCargo,
                                                tco.cargo as Cargo,
                                                tce.cidade as Cidade,
                                                tce.estado as Estado,
                                            	tac.Clientes,
                                                tco.tb_org_id AS OrgId,
                                                acesso.nome_grupo_acesso,
                                                tpcd.Cidadanias,
                                                tv.Vistos,
                                                tp.Passaportes
                                            FROM
                                                tb_colaborador tc
                                            JOIN
                                                tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                            JOIN
                                                tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador
                                            LEFT JOIN
                                                vw_colaboradores_gestor vcg ON tco.codigo_interno_colaborador = vcg.codigo_interno_colaborador
                                                AND tco.tb_org_id = vcg.tb_org_id
                                            LEFT JOIN
                                                temp_competencia ths ON tc.codigo_interno_colaborador = ths.codigo_interno_colaborador
                                            LEFT JOIN
                                                temp_softskill tss ON tc.codigo_interno_colaborador = tss.codigo_interno_colaborador
                                            LEFT JOIN
                                                temp_metodologia tm ON tc.codigo_interno_colaborador = tm.codigo_interno_colaborador
                                            LEFT JOIN
                                                temp_dominionegocio tdn ON tc.codigo_interno_colaborador = tdn.codigo_interno_colaborador
                                            LEFT JOIN
                                                temp_idioma ti ON tc.codigo_interno_colaborador = ti.codigo_interno_colaborador
                                            LEFT JOIN
                                                temp_formacao tf ON tc.codigo_interno_colaborador = tf.codigo_interno_colaborador
                                            LEFT JOIN
                                                temp_alocacoes tac ON tc.codigo_interno_colaborador = tac.codigo_interno_colaborador
                                            LEFT JOIN
                                                tb_endereco tce on tc.endereco_id = tce.id
                                            LEFT JOIN
                                                temp_cidadania tpcd ON tc.codigo_interno_colaborador = tpcd.codigo_interno_colaborador
                                            LEFT JOIN temp_visto tv ON tc.codigo_interno_colaborador = tv.codigo_interno_colaborador
                                            LEFT JOIN temp_passaporte tp ON tc.codigo_interno_colaborador = tp.codigo_interno_colaborador
                                            LEFT JOIN (
                                                SELECT
                                                    tuga.tb_usuario_id,
                                                    tga.descricao AS nome_grupo_acesso
                                                FROM
                                                    tb_usuario_grupo_acesso tuga
                                                JOIN
                                                    tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                                                WHERE
                                                    tga.tb_org_id = @OrgId
                                            ) AS acesso
                                                ON acesso.tb_usuario_id = tu.id
                                                AND acesso.nome_grupo_acesso = 'GESTORES'
                                            WHERE
                                                tco.tb_org_id = @OrgId
                                                AND tco.ativo = 1
                                                AND acesso.nome_grupo_acesso IS NULL
                                            GROUP BY
                                                tco.cod_colaborador_externo,
                                                tc.nome_completo,
                                                tu.email,
                                                tco.cod_diretoria,
                                                tco.diretoria,
                                                vcg.cod_gerente,
                                                vcg.nome_completo_gerente,
                                                tco.data_admissao ,
                                                DATEDIFF(CURDATE(), data_admissao),
                                                tco.codigo_cargo,
                                                tco.cargo,
                                                tce.cidade,
                                                tce.estado,
                                            	tac.Clientes,
                                                tco.tb_org_id
                                            ORDER BY
                                            	tc.nome_completo;

                                            {dropTempTables}
                                            DROP TEMPORARY TABLE IF EXISTS temp_alocacoes;
                                            DROP TEMPORARY TABLE IF EXISTS temp_formacao;
                                            DROP TEMPORARY TABLE IF EXISTS temp_cidadania;
                                            DROP TEMPORARY TABLE IF EXISTS temp_visto;
                                            DROP TEMPORARY TABLE IF EXISTS temp_passaporte;

                                            SET SESSION TRANSACTION ISOLATION LEVEL REPEATABLE READ;
                                        ";

                    query = query.Replace(SEPARATOR, separador);

                    var result = await _connection.QueryAsync<ColaboradorSkillDetalheDTO>(query, new
                    {
                        OrgId = orgId
                    });

                    return result.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar os colaboradores.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<List<ColaboradorSkillTotalizadorDTO>> ListaColaboradoresSkillsTotalizador(int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    var queryMain = @"
                                            CREATE TEMPORARY TABLE temp_final AS
                                            SELECT
                                                'Hardskill' AS tipo,
                                                tcpt.descricao AS Descricao,
                                                tcpt_nv.descricao AS Nivel,
                                                COUNT(*) AS Quantidade
                                            FROM
                                                tb_colaborador tc
                                                LEFT JOIN tb_colaborador_competencia tcc ON tc.codigo_interno_colaborador = tcc.codigo_interno_colaborador
                                                LEFT JOIN tb_competencia tcpt ON tcc.competencia_id = tcpt.id
                                                LEFT JOIN tb_nivel tcpt_nv ON tcc.tb_nivel_id = tcpt_nv.id
                                                JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                            WHERE
                                                tco.ativo = 1
                                                AND (tcc.ativo = 1 or tcpt.descricao is null)
                                                AND tco.tb_org_id = @OrgId
                                            GROUP BY
                                                tcpt.descricao, tcpt_nv.descricao

                                            UNION ALL

                                            SELECT
                                                'SoftSkill' AS tipo,
                                                tsft.descricao AS Descricao,
                                                tsft_nv.descricao AS Nivel,
                                                COUNT(*) AS Quantidade
                                            FROM
                                                tb_colaborador tc
                                                LEFT JOIN tb_colaborador_softskill tcs ON tc.codigo_interno_colaborador = tcs.codigo_interno_colaborador
                                                LEFT JOIN tb_softskill tsft ON tcs.softskill_id = tsft.id
                                                LEFT JOIN tb_nivel tsft_nv ON tcs.tb_nivel_id = tsft_nv.id
                                                JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                            WHERE
                                                tco.ativo = 1
                                                AND (tcs.ativo = 1 or tsft.descricao is null)
                                                AND tco.tb_org_id = @OrgId
                                            GROUP BY
                                                tsft.descricao, tsft_nv.descricao

                                            UNION ALL

                                            SELECT
                                                'Metodologia' AS tipo,
                                                tm.descricao AS Descricao,
                                                tm_nv.descricao AS Nivel,
                                                COUNT(*) AS Quantidade
                                            FROM
                                                tb_colaborador tc
                                                LEFT JOIN tb_colaborador_metodologia tcm ON tc.codigo_interno_colaborador = tcm.codigo_interno_colaborador
                                                LEFT JOIN tb_metodologia tm ON tcm.metodologia_id = tm.id
                                                LEFT JOIN tb_nivel tm_nv ON tcm.tb_nivel_id = tm_nv.id
                                                JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                            WHERE
                                                tco.ativo = 1
                                                AND (tcm.ativo = 1 or tm.descricao is null)
                                                AND tco.tb_org_id = @OrgId
                                            GROUP BY
                                                tm.descricao, tm_nv.descricao

                                            UNION ALL

                                            SELECT
                                                'DominioNegocio' AS tipo,
                                                tdnb.descricao AS Descricao,
                                                tdnb_nv.descricao AS Nivel,
                                                COUNT(*) AS Quantidade
                                            FROM
                                                tb_colaborador tc
                                                LEFT JOIN tb_colaborador_dominionegocio tcd ON tc.codigo_interno_colaborador = tcd.codigo_interno_colaborador
                                                LEFT JOIN tb_dominionegocio tdnb ON tcd.dominionegocio_id = tdnb.id
                                                LEFT JOIN tb_nivel tdnb_nv ON tcd.tb_nivel_id = tdnb_nv.id
                                                JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                            WHERE
                                                tco.ativo = 1
                                                AND (tcd.ativo = 1 or tdnb.descricao is null)
                                                AND tco.tb_org_id = @OrgId
                                            GROUP BY
                                                tdnb.descricao, tdnb_nv.descricao

                                            UNION ALL

                                            SELECT
                                                'Idioma' AS tipo,
                                                ti.descricao AS Descricao,
                                                ti_nv.descricao AS Nivel,
                                                COUNT(*) AS Quantidade
                                            FROM
                                                tb_colaborador tc
                                                LEFT JOIN tb_colaborador_idioma tci ON tc.codigo_interno_colaborador = tci.codigo_interno_colaborador
                                                LEFT JOIN tb_idioma ti ON tci.idioma_id = ti.id
                                                LEFT JOIN tb_nivel ti_nv ON tci.tb_nivel_id = ti_nv.id
                                                JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                            WHERE
                                                tco.ativo = 1
                                                AND (tci.ativo = 1 or ti.descricao is null)
                                                AND tco.tb_org_id = @OrgId
                                            GROUP BY
                                                ti.descricao, ti_nv.descricao;

                                            -- Consulta final
                                            SELECT
                                                tipo,
                                                Descricao,
                                                COALESCE(Nivel, 'Não Informado') AS Nivel,
                                                Quantidade
                                            FROM
                                                temp_final
                                            WHERE Descricao is not null OR Nivel is not null
                                            ORDER BY
                                                tipo, Descricao, Nivel;

                                            -- Limpar tabela temporária
                                            DROP TEMPORARY TABLE IF EXISTS temp_final;
                                   ";

                    var query = queryMain;

                    var result = await _connection.QueryAsync<ColaboradorSkillTotalizadorDTO>(query, new
                    {
                        OrgId = orgId
                    });

                    return result.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar os colaboradores.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}