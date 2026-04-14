DROP VIEW `vw_alocacacao_recurso_calculo_mensal`;
CREATE VIEW `vw_alocacacao_recurso_calculo_mensal` AS
    SELECT 
        `tco`.`cod_colaborador_externo` AS `codigo_colaborador`,
        `tco`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `tc`.`nome_completo` AS `nome`,
        `tco`.`ativo` AS `ativo`,
        `tco`.`tb_org_id` AS `tb_org_id`,
        `tco`.`cod_diretoria` AS `cod_diretoria`,
        `tco`.`diretoria` AS `diretoria`,
        0 AS `eh_tbd`,
        CAST(NULL AS CHAR (11) CHARSET UTF8MB4) AS `codigo_interno_colaborador_gestor`
    FROM
        (`tb_colaborador` `tc`
        JOIN `tb_colaborador_org` `tco` ON ((`tco`.`codigo_interno_colaborador` = `tc`.`codigo_interno_colaborador`))) 
    UNION ALL SELECT 
        `tta`.`cod_tbd_alocado` AS `codigo_colaborador`,
        NULL AS `codigo_interno_colaborador`,
        `tta`.`descricao` AS `nome_completo`,
        1 AS `ativo`,
        `tta`.`tb_org_id` AS `tb_org_id`,
        `tta`.`cod_diretoria` AS `cod_diretoria`,
        `tta`.`diretoria` AS `diretoria`,
        1 AS `eh_tbd`,
        `tta`.`codigo_interno_colaborador_gestor` AS `codigo_interno_colaborador_gestor`
    FROM
        `tb_tbd_alocado` `tta`;

DROP VIEW `vw_gestores_colaborador_tbd`;
CREATE VIEW `vw_gestores_colaborador_tbd` AS
    SELECT 
        `tta`.`cod_tbd_alocado` AS `cod_colaborador_externo`,
        `tco_gestor`.`cod_colaborador_externo` AS `cod_colaborador_superior`,
        1 AS `eh_tbd`,
        `tta`.`tb_org_id` AS `tb_org_id`,
        `tc_gestor_tbd`.`nome_completo` AS `nome_gestor_adm`
    FROM
        ((`tb_tbd_alocado` `tta`
        LEFT JOIN `tb_colaborador_org` `tco_gestor` ON (((`tta`.`codigo_interno_colaborador_gestor` = `tco_gestor`.`codigo_interno_colaborador`)
            AND (`tta`.`tb_org_id` = `tco_gestor`.`tb_org_id`))))
        LEFT JOIN `tb_colaborador` `tc_gestor_tbd` ON ((`tco_gestor`.`codigo_interno_colaborador` = `tc_gestor_tbd`.`codigo_interno_colaborador`)))
    WHERE
        (`tta`.`codigo_interno_colaborador_gestor` IS NOT NULL) 
    UNION
    SELECT 
        `tch`.`cod_colaborador_externo` AS `cod_colaborador_externo`,
        `tch`.`cod_colaborador_superior` AS `cod_colaborador_superior`,
        0 AS `eh_tbd`,
        `tch`.`tb_org_id` AS `tb_org_id`,
        `tc_gestor_colab`.`nome_completo` AS `nome_gestor_adm`
    FROM
        ((`tb_colaborador_hierarquia` `tch`
        LEFT JOIN `tb_colaborador_org` `tco_gestor_colab` ON (((`tco_gestor_colab`.`cod_colaborador_externo` = `tch`.`cod_colaborador_superior`)
            AND (`tch`.`tb_org_id` = `tco_gestor_colab`.`tb_org_id`))))
        LEFT JOIN `tb_colaborador` `tc_gestor_colab` ON ((`tco_gestor_colab`.`codigo_interno_colaborador` = `tc_gestor_colab`.`codigo_interno_colaborador`)));


DROP VIEW `vw_mapa_alocacao_colaborador_tbd`;
CREATE VIEW `vw_mapa_alocacao_colaborador_tbd` AS
    SELECT 
        `x`.`cod_profisisonal` AS `cod_profisisonal`,
        `x`.`nome_profissional` AS `nome_profissional`,
        `x`.`codigo_interno_colaborador_gestor_tbd` AS `codigo_interno_colaborador_gestor_tbd`,
        `x`.`codigo_diretoria` AS `codigo_diretoria`,
        `x`.`codigo_departamento` AS `codigo_departamento`,
        `x`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `x`.`eh_tbd` AS `eh_tbd`,
        `x`.`tb_org_id` AS `tb_org_id`
    FROM
        (SELECT 
            `tco`.`cod_colaborador_externo` AS `cod_profisisonal`,
                `tc`.`nome_completo` AS `nome_profissional`,
                NULL AS `codigo_interno_colaborador_gestor_tbd`,
                `tco`.`cod_diretoria` AS `codigo_diretoria`,
                `tco`.`cod_departamento` AS `codigo_departamento`,
                `tco`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
                FALSE AS `eh_tbd`,
                `tco`.`tb_org_id` AS `tb_org_id`
        FROM
            (`tb_colaborador_org` `tco`
        JOIN `tb_colaborador` `tc` ON ((`tco`.`codigo_interno_colaborador` = `tc`.`codigo_interno_colaborador`)))
        WHERE
            ((`tc`.`ativo` = 1)
                AND (`tco`.`ativo` = 1)) UNION SELECT 
            `tbd`.`cod_tbd_alocado` AS `cod_profisisonal`,
                `tbd`.`descricao` AS `nome_profissional`,
                `tbd`.`codigo_interno_colaborador_gestor` AS `codigo_interno_colaborador_gestor_tbd`,
                `tbd`.`cod_diretoria` AS `codigo_diretoria`,
                NULL AS `codigo_departamento`,
                NULL AS `codigo_interno_colaborador`,
                TRUE AS `EhTbd`,
                `tbd`.`tb_org_id` AS `tb_org_id`
        FROM
            `tb_tbd_alocado` `tbd`) `x`;
