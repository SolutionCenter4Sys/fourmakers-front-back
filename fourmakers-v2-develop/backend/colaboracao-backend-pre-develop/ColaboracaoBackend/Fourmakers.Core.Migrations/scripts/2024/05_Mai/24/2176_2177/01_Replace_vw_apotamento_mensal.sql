CREATE OR REPLACE VIEW `vw_apontamento_mensal` AS
    SELECT 
        `resultado_select`.`cod_projeto` AS `cod_projeto`,
        `resultado_select`.`nome_projeto` AS `nome_projeto`,
        `resultado_select`.`gerente` AS `gerente`,
        `resultado_select`.`tipo_gerente` AS `tipo_gerente`,
        `resultado_select`.`cpf_gerente` AS `cpf_gerente`,
        `resultado_select`.`cod_gerente` AS `cod_gerente`,
        `resultado_select`.`total_horas` AS `total_horas`,
        `resultado_select`.`mes` AS `mes`,
        `resultado_select`.`ano` AS `ano`,
        `resultado_select`.`apontamento_reprovado` AS `apontamento_reprovado`,
        `resultado_select`.`tb_colaborador_cpf` AS `tb_colaborador_cpf`,
        `resultado_select`.`tb_org_id` AS `tb_org_id`,
        `resultado_select`.`cod_status_mensal` AS `cod_status_mensal`,
        `tsa_resultado`.`descricao` AS `descricao_status_mensal`,
        `tsa_resultado`.`prioridade` AS `status_apontamento_grupo_prioridade`,
        `resultado_select`.`cod_cliente` AS `cod_cliente`,
        `resultado_select`.`nome_cliente` AS `nome_cliente`
    FROM
        ((SELECT 
            `tpo`.`cod_projeto` AS `cod_projeto`,
                `tpo`.`projeto` AS `nome_projeto`,
                COALESCE(`tc_gerente`.`nome_completo`, 'GERENTE NÃO INFORMADO') AS gerente,
                tpg.tipo_gerente AS tipo_gerente,
                tc_gerente.cpf AS cpf_gerente,
                tco_gerente.cod_colaborador_externo AS cod_gerente,
                SUM(tca.horas) AS total_horas,
                tv.mes AS mes,
                tv.ano AS ano,
                (CASE
                    WHEN
                        EXISTS( SELECT 
                                1
                            FROM
                                tb_status_apontamento_grupo tsag2
                            WHERE
                                ((tsag2.id = tsag.id)
                                    AND (tsag2.cod_status_grupo = 3)))
                    THEN
                        1
                    ELSE 0
                END) AS apontamento_reprovado,
                tca.tb_colaborador_org_tb_colaborador_cpf AS tb_colaborador_cpf,
                tca.tb_org_id AS tb_org_id,
                (CASE
                    WHEN
                        (SUM((CASE
                            WHEN (tsag.cod_status_grupo = 3) THEN 1
                            ELSE 0
                        END)) > 0)
                    THEN
                        3
                    WHEN
                        (SUM((CASE
                            WHEN (tsag.cod_status_grupo = 2) THEN 1
                            ELSE 0
                        END)) = COUNT(0))
                    THEN
                        2
                    ELSE 1
                END) AS cod_status_mensal,
                tpo.cod_cliente AS cod_cliente,
                tpo.cliente AS nome_cliente
        FROM
            (((((((tb_colaborador_apontamento tca
        JOIN tb_status_apontamento tsa ON ((tca.tb_status_apontamento_id = tsa.id)))
        JOIN tb_status_apontamento_grupo tsag ON ((tsa.tb_cod_status_grupo = tsag.cod_status_grupo)))
        JOIN tb_projeto_org tpo ON (((tca.tb_projeto_org_cod_projeto = tpo.cod_projeto)
            AND (tca.tb_org_id = tpo.tb_org_id))))
        LEFT JOIN tb_projeto_gerente tpg ON (((tpo.cod_projeto = tpg.cod_projeto)
            AND (tca.tb_org_id = tpg.tb_org_id))))
        LEFT JOIN tb_colaborador_org tco_gerente ON (((tpg.cod_colaborador_gerente = tco_gerente.cod_colaborador_externo)
            AND (tca.tb_org_id = tco_gerente.tb_org_id))))
        LEFT JOIN tb_colaborador tc_gerente ON ((tco_gerente.tb_colaborador_cpf = tc_gerente.cpf)))
        JOIN tb_vigencia tv ON ((tca.tb_vigencia_id = tv.id)))
        WHERE
            (tsag.cod_status_grupo <> 4)
        GROUP BY tv.mes , tv.ano , tpo.cod_projeto , tca.tb_colaborador_org_tb_colaborador_cpf , tc_gerente.cpf , tca.tb_org_id) resultado_select
        JOIN tb_status_apontamento_grupo tsa_resultado ON ((tsa_resultado.cod_status_grupo = resultado_select.cod_status_mensal)))