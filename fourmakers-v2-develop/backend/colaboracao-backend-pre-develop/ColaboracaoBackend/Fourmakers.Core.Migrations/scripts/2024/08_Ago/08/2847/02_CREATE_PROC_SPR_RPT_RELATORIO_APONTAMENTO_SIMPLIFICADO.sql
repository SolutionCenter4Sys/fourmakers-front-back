DROP PROCEDURE IF EXISTS `spr_rpt_relatorio_apontamento_simplificado`;
DELIMITER //
CREATE PROCEDURE `spr_rpt_relatorio_apontamento_simplificado`(
    IN mes INT,
    IN ano INT,
    IN org_id INT,
    IN codigo_interno_colaborador_gerente VARCHAR(20) -- Adicionando o parâmetro cpf_gerente
)
BEGIN
DECLARE v_valor_parametro VARCHAR(255);
    DECLARE v_sql TEXT;

    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    -- Verifica se o cpf_gerente foi passado
    IF codigo_interno_colaborador_gerente IS NOT NULL THEN
        SELECT 
			DATE_FORMAT(ca.data, '%d/%m/%Y') AS Data,
            fn_get_dia_semana(ca.data) AS Dia,
			c.nome_completo AS Nome,
			co.cod_colaborador_externo AS "Matrícula",
			atv.descricao AS Atividade,
			po.projeto AS Projeto,
            TIME_FORMAT(SEC_TO_TIME(ca.horas * 60), '%H:%i') AS Horas,
			ca.observacao AS "Resumo das atividades",
            REPLACE(CONVERT(ROUND((ca.horas / 60), 2), CHAR), '.', ',') AS "Horas Decimal"
		FROM
			tb_colaborador_apontamento ca
			JOIN tb_colaborador c ON ca.codigo_interno_colaborador = c.codigo_interno_colaborador
			JOIN tb_colaborador_org co ON ca.codigo_interno_colaborador = co.codigo_interno_colaborador AND ca.tb_org_id = co.tb_org_id 
			JOIN tb_projeto_org po ON ca.tb_projeto_org_cod_projeto = po.cod_projeto AND ca.tb_org_id = po.tb_org_id
			JOIN tb_projeto_gerente tpg ON po.cod_projeto = tpg.cod_projeto AND po.tb_org_id = tpg.tb_org_id
			JOIN tb_colaborador_org tpog on tpg.cod_colaborador_gerente = tpog.cod_colaborador_externo
			JOIN tb_atividade atv ON ca.tb_atividade_id = atv.id
			JOIN tb_status_apontamento sa ON ca.tb_status_apontamento_id = sa.id
			LEFT JOIN tb_colaborador c_modificador ON ca.codigo_interno_colaborador_alteracao = c_modificador.codigo_interno_colaborador
		WHERE 
			ca.tb_org_id = org_id
			AND MONTH(ca.data) = mes
			AND YEAR(ca.data) = ano
			AND tpog.codigo_interno_colaborador = codigo_interno_colaborador_gerente
		ORDER BY 
			ca.data
		DESC;
    ELSE
        -- Insere os dados na tabela temporária quando o cpf_gerente não é passado
        SELECT 
			DATE_FORMAT(ca.data, '%d/%m/%Y') AS Data,
            fn_get_dia_semana(ca.data) AS Dia,
			c.nome_completo AS Nome,
			co.cod_colaborador_externo AS "Matrícula",
			atv.descricao AS Atividade,
			po.projeto AS Projeto,
            TIME_FORMAT(SEC_TO_TIME(ca.horas * 60), '%H:%i') AS Horas,
			ca.observacao AS "Resumo das atividades",
            REPLACE(CONVERT(ROUND((ca.horas / 60), 2), CHAR), '.', ',') AS "Horas Decimal"
        FROM
            tb_colaborador_apontamento ca
            JOIN tb_colaborador c ON ca.codigo_interno_colaborador = c.codigo_interno_colaborador
            JOIN tb_colaborador_org co ON ca.codigo_interno_colaborador = co.codigo_interno_colaborador AND ca.tb_org_id = co.tb_org_id 
            JOIN tb_projeto_org po ON ca.tb_projeto_org_cod_projeto = po.cod_projeto AND ca.tb_org_id = po.tb_org_id
            JOIN tb_atividade atv ON ca.tb_atividade_id = atv.id 
            JOIN tb_status_apontamento sa ON ca.tb_status_apontamento_id = sa.id
            LEFT JOIN tb_colaborador c_modificador ON ca.codigo_interno_colaborador_alteracao = c_modificador.codigo_interno_colaborador
        WHERE 
            ca.tb_org_id = org_id
            AND MONTH(ca.data) = mes
            AND YEAR(ca.data) = ano
        ORDER BY
			ca.data
		DESC;
    END IF;

COMMIT;
END//
DELIMITER ;