CREATE VIEW `vw_colaborador_skills` AS
select
    `hab`.`id` AS `id`,
    `hab`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    `hab`.`nivel_id` AS `nivel_id`,
    `hab`.`nivel` AS `nivel`,
    `hab`.`tipo` AS `tipo`,
    `hab`.`descricao` AS `descricao`
from
    (
    select
        `tc`.`id` AS `id`,
        `c`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `n`.`id` AS `nivel_id`,
        `n`.`descricao` AS `nivel`,
        'Competência' AS `tipo`,
        `tc`.`descricao` AS `descricao`
    from
        ((`tb_colaborador_competencia` `c`
    join `tb_nivel` `n` on
        ((`c`.`tb_nivel_id` = `n`.`id`)))
    join `tb_competencia` `tc` on
        ((`c`.`competencia_id` = `tc`.`id`)))
    where
        (`c`.`ativo` = 1)
union all
    select
        `tm`.`id` AS `id`,
        `m`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `n`.`id` AS `nivel_id`,
        `n`.`descricao` AS `nivel`,
        'Metodologia' AS `tipo`,
        `tm`.`descricao` AS `descricao`
    from
        ((`tb_colaborador_metodologia` `m`
    join `tb_nivel` `n` on
        ((`m`.`tb_nivel_id` = `n`.`id`)))
    join `tb_metodologia` `tm` on
        ((`m`.`metodologia_id` = `tm`.`id`)))
    where
        (`m`.`ativo` = 1)
union all
    select
        `ti`.`id` AS `id`,
        `tci`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `n`.`id` AS `nivel_id`,
        `n`.`descricao` AS `nivel`,
        'Idioma' AS `tipo`,
        `ti`.`descricao` AS `descricao`
    from
        ((`tb_colaborador_idioma` `tci`
    join `tb_nivel` `n` on
        ((`tci`.`tb_nivel_id` = `n`.`id`)))
    join `tb_idioma` `ti` on
        ((`tci`.`idioma_id` = `ti`.`id`)))
    where
        (`tci`.`ativo` = 1)
union all
    select
        `ts`.`id` AS `id`,
        `tcs`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `n`.`id` AS `nivel_id`,
        `n`.`descricao` AS `nivel`,
        'Softskill' AS `tipo`,
        `ts`.`descricao` AS `descricao`
    from
        ((`tb_colaborador_softskill` `tcs`
    join `tb_nivel` `n` on
        ((`tcs`.`tb_nivel_id` = `n`.`id`)))
    join `tb_softskill` `ts` on
        ((`tcs`.`softskill_id` = `ts`.`id`)))
    where
        (`tcs`.`ativo` = 1)
union all
    select
        `td`.`id` AS `id`,
        `tcd`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `n`.`id` AS `nivel_id`,
        `n`.`descricao` AS `nivel`,
        'Dominio' AS `tipo`,
        `td`.`descricao` AS `descricao`
    from
        ((`tb_colaborador_dominionegocio` `tcd`
    join `tb_nivel` `n` on
        ((`tcd`.`tb_nivel_id` = `n`.`id`)))
    join `tb_dominionegocio` `td` on
        ((`tcd`.`dominionegocio_id` = `td`.`id`)))
    where
        (`tcd`.`ativo` = 1)) `hab`;