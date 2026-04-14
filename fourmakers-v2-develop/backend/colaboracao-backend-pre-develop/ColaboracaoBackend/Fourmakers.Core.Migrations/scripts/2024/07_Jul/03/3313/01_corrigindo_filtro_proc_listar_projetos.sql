-- foram add as duas condições no WHERE -> OR tpo.data_fim IS NULL OR tpo.data_fim = '0001-01-01'
DROP PROCEDURE spr_get_projeto_com_atividades_por_cpf;
DELIMITER //
CREATE PROCEDURE `spr_get_projeto_com_atividades_por_cpf`(
    IN p_cpf VARCHAR(11),
    IN p_org_id INT,
    IN p_lancamento_para_outro_colaborador BOOLEAN
)
BEGIN
    SELECT
        tco_cod.cod_colaborador_externo, 
        tpo.cod_projeto, 
        tpo.tb_org_id, 
        tpo.projeto AS nome_projeto,
        tpo.cod_cliente,
        tpo.cliente,
        ta.id AS atividade_id,
        ta.descricao AS descricao_atividade,
        tco_cod.tb_colaborador_cpf
    FROM
		tb_atividade ta 
    JOIN
		tb_projeto_org_atividade tpoa ON ta.id = tpoa.tb_atividade_id AND ta.tb_org_id = tpoa.tb_projeto_tb_org_id
    JOIN
		tb_projeto_org tpo ON tpoa.tb_projeto_org_cod_projeto = tpo.cod_projeto AND ta.tb_org_id = tpo.tb_org_id
    LEFT JOIN
		tb_colaborador_org tco_cod ON tco_cod.tb_colaborador_cpf = p_cpf AND tpo.tb_org_id = tco_cod.tb_org_id
    LEFT JOIN
		tb_colaborador_projeto_org tcpo ON tpo.cod_projeto = tcpo.cod_projeto AND tpo.tb_org_id = tcpo.tb_org_id 
			AND tcpo.cod_colaborador = tco_cod.cod_colaborador_externo
    WHERE 
        (tcpo.cod_colaborador IS NOT NULL 
        OR (tpo.permite_apont_sem_alocacao = 1 OR (tpo.permite_apont_sem_alocacao_outro_colab = 1 AND p_lancamento_para_outro_colaborador)))
        AND ta.tb_org_id = p_org_id
        AND (tpo.data_fim >= CURDATE() OR tpo.data_fim IS NULL OR tpo.data_fim = '0001-01-01')
    ORDER BY tcpo.cod_colaborador, ta.descricao;
END //
DELIMITER ;