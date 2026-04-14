CREATE OR REPLACE VIEW `vw_skills` AS
SELECT
    `tc`.`descricao` AS `descricao`,
    `tc`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
FROM
    `tb_competencia` `tc`
JOIN
    `tb_item_perfil` `tip` ON `tip`.`descricao` = 'COMPETENCIA'
WHERE
    `tc`.`ativo` = true
UNION ALL
SELECT
    `ts`.`descricao` AS `descricao`,
    `ts`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
FROM
    `tb_softskill` `ts`
JOIN
    `tb_item_perfil` `tip` ON `tip`.`descricao` = 'SOFTSKILL'
WHERE
    `ts`.`ativo` = true
UNION ALL
SELECT
    `tm`.`descricao` AS `descricao`,
    `tm`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
FROM
    `tb_metodologia` `tm`
JOIN
    `tb_item_perfil` `tip` ON `tip`.`descricao` = 'METODOLOGIA'
WHERE
    `tm`.`ativo` = true
UNION ALL
SELECT
    `td`.`descricao` AS `descricao`,
    `td`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
FROM
    `tb_dominionegocio` `td`
JOIN
    `tb_item_perfil` `tip` ON `tip`.`descricao` = 'DOMINIONEGOCIO'
WHERE
    `td`.`ativo` = true
UNION ALL
SELECT
    `ti`.`descricao` AS `descricao`,
    `ti`.`id` AS `id`,
    `tip`.`id` AS `tipo_id`
FROM
    `tb_idioma` `ti`
JOIN
    `tb_item_perfil` `tip` ON `tip`.`descricao` = 'IDIOMA'
WHERE
    `ti`.`ativo` = true;