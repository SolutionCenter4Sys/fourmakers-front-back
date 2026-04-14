CREATE VIEW `vw_gestores_colaboradores_org` AS
    SELECT 
        `b`.`cod_colaborador_externo` AS `cod_colaborador_externo_subordinado`,
        `a`.`cpf` AS `cpf_subordinado`,
        `a`.`nome_completo` AS `nome_completo_subordinado`,
        `c`.`cod_colaborador_superior` AS `cod_colaborador_externo_gestor`,
        `gerente`.`cpf` AS `cpf_gestor`,
        `gerente`.`nome_completo` AS `nome_completo_gestor`,
        `b`.`tb_org_id` AS `tb_org_id`
    FROM
        ((((`tb_colaborador` `a`
        JOIN `tb_colaborador_org` `b` ON ((`a`.`cpf` = `b`.`tb_colaborador_cpf`)))
        JOIN `tb_colaborador_hierarquia` `c` ON ((`c`.`cod_colaborador_externo` = `b`.`cod_colaborador_externo`)))
        LEFT JOIN `tb_colaborador_org` `gerente_org` ON ((`gerente_org`.`cod_colaborador_externo` = `c`.`cod_colaborador_superior`)))
        LEFT JOIN `tb_colaborador` `gerente` ON ((`gerente`.`cpf` = `gerente_org`.`tb_colaborador_cpf`)))
    GROUP BY `a`.`cpf` , `a`.`nome_completo` , `b`.`tb_org_id` , `c`.`cod_colaborador_superior` , `b`.`cod_colaborador_externo`