Create or replace VIEW `vw_gestores_colaboradores_org` AS
    SELECT 
        tco.cod_colaborador_externo AS cod_colaborador_externo_subordinado,
        tc.cpf AS cpf_subordinado,
        tc.nome_completo AS nome_completo_subordinado,
        tch.cod_colaborador_superior AS cod_colaborador_externo_gestor,
        tc_gerente.cpf AS cpf_gestor,
        tc_gerente.nome_completo AS nome_completo_gestor,
        tch.tb_org_id AS tb_org_id
    FROM
		tb_colaborador_hierarquia tch
        JOIN tb_colaborador_org tco ON tch.cod_colaborador_externo = tco.cod_colaborador_externo and tch.tb_org_id = tco.tb_org_id
        JOIN tb_colaborador tc ON tco.tb_colaborador_cpf = tc.cpf
        JOIN tb_colaborador_org tco_gerente ON tco_gerente.cod_colaborador_externo = tch.cod_colaborador_superior and tch.tb_org_id = tco_gerente.tb_org_id
        JOIN tb_colaborador tc_gerente ON tc_gerente.cpf = tco_gerente.tb_colaborador_cpf