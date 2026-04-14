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
			`tb_colaborador_org` `tco`
		 JOIN
			`tb_colaborador` `tc` ON `tco`.`codigo_interno_colaborador` = `tc`.`codigo_interno_colaborador`
		 WHERE
			`tc`.`ativo` = 1
         AND 
			`tco`.`ativo` = 1
                
         UNION
		 
         SELECT 
			`tbd`.`cod_tbd_alocado` AS `cod_profisisonal`,
			`tbd`.`descricao` AS `nome_profissional`,
			`tbd`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gestor_tbd`,
			`tbd`.`cod_diretoria` AS `codigo_diretoria`,
			NULL AS `codigo_departamento`,
			NULL AS `codigo_interno_colaborador`,
			TRUE AS `EhTbd`,
			`tbd`.`tb_org_id` AS `tb_org_id`
		 FROM
			`tb_tbd_alocado` `tbd`
		) `x`