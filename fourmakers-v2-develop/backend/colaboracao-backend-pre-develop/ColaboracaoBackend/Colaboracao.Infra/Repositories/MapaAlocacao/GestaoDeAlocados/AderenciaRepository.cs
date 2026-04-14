using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.DomainModel.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.Aderencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados
{
    public class AderenciaRepository : IAderenciaRepository
    {
        private readonly IDBConnection _dapperConnection;

        public AderenciaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<List<PerfilAderenteDTO>> ListarPerfisDaOrgID(int orgId, string? filtro)
        {
            var _connection = _dapperConnection.GetConnection();
            var perfisAderentes = new List<PerfilAderenteDTO>();

            var query = @"
                                SELECT
                                    tgep.id as idPerfil,
                                    tgep.cod_gestor_externo  as codGestorExterno,
                                    tgep.nome_perfil,
                                    tge.nome as nome_gestor_externo,
                                    tpo.projeto as nome_projeto,
                                    tco.nome_cliente,
                                    tgep.tb_org_id,
                                    tcpa.data_inicio,
                                    tcpa.quantidade_horas,
                                    concat(coalesce(tgep.estado, ''),' - ',coalesce(tgep.cidade, '')) as ufcidade,
                                    tgep.custo_perfil,
                                    tgep.ratecard_perfil
                                FROM
                                    tb_gestor_externo_perfil tgep
                                    JOIN tb_gestor_externo tge ON tgep.cod_gestor_externo = tge.cod_gestor_externo AND tgep.tb_org_id = tge.tb_org_id
                                    LEFT JOIN tb_perfil_alocacao tpa ON tgep.id = tpa.tb_gestor_externo_perfil_id AND tgep.tb_org_id = tpa.tb_org_id
                                    LEFT JOIN tb_gestor_externo_perfil_skill tgeps ON tgep.id = tgeps.tb_gestor_externo_perfil_id
                                    LEFT JOIN tb_colaborador_periodo_alocacao tcpa ON tpa.tb_colaborador_periodo_alocacao_id = tcpa.id AND tgep.tb_org_id = tcpa.tb_org_id
                                													  AND tcpa.cod_tbd_alocado is not null --
                                    LEFT JOIN tb_projeto_org tpo ON tcpa.codigo_projeto = tpo.cod_projeto  AND tcpa.tb_org_id = tpo.tb_org_id
                                    LEFT JOIN tb_cliente_org tco ON tpo.cod_cliente = tco.codigo_cliente AND tpo.tb_org_id = tco.tb_org_id
                                    LEFT JOIN tb_modelo_trabalho tmt ON tgep.tb_modelo_trabalho_id = tmt.id
                                WHERE
                                    tgep.tb_org_id = @OrgId
                                    AND tgep.ativo = 1
                                    AND (
                                        @Filtro IS NULL
                                        OR (
                                            LOWER(tgep.nome_perfil) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                            OR LOWER(tge.nome) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                            OR LOWER(tpo.projeto) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                            OR LOWER(tco.nome_cliente) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                            OR LOWER(tgep.estado) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                            OR LOWER(tgep.cidade) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                        )
                                    )
                                GROUP BY
                                    tgep.id,
                                    tgep.tb_org_id,
                                    tgep.cod_gestor_externo,
                                    tgep.nome_perfil,
                                    tge.nome,
                                    tpo.projeto,
                                    tco.nome_cliente,
                                    tgep.tb_org_id,
                                    tcpa.data_inicio,
                                    tcpa.quantidade_horas,
                                    concat(coalesce(tgep.estado, ''),' - ',coalesce(tgep.cidade, '')),
                                    tgep.custo_perfil,
                                    tgep.ratecard_perfil;
        ";

            var parametros = new { OrgId = orgId, Filtro = filtro ?? string.Empty };
            var resultado = await _connection.QueryAsync(query, parametros);

            foreach (var row in resultado)
            {
                var perfil = new PerfilAderenteDTO
                {
                    IdPerfil = row.idPerfil,
                    CodGestorExterno = row.codGestorExterno,
                    Cargo = row.nome_perfil,
                    NomeGestor = row.nome_gestor_externo,
                    NomeProjeto = row.nome_projeto,
                    NomeCliente = row.nome_cliente,
                    Disponibilidade = row.data_inicio,
                    HorasDisponiveis = Convert.ToDouble(row.quantidade_horas),
                    UfCidade = row.ufcidade,
                    Custo = row.custo_perfil,
                    RateCard = row.ratecard_perfil
                };

                perfisAderentes.Add(perfil);
            }

            return perfisAderentes;
        }

        public async Task<List<ColaboradorAderenciaDTO>> ListarColaboradoresAderentesPorOrgId(List<string> orgsId, string filtro, List<string> skillsDescricao, string codigoInternoColaborador, Guid? gestorPerfilExterno)
        {
            var _connection = _dapperConnection.GetConnection();
            var query = @"
                    SELECT
                        tc.codigo_interno_colaborador,
                        tc.nome_completo,
                        tco.ativo,
                        NULLIF(tco.modelo_trabalho, '') AS modelo_trabalho,
                        tco.custo_hora,
                        tco.cargo,
                        TIMESTAMPDIFF(YEAR, tco.data_admissao, CURDATE()) AS tempo_de_casa_anos,
                        NULLIF(te.cidade, '') AS cidade,
                        NULLIF(te.estado, '') AS estado,
                        tco.data_admissao,
                        tcpa.id,
                        tcpa.data_inicio,
                        tcpa.data_fim,
                        tcpa.quantidade_horas,
                        tcpa.inclui_fimdesemana,
                        tcpa.ativo,
                        tcpa.data_criacao,
                        tcpa.data_alteracao,
                        tcpa.observacao,
                        tcpa.oportunidade,
                        tcpa.prioritario,
                        tcpa.percentual,
                        tcpa.codigo_colaborador,
                        tcpa.codigo_projeto,
                        tcpa.codigo_interno_colaborador,
                        tcpa.tb_org_id,
                        tcpa.cod_tbd_alocado,
                        tcpa.tb_atividade_id,
                        tcpa.retroalimenta_cv,
                        hab.id AS skill_id,
                        hab.nivel_id,
                        hab.nivel AS nivel_descricao,
                        hab.tipo AS tipo_skill,
                        hab.descricao AS skill_descricao,
                        CASE
                            WHEN tco.tb_org_id = 1 OR tbt.codigo_interno_colaborador IS NOT NULL THEN 'Banco de Talentos'
                            WHEN tco.tb_org_id = 2 THEN 'Colaborador'
                            WHEN tco.tb_org_id = 7 THEN 'Residente'
                            ELSE ''
                        END AS origem,
                        CASE
                            WHEN tcv.id IS NULL THEN 0
                            ELSE 1
                        END AS interessado,
                        acesso.nome_grupo_acesso,
                        tpo.projeto
                    FROM
                        tb_colaborador tc
                    LEFT JOIN
                        tb_colaborador_org tco
                        ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    LEFT JOIN
                        tb_endereco te
                        ON te.id = tc.endereco_id
                    LEFT JOIN
                        tb_vaga_gestor_externo_perfil tvgep
                        ON tvgep.tb_gestor_externo_perfil_id = @Perfil
                    LEFT JOIN tb_vaga_candidato tcv
                        ON tcv.vaga_id = tvgep.codigo_vaga
                        AND tcv.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN
                        tb_colaborador_periodo_alocacao tcpa
                        ON tcpa.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        AND tcpa.ativo = 1
                        AND tcpa.data_fim >= current_date()
                        AND tco.tb_org_id = tcpa.tb_org_id
                    LEFT JOIN
                        tb_projeto_org tpo
                        ON tpo.cod_projeto = tcpa.codigo_projeto
                        AND tpo.tb_org_id = tco.tb_org_id
                    LEFT JOIN (
                        -- Subconsulta para Competências
                        SELECT
                            tc.id,
                            c.codigo_interno_colaborador,
                            n.id AS nivel_id,
                            n.descricao AS nivel,
                            'Competência' AS tipo,
                            tc.descricao as descricao
                        FROM
                            tb_colaborador_competencia c
                        INNER JOIN
                            tb_nivel n ON c.tb_nivel_id = n.id
                        INNER JOIN tb_competencia tc ON c.competencia_id = tc.id
                        WHERE
                            c.ativo = 1

                        UNION ALL

                        -- Subconsulta para Metodologias
                        SELECT
                            tm.id,
                            m.codigo_interno_colaborador,
                            n.id AS nivel_id,
                            n.descricao AS nivel,
                            'Metodologia' AS tipo,
                            tm.descricao AS descricao
                        FROM
                            tb_colaborador_metodologia m
                        INNER JOIN
                            tb_nivel n ON m.tb_nivel_id = n.id
                        INNER JOIN tb_metodologia tm ON m.metodologia_id = tm.id
                        WHERE
                            m.ativo = 1

                        UNION ALL

                        -- Subconsulta para Idiomas
                        SELECT
                            ti.id,
                            tci.codigo_interno_colaborador,
                            n.id AS nivel_id,
                            n.descricao AS nivel,
                            'Idioma' AS tipo,
                            ti.descricao AS descricao
                        FROM
                            tb_colaborador_idioma tci
                        INNER JOIN
                            tb_nivel n ON tci.tb_nivel_id = n.id
                        INNER JOIN tb_idioma ti ON tci.idioma_id = ti.id
                        WHERE
                            tci.ativo = 1

                        UNION ALL

                        -- Subconsulta para Softskills
                        SELECT
                            ts.id,
                            tcs.codigo_interno_colaborador,
                            n.id AS nivel_id,
                            n.descricao AS nivel,
                            'Softskill' AS tipo,
                            ts.descricao AS descricao
                        FROM
                            tb_colaborador_softskill tcs
                        INNER JOIN
                            tb_nivel n ON tcs.tb_nivel_id = n.id
                        INNER JOIN tb_softskill ts ON tcs.softskill_id = ts.id
                        WHERE
                            tcs.ativo = 1

                        UNION ALL
                        -- Subconsulta para Dominios
                        SELECT
                            td.id,
                            tcd.codigo_interno_colaborador,
                            n.id AS nivel_id,
                            n.descricao AS nivel,
                            'Dominio' AS tipo,
                            td.descricao AS descricao
                        FROM
                            tb_colaborador_dominionegocio tcd
                        INNER JOIN
                            tb_nivel n ON tcd.tb_nivel_id = n.id
                        INNER JOIN tb_dominionegocio td ON tcd.dominionegocio_id = td.id
                        WHERE
                            tcd.ativo = 1
                    ) hab
                        ON hab.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN tb_usuario tu ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN (
                        SELECT
                            tuga.tb_usuario_id,
                            tga.descricao AS nome_grupo_acesso,
                            tga.tb_org_id
                            FROM
                                tb_usuario_grupo_acesso tuga
                            JOIN
                                tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                    ) AS acesso
                        ON acesso.tb_usuario_id = tu.id
                        AND acesso.nome_grupo_acesso = 'GESTORES'
                        AND acesso.tb_org_id = tco.tb_org_id
                    LEFT JOIN tb_banco_talentos tbt ON tbt.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    WHERE
                        (FIND_IN_SET(tco.tb_org_id, @OrgId) > 0 OR FIND_IN_SET(tbt.tb_org_id, @OrgId) > 0)
                        AND acesso.nome_grupo_acesso IS NULL
                        AND tc.visualizar_busca_aderencia = 1
                        AND (@CodigoInternoColaborador IS NULL OR @CodigoInternoColaborador = tco.codigo_interno_colaborador)
                        AND (@DescricoesHabilidade IS NULL OR FIND_IN_SET(hab.descricao, @DescricoesHabilidade) > 0)
                        AND (
                            @Filtro IS NULL
                            OR (
                                LOWER(tc.nome_completo) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                                LOWER(tco.cargo) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                                LOWER(tco.modelo_trabalho) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                                (LOWER(te.cidade) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                                LOWER(te.estado) LIKE LOWER(CONCAT('%', @Filtro, '%')))
                            )
                            OR EXISTS (
                                -- As subconsultas também precisam de `CONCAT` no filtro
                                SELECT 1
                                    FROM
                                        tb_colaborador_competencia c
                                    INNER JOIN tb_nivel n
                                        ON c.tb_nivel_id = n.id
                                    INNER JOIN tb_competencia tc
                                        ON c.competencia_id = tc.id
                                    WHERE
                                        c.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                        AND LOWER(tc.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                UNION ALL
                                SELECT 1
                                    FROM
                                        tb_colaborador_metodologia m
                                    INNER JOIN tb_nivel n
                                        ON m.tb_nivel_id = n.id
                                    INNER JOIN tb_metodologia tm
                                        ON m.metodologia_id = tm.id
                                    WHERE
                                        m.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                        AND LOWER(tm.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                UNION ALL
                                SELECT 1
                                    FROM
                                        tb_colaborador_idioma tci
                                    INNER JOIN tb_nivel n
                                        ON tci.tb_nivel_id = n.id
                                    INNER JOIN tb_idioma ti
                                        ON tci.idioma_id = ti.id
                                    WHERE
                                        tci.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                        AND LOWER(ti.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                UNION ALL
                                SELECT 1
                                    FROM
                                        tb_colaborador_softskill tcs
                                    INNER JOIN tb_nivel n
                                        ON tcs.tb_nivel_id = n.id
                                    INNER JOIN tb_softskill ts
                                        ON tcs.softskill_id = ts.id
                                    WHERE
                                        tcs.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                        AND LOWER(ts.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                UNION ALL
                                SELECT 1
                                    FROM
                                        tb_colaborador_dominionegocio tcd
                                    INNER JOIN tb_nivel n
                                        ON tcd.tb_nivel_id = n.id
                                    INNER JOIN tb_dominionegocio td
                                        ON tcd.dominionegocio_id = td.id
                                    WHERE
                                        tcd.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                        AND LOWER(td.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                            )
                        ); ";

            var parametros = new
            {
                OrgId = StringUtil.JuntarListaDeStrings(orgsId, ","),
                Filtro = filtro,
                @CodigoInternoColaborador = codigoInternoColaborador,
                @Perfil = gestorPerfilExterno,
                DescricoesHabilidade = skillsDescricao.Count > 0 ? StringUtil.JuntarListaDeStrings(skillsDescricao, ",") : null
            };

            var resultado = await _connection.QueryAsync<dynamic>(query, parametros);

            var colaboradores = resultado
                .GroupBy(item => new
                {
                    item.codigo_interno_colaborador,
                    item.nome_completo,
                    item.ativo,
                    item.modelo_trabalho,
                    item.custo_hora,
                    item.cargo,
                    item.tempo_de_casa_anos,
                    item.cidade,
                    item.estado,
                    item.data_admissao,
                    item.total_horas_15_dias,
                    item.total_horas_entre_15_30_dias,
                    item.disponibilidade_15_dias,
                    item.disponibilidade_entre_15_30_dias,
                    item.alocado,
                    item.primeira_data_inicio,
                    item.origem,
                    item.interessado
                })
                .Select(group => new ColaboradorAderenciaDTO
                {
                    Cpf = group.Key.codigo_interno_colaborador,
                    Nome = group.Key.nome_completo,
                    Ativo = group.Key.ativo == 1,
                    ModeloTrabalhoAtual = group.Key.modelo_trabalho,
                    CustoHora = group.Key.custo_hora,
                    Cargo = group.Key.cargo,
                    TempoDeCasa = Convert.ToInt32(group.Key.tempo_de_casa_anos),
                    DataAdmissao = group.Key.data_admissao,
                    Localidade =
                        (group.Key.cidade == null || group.Key.estado == null) ? null
                        : new()
                        {
                            Cidade = group.Key.cidade,
                            Estado = group.Key.estado
                        },
                    PeriodoDTOs = group.GroupBy(item => new
                    {
                        item.id,
                        item.data_inicio,
                        item.data_fim,
                        item.quantidade_horas,
                        item.inclui_fimdesemana,
                        item.ativo,
                        item.data_criacao,
                        item.data_alteracao,
                        item.observacao,
                        item.oportunidade,
                        item.prioritario,
                        item.percentual,
                        item.codigo_colaborador,
                        item.codigo_projeto,
                        item.codigo_interno_colaborador,
                        item.tb_org_id,
                        item.cod_tbd_alocado,
                        item.tb_atividade_id,
                        item.retroalimenta_cv
                    }).Where(x => x.Key.id != null).Select(skill => new PeriodoDTO
                    {
                        DataAlteracao = skill.Key.data_alteracao,
                        DataFim = skill.Key.data_fim,
                        DataInicio = skill.Key.data_inicio,
                        IncluiFimDeSemana = skill.Key.inclui_fimdesemana == 1,
                        Observacao = skill.Key.observacao,
                        Oportunidade = skill.Key.oportunidade,
                        Percentual = skill.Key.percentual,
                        Prioritario = skill.Key.prioritario,
                        QuantidadeHoras = skill.Key.quantidade_horas
                    }).ToList(),
                    Skills = group.GroupBy(item => new
                    {
                        item.skill_id,
                        item.nivel_id,
                        item.nivel_descricao,
                        item.tipo_skill,
                        item.skill_descricao
                    }).Where(x => x.Key.skill_id != null).Select(skill => new SkillNivelDTO
                    {
                        Id = skill.Key.skill_id,
                        Descricao = skill.Key.skill_descricao,
                        TipoSkill = skill.Key.tipo_skill,
                        Nivel = new()
                        {
                            Id = skill.Key.nivel_id,
                            Descricao = skill.Key.nivel_descricao
                        }
                    }).OrderBy(x => x.Descricao).ToList(),
                    Origem = group.Key.origem,
                    Interessado = Convert.ToBoolean(group.Key.interessado),
                    Projetos = group
                        .GroupBy(item => (string)item.projeto)
                        .Where(x => x.Key != null)
                        .Select(x => x.Key)
                        .OrderBy(x => x)
                        .ToList()
                }).ToList();

            return colaboradores;
        }
        public async Task<List<ColaboradorAderenciaDTO>> ListarColaboradoresAderentesPorOrgIdLimite(
                                                            List<string> orgsId,
                                                            string filtro,
                                                            List<string> skillsDescricao,
                                                            int cursor,
                                                            int limite,
                                                            string codigoInternoColaborador,
                                                            Guid? gestorPerfilExterno)
        {
            var _connection = _dapperConnection.GetConnection();

            var query = @"
                        WITH ColaboradoresFiltrados AS (
                            SELECT DISTINCT
                                tc.codigo_interno_colaborador
                            FROM
                                tb_colaborador tc
                            LEFT JOIN
                                tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                            LEFT JOIN
                                tb_endereco te ON te.id = tc.endereco_id
                            LEFT JOIN
                                tb_vaga_gestor_externo_perfil tvgep ON tvgep.tb_gestor_externo_perfil_id = @Perfil
                            LEFT JOIN 
                                tb_vaga_candidato tcv ON tcv.vaga_id = tvgep.codigo_vaga 
                                AND tcv.codigo_interno_colaborador = tc.codigo_interno_colaborador
                            LEFT JOIN
                                tb_colaborador_periodo_alocacao tcpa ON tcpa.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                AND tcpa.ativo = 1
                                AND tcpa.data_fim >= current_date()
                                AND tco.tb_org_id = tcpa.tb_org_id
                            LEFT JOIN
                                tb_projeto_org tpo ON tpo.cod_projeto = tcpa.codigo_projeto AND tpo.tb_org_id = tco.tb_org_id
                            LEFT JOIN (
                                SELECT c.codigo_interno_colaborador, tc.descricao
                                FROM tb_colaborador_competencia c
                                INNER JOIN tb_competencia tc ON c.competencia_id = tc.id
                                WHERE c.ativo = 1
                                UNION ALL
                                SELECT m.codigo_interno_colaborador, tm.descricao
                                FROM tb_colaborador_metodologia m
                                INNER JOIN tb_metodologia tm ON m.metodologia_id = tm.id
                                WHERE m.ativo = 1
                                UNION ALL
                                SELECT tci.codigo_interno_colaborador, ti.descricao
                                FROM tb_colaborador_idioma tci
                                INNER JOIN tb_idioma ti ON tci.idioma_id = ti.id
                                WHERE tci.ativo = 1
                                UNION ALL
                                SELECT tcs.codigo_interno_colaborador, ts.descricao
                                FROM tb_colaborador_softskill tcs
                                INNER JOIN tb_softskill ts ON tcs.softskill_id = ts.id
                                WHERE tcs.ativo = 1
                                UNION ALL
                                SELECT tcd.codigo_interno_colaborador, td.descricao
                                FROM tb_colaborador_dominionegocio tcd
                                INNER JOIN tb_dominionegocio td ON tcd.dominionegocio_id = td.id
                                WHERE tcd.ativo = 1
                            ) hab ON hab.codigo_interno_colaborador = tc.codigo_interno_colaborador
                            LEFT JOIN 
                                tb_usuario tu ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                            LEFT JOIN (
                                SELECT tuga.tb_usuario_id, tga.descricao AS nome_grupo_acesso, tga.tb_org_id
                                FROM tb_usuario_grupo_acesso tuga
                                JOIN tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                            ) AS acesso ON acesso.tb_usuario_id = tu.id 
                                AND acesso.nome_grupo_acesso = 'GESTORES' 
                                AND acesso.tb_org_id = tco.tb_org_id
                            LEFT JOIN 
                                tb_banco_talentos tbt ON tbt.codigo_interno_colaborador = tc.codigo_interno_colaborador
                            WHERE
                                (FIND_IN_SET(tco.tb_org_id, @OrgId) > 0 OR FIND_IN_SET(tbt.tb_org_id, @OrgId) > 0)
                                AND acesso.nome_grupo_acesso IS NULL
                                AND tc.visualizar_busca_aderencia = 1
                                AND (@CodigoInternoColaborador IS NULL OR @CodigoInternoColaborador = tco.codigo_interno_colaborador)
                                AND (@DescricoesHabilidade IS NULL OR FIND_IN_SET(hab.descricao, @DescricoesHabilidade) > 0)
                                AND (
                                    @Filtro IS NULL OR (
                                        LOWER(tc.nome_completo) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                                        LOWER(tco.cargo) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                                        LOWER(tco.modelo_trabalho) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                                        (LOWER(te.cidade) LIKE LOWER(CONCAT('%', @Filtro, '%')) OR
                                        LOWER(te.estado) LIKE LOWER(CONCAT('%', @Filtro, '%')))
                                    )
                                    OR EXISTS (
                                        SELECT 1 FROM tb_colaborador_competencia c 
                                        INNER JOIN tb_competencia tc_comp ON c.competencia_id = tc_comp.id 
                                        WHERE c.codigo_interno_colaborador = tc.codigo_interno_colaborador 
                                        AND LOWER(tc_comp.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                        UNION ALL
                                        SELECT 1 FROM tb_colaborador_metodologia m 
                                        INNER JOIN tb_metodologia tm_met ON m.metodologia_id = tm_met.id 
                                        WHERE m.codigo_interno_colaborador = tc.codigo_interno_colaborador 
                                        AND LOWER(tm_met.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                        UNION ALL
                                        SELECT 1 FROM tb_colaborador_idioma tci_idioma 
                                        INNER JOIN tb_idioma ti_idioma ON tci_idioma.idioma_id = ti_idioma.id 
                                        WHERE tci_idioma.codigo_interno_colaborador = tc.codigo_interno_colaborador 
                                        AND LOWER(ti_idioma.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                        UNION ALL
                                        SELECT 1 FROM tb_colaborador_softskill tcs_softskill 
                                        INNER JOIN tb_softskill ts_softskill ON tcs_softskill.softskill_id = ts_softskill.id 
                                        WHERE tcs_softskill.codigo_interno_colaborador = tc.codigo_interno_colaborador 
                                        AND LOWER(ts_softskill.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                        UNION ALL
                                        SELECT 1 FROM tb_colaborador_dominionegocio tcd_dom 
                                        INNER JOIN tb_dominionegocio td_dom ON tcd_dom.dominionegocio_id = td_dom.id 
                                        WHERE tcd_dom.codigo_interno_colaborador = tc.codigo_interno_colaborador 
                                        AND LOWER(td_dom.descricao) LIKE LOWER(CONCAT('%', @Filtro, '%'))
                                    )
                                )
                        ),
                        ColaboradoresPaginados AS (
                            SELECT 
                                codigo_interno_colaborador
                            FROM 
                                ColaboradoresFiltrados
                            ORDER BY 
                                codigo_interno_colaborador
                            LIMIT @Limite OFFSET @Cursor
                        )
                        SELECT
                            tc.codigo_interno_colaborador,
                            tc.nome_completo,
                            tco.ativo,
                            NULLIF(tco.modelo_trabalho, '') AS modelo_trabalho,
                            tco.custo_hora,
                            tco.cargo,
                            TIMESTAMPDIFF(YEAR, tco.data_admissao, CURDATE()) AS tempo_de_casa_anos,
                            NULLIF(te.cidade, '') AS cidade,
                            NULLIF(te.estado, '') AS estado,
                            tco.data_admissao,
                            tcpa.id,
                            tcpa.data_inicio,
                            tcpa.data_fim,
                            tcpa.quantidade_horas,
                            tcpa.inclui_fimdesemana,
                            tcpa.ativo,
                            tcpa.data_criacao,
                            tcpa.data_alteracao,
                            tcpa.observacao,
                            tcpa.oportunidade,
                            tcpa.prioritario,
                            tcpa.percentual,
                            tcpa.codigo_colaborador,
                            tcpa.codigo_projeto,
                            tcpa.codigo_interno_colaborador,
                            tcpa.tb_org_id,
                            tcpa.cod_tbd_alocado,
                            tcpa.tb_atividade_id,
                            tcpa.retroalimenta_cv,
                            hab.id AS skill_id,
                            hab.nivel_id,
                            hab.nivel AS nivel_descricao,
                            hab.tipo AS tipo_skill,
                            hab.descricao AS skill_descricao,
                            CASE
                                WHEN tco.tb_org_id = 1 OR tbt.codigo_interno_colaborador IS NOT NULL THEN 'Banco de Talentos'
                                WHEN tco.tb_org_id = 2 THEN 'Colaborador'
                                WHEN tco.tb_org_id = 7 THEN 'Residente'
                                ELSE ''
                            END AS origem,
                            CASE
                                WHEN tcv.id IS NULL THEN 0
                                ELSE 1
                            END AS interessado,
                            acesso.nome_grupo_acesso,
                            tpo.projeto
                        FROM
                            ColaboradoresPaginados cp
                        JOIN
                            tb_colaborador tc ON cp.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        LEFT JOIN
                            tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        LEFT JOIN
                            tb_endereco te ON te.id = tc.endereco_id
                        LEFT JOIN
                            tb_vaga_gestor_externo_perfil tvgep ON tvgep.tb_gestor_externo_perfil_id = @Perfil
                        LEFT JOIN 
                            tb_vaga_candidato tcv ON tcv.vaga_id = tvgep.codigo_vaga 
                            AND tcv.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        LEFT JOIN
                            tb_colaborador_periodo_alocacao tcpa ON tcpa.codigo_interno_colaborador = tco.codigo_interno_colaborador
                            AND tcpa.ativo = 1
                            AND tcpa.data_fim >= current_date()
                            AND tco.tb_org_id = tcpa.tb_org_id
                        LEFT JOIN
                            tb_projeto_org tpo ON tpo.cod_projeto = tcpa.codigo_projeto AND tpo.tb_org_id = tco.tb_org_id
                        LEFT JOIN (
                            SELECT
                                tc_hab.id,
                                c_hab.codigo_interno_colaborador,
                                n_hab.id AS nivel_id,
                                n_hab.descricao AS nivel,
                                'Competência' AS tipo,
                                tc_hab.descricao as descricao
                            FROM tb_colaborador_competencia c_hab
                            INNER JOIN tb_nivel n_hab ON c_hab.tb_nivel_id = n_hab.id
                            INNER JOIN tb_competencia tc_hab ON c_hab.competencia_id = tc_hab.id
                            WHERE c_hab.ativo = 1
                            UNION ALL
                            SELECT
                                tm_hab.id,
                                m_hab.codigo_interno_colaborador,
                                n_hab.id AS nivel_id,
                                n_hab.descricao AS nivel,
                                'Metodologia' AS tipo,
                                tm_hab.descricao AS descricao
                            FROM tb_colaborador_metodologia m_hab
                            INNER JOIN tb_nivel n_hab ON m_hab.tb_nivel_id = n_hab.id
                            INNER JOIN tb_metodologia tm_hab ON m_hab.metodologia_id = tm_hab.id
                            WHERE m_hab.ativo = 1
                            UNION ALL
                            SELECT
                                ti_hab.id,
                                tci_hab.codigo_interno_colaborador,
                                n_hab.id AS nivel_id,
                                n_hab.descricao AS nivel,
                                'Idioma' AS tipo,
                                ti_hab.descricao AS descricao
                            FROM tb_colaborador_idioma tci_hab
                            INNER JOIN tb_nivel n_hab ON tci_hab.tb_nivel_id = n_hab.id
                            INNER JOIN tb_idioma ti_hab ON tci_hab.idioma_id = ti_hab.id
                            WHERE tci_hab.ativo = 1
                            UNION ALL
                            SELECT
                                ts_hab.id,
                                tcs_hab.codigo_interno_colaborador,
                                n_hab.id AS nivel_id,
                                n_hab.descricao AS nivel,
                                'Softskill' AS tipo,
                                ts_hab.descricao AS descricao
                            FROM tb_colaborador_softskill tcs_hab
                            INNER JOIN tb_nivel n_hab ON tcs_hab.tb_nivel_id = n_hab.id
                            INNER JOIN tb_softskill ts_hab ON tcs_hab.softskill_id = ts_hab.id
                            WHERE tcs_hab.ativo = 1
                            UNION ALL
                            SELECT
                                td_hab.id,
                                tcd_hab.codigo_interno_colaborador,
                                n_hab.id AS nivel_id,
                                n_hab.descricao AS nivel,
                                'Dominio' AS tipo,
                                td_hab.descricao AS descricao
                            FROM tb_colaborador_dominionegocio tcd_hab
                            INNER JOIN tb_nivel n_hab ON tcd_hab.tb_nivel_id = n_hab.id
                            INNER JOIN tb_dominionegocio td_hab ON tcd_hab.dominionegocio_id = td_hab.id
                            WHERE tcd_hab.ativo = 1
                        ) hab ON hab.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        LEFT JOIN 
                            tb_usuario tu ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        LEFT JOIN (
                            SELECT tuga.tb_usuario_id, tga.descricao AS nome_grupo_acesso, tga.tb_org_id
                            FROM tb_usuario_grupo_acesso tuga
                            JOIN tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                        ) AS acesso ON acesso.tb_usuario_id = tu.id 
                            AND acesso.nome_grupo_acesso = 'GESTORES' 
                            AND acesso.tb_org_id = tco.tb_org_id
                        LEFT JOIN 
                            tb_banco_talentos tbt ON tbt.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        ORDER BY 
                            tc.codigo_interno_colaborador;";

            var parametros = new
            {
                OrgId = StringUtil.JuntarListaDeStrings(orgsId, ","),
                Filtro = filtro,
                CodigoInternoColaborador = codigoInternoColaborador,
                Perfil = gestorPerfilExterno,
                DescricoesHabilidade = skillsDescricao.Count > 0 ? StringUtil.JuntarListaDeStrings(skillsDescricao, ",") : null,
                Cursor = cursor,
                Limite = limite
            };

            var resultado = await _connection.QueryAsync<dynamic>(query, parametros);

            var colaboradores = resultado
                .GroupBy(item => new
                {
                    item.codigo_interno_colaborador,
                    item.nome_completo,
                    item.ativo,
                    item.modelo_trabalho,
                    item.custo_hora,
                    item.cargo,
                    item.tempo_de_casa_anos,
                    item.cidade,
                    item.estado,
                    item.data_admissao,
                    item.origem,
                    item.interessado
                })
                .Select(group => new ColaboradorAderenciaDTO
                {
                    Cpf = group.Key.codigo_interno_colaborador,
                    Nome = group.Key.nome_completo,
                    Ativo = group.Key.ativo == 1,
                    ModeloTrabalhoAtual = group.Key.modelo_trabalho,
                    CustoHora = group.Key.custo_hora,
                    Cargo = group.Key.cargo,
                    TempoDeCasa = Convert.ToInt32(group.Key.tempo_de_casa_anos),
                    DataAdmissao = group.Key.data_admissao,
                    Localidade = (group.Key.cidade == null || group.Key.estado == null) ? null : new()
                    {
                        Cidade = group.Key.cidade,
                        Estado = group.Key.estado
                    },
                    PeriodoDTOs = group.GroupBy(item => new
                    {
                        item.id,
                        item.data_inicio,
                        item.data_fim,
                        item.quantidade_horas,
                        item.inclui_fimdesemana,
                        item.ativo,
                        item.data_criacao,
                        item.data_alteracao,
                        item.observacao,
                        item.oportunidade,
                        item.prioritario,
                        item.percentual,
                        item.codigo_colaborador,
                        item.codigo_projeto,
                        item.codigo_interno_colaborador,
                        item.tb_org_id,
                        item.cod_tbd_alocado,
                        item.tb_atividade_id,
                        item.retroalimenta_cv
                    }).Where(x => x.Key.id != null).Select(periodo => new PeriodoDTO
                    {
                        DataAlteracao = periodo.Key.data_alteracao,
                        DataFim = periodo.Key.data_fim,
                        DataInicio = periodo.Key.data_inicio,
                        IncluiFimDeSemana = periodo.Key.inclui_fimdesemana == 1,
                        Observacao = periodo.Key.observacao,
                        Oportunidade = periodo.Key.oportunidade,
                        Percentual = periodo.Key.percentual,
                        Prioritario = periodo.Key.prioritario,
                        QuantidadeHoras = periodo.Key.quantidade_horas
                    }).ToList(),
                    Skills = group.GroupBy(item => new
                    {
                        item.skill_id,
                        item.nivel_id,
                        item.nivel_descricao,
                        item.tipo_skill,
                        item.skill_descricao
                    }).Where(x => x.Key.skill_id != null).Select(skill => new SkillNivelDTO
                    {
                        Id = skill.Key.skill_id,
                        Descricao = skill.Key.skill_descricao,
                        TipoSkill = skill.Key.tipo_skill,
                        Nivel = new()
                        {
                            Id = skill.Key.nivel_id,
                            Descricao = skill.Key.nivel_descricao
                        }
                    }).OrderBy(x => x.Descricao).ToList(),
                    Origem = group.Key.origem,
                    Interessado = Convert.ToBoolean(group.Key.interessado),
                    Projetos = group
                        .GroupBy(item => (string)item.projeto)
                        .Where(x => x.Key != null)
                        .Select(x => x.Key)
                        .OrderBy(x => x)
                        .ToList()
                }).ToList();

            return colaboradores;
        }
        public async Task<List<ColaboradorAderenciaSimplificadoDTO>> ListarColaboradoresAderentesPorListaId(List<string> ids, int orgId)
        {
            var _connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT
                        tc.codigo_interno_colaborador,
                        tc.nome_completo,
                        tco.ativo,
                        tco.custo_hora,
                        tco.cargo,
                        NULLIF(te.cidade, '') AS cidade,
                        NULLIF(te.estado, '') AS estado,
                        tco.data_admissao,
                        tcpa.id,
                        tcpa.data_inicio,
                        tcpa.data_fim,
                        tcpa.quantidade_horas,
                        tcpa.inclui_fimdesemana,
                        tcpa.ativo,
                        tcpa.data_criacao,
                        tcpa.data_alteracao,
                        tcpa.observacao,
                        tcpa.oportunidade,
                        tcpa.prioritario,
                        tcpa.percentual,
                        tcpa.codigo_colaborador,
                        tcpa.codigo_projeto,
                        tcpa.codigo_interno_colaborador,
                        tcpa.tb_org_id,
                        tcpa.cod_tbd_alocado,
                        tcpa.tb_atividade_id,
                        tcpa.retroalimenta_cv,
                        hab.id AS skill_id,
                        hab.nivel_id,
                        hab.nivel AS nivel_descricao,
                        hab.tipo AS tipo_skill,
                        hab.descricao AS skill_descricao
                    FROM
                        tb_colaborador tc
                    LEFT JOIN
                        tb_colaborador_org tco
                        ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    LEFT JOIN
                        tb_endereco te
                        ON te.id = tc.endereco_id
                    LEFT JOIN
                        tb_colaborador_periodo_alocacao tcpa
                        ON tcpa.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        AND tcpa.ativo = 1
                        AND tcpa.data_fim >= current_date()
                        AND tco.tb_org_id = tcpa.tb_org_id
                    LEFT JOIN vw_colaborador_skills hab
                        ON hab.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN tb_usuario tu ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    WHERE
                        FIND_IN_SET(tc.codigo_interno_colaborador, @Ids) > 0
                        AND tco.tb_org_id = @OrgId;";

            var parametros = new { Ids = StringUtil.JuntarListaDeStrings(ids, ","), OrgId = orgId };

            var resultado = await _connection.QueryAsync<dynamic>(query, parametros);

            var colaboradores = resultado
                .GroupBy(item => new
                {
                    item.codigo_interno_colaborador,
                    item.nome_completo,
                    item.cargo,
                    item.ativo,
                    item.custo_hora,
                    item.cidade,
                    item.estado,
                    item.data_admissao,
                })
                .Select(group => new ColaboradorAderenciaSimplificadoDTO
                {
                    CodigoInternoColaborador = group.Key.codigo_interno_colaborador,
                    NomeCompleto = group.Key.nome_completo,
                    Cargo = group.Key.cargo,
                    Ativo = group.Key.ativo == 1,
                    CustoHora = group.Key.custo_hora,
                    DataAdmissao = group.Key.data_admissao,
                    Localidade =
                        (group.Key.cidade == null || group.Key.estado == null) ? null
                        : new()
                        {
                            Cidade = group.Key.cidade,
                            Estado = group.Key.estado
                        },
                    PeriodoDTOs = group.GroupBy(item => new
                    {
                        item.id,
                        item.data_inicio,
                        item.data_fim,
                        item.quantidade_horas,
                        item.inclui_fimdesemana,
                        item.ativo,
                        item.data_criacao,
                        item.data_alteracao,
                        item.observacao,
                        item.oportunidade,
                        item.prioritario,
                        item.percentual,
                        item.codigo_colaborador,
                        item.codigo_projeto,
                        item.codigo_interno_colaborador,
                        item.tb_org_id,
                        item.cod_tbd_alocado,
                        item.tb_atividade_id,
                        item.retroalimenta_cv
                    }).Where(x => x.Key.id != null).Select(skill => new PeriodoDTO
                    {

                        DataAlteracao = skill.Key.data_alteracao,
                        DataFim = skill.Key.data_fim,
                        DataInicio = skill.Key.data_inicio,
                        IncluiFimDeSemana = skill.Key.inclui_fimdesemana == 1,
                        Observacao = skill.Key.observacao,
                        Oportunidade = skill.Key.oportunidade,
                        Percentual = skill.Key.percentual,
                        Prioritario = skill.Key.prioritario,
                        QuantidadeHoras = skill.Key.quantidade_horas
                    }).ToList(),
                    Skills = group.GroupBy(item => new
                    {
                        item.skill_id,
                        item.nivel_id,
                        item.nivel_descricao,
                        item.tipo_skill,
                        item.skill_descricao
                    }).Where(x => x.Key.skill_id != null).Select(skill => new SkillNivelDTO
                    {
                        Id = skill.Key.skill_id,
                        Descricao = skill.Key.skill_descricao,
                        TipoSkill = skill.Key.tipo_skill,
                        Nivel = new()
                        {
                            Id = skill.Key.nivel_id,
                            Descricao = skill.Key.nivel_descricao
                        }
                    }).OrderBy(x => x.Descricao).ToList(),
                }).ToList();

            return colaboradores;
        }

        public async Task<List<PerfilEColaboradorAderenciaDTO>> ListarAlocadosColabEPerfil(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    tcpa.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tgep.id AS PerfilId
                FROM tb_colaborador_periodo_alocacao tcpa
                INNER JOIN tb_colaborador_org tco
                    ON tco.codigo_interno_colaborador = tcpa.codigo_interno_colaborador 
                INNER JOIN tb_perfil_alocacao tpa 
                    ON tpa.tb_colaborador_periodo_alocacao_id = tcpa.id 
                    AND tpa.tb_gestor_externo_perfil_id IS NOT NULL
                INNER JOIN tb_gestor_externo_perfil tgep 
                    ON tpa.tb_gestor_externo_perfil_id = tgep.id 
                    AND tgep.ativo = 1
                WHERE 
                    tco.ativo = 1
                    AND tco.tb_org_id = @OrgId
                    AND tcpa.ativo = 1;
            ";

            var parametros = new
            {
                OrgId = orgId
            };
            
            var result = await connection.QueryAsync<dynamic>(query, parametros);

            var list = result.GroupBy(x => new
            {
                x.CodigoInternoColaborador,
                x.PerfilId
            }).Select(group => new PerfilEColaboradorAderenciaDTO
            {
                CodigoInternoColaborador = group.Key.CodigoInternoColaborador,
                PerfilId = group.Key.PerfilId
            }).ToList();
            
            return list;
        }
    }
}