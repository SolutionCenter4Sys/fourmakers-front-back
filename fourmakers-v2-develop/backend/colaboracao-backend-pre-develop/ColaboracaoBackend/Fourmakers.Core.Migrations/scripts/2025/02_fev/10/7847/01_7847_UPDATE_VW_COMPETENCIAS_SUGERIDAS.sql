DROP VIEW `vw_competencias_sugeridas`;
CREATE VIEW `vw_competencias_sugeridas` AS
select
    `tc`.`id` AS `CompetenciaId`,
    `tc`.`descricao` AS `Descricao`,
    `tc`.`data_criacao` AS `DataCriacao`,
    `tc`.`usuario_criacao_id` AS `UsuarioCriacaoId`,
    'HARDSKILL' AS `CompetenciaTipo`,
    (
    select
        count(0)
    from
        `tb_colaborador_competencia` `tcc`
    where
        ((`tcc`.`competencia_id` = `tc`.`id`)
            and (`tcc`.`ativo` = 1))) AS `qtdUsuariosCompetencia`
from
    `tb_competencia` `tc`
where
    ((`tc`.`ativo` = 1)
        and (`tc`.`confirmada` = 0))
union all
select
    `ts`.`id` AS `CompetenciaId`,
    `ts`.`descricao` AS `Descricao`,
    `ts`.`data_criacao` AS `DataCriacao`,
    NULL AS `UsuarioCriacaoId`,
    'SOFTSKILL' AS `CompetenciaTipo`,
    (
    select
        count(0)
    from
        `tb_colaborador_softskill` `tcs`
    where
        ((`tcs`.`softskill_id` = `ts`.`id`)
            and (`tcs`.`ativo` = 1))) AS `qtdUsuariosCompetencia`
from
    `tb_softskill` `ts`
where
    ((`ts`.`ativo` = 1)
        and (`ts`.`confirmada` = 0))
union all
select
    `tm`.`id` AS `CompetenciaId`,
    `tm`.`descricao` AS `Descricao`,
    `tm`.`data_criacao` AS `DataCriacao`,
    NULL AS `UsuarioCriacaoId`,
    'METODOLOGIA' AS `CompetenciaTipo`,
    (
    select
        count(0)
    from
        `tb_colaborador_metodologia` `tcm`
    where
        ((`tcm`.`metodologia_id` = `tm`.`id`)
            and (`tcm`.`ativo` = 1))) AS `qtdUsuariosCompetencia`
from
    `tb_metodologia` `tm`
where
    ((`tm`.`ativo` = 1)
        and (`tm`.`confirmada` = 0))
union all
select
    `td`.`id` AS `CompetenciaId`,
    `td`.`descricao` AS `Descricao`,
    `td`.`data_criacao` AS `DataCriacao`,
    NULL AS `UsuarioCriacaoId`,
    'DOMINIO' AS `CompetenciaTipo`,
    (
    select
        count(0)
    from
        `tb_colaborador_dominionegocio` `tcd`
    where
        ((`tcd`.`dominionegocio_id` = `td`.`id`)
            and (`tcd`.`ativo` = 1))) AS `qtdUsuariosCompetencia`
from
    `tb_dominionegocio` `td`
where
    ((`td`.`ativo` = 1)
        and (`td`.`confirmada` = 0))
union all
select
    `ti`.`id` AS `CompetenciaId`,
    `ti`.`descricao` AS `Descricao`,
    `ti`.`data_criacao` AS `DataCriacao`,
    NULL AS `UsuarioCriacaoId`,
    'IDIOMA' AS `CompetenciaTipo`,
    (
    select
        count(0)
    from
        `tb_colaborador_idioma` `tci`
    where
        ((`tci`.`idioma_id` = `ti`.`id`)
            and (`tci`.`ativo` = 1))) AS `qtdUsuariosCompetencia`
from
    `tb_idioma` `ti`
where
    ((`ti`.`ativo` = 1)
        and (`ti`.`confirmada` = 0))
union all
select
    `tsd`.`id` AS `CompetenciaId`,
    `tsd`.`descricao` AS `Descricao`,
    `tsd`.`data_criacao` AS `DataCriacao`,
    NULL AS `UsuarioCriacaoId`,
    'DESCONHECIDA' AS `CompetenciaTipo`,
    (
    select
        count(0)
    from
        `tb_colaborador_skill_desconhecida` `tcsd`
    where
        ((`tcsd`.`skill_desconhecida_id` = `tsd`.`id`)
            and (`tcsd`.`ativo` = 1))) AS `qtdUsuariosCompetencia`
from
    `tb_skill_desconhecida` `tsd`
where
    ((`tsd`.`ativo` = 1));