CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `buscacolaborador` AS
select
    `ret`.`documento_colaborador` AS `documento_colaborador`,
    `ret`.`ativo` AS `ativo`,
    `ret`.`candidato` AS `candidato`,
	`ret`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    coalesce(`ret`.`Bloco01`, '') AS `BuscaGR1`,
    coalesce(`ret`.`Bloco03`, '') AS `BuscaGR2`,
    coalesce(`ret`.`Bloco04`, '') AS `BuscaGR3`,
    concat(coalesce(`ret`.`Bloco02`, ''), '|', coalesce(`ret`.`Bloco05`, ''), '|', coalesce(`ret`.`Bloco06`, ''), '|', coalesce(`ret`.`Bloco07`, ''), '|', coalesce(`ret`.`Bloco08`, ''), '|', coalesce(`ret`.`Bloco09`, ''), '|', coalesce(`ret`.`Bloco10`, '')) AS `BuscaGR4`
from
    (
    select
        `c`.`documento_colaborador` AS `documento_colaborador`,
        `c`.`ativo` AS `ativo`,
        `c`.`candidato` AS `candidato`,
        `c`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        concat(`c`.`nome_completo`, '|', `c`.`documento_colaborador`, '|', coalesce(`c`.`matricula`, ''), '|', coalesce(`c`.`rg`, ''), '|') AS `Bloco01`,
        (
        select
            concat(`e`.`cidade`, '|', `e`.`bairro`, '|', `e`.`estado`, '|')
        from
            `tb_endereco` `e`
        where
            ((`e`.`id` = `c`.`endereco_id`)
                and (`e`.`ativo` = 1))
        order by
            `e`.`data_alteracao` desc
        limit 1) AS `Bloco02`,
        (
        select
            group_concat(`tc`.`descricao` separator '|')
        from
            (`tb_colaborador_cargo` `cca`
        join `tb_cargo` `tc` on
            ((`tc`.`id` = `cca`.`cargo_id`)))
        where
            ((`cca`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
                and (`cca`.`ativo` = 1))
        group by
            `cca`.`codigo_interno_colaborador`) AS `Bloco03`,
        (
        select
            group_concat(`tco`.`descricao` separator '|')
        from
            (`tb_colaborador_competencia` `cco`
        left join `tb_competencia` `tco` on
            ((`tco`.`id` = `cco`.`competencia_id`)))
        where
            ((`cco`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
                and (`cco`.`ativo` = 1))
        group by
            `cco`.`codigo_interno_colaborador`) AS `Bloco04`,
        (
        select
            group_concat(`tf`.`descricao` separator '|')
        from
            (`tb_colaborador_formacao` `cf`
        left join `tb_formacao` `tf` on
            ((`tf`.`id` = `cf`.`formacao_id`)))
        where
            ((`cf`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
                and (`cf`.`ativo` = 1))
        group by
            `cf`.`codigo_interno_colaborador`) AS `Bloco05`,
        (
        select
            group_concat(`td`.`descricao` separator '|')
        from
            (`tb_colaborador_dominionegocio` `cdn`
        left join `tb_dominionegocio` `td` on
            ((`td`.`id` = `cdn`.`dominionegocio_id`)))
        where
            ((`cdn`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
                and (`cdn`.`ativo` = 1))
        group by
            `cdn`.`codigo_interno_colaborador`) AS `Bloco06`,
        (
        select
            group_concat(`tm`.`descricao` separator '|')
        from
            (`tb_colaborador_metodologia` `cm`
        left join `tb_metodologia` `tm` on
            ((`tm`.`id` = `cm`.`metodologia_id`)))
        where
            ((`cm`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
                and (`cm`.`ativo` = 1))
        group by
            `cm`.`codigo_interno_colaborador`) AS `Bloco07`,
        (
        select
            group_concat(`tmr`.`descricao` separator '|')
        from
            (`tb_colaborador_modeloreferencia` `cmr`
        left join `tb_modeloreferencia` `tmr` on
            ((`tmr`.`id` = `cmr`.`modeloreferencia_id`)))
        where
            ((`cmr`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
                and (`cmr`.`ativo` = 1))
        group by
            `cmr`.`codigo_interno_colaborador`) AS `Bloco08`,
        (
        select
            group_concat(`ti`.`descricao` separator '|')
        from
            (`tb_colaborador_interesse` `ci`
        left join `tb_interesse` `ti` on
            ((`ti`.`id` = `ci`.`interesse_id`)))
        where
            ((`ci`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
                and (`ci`.`ativo` = 1))
        group by
            `ci`.`codigo_interno_colaborador`) AS `Bloco09`,
        (
        select
            group_concat(`th`.`descricao` separator '|')
        from
            (`tb_colaborador_hobbies` `ch`
        left join `tb_hobbies` `th` on
            ((`th`.`id` = `ch`.`hobbies_id`)))
        where
            ((`ch`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
                and (`ch`.`ativo` = 1))
        group by
            `ch`.`codigo_interno_colaborador`) AS `Bloco10`
    from
        `tb_colaborador` `c`) `ret`;


CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `estatisticas_tempo_servico` AS
select
    count((case when ((to_days(curdate()) - to_days(`tb_colaborador_org`.`data_admissao`)) <= 365) then 1 end)) AS `ate_1_ano`,
    count((case when ((to_days(curdate()) - to_days(`tb_colaborador_org`.`data_admissao`)) between 366 and 730) then 1 end)) AS `entre_1_e_2_anos`,
    count((case when ((to_days(curdate()) - to_days(`tb_colaborador_org`.`data_admissao`)) between 731 and 1825) then 1 end)) AS `entre_3_e_5_anos`,
    count((case when ((to_days(curdate()) - to_days(`tb_colaborador_org`.`data_admissao`)) between 1826 and 3650) then 1 end)) AS `entre_6_e_10_anos`,
    count((case when ((to_days(curdate()) - to_days(`tb_colaborador_org`.`data_admissao`)) between 3651 and 5475) then 1 end)) AS `entre_11_e_15_anos`,
    count((case when ((to_days(curdate()) - to_days(`tb_colaborador_org`.`data_admissao`)) between 5476 and 7300) then 1 end)) AS `entre_16_e_20_anos`,
    count((case when ((to_days(curdate()) - to_days(`tb_colaborador_org`.`data_admissao`)) > 7300) then 1 end)) AS `mais_de_20_anos`
from
    `tb_colaborador_org`
group by
    `tb_colaborador_org`.`codigo_interno_colaborador`,
    `tb_colaborador_org`.`tb_org_id`;
       
       
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `colaboradorpordiretoria` AS
	select
	    `cs`.`diretoria` AS `descricao`,
	    `cs`.`cod_diretoria` AS `diretoria_id`,
	    count(0) AS `quantidade`
	from
	    (`tb_colaborador` `c`
	join `tb_colaborador_org` `cs` on
	    ((`c`.`codigo_interno_colaborador` = `cs`.`codigo_interno_colaborador`)))
	where
	    ((`c`.`ativo` = 1)
	        and (`cs`.`ativo` = 1))
	group by
	    `cs`.`cod_diretoria`,
	    `cs`.`diretoria`
	order by
	    `cs`.`cod_diretoria`;
       
DROP VIEW forcaperfilcolaborador;


CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `subgraficocolaboradorcompetencia` AS
select
    `c`.`id` AS `id`,
    `c`.`descricao` AS `descricao`,
    count(0) AS `quantidade`
from
    ((`tb_colaborador_competencia` `cb`
join `tb_competencia` `c` on
    ((`cb`.`competencia_id` = `c`.`id`)))
join `tb_colaborador` `colab` on
    ((`colab`.`codigo_interno_colaborador` = `cb`.`codigo_interno_colaborador`)))
where
    ((`cb`.`ativo` = 1)
        and (`c`.`ativo` = 1)
            and (`colab`.`candidato` = 0))
group by
    `c`.`descricao`
order by
    `quantidade` desc
limit 10;

CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `subgraficocompetenciacandidato` AS
select
    `c`.`id` AS `id`,
    `c`.`descricao` AS `descricao`,
    count(0) AS `quantidade`
from
    ((`tb_colaborador_competencia` `cb`
join `tb_competencia` `c` on
    ((`cb`.`competencia_id` = `c`.`id`)))
join `tb_colaborador` `colab` on
    ((`colab`.`codigo_interno_colaborador` = `cb`.`codigo_interno_colaborador`)))
where
    ((`cb`.`ativo` = 1)
        and (`c`.`ativo` = 1)
            and (`colab`.`candidato` = 1))
group by
    `c`.`descricao`
order by
    `quantidade` desc
limit 10;

CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_alocacacao_recurso_calculo_mensal` AS
select
    `tco`.`cod_colaborador_externo` AS `codigo_colaborador`,
    `tco`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    `tc`.`nome_completo` AS `nome`,
    `tco`.`ativo` AS `ativo`,
    `tco`.`tb_org_id` AS `tb_org_id`,
    `tco`.`cod_diretoria` AS `cod_diretoria`,
    `tco`.`diretoria` AS `diretoria`,
    0 AS `eh_tbd`,
    cast(NULL as char(11) charset utf8mb4) AS `codigo_interno_colaborador_gestor`
from
    (`tb_colaborador` `tc`
join `tb_colaborador_org` `tco` on
    ((`tco`.`codigo_interno_colaborador` = `tc`.`codigo_interno_colaborador`)))
union all
select
    `tta`.`cod_tbd_alocado` AS `codigo_colaborador`,
    NULL AS `codigo_interno_colaborador`,
    `tta`.`descricao` AS `nome_completo`,
    1 AS `ativo`,
    `tta`.`tb_org_id` AS `tb_org_id`,
    `tta`.`cod_diretoria` AS `cod_diretoria`,
    `tta`.`diretoria` AS `diretoria`,
    1 AS `eh_tbd`,
    `tta`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gestor`
from
    `tb_tbd_alocado` `tta`;
   
   
DROP VIEW vw_alocacao_hierarquia_calculo_mensal;

CREATE VIEW `vw_gestores_colaborador_tbd` AS
SELECT 
        tta.cod_tbd_alocado AS cod_colaborador_externo,
        tco_gestor.cod_colaborador_externo AS cod_colaborador_superior,
        1 AS eh_tbd,
        tta.tb_org_id AS tb_org_id,
        tc_gestor_tbd.nome_completo as nome_gestor_adm
    FROM
        tb_tbd_alocado tta
	LEFT JOIN
		tb_colaborador_org tco_gestor ON tta.codigo_interno_colaborador = tco_gestor.codigo_interno_colaborador AND tta.tb_org_id = tco_gestor.tb_org_id
    LEFT JOIN
        tb_colaborador tc_gestor_tbd ON tco_gestor.codigo_interno_colaborador = tc_gestor_tbd.codigo_interno_colaborador
    WHERE
        tta.codigo_interno_colaborador IS NOT NULL 
    UNION SELECT 
        tch.cod_colaborador_externo AS cod_colaborador_externo,
        tch.cod_colaborador_superior AS cod_colaborador_superior,
        0 AS eh_tbd,
        tch.tb_org_id AS tb_org_id,
        tc_gestor_colab.nome_completo as nome_gestor_adm
    FROM
        tb_colaborador_hierarquia tch
	LEFT JOIN
        tb_colaborador_org tco_gestor_colab ON tco_gestor_colab.cod_colaborador_externo = tch.cod_colaborador_superior AND tch.tb_org_id = tco_gestor_colab.tb_org_id
	LEFT JOIN
        tb_colaborador tc_gestor_colab ON tco_gestor_colab.codigo_interno_colaborador = tc_gestor_colab.codigo_interno_colaborador;

CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_apontamento_mensal` AS
select
    `resultado_select`.`cod_projeto` AS `cod_projeto`,
    `resultado_select`.`nome_projeto` AS `nome_projeto`,
    `resultado_select`.`data_fim_projeto` AS `data_fim_projeto`,
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
    `resultado_select`.`observacao` AS `observacao`
from
    ((
    select
        `tpo`.`cod_projeto` AS `cod_projeto`,
        `tpo`.`projeto` AS `nome_projeto`,
        `tpo`.`data_fim` AS `data_fim_projeto`,
        coalesce(`tc_gerente`.`nome_completo`, 'GERENTE NÃO INFORMADO') AS `gerente`,
        `tpg`.`tipo_gerente` AS `tipo_gerente`,
        `tc_gerente`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gerente`,
        `tco_gerente`.`cod_colaborador_externo` AS `cod_gerente`,
        sum(`tca`.`horas`) AS `total_horas`,
        `tv`.`mes` AS `mes`,
        `tv`.`ano` AS `ano`,
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
        `tpo`.`cod_cliente` AS `cod_cliente`,
        `tpo`.`cliente` AS `nome_cliente`,
        `tca`.`observacao` AS `observacao`
    from
        (((((((`tb_colaborador_apontamento` `tca`
    join `tb_status_apontamento` `tsa` on
        ((`tca`.`tb_status_apontamento_id` = `tsa`.`id`)))
    join `tb_status_apontamento_grupo` `tsag` on
        ((`tsa`.`tb_cod_status_grupo` = `tsag`.`cod_status_grupo`)))
    join `tb_projeto_org` `tpo` on
        (((`tca`.`tb_projeto_org_cod_projeto` = `tpo`.`cod_projeto`)
            and (`tca`.`tb_org_id` = `tpo`.`tb_org_id`))))
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
        `tca`.`observacao`,
        `tsag`.`id`,
        `tca`.`tb_org_id`) `resultado_select`
join `tb_status_apontamento_grupo` `tsa_resultado` on
    ((`tsa_resultado`.`cod_status_grupo` = `resultado_select`.`cod_status_mensal`)));
   
   
   
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_apontamento_mensal_visao_gerente` AS
select
    `resultado_select`.`cod_projeto` AS `cod_projeto`,
    `resultado_select`.`nome_projeto` AS `nome_projeto`,
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
    `tsa_resultado`.`prioridade` AS `status_apontamento_grupo_prioridade`
from
    ((
    select
        `tpo`.`cod_projeto` AS `cod_projeto`,
        `tpo`.`projeto` AS `nome_projeto`,
        `tc_gerente`.`nome_completo` AS `gerente`,
        `tpg`.`tipo_gerente` AS `tipo_gerente`,
        `tc_gerente`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gerente`,
        `tco_gerente`.`cod_colaborador_externo` AS `cod_gerente`,
        sum(`tca`.`horas`) AS `total_horas`,
        `tv`.`mes` AS `mes`,
        `tv`.`ano` AS `ano`,
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
        end) AS `cod_status_mensal`
    from
        (((((((`tb_colaborador_apontamento` `tca`
    join `tb_status_apontamento` `tsa` on
        ((`tca`.`tb_status_apontamento_id` = `tsa`.`id`)))
    join `tb_status_apontamento_grupo` `tsag` on
        ((`tsa`.`tb_cod_status_grupo` = `tsag`.`cod_status_grupo`)))
    join `tb_projeto_org` `tpo` on
        (((`tca`.`tb_projeto_org_cod_projeto` = `tpo`.`cod_projeto`)
            and (`tca`.`tb_org_id` = `tpo`.`tb_org_id`))))
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
        `tca`.`observacao`,
        `tsag`.`id`,
        `tca`.`tb_org_id`) `resultado_select`
join `tb_status_apontamento_grupo` `tsa_resultado` on
    ((`tsa_resultado`.`cod_status_grupo` = `resultado_select`.`cod_status_mensal`)));
   
   
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_colaborador_apontamento` AS
select
    `tca`.`id` AS `tb_colaborador_apontamento_id`,
    `tca`.`horas` AS `horas`,
    `tca`.`justificativa` AS `justificativa`,
    `tca`.`data_justificativa` AS `data_justificativa`,
    `colab_justificativa`.`nome_completo` AS `nome_usuario_justificativa`,
    `tca`.`data` AS `data_registro`,
    `tv`.`mes` AS `mes`,
    `tv`.`ano` AS `ano`,
    `tca`.`numero_semana` AS `numero_semana`,
    `tca`.`numero_semana_dia` AS `numero_semana_dia`,
    `tca`.`tipo_apontamento_id` AS `tipo_apontamento`,
    `tsag`.`descricao` AS `status_apontamento_grupo`,
    `tsa`.`cod_status_apontamento` AS `cod_status_apontamento`,
    `tsa`.`descricao` AS `status_apontamento`,
    `tpo`.`cod_projeto` AS `cod_projeto`,
    `tpo`.`projeto` AS `nome_projeto`,
    `tpo`.`cod_cliente` AS `cod_cliente`,
    `tpo`.`cliente` AS `nome_cliente`,
    `tpo`.`permite_apont_sem_alocacao` AS `permite_apont_sem_alocacao`,
    `tpo`.`permite_apont_sem_alocacao_outro_colab` AS `permite_apont_sem_alocacao_outro_colab`,
    coalesce(`tc_gerente`.`nome_completo`, 'GERENTE NÃO INFORMADO') AS `gerente`,
    `tpg`.`tipo_gerente` AS `tipo_gerente`,
    `tc_gerente`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gerente`,
    `tco_gerente`.`cod_colaborador_externo` AS `cod_gerente`,
    `ta`.`id` AS `atividade_id`,
    `ta`.`descricao` AS `atividade_descricao`,
    `tca`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    `tca`.`tb_org_id` AS `tb_org_id`,
    `tca`.`observacao` AS `observacao`
from
    (((((((((`tb_colaborador_apontamento` `tca`
join `tb_status_apontamento` `tsa` on
    ((`tca`.`tb_status_apontamento_id` = `tsa`.`id`)))
join `tb_status_apontamento_grupo` `tsag` on
    ((`tsa`.`tb_cod_status_grupo` = `tsag`.`cod_status_grupo`)))
join `tb_projeto_org` `tpo` on
    (((`tca`.`tb_projeto_org_cod_projeto` = `tpo`.`cod_projeto`)
        and (`tca`.`tb_org_id` = `tpo`.`tb_org_id`))))
left join `tb_projeto_gerente` `tpg` on
    (((`tpo`.`cod_projeto` = `tpg`.`cod_projeto`)
        and (`tca`.`tb_org_id` = `tpg`.`tb_org_id`))))
left join `tb_colaborador_org` `tco_gerente` on
    (((`tpg`.`cod_colaborador_gerente` = `tco_gerente`.`cod_colaborador_externo`)
        and (`tca`.`tb_org_id` = `tco_gerente`.`tb_org_id`))))
left join `tb_colaborador` `tc_gerente` on
    ((`tco_gerente`.`codigo_interno_colaborador` = `tc_gerente`.`codigo_interno_colaborador`)))
join `tb_atividade` `ta` on
    ((`tca`.`tb_atividade_id` = `ta`.`id`)))
join `tb_vigencia` `tv` on
    ((`tca`.`tb_vigencia_id` = `tv`.`id`)))
left join `tb_colaborador` `colab_justificativa` on
    ((`colab_justificativa`.`codigo_interno_colaborador` = `tca`.`codigo_interno_colaborador`)))
where
    (`tsag`.`cod_status_grupo` <> 4);

   
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_colaboradores_gestor` AS
select
    `tc`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    `tc`.`nome_completo` AS `nome_completo_colaborador`,
    `tc`.`nome_completo` AS `nome_completo`,
    `tco`.`tb_org_id` AS `tb_org_id`,
    `tcho`.`cod_colaborador_superior` AS `cod_gerente`,
    `tco`.`cod_colaborador_externo` AS `cod_colaborador`,
    `gerente`.`nome_completo` AS `nome_completo_gerente`
from
    ((((`tb_colaborador` `tc`
join `tb_colaborador_org` `tco` on
    ((`tc`.`codigo_interno_colaborador` = `tco`.`codigo_interno_colaborador`)))
join `tb_colaborador_hierarquia` `tcho` on
    (((`tcho`.`cod_colaborador_externo` = `tco`.`cod_colaborador_externo`)
        and (`tco`.`tb_org_id` = `tcho`.`tb_org_id`))))
left join `tb_colaborador_org` `gerente_org` on
    (((`gerente_org`.`cod_colaborador_externo` = `tcho`.`cod_colaborador_superior`)
        and (`tco`.`tb_org_id` = `gerente_org`.`tb_org_id`))))
left join `tb_colaborador` `gerente` on
    ((`gerente`.`codigo_interno_colaborador` = `gerente_org`.`codigo_interno_colaborador`)))
group by
    `tc`.`codigo_interno_colaborador`,
    `tc`.`nome_completo`,
    `tco`.`tb_org_id`,
    `tcho`.`cod_colaborador_superior`,
    `tco`.`cod_colaborador_externo`,
    `gerente`.`nome_completo`;
    
   
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_filtro_mapaalocacao` AS
select
    `c`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    `c`.`nome_completo` AS `nome_completo`,
    `co`.`ativo` AS `ativo`,
    `co`.`tb_org_id` AS `tb_org_id`,
    `co`.`cod_diretoria` AS `cod_diretoria`,
    `co`.`diretoria` AS `diretoria`,
    (
    select
        concat('||', group_concat(distinct `co`.`descricao` separator '||'), '||')
    from
        (`tb_colaborador_competencia` `cc`
    join `tb_competencia` `co` on
        ((`co`.`id` = `cc`.`competencia_id`)))
    where
        (`cc`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
    group by
        `cc`.`codigo_interno_colaborador`) AS `hardskills`,
    (
    select
        concat('||', group_concat(distinct `so`.`descricao` separator '||'), '||')
    from
        (`tb_colaborador_idioma` `cs`
    join `tb_idioma` `so` on
        ((`so`.`id` = `cs`.`idioma_id`)))
    where
        (`cs`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)
    group by
        `cs`.`codigo_interno_colaborador`) AS `idiomas`
from
    (`tb_colaborador` `c`
join `tb_colaborador_org` `co` on
    ((`co`.`codigo_interno_colaborador` = `c`.`codigo_interno_colaborador`)))
group by
    `c`.`codigo_interno_colaborador`,
    `co`.`tb_org_id`;

   
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_gestores_colaboradores_org` AS
select
    `tco`.`cod_colaborador_externo` AS `cod_colaborador_externo_subordinado`,
    `tc`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_subordinado`,
    `tc`.`nome_completo` AS `nome_completo_subordinado`,
    `tch`.`cod_colaborador_superior` AS `cod_colaborador_externo_gestor`,
    `tc_gerente`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gestor`,
    `tc_gerente`.`nome_completo` AS `nome_completo_gestor`,
    `tch`.`tb_org_id` AS `tb_org_id`
from
    ((((`tb_colaborador_hierarquia` `tch`
join `tb_colaborador_org` `tco` on
    (((`tch`.`cod_colaborador_externo` = `tco`.`cod_colaborador_externo`)
        and (`tch`.`tb_org_id` = `tco`.`tb_org_id`))))
join `tb_colaborador` `tc` on
    ((`tco`.`codigo_interno_colaborador` = `tc`.`codigo_interno_colaborador`)))
join `tb_colaborador_org` `tco_gerente` on
    (((`tco_gerente`.`cod_colaborador_externo` = `tch`.`cod_colaborador_superior`)
        and (`tch`.`tb_org_id` = `tco_gerente`.`tb_org_id`))))
join `tb_colaborador` `tc_gerente` on
    ((`tc_gerente`.`codigo_interno_colaborador` = `tco_gerente`.`codigo_interno_colaborador`)));
    
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_gestores_estatisticas_org` AS
select
    `tc`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    `tch`.`tb_org_id` AS `org_id`
from
    ((`tb_colaborador_hierarquia` `tch`
join `tb_colaborador_org` `tco` on
    (((`tch`.`cod_colaborador_superior` = `tco`.`cod_colaborador_externo`)
        and (`tch`.`tb_org_id` = `tco`.`tb_org_id`))))
join `tb_colaborador` `tc` on
    ((`tc`.`codigo_interno_colaborador` = `tco`.`codigo_interno_colaborador`)))
where
    ((`tc`.`ativo` = 1)
        and (`tco`.`ativo` = 1))
group by
    `tch`.`cod_colaborador_superior`,
    `tc`.`nome_completo`,
    `tch`.`tb_org_id`;
    
   
   
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_gestores_org` AS
select
    `a`.`cod_colaborador_superior` AS `cod_colaborador_superior`,
    `c`.`nome_completo` AS `nome_completo`,
    `b`.`cod_diretoria` AS `cod_diretoria`,
    `b`.`diretoria` AS `diretoria`,
    `a`.`tb_org_id` AS `tb_org_id`,
    `b`.`ativo` AS `ativo`,
    `b`.`cod_departamento` AS `cod_departamento`
from
    ((`tb_colaborador_hierarquia` `a`
join `tb_colaborador_org` `b` on
    (((`a`.`cod_colaborador_superior` = `b`.`cod_colaborador_externo`)
        and (`a`.`tb_org_id` = `b`.`tb_org_id`))))
join `tb_colaborador` `c` on
    ((`c`.`codigo_interno_colaborador` = `b`.`codigo_interno_colaborador`)))
group by
    `a`.`cod_colaborador_superior`,
    `c`.`nome_completo`,
    `b`.`cod_diretoria`,
    `b`.`diretoria`,
    `b`.`ativo`,
    `b`.`cod_departamento`,
    `a`.`tb_org_id`;
    
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_mapa_alocacao_colaborador_tbd` AS
select
    `x`.`cod_profisisonal` AS `cod_profisisonal`,
    `x`.`nome_profissional` AS `nome_profissional`,
    `x`.`codigo_interno_colaborador_gestor` AS `codigo_interno_colaborador_gestor`,
    `x`.`codigo_diretoria` AS `codigo_diretoria`,
    `x`.`codigo_departamento` AS `codigo_departamento`,
    `x`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
    `x`.`eh_tbd` AS `eh_tbd`,
    `x`.`tb_org_id` AS `tb_org_id`
from
    (
    select
        `tco`.`cod_colaborador_externo` AS `cod_profisisonal`,
        `tc`.`nome_completo` AS `nome_profissional`,
        NULL AS `codigo_interno_colaborador_gestor`,
        `tco`.`cod_diretoria` AS `codigo_diretoria`,
        `tco`.`cod_departamento` AS `codigo_departamento`,
        `tco`.`codigo_interno_colaborador` AS `codigo_interno_colaborador`,
        false AS `eh_tbd`,
        `tco`.`tb_org_id` AS `tb_org_id`
    from
        (`tb_colaborador_org` `tco`
    join `tb_colaborador` `tc` on
        ((`tco`.`codigo_interno_colaborador` = `tc`.`codigo_interno_colaborador`)))
    where
        ((`tc`.`ativo` = 1)
            and (`tco`.`ativo` = 1))
union
    select
        `tbd`.`cod_tbd_alocado` AS `cod_profisisonal`,
        `tbd`.`descricao` AS `nome_profissional`,
        `tbd`.`codigo_interno_colaborador` AS `codigo_interno_colaborador_gestor`,
        `tbd`.`cod_diretoria` AS `codigo_diretoria`,
        NULL AS `codigo_departamento`,
        NULL AS `codigo_interno_colaborador`,
        true AS `EhTbd`,
        `tbd`.`tb_org_id` AS `tb_org_id`
    from
        `tb_tbd_alocado` `tbd`) `x`;
        
       
CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `vw_totalizadores_unidades` AS
select
    (case
        when (`tco`.`diretoria` = '') then 'Sem Diretoria'
        else `tco`.`diretoria`
    end) AS `Diretoria`,
    `tco`.`tb_org_id` AS `tb_org_id`,
    count(`tco`.`codigo_interno_colaborador`) AS `Count`
from
    (`tb_colaborador_org` `tco`
join `tb_colaborador` `tc` on
    ((`tco`.`codigo_interno_colaborador` = `tc`.`codigo_interno_colaborador`)))
where
    ((`tco`.`ativo` = 1)
        and (`tc`.`ativo` = 1))
group by
    `tco`.`diretoria`,
    `tco`.`tb_org_id`;