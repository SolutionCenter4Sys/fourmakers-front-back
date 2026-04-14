CREATE OR REPLACE
VIEW `vw_gestores_org` AS
    SELECT 
        `a`.`cod_colaborador_superior` AS `cod_colaborador_superior`,
        `c`.`nome_completo` AS `nome_completo`,
        `b`.`cod_diretoria` AS `cod_diretoria`,
        `b`.`diretoria` AS `diretoria`,
        `a`.`tb_org_id` AS `tb_org_id`
    FROM
        ((`tb_colaborador_hierarquia` `a`
        JOIN `tb_colaborador_org` `b` ON (((`a`.`cod_colaborador_superior` = `b`.`cod_colaborador_externo`)
            AND (`a`.`tb_org_id` = `b`.`tb_org_id`))))
        JOIN `tb_colaborador` `c` ON ((`c`.`cpf` = `b`.`tb_colaborador_cpf`)))
    GROUP BY `a`.`cod_colaborador_superior` , `c`.`nome_completo` , `a`.`tb_org_id`;
