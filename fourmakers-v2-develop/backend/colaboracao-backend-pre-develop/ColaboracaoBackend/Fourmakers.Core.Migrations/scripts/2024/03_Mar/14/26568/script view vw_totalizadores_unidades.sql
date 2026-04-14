CREATE VIEW `vw_totalizadores_unidades` AS
    SELECT 
        (CASE
            WHEN (`tco`.`diretoria` = '') THEN 'Sem Diretoria'
            ELSE `tco`.`diretoria`
        END) AS `Diretoria`,
        `tco`.`tb_org_id` AS `tb_org_id`,
        COUNT(`tco`.`tb_colaborador_cpf`) AS `Count`
    FROM
        (`tb_colaborador_org` `tco`
        JOIN `tb_colaborador` `tc` ON ((`tco`.`tb_colaborador_cpf` = `tc`.`cpf`)))
    WHERE
        ((`tco`.`ativo` = 1)
            AND (`tc`.`ativo` = 1))
    GROUP BY `tco`.`diretoria` , `tco`.`tb_org_id`