DROP VIEW `vw_colaborador_apontamento`;
DELIMITER //
CREATE VIEW `vw_colaborador_apontamento` AS
    SELECT 
        `tca`.`id` AS `tb_colaborador_apontamento_id`,
        `tca`.`horas` AS `horas`,
        `tca`.`justificativa` AS `justificativa`,
        `tca`.`data_justificativa` AS `data_justificativa`,
        `colab_justificativa`.`nome_completo` AS `nome_usuario_justificativa`,
        `tca`.`data` AS `data_registro`,
        `tv`.`mes` AS `mes`,
        `tv`.`ano` AS `ano`,
        `tca`.`numero_semana` AS `numero_semana`,
        `tca`.`numero_semana_dia` AS `numero_semana_dia`,
        `tca`.`tipo_apontamento_id` AS `tipo_apontamento`,
        `tsag`.`cod_status_grupo` AS `cod_status_apontamento_grupo`,
        `tsag`.`descricao` AS `status_apontamento_grupo`,
        `tsa`.`cod_status_apontamento` AS `cod_status_apontamento`,
        `tsa`.`descricao` AS `status_apontamento`,
        `tpo`.`cod_projeto` AS `cod_projeto`,
        `tpo`.`projeto` AS `nome_projeto`,
        `tclo`.`codigo_cliente` AS `cod_cliente`,
        `tclo`.`nome_cliente` AS `nome_cliente`,
        `tpo`.`permite_apont_sem_alocacao` AS `permite_apont_sem_alocacao`,
        `tpo`.`permite_apont_sem_alocacao_outro_colab` AS `permite_apont_sem_alocacao_outro_colab`,
        `tc_gerente`.`nome_completo` AS `gerente`,
        `tpg`.`tipo_gerente` AS `tipo_gerente`,
        `tc_gerente`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gerente`,
        `tco_gerente`.`cod_colaborador_externo` AS `cod_gerente`,
        `ta`.`id` AS `atividade_id`,
        `ta`.`descricao` AS `atividade_descricao`,
        `tca`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `tca`.`tb_org_id` AS `tb_org_id`,
        `tca`.`observacao` AS `observacao`
    FROM
        ((((((((((`tb_colaborador_apontamento` `tca`
        JOIN `tb_status_apontamento` `tsa` ON ((`tca`.`tb_status_apontamento_id` = `tsa`.`id`)))
        JOIN `tb_status_apontamento_grupo` `tsag` ON ((`tsa`.`tb_cod_status_grupo` = `tsag`.`cod_status_grupo`)))
        JOIN `tb_projeto_org` `tpo` ON (((`tca`.`tb_projeto_org_cod_projeto` = `tpo`.`cod_projeto`)
            AND (`tca`.`tb_org_id` = `tpo`.`tb_org_id`))))
        LEFT JOIN `tb_cliente_org` `tclo` ON (((`tpo`.`cod_cliente` = `tclo`.`codigo_cliente`)
            AND (`tpo`.`tb_org_id` = `tclo`.`tb_org_id`))))
        LEFT JOIN `tb_projeto_gerente` `tpg` ON (((`tpo`.`cod_projeto` = `tpg`.`cod_projeto`)
            AND (`tca`.`tb_org_id` = `tpg`.`tb_org_id`))))
        LEFT JOIN `tb_colaborador_org` `tco_gerente` ON (((`tpg`.`cod_colaborador_gerente` = `tco_gerente`.`cod_colaborador_externo`)
            AND (`tca`.`tb_org_id` = `tco_gerente`.`tb_org_id`))))
        LEFT JOIN `tb_colaborador` `tc_gerente` ON ((`tco_gerente`.`codigo_interno_colaborador` = `tc_gerente`.`codigo_interno_colaborador`)))
        JOIN `tb_atividade` `ta` ON ((`tca`.`tb_atividade_id` = `ta`.`id`)))
        JOIN `tb_vigencia` `tv` ON ((`tca`.`tb_vigencia_id` = `tv`.`id`)))
        LEFT JOIN `tb_colaborador` `colab_justificativa` ON ((`colab_justificativa`.`codigo_interno_colaborador` = `tca`.`codigo_interno_colaborador_justificativa`)))
    WHERE
        (`tsag`.`cod_status_grupo` <> 4)
;
DELIMITER //