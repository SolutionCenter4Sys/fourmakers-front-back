CREATE VIEW `vw_disponibilidade_colaboradores` AS
select
    `tco`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    `tco`.`tb_org_id` AS `tb_org_id`,
    coalesce(sum((case when ((`tcpa`.`data_fim` >= curdate()) and (`tcpa`.`data_inicio` <= (curdate() + interval 15 day))) then `tcpa`.`quantidade_horas` else 0 end)), 0) AS `total_horas_15_dias`,
    coalesce(sum((case when ((`tcpa`.`data_fim` >= (curdate() + interval 15 day)) and (`tcpa`.`data_inicio` <= (curdate() + interval 30 day))) then `tcpa`.`quantidade_horas` else 0 end)), 0) AS `total_horas_entre_15_30_dias`,
    (case
        when (sum((case when ((`tcpa`.`data_fim` >= curdate()) and (`tcpa`.`data_inicio` <= (curdate() + interval 15 day))) then `tcpa`.`quantidade_horas` else 0 end)) >= 8) then 'NO'
        when (sum((case when ((`tcpa`.`data_fim` >= curdate()) and (`tcpa`.`data_inicio` <= (curdate() + interval 15 day))) then `tcpa`.`quantidade_horas` else 0 end)) > 0) then 'YES'
        else 'YES'
    end) AS `disponibilidade_15_dias`,
    (case
        when (sum((case when ((`tcpa`.`data_fim` >= (curdate() + interval 15 day)) and (`tcpa`.`data_inicio` <= (curdate() + interval 30 day))) then `tcpa`.`quantidade_horas` else 0 end)) >= 8) then 'NO'
        when (sum((case when ((`tcpa`.`data_fim` >= (curdate() + interval 15 day)) and (`tcpa`.`data_inicio` <= (curdate() + interval 30 day))) then `tcpa`.`quantidade_horas` else 0 end)) > 0) then 'YES'
        else 'YES'
    end) AS `disponibilidade_entre_15_30_dias`,
    (case
        when exists(
        select
            1
        from
            `tb_colaborador_periodo_alocacao` `tcpa`
        where
            ((`tcpa`.`codigo_interno_colaborador` = `tco`.`codigo_interno_colaborador`)
                and (`tcpa`.`tb_org_id` = `tco`.`tb_org_id`)
                    and (`tcpa`.`data_inicio` >= curdate()))) then 'YES'
        else 'NO'
    end) AS `alocado`,
    (
    select
        min(`tcpa`.`data_inicio`)
    from
        `tb_colaborador_periodo_alocacao` `tcpa`
    where
        ((`tcpa`.`codigo_interno_colaborador` = `tco`.`codigo_interno_colaborador`)
            and (`tcpa`.`tb_org_id` = `tco`.`tb_org_id`)
                and (`tcpa`.`quantidade_horas` < 8)
                    and (`tcpa`.`data_inicio` >= curdate()))) AS `primeira_data_inicio`
from
    (`tb_colaborador_org` `tco`
left join `tb_colaborador_periodo_alocacao` `tcpa` on
    (((`tco`.`codigo_interno_colaborador` = `tcpa`.`codigo_interno_colaborador`)
        and (`tco`.`tb_org_id` = `tcpa`.`tb_org_id`))))
group by
    `tco`.`codigo_interno_colaborador`;