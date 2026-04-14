CREATE VIEW `vw_colaborador_apontamento` 
AS 
select 
	`tca`.`id` AS `tb_colaborador_apontamento_id`,
    `tca`.`horas` AS `horas`,`tca`.`justificativa` AS `justificativa`,
    `tca`.`data` AS `data_registro`,`tv`.`mes` AS `mes`,`tv`.`ano` AS `ano`,
    `tca`.`numero_semana` AS `numero_semana`,
    `tca`.`numero_semana_dia` AS `numero_semana_dia`,
    `tca`.`tipo_apontamento_id` AS `tipo_apontamento`,
    `tsag`.`descricao` AS `status_apontamento_grupo`,
    `tpo`.`cod_projeto` AS `cod_projeto`,
    `tpo`.`projeto` AS `nome_projeto`,
    `tc_gerente`.`nome_completo` AS `gerente`,
    `tpg`.`tipo_gerente` AS `tipo_gerente`,
    `tc_gerente`.`cpf` AS `cpf_gerente`,
    `tco_gerente`.`cod_colaborador_externo` AS `cod_gerente`,
    (row_number() OVER (PARTITION BY `tca`.`data`,`tpg`.`cod_projeto`,`ta`.`descricao` ORDER BY `tpg`.`cod_colaborador_gerente` )  - 1) AS `gerente_prioridade`,
    `ta`.`id` AS `atividade_id`,
    `ta`.`descricao` AS `atividade_descricao`,
    `tca`.`tb_colaborador_org_tb_colaborador_cpf` AS `tb_colaborador_cpf`,
    `tca`.`tb_org_id` AS `tb_org_id` 
from 
	`tb_colaborador_apontamento` `tca`
    join `tb_status_apontamento` `tsa` on`tca`.`tb_status_apontamento_id` = `tsa`.`id`
    join `tb_status_apontamento_grupo` `tsag` on`tsa`.`tb_cod_status_grupo` = `tsag`.`cod_status_grupo`
    join `tb_projeto_org` `tpo` on`tca`.`tb_projeto_org_cod_projeto` = `tpo`.`cod_projeto` and `tca`.`tb_org_id` = `tpo`.`tb_org_id`
    join `tb_projeto_gerente` `tpg` on`tpo`.`cod_projeto` = `tpg`.`cod_projeto` and `tca`.`tb_org_id` = `tpg`.`tb_org_id`
    join `tb_colaborador_org` `tco_gerente` on `tpg`.`cod_colaborador_gerente` = `tco_gerente`.`cod_colaborador_externo` and `tca`.`tb_org_id` = `tco_gerente`.`tb_org_id`
    join `tb_colaborador` `tc_gerente` on`tco_gerente`.`tb_colaborador_cpf` = `tc_gerente`.`cpf` join `tb_atividade` `ta` on`tca`.`tb_atividade_id` = `ta`.`id`
    join `tb_vigencia` `tv` on`tca`.`tb_vigencia_id` = `tv`.`id`
where (`tsag`.`cod_status_grupo` <> 4)