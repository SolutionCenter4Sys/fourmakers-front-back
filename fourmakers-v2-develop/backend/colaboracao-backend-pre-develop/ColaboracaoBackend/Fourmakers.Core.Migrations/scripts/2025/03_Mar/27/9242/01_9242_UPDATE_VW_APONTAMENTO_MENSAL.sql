DROP VIEW IF EXISTS `vw_apontamento_mensal`;

CREATE VIEW `vw_apontamento_mensal` AS
select
    `resultado_select`.`cod_projeto` AS `cod_projeto`,
    `resultado_select`.`nome_projeto` AS `nome_projeto`,
    `resultado_select`.`data_fim_projeto` AS `data_fim_projeto`,
    `resultado_select`.`atividade` AS `atividade`,
    `resultado_select`.`gerente` AS `gerente`,
    `resultado_select`.`tipo_gerente` AS `tipo_gerente`,
    `resultado_select`.`codigo_interno_colaborador_gerente` AS `codigo_interno_colaborador_gerente`,
    `resultado_select`.`cod_gerente` AS `cod_gerente`,
    `resultado_select`.`total_horas` AS `total_horas`,
    `resultado_select`.`mes` AS `mes`,
    `resultado_select`.`ano` AS `ano`,
    `resultado_select`.`apontamento_reprovado` AS `apontamento_reprovado`,
    `resultado_select`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    `resultado_select`.`tb_org_id` AS `tb_org_id`,
    `resultado_select`.`cod_status_mensal` AS `cod_status_mensal`,
    `tsa_resultado`.`descricao` AS `descricao_status_mensal`,
    `tsa_resultado`.`prioridade` AS `status_apontamento_grupo_prioridade`,
    `resultado_select`.`cod_cliente` AS `cod_cliente`,
    `resultado_select`.`nome_cliente` AS `nome_cliente`,
    `resultado_select`.`id_apontamento` AS `id_apontamento`,
    `resultado_select`.`data_alteracao` AS `data_alteracao`
from
    ((
    select
        `tpo`.`cod_projeto` AS `cod_projeto`,
        `tpo`.`projeto` AS `nome_projeto`,
        `tpo`.`data_fim` AS `data_fim_projeto`,
        `tc_gerente`.`nome_completo` AS `gerente`,
        `tpg`.`tipo_gerente` AS `tipo_gerente`,
        `tc_gerente`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gerente`,
        `tco_gerente`.`cod_colaborador_externo` AS `cod_gerente`,
        sum(`tca`.`horas`) AS `total_horas`,
        `tv`.`mes` AS `mes`,
        `tv`.`ano` AS `ano`,
        `ta`.`descricao` AS `atividade`,
        (case
            when exists(
            select
                1
            from
                `tb_status_apontamento_grupo` `tsag2`
            where
                ((`tsag2`.`id` = `tsag`.`id`)
                    and (`tsag2`.`cod_status_grupo` = 3))) then 1
            else 0
        end) AS `apontamento_reprovado`,
        `tca`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        `tca`.`tb_org_id` AS `tb_org_id`,
        (case
            when (sum((case when (`tsag`.`cod_status_grupo` = 3) then 1 else 0 end)) > 0) then 3
            when (sum((case when (`tsag`.`cod_status_grupo` = 2) then 1 else 0 end)) = count(0)) then 2
            else 1
        end) AS `cod_status_mensal`,
        `tclo`.`codigo_cliente` AS `cod_cliente`,
        `tclo`.`nome_cliente` AS `nome_cliente`,
        `tca`.`id` AS `id_apontamento`,
        `tca`.`data_alteracao`
    from
        (((((((((`tb_colaborador_apontamento` `tca`
    join `tb_status_apontamento` `tsa` on
        ((`tca`.`tb_status_apontamento_id` = `tsa`.`id`)))
    join `tb_status_apontamento_grupo` `tsag` on
        ((`tsa`.`tb_cod_status_grupo` = `tsag`.`cod_status_grupo`)))
    join `tb_projeto_org` `tpo` on
        (((`tca`.`tb_projeto_org_cod_projeto` = `tpo`.`cod_projeto`)
            and (`tca`.`tb_org_id` = `tpo`.`tb_org_id`))))
    left join `tb_cliente_org` `tclo` on
        (((`tpo`.`cod_cliente` = `tclo`.`codigo_cliente`)
            and (`tpo`.`tb_org_id` = `tclo`.`tb_org_id`))))
    left join `tb_projeto_gerente` `tpg` on
        (((`tpo`.`cod_projeto` = `tpg`.`cod_projeto`)
            and (`tca`.`tb_org_id` = `tpg`.`tb_org_id`))))
    left join `tb_colaborador_org` `tco_gerente` on
        (((`tpg`.`cod_colaborador_gerente` = `tco_gerente`.`cod_colaborador_externo`)
            and (`tca`.`tb_org_id` = `tco_gerente`.`tb_org_id`))))
    left join `tb_colaborador` `tc_gerente` on
        ((`tco_gerente`.`codigo_interno_colaborador` = `tc_gerente`.`codigo_interno_colaborador`)))
    join `tb_vigencia` `tv` on
        ((`tca`.`tb_vigencia_id` = `tv`.`id`)))
    join `tb_atividade` `ta` on
        ((`ta`.`id` = `tca`.`tb_atividade_id`)))
    where
        (`tsag`.`cod_status_grupo` <> 4)
    group by
        `tv`.`mes`,
        `tv`.`ano`,
        `tpo`.`cod_projeto`,
        `tca`.`codigo_interno_colaborador`,
        `tc_gerente`.`codigo_interno_colaborador`,
        `tpg`.`tipo_gerente`,
        `tco_gerente`.`cod_colaborador_externo`,
        `tsag`.`id`,
        `tca`.`tb_org_id`,
        `tca`.`tb_atividade_id`) `resultado_select`
join `tb_status_apontamento_grupo` `tsa_resultado` on
((`tsa_resultado`.`cod_status_grupo` = `resultado_select`.`cod_status_mensal`)));