CREATE VIEW vw_mapa_alocacao_colaborador_tbd AS
    SELECT 
        cod_profisisonal AS cod_profisisonal,
        nome_profissional AS nome_profissional,
        cpf_gestor_tbd AS cpf_gestor_tbd,
        codigo_diretoria AS codigo_diretoria,
        cpf AS cpf,
        eh_tbd AS eh_tbd,
        tb_org_id AS tb_org_id
    FROM
        (SELECT 
            tco.cod_colaborador_externo AS cod_profisisonal,
                tc.nome_completo AS nome_profissional,
                NULL AS cpf_gestor_tbd,
                tco.cod_diretoria AS codigo_diretoria,
                tco.tb_colaborador_cpf AS cpf,
                FALSE AS eh_tbd,
                tco.tb_org_id AS tb_org_id
        FROM
            tb_colaborador_org tco
        JOIN tb_colaborador tc ON tco.tb_colaborador_cpf = tc.cpf
        WHERE
            tc.ativo = 1 AND tco.ativo = 1
	UNION
		SELECT 
            tbd.cod_tbd_alocado AS cod_profisisonal,
            tbd.descricao AS nome_profissional,
            tbd.tb_colaborador_cpf_gestor AS cpf_gestor_tbd,
            tbd.cod_diretoria AS codigo_diretoria,
            NULL AS cpf,
            TRUE AS EhTbd,
            tbd.tb_org_id AS tb_org_id
        FROM
            tb_tbd_alocado tbd) x