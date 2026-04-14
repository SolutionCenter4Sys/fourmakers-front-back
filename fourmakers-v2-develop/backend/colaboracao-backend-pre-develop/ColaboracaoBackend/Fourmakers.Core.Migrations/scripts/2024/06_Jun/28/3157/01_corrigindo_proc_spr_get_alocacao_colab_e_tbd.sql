CREATE DEFINER=`usuariohml`@`%` PROCEDURE `spr_get_alocacao_colab_e_tbd`(
    IN p_pesquisa VARCHAR(255),
    IN p_org_id INT,
    IN p_periodo_alocado_id INT
)
BEGIN
    SELECT 
        tpa.id AS periodo_alocado_id,
        tca.id AS colaborador_alocado_id, 
        tco.cod_departamento,
        tco.departamento,
        CASE 
            WHEN tca.cod_tbd_alocado IS NOT NULL THEN tca.cod_tbd_alocado
            ELSE tca.codigo_colaborador
        END AS codigo_colaborador,
        CASE 
            WHEN tca.cod_tbd_alocado IS NOT NULL THEN ttbd.descricao
            ELSE tc.nome_completo
        END AS nome_completo,
        tca.codigo_projeto,
        tpo.projeto,
        tpo.cod_cliente,
        tpo.cliente,
        tpo.status,
        tpg.cod_colaborador_gerente AS cod_gerente,
        tg.nome_completo AS nome_gerente,
        tpa.data_inicio,
        tpa.data_fim,
        tpa.quantidade_horas,
        tpa.percentual,
        CASE 
            WHEN tca.cod_tbd_alocado IS NOT NULL THEN 1
            ELSE 0
        END AS tbd
    FROM 
        tb_colaborador_alocado tca
    LEFT JOIN 
        tb_periodo_alocacao tpa ON tca.id = tpa.tb_colaborador_alocado_id
    LEFT JOIN
        tb_colaborador_org tco ON tca.codigo_colaborador = tco.cod_colaborador_externo AND tco.tb_org_id = tca.tb_org_id
    LEFT JOIN
        tb_colaborador tc ON tco.tb_colaborador_cpf = tc.cpf 
	LEFT JOIN
        tb_tbd_alocado ttbd ON tca.cod_tbd_alocado = ttbd.cod_tbd_alocado and tca.tb_org_id = ttbd.tb_org_id
    LEFT JOIN
        tb_projeto_org tpo ON tca.codigo_projeto = tpo.cod_projeto AND tpo.tb_org_id = tca.tb_org_id
    LEFT JOIN 
        tb_projeto_gerente tpg ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tca.tb_org_id
    LEFT JOIN
        tb_colaborador_org tco_g ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo AND tco_g.tb_org_id = tca.tb_org_id
    LEFT JOIN
        tb_colaborador tg ON tco_g.tb_colaborador_cpf = tg.cpf
    WHERE
        tca.ativo = 1 AND tpa.ativo = 1 AND
        tca.tb_org_id = p_org_id AND
        (p_periodo_alocado_id IS NULL OR tpa.id = p_periodo_alocado_id) AND
        (p_pesquisa IS NULL OR
         (tca.cod_tbd_alocado IS NOT NULL AND tca.cod_tbd_alocado = p_pesquisa) OR
         (tca.cod_tbd_alocado IS NULL AND tca.codigo_colaborador = p_pesquisa) OR
         (tca.cod_tbd_alocado IS NOT NULL AND ttbd.descricao LIKE CONCAT('%', p_pesquisa, '%')) OR
         (tca.cod_tbd_alocado IS NULL AND tc.nome_completo LIKE CONCAT('%', p_pesquisa, '%')) OR
         (tca.cod_tbd_alocado IS NULL AND tpo.projeto LIKE CONCAT('%', p_pesquisa, '%')) OR
         tpo.cod_cliente = p_pesquisa OR
         tca.codigo_projeto = p_pesquisa OR
         tpo.cliente LIKE CONCAT('%', p_pesquisa, '%') OR
         tpg.cod_colaborador_gerente = p_pesquisa OR
         tg.nome_completo LIKE CONCAT('%', p_pesquisa, '%'))
    ;
END