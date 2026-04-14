CREATE VIEW `vw_gestores_estatisticas_org` AS
    SELECT 
        `tc`.`cpf` AS `cpf`, `tch`.`tb_org_id` AS `org_id`
    FROM
        ((`tb_colaborador_hierarquia` `tch`
        JOIN `tb_colaborador_org` `tco` ON (((`tch`.`cod_colaborador_superior` = `tco`.`cod_colaborador_externo`)
            AND (`tch`.`tb_org_id` = `tco`.`tb_org_id`))))
        JOIN `tb_colaborador` `tc` ON ((`tc`.`cpf` = `tco`.`tb_colaborador_cpf`)))
    WHERE
        ((`tc`.`ativo` = 1)
            AND (`tco`.`ativo` = 1))
    GROUP BY `tch`.`cod_colaborador_superior` , `tc`.`nome_completo` , `tch`.`tb_org_id`