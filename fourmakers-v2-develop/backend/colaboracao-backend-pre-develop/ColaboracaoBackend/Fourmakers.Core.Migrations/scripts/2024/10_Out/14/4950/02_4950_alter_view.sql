DROP VIEW `vw_apontamento_mensal`;
DELIMITER //
CREATE VIEW `vw_apontamento_mensal` AS
    SELECT 
        `resultado_select`.`cod_projeto` AS `cod_projeto`,
        `resultado_select`.`nome_projeto` AS `nome_projeto`,
        `resultado_select`.`data_fim_projeto` AS `data_fim_projeto`,
        `resultado_select`.`atividade` AS `atividade`,
        `resultado_select`.`gerente` AS `gerente`,
        `resultado_select`.`tipo_gerente` AS `tipo_gerente`,
        `resultado_select`.`codigo_interno_colaborador_gerente` AS `codigo_interno_colaborador_gerente`,
        `resultado_select`.`cod_gerente` AS `cod_gerente`,
        `resultado_select`.`total_horas` AS `total_horas`,
        `resultado_select`.`mes` AS `mes`,
        `resultado_select`.`ano` AS `ano`,
        `resultado_select`.`apontamento_reprovado` AS `apontamento_reprovado`,
        `resultado_select`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `resultado_select`.`tb_org_id` AS `tb_org_id`,
        `resultado_select`.`cod_status_mensal` AS `cod_status_mensal`,
        `tsa_resultado`.`descricao` AS `descricao_status_mensal`,
        `tsa_resultado`.`prioridade` AS `status_apontamento_grupo_prioridade`,
        `resultado_select`.`cod_cliente` AS `cod_cliente`,
        `resultado_select`.`nome_cliente` AS `nome_cliente`,
        `resultado_select`.`id_apontamento` AS `id_apontamento`
    FROM
        ((SELECT 
            `tpo`.`cod_projeto` AS `cod_projeto`,
                `tpo`.`projeto` AS `nome_projeto`,
                `tpo`.`data_fim` AS `data_fim_projeto`,
                `tc_gerente`.`nome_completo` AS `gerente`,
                `tpg`.`tipo_gerente` AS `tipo_gerente`,
                `tc_gerente`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gerente`,
                `tco_gerente`.`cod_colaborador_externo` AS `cod_gerente`,
                SUM(`tca`.`horas`) AS `total_horas`,
                `tv`.`mes` AS `mes`,
                `tv`.`ano` AS `ano`,
                `ta`.`descricao` AS `atividade`,
                (CASE
                    WHEN
                        EXISTS( SELECT 
                                1
                            FROM
                                `tb_status_apontamento_grupo` `tsag2`
                            WHERE
                                ((`tsag2`.`id` = `tsag`.`id`)
                                    AND (`tsag2`.`cod_status_grupo` = 3)))
                    THEN
                        1
                    ELSE 0
                END) AS `apontamento_reprovado`,
                `tca`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
                `tca`.`tb_org_id` AS `tb_org_id`,
                (CASE
                    WHEN
                        (SUM((CASE
                            WHEN (`tsag`.`cod_status_grupo` = 3) THEN 1
                            ELSE 0
                        END)) > 0)
                    THEN
                        3
                    WHEN
                        (SUM((CASE
                            WHEN (`tsag`.`cod_status_grupo` = 2) THEN 1
                            ELSE 0
                        END)) = COUNT(0))
                    THEN
                        2
                    ELSE 1
                END) AS `cod_status_mensal`,
                `tpo`.`cod_cliente` AS `cod_cliente`,
                `tpo`.`cliente` AS `nome_cliente`,
                `tca`.`id` AS `id_apontamento`
        FROM
            ((((((((`tb_colaborador_apontamento` `tca`
        JOIN `tb_status_apontamento` `tsa` ON ((`tca`.`tb_status_apontamento_id` = `tsa`.`id`)))
        JOIN `tb_status_apontamento_grupo` `tsag` ON ((`tsa`.`tb_cod_status_grupo` = `tsag`.`cod_status_grupo`)))
        JOIN `tb_projeto_org` `tpo` ON (((`tca`.`tb_projeto_org_cod_projeto` = `tpo`.`cod_projeto`)
            AND (`tca`.`tb_org_id` = `tpo`.`tb_org_id`))))
        LEFT JOIN `tb_projeto_gerente` `tpg` ON (((`tpo`.`cod_projeto` = `tpg`.`cod_projeto`)
            AND (`tca`.`tb_org_id` = `tpg`.`tb_org_id`))))
        LEFT JOIN `tb_colaborador_org` `tco_gerente` ON (((`tpg`.`cod_colaborador_gerente` = `tco_gerente`.`cod_colaborador_externo`)
            AND (`tca`.`tb_org_id` = `tco_gerente`.`tb_org_id`))))
        LEFT JOIN `tb_colaborador` `tc_gerente` ON ((`tco_gerente`.`codigo_interno_colaborador` = `tc_gerente`.`codigo_interno_colaborador`)))
        JOIN `tb_vigencia` `tv` ON ((`tca`.`tb_vigencia_id` = `tv`.`id`)))
        JOIN `tb_atividade` `ta` ON ((`ta`.`id` = `tca`.`tb_atividade_id`)))
        WHERE
            (`tsag`.`cod_status_grupo` <> 4)
        GROUP BY `tv`.`mes` , `tv`.`ano` , `tpo`.`cod_projeto` , `tca`.`codigo_interno_colaborador` , `tc_gerente`.`codigo_interno_colaborador` , `tpg`.`tipo_gerente` , `tco_gerente`.`cod_colaborador_externo` , `tsag`.`id` , `tca`.`tb_org_id` , `tca`.`tb_atividade_id`) `resultado_select`
        JOIN `tb_status_apontamento_grupo` `tsa_resultado` ON ((`tsa_resultado`.`cod_status_grupo` = `resultado_select`.`cod_status_mensal`)))
        //
        DELIMITER ;