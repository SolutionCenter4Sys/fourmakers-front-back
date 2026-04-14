DROP VIEW `vw_skills`;
CREATE VIEW `vw_skills` AS
select
    `tc`.`descricao` AS `descricao`,
    `tc`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
from
    (`tb_competencia` `tc`
join `tb_item_perfil` `tip` on
    ((`tip`.`descricao` = 'COMPETENCIA')))
where
    (`tc`.`ativo` = true)
union all
select
    `ts`.`descricao` AS `descricao`,
    `ts`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
from
    (`tb_softskill` `ts`
join `tb_item_perfil` `tip` on
    ((`tip`.`descricao` = 'SOFTSKILL')))
where
    (`ts`.`ativo` = true)
union all
select
    `tm`.`descricao` AS `descricao`,
    `tm`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
from
    (`tb_metodologia` `tm`
join `tb_item_perfil` `tip` on
    ((`tip`.`descricao` = 'METODOLOGIA')))
where
    (`tm`.`ativo` = true)
union all
select
    `td`.`descricao` AS `descricao`,
    `td`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
from
    (`tb_dominionegocio` `td`
join `tb_item_perfil` `tip` on
    ((`tip`.`descricao` = 'DOMINIONEGOCIO')))
where
    (`td`.`ativo` = true)
union all
select
    `ti`.`descricao` AS `descricao`,
    `ti`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
from
    (`tb_idioma` `ti`
join `tb_item_perfil` `tip` on
    ((`tip`.`descricao` = 'IDIOMA')))
where
    (`ti`.`ativo` = true)
union all
select
    `tsd`.`descricao` AS `descricao`,
    `tsd`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
from
    (`tb_skill_desconhecida` `tsd`
join `tb_item_perfil` `tip` on
    ((`tip`.`descricao` = 'DESCONHECIDO')))
where
    (`tsd`.`ativo` = true);