DROP PROCEDURE spr_get_alocacao_colaborador_e_tbd;
DELIMITER //
CREATE PROCEDURE `spr_get_alocacao_colaborador_e_tbd`(
    IN p_pesquisa VARCHAR(255),
    IN p_org_id INT,
    IN p_periodo_alocado_id INT
)
BEGIN
    SELECT 
        tcpa.id AS periodo_alocado_id,
        tco.cod_departamento,
        tco.departamento,
        CASE 
            WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN tcpa.cod_tbd_alocado
            ELSE tcpa.codigo_colaborador
        END AS codigo_colaborador,
        CASE 
            WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN ttbd.descricao
            ELSE tc.nome_completo
        END AS nome_completo,
        tcpa.codigo_projeto,
        tpo.projeto,
        tpo.cod_cliente,
        tpo.cliente,
        tpo.status,
        tpg.cod_colaborador_gerente AS cod_gerente,
        tg.nome_completo AS nome_gerente,
        tcpa.data_inicio,
        tcpa.data_fim,
        tcpa.quantidade_horas,
        tcpa.percentual,
        CASE 
            WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1
            ELSE 0
        END AS tbd,
        CASE 
            WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN tc_gestor_tbd.nome_completo
            ELSE vw_gestor_adm.nome_completo_gerente
        END AS nome_gestor_adm
    FROM 
        tb_colaborador_periodo_alocacao tcpa
    LEFT JOIN
        tb_colaborador_org tco ON tcpa.codigo_colaborador = tco.cod_colaborador_externo AND tco.tb_org_id = tcpa.tb_org_id
    LEFT JOIN
        tb_colaborador tc ON tco.tb_colaborador_cpf = tc.cpf 
	LEFT JOIN
        tb_tbd_alocado ttbd ON tcpa.cod_tbd_alocado = ttbd.cod_tbd_alocado and tcpa.tb_org_id = ttbd.tb_org_id
	LEFT JOIN 
		tb_colaborador_org tco_gestor_tbd ON tco_gestor_tbd.tb_colaborador_cpf = ttbd.tb_colaborador_cpf_gestor = ttbd.tb_org_id = tco_gestor_tbd.tb_org_id
	LEFT JOIN
		tb_colaborador tc_gestor_tbd ON ttbd.tb_colaborador_cpf_gestor = tc_gestor_tbd.cpf
    LEFT JOIN
        tb_projeto_org tpo ON tcpa.codigo_projeto = tpo.cod_projeto AND tpo.tb_org_id = tcpa.tb_org_id
    LEFT JOIN 
        tb_projeto_gerente tpg ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tcpa.tb_org_id
    LEFT JOIN
        tb_colaborador_org tco_g ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo AND tco_g.tb_org_id = tcpa.tb_org_id
    LEFT JOIN
        tb_colaborador tg ON tco_g.tb_colaborador_cpf = tg.cpf
	LEFT JOIN
		vw_colaboradores_gestor vw_gestor_adm ON vw_gestor_adm.cod_colaborador = tco.cod_colaborador_externo AND vw_gestor_adm.tb_org_id = tco.tb_org_id
    WHERE
        tcpa.ativo = 1 AND tcpa.ativo = 1 AND
        tcpa.tb_org_id = p_org_id AND
        (p_periodo_alocado_id IS NULL OR tcpa.id = p_periodo_alocado_id) AND
        (p_pesquisa IS NULL OR
         (tcpa.cod_tbd_alocado IS NOT NULL AND tcpa.cod_tbd_alocado = p_pesquisa) OR
         (tcpa.cod_tbd_alocado IS NULL AND tcpa.codigo_colaborador = p_pesquisa) OR
         (tcpa.cod_tbd_alocado IS NOT NULL AND ttbd.descricao LIKE CONCAT('%', p_pesquisa, '%')) OR
         (tcpa.cod_tbd_alocado IS NULL AND tc.nome_completo LIKE CONCAT('%', p_pesquisa, '%')) OR
         (tcpa.cod_tbd_alocado IS NULL AND tpo.projeto LIKE CONCAT('%', p_pesquisa, '%')) OR
         tpo.cod_cliente = p_pesquisa OR
         tcpa.codigo_projeto = p_pesquisa OR
         tpo.cliente LIKE CONCAT('%', p_pesquisa, '%') OR
         tpg.cod_colaborador_gerente = p_pesquisa OR
         tg.nome_completo LIKE CONCAT('%', p_pesquisa, '%'))
    ;
END //
DELIMITER ;