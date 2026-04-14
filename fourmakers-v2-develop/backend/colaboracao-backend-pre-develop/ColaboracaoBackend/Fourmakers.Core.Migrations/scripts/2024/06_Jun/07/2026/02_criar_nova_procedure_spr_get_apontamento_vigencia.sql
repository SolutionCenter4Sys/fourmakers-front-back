DELIMITER //

CREATE PROCEDURE `spr_get_apontamento_vigencia`(
    IN p_limite INT,
    IN p_offset INT,
    IN p_mes INT,
    IN p_ano INT,
    IN p_tb_org_id INT,
    IN p_nome VARCHAR(255),
    IN p_cod_gerente INT,
    IN p_cod_status_apontamento INT
)
BEGIN

    CREATE TEMPORARY TABLE temp_result AS
	SELECT
        resultado_select.nome_completo AS colaborador_nome,
        resultado_select.cpf AS cpf,
        resultado_select.ativo AS ativo,
        resultado_select.data_admissao AS data_admissao,
        resultado_select.data_inativacao AS data_inativacao,
        resultado_select.nome_completo_gerente AS nome_completo_gerente,
        resultado_select.codigo_gerente AS codigo_gerente,
        resultado_select.cod_status_apontamento_periodo AS cod_status_apontamento_periodo,
        tsa.descricao AS descricao_status_apontamento,
        resultado_select.soma_horas AS soma_horas,
        resultado_select.mes AS mes,
        resultado_select.ano AS ano,
        resultado_select.tb_org_id AS tb_org_id,
        resultado_select.observacao AS observacao
    FROM
    (
		SELECT 
        tc.nome_completo AS nome_completo,
        tc.cpf AS cpf,
        tco.ativo,
        tco.data_admissao,
        tco.data_inativacao,
        tcg.nome_completo AS nome_completo_gerente,
        tch.cod_colaborador_superior AS codigo_gerente,
        (
            CASE
                WHEN (tsa.cod_status_apontamento IS NULL) THEN 7
                WHEN (SUM((CASE WHEN (tsa.cod_status_apontamento = 4) THEN 1 ELSE 0 END)) > 0) THEN 4
                WHEN (SUM((CASE WHEN (tsa.cod_status_apontamento = 5) THEN 1 ELSE 0 END)) = COUNT(0)) THEN 5
                ELSE 1
            END
        ) AS cod_status_apontamento_periodo,
        COALESCE(SUM(tca.horas), 0) AS soma_horas,
        tco.tb_org_id AS tb_org_id,
        COALESCE(tv.mes, p_mes) AS mes,
        COALESCE(tv.ano, p_ano) AS ano,
        tca.observacao AS observacao
    FROM
        tb_colaborador tc
    JOIN tb_colaborador_org tco ON tc.cpf = tco.tb_colaborador_cpf
    LEFT JOIN tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo AND tch.tb_org_id = tco.tb_org_id
    LEFT JOIN tb_colaborador_org tcog ON tcog.cod_colaborador_externo = tch.cod_colaborador_superior AND tco.tb_org_id = tcog.tb_org_id
    LEFT JOIN tb_colaborador tcg ON tcg.cpf = tcog.tb_colaborador_cpf
    LEFT JOIN tb_colaborador_apontamento tca ON tc.cpf = tca.tb_colaborador_org_tb_colaborador_cpf AND tco.tb_org_id = tca.tb_org_id AND tca.tb_vigencia_id = (SELECT id FROM tb_vigencia WHERE mes = p_mes AND ano = p_ano)
    LEFT JOIN tb_status_apontamento tsa ON tca.tb_status_apontamento_id = tsa.id
    LEFT JOIN tb_vigencia tv ON tca.tb_vigencia_id = tv.id
    LEFT JOIN tb_status_apontamento_grupo tsag ON tsa.tb_cod_status_grupo = tsag.cod_status_grupo
    WHERE 
        tc.ativo = 1
        AND (tsag.cod_status_grupo <> 4 OR tsag.cod_status_grupo IS NULL)
        AND tco.tb_org_id = p_tb_org_id
        AND (COALESCE(p_cod_gerente, 0) = 0 OR tch.cod_colaborador_superior = p_cod_gerente)
        AND (COALESCE(p_nome,'') = '' OR tc.nome_completo LIKE CONCAT('%',p_nome,'%'))
    GROUP BY 
        tc.nome_completo,
        tc.cpf,
        tcg.nome_completo,
        tch.cod_colaborador_superior,
        tco.tb_org_id,
        tv.mes,
        tv.ano
        ORDER BY 
            nome_completo
    ) resultado_select
    LEFT JOIN tb_status_apontamento tsa ON resultado_select.cod_status_apontamento_periodo = tsa.cod_status_apontamento
    WHERE 
        (COALESCE(p_cod_status_apontamento, 0) = 0 OR tsa.cod_status_apontamento = p_cod_status_apontamento);
        
	SELECT *
        FROM temp_result
        ORDER BY 
            colaborador_nome
        LIMIT 
            p_limite 
        OFFSET 
            p_offset;

    SELECT COUNT(*) as total, SUM(soma_horas) as soma_horas 
    FROM temp_result;
    
    DROP TEMPORARY TABLE temp_result;
END //

DELIMITER ;