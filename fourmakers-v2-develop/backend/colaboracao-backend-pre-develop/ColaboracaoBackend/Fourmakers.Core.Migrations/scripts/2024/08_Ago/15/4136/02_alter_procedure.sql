DROP procedure IF EXISTS `spr_rpt_relatorio_apontamento_simplificado`;
DELIMITER $$
CREATE DEFINER=`admin`@`%` PROCEDURE `spr_rpt_relatorio_apontamento_simplificado`(
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
			CONCAT(LOWER(CONVERT(fn_get_dia_semana(tca.data), CHAR)), ' ', CONVERT(DATE_FORMAT(tca.data, '%d/%m'), CHAR)) AS Data,
			tc.nome_completo AS Nome,
			tco.cod_colaborador_externo AS "Matrícula",
			ta.descricao AS Tarefa,
			tpo.projeto AS Projeto,
            TIME_FORMAT(SEC_TO_TIME(tca.horas * 60), '%H:%i') AS Horas,
			tca.observacao AS "Resumo das atividades",
            tc_aprovador.nome_completo AS Aprovador,
            REPLACE(CONVERT(ROUND((tca.horas / 60), 2), CHAR), '.', ',') AS "Hora (Decimal)"
		FROM
			tb_colaborador_apontamento tca
			JOIN tb_colaborador tc ON tca.codigo_interno_colaborador = tc.codigo_interno_colaborador
			JOIN tb_colaborador_org tco ON tca.codigo_interno_colaborador = tco.codigo_interno_colaborador AND tca.tb_org_id = tco.tb_org_id 
			JOIN tb_projeto_org tpo ON tca.tb_projeto_org_cod_projeto = tpo.cod_projeto AND tca.tb_org_id = tpo.tb_org_id
			JOIN tb_projeto_gerente tpg ON tpo.cod_projeto = tpg.cod_projeto AND tpo.tb_org_id = tpg.tb_org_id
			JOIN tb_colaborador_org tpog on tpg.cod_colaborador_gerente = tpog.cod_colaborador_externo
			JOIN tb_atividade ta ON tca.tb_atividade_id = ta.id
			JOIN tb_status_apontamento tsa ON tca.tb_status_apontamento_id = tsa.id
            LEFT JOIN tb_colaborador tc_aprovador ON tca.codigo_interno_colaborador_justificativa = tc_aprovador.codigo_interno_colaborador and tsa.tb_cod_status_grupo = 2 -- aprovado
		WHERE 
			tca.tb_org_id = org_id
			AND MONTH(tca.data) = mes
			AND YEAR(tca.data) = ano
			AND tpog.codigo_interno_colaborador = codigo_interno_colaborador_gerente
		ORDER BY 
			tca.data
		DESC;
    ELSE
        -- Insere os dados na tabela temporária quando o cpf_gerente não é passado
        SELECT 
			CONCAT(LOWER(CONVERT(fn_get_dia_semana(tca.data), CHAR)), ' ', CONVERT(DATE_FORMAT(tca.data, '%d/%m'), CHAR)) AS Data,
			tc.nome_completo AS Nome,
			tco.cod_colaborador_externo AS "Matrícula",
			ta.descricao AS Tarefa,
			tpo.projeto AS Projeto,
            TIME_FORMAT(SEC_TO_TIME(tca.horas * 60), '%H:%i') AS Horas,
			tca.observacao AS "Resumo das atividades",
            tc_aprovador.nome_completo AS Aprovador,
            REPLACE(CONVERT(ROUND((tca.horas / 60), 2), CHAR), '.', ',') AS "Hora (Decimal)"
		FROM
			tb_colaborador_apontamento tca
			JOIN tb_colaborador tc ON tca.codigo_interno_colaborador = tc.codigo_interno_colaborador
			JOIN tb_colaborador_org tco ON tca.codigo_interno_colaborador = tco.codigo_interno_colaborador AND tca.tb_org_id = tco.tb_org_id 
			JOIN tb_projeto_org tpo ON tca.tb_projeto_org_cod_projeto = tpo.cod_projeto AND tca.tb_org_id = tpo.tb_org_id
			JOIN tb_atividade ta ON tca.tb_atividade_id = ta.id
			JOIN tb_status_apontamento tsa ON tca.tb_status_apontamento_id = tsa.id
            LEFT JOIN tb_colaborador tc_aprovador ON tca.codigo_interno_colaborador_justificativa = tc_aprovador.codigo_interno_colaborador and tsa.tb_cod_status_grupo = 2 -- aprovado
		WHERE 
			tca.tb_org_id = org_id
			AND MONTH(tca.data) = mes
			AND YEAR(tca.data) = ano
		ORDER BY 
			tca.data
		DESC;
    END IF;

COMMIT;
END$$

DELIMITER ;