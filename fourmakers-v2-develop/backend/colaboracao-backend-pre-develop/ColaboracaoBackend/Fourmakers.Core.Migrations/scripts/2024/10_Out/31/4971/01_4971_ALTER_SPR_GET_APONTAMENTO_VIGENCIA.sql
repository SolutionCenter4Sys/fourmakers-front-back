drop procedure `spr_get_apontamento_vigencia`;
DELIMITER //
CREATE PROCEDURE `spr_get_apontamento_vigencia`(
    IN p_limite INT,
    IN p_offset INT,
    IN p_mes INT,
    IN p_ano INT,
    IN p_tb_org_id INT,
    IN p_nome VARCHAR(255),
    IN p_cod_gerente INT,
    IN p_cod_status_apontamento INT,
    IN p_cod_projeto VARCHAR(255),
    IN p_cod_colaborador_externo_aprovador VARCHAR(255)
)
BEGIN
    -- Definir variáveis para evitar "magic numbers"
    DECLARE STATUS_TODOS INT DEFAULT 0;
    DECLARE STATUS_PENDENTE INT DEFAULT 1;
	DECLARE STATUS_APROVADO INT DEFAULT 2;
    DECLARE STATUS_REPROVADO INT DEFAULT 3;
    DECLARE STATUS_NAO_APONTADO INT DEFAULT 5;

	DROP TEMPORARY TABLE IF EXISTS temp_result;
	DROP TEMPORARY TABLE IF EXISTS temp_aprovadores_para_filtro;
    DROP TEMPORARY TABLE IF EXISTS temp_aprovadores_filtrado;
	DROP TEMPORARY TABLE IF EXISTS temp_result_para_filtro;
    DROP TEMPORARY TABLE IF EXISTS temp_result_filtrada;

	-- Criação da tabela de aprovadores
	CREATE TEMPORARY TABLE temp_aprovadores_para_filtro AS
    SELECT
        resultado_select.codigo_interno_colaborador,
        resultado_select.cod_status_apontamento_periodo,
        resultado_select.tb_org_id,
        resultado_select.mes,
        resultado_select.ano,
        resultado_select.cod_projeto,
        CASE 
            WHEN resultado_select.cod_status_apontamento_periodo IN (2, 3) 
                THEN resultado_select.codigo_interno_aprovador
            WHEN resultado_select.cod_status_apontamento_periodo = 1 
                THEN tco_ger.codigo_interno_colaborador
            ELSE NULL 
        END AS codigo_interno_aprovador,
        tsag.descricao
    FROM (
        SELECT 
            tca.codigo_interno_colaborador AS codigo_interno_colaborador,
            (
                CASE
                    WHEN (tsa.tb_cod_status_grupo IS NULL) THEN 5
                    WHEN (SUM((CASE WHEN (tsa.tb_cod_status_grupo = 3) THEN 1 ELSE 0 END)) > 0) THEN 3
                    WHEN (SUM((CASE WHEN (tsa.tb_cod_status_grupo = 2) THEN 1 ELSE 0 END)) = COUNT(0)) THEN 2
                    ELSE 1
                END
            ) AS cod_status_apontamento_periodo,
            tca.tb_org_id,
            COALESCE(tv.mes, p_mes) AS mes,
            COALESCE(tv.ano, p_ano) AS ano,
            tca.tb_projeto_org_cod_projeto as cod_projeto,
            COALESCE(tca.codigo_interno_colaborador_alteracao, tca.codigo_interno_colaborador_criacao) as codigo_interno_aprovador
        FROM
            tb_colaborador_apontamento tca
            JOIN tb_colaborador_org tco ON tca.codigo_interno_colaborador = tco.codigo_interno_colaborador AND tca.tb_org_id = tco.tb_org_id
            JOIN tb_status_apontamento tsa ON tca.tb_status_apontamento_id = tsa.id
            JOIN tb_colaborador_org tco_aprov ON COALESCE(tca.codigo_interno_colaborador_alteracao, tca.codigo_interno_colaborador_criacao) = tco_aprov.codigo_interno_colaborador 
				AND tca.tb_org_id = tco_aprov.tb_org_id
            JOIN tb_vigencia tv ON tca.tb_vigencia_id = tv.id
        WHERE 
            tco.tb_org_id = p_tb_org_id AND tca.tb_vigencia_id = (SELECT id FROM tb_vigencia WHERE mes = p_mes AND ano = p_ano)
        GROUP BY 
            tca.codigo_interno_colaborador,
            tca.tb_org_id,
            tv.mes,
            tv.ano,
            tsa.cod_status_apontamento,
            tca.tb_projeto_org_cod_projeto,
            COALESCE(tca.codigo_interno_colaborador_alteracao, tca.codigo_interno_colaborador_criacao)
    ) resultado_select
    LEFT JOIN tb_status_apontamento_grupo tsag ON resultado_select.cod_status_apontamento_periodo = tsag.cod_status_grupo
    LEFT JOIN tb_projeto_gerente tpg ON resultado_select.cod_status_apontamento_periodo = 1 AND resultado_select.cod_projeto = tpg.cod_projeto AND resultado_select.tb_org_id = tpg.tb_org_id
    LEFT JOIN tb_colaborador_org tco_ger ON tpg.cod_colaborador_gerente = tco_ger.cod_colaborador_externo AND resultado_select.tb_org_id = tco_ger.tb_org_id
    WHERE 
        (COALESCE(p_cod_status_apontamento, 0) = 0 OR resultado_select.cod_status_apontamento_periodo = p_cod_status_apontamento)
        AND (COALESCE(p_cod_projeto, '0') = '0' OR resultado_select.cod_projeto = p_cod_projeto);
    
    
    CREATE TEMPORARY TABLE temp_aprovadores_filtrado AS 
    SELECT 
		*
	FROM
		temp_aprovadores_para_filtro
	WHERE 
    COALESCE(p_cod_colaborador_externo_aprovador,0) = '0'
    OR codigo_interno_aprovador =
        (SELECT codigo_interno_colaborador 
             FROM tb_colaborador_org 
             WHERE cod_colaborador_externo = p_cod_colaborador_externo_aprovador  
             AND tb_org_id = p_tb_org_id);
    
    -- Criação da tabela temporária sem filtro
    CREATE TEMPORARY TABLE temp_result_para_filtro AS
    SELECT
        resultado_select.nome_completo AS colaborador_nome,
        resultado_select.codigo_interno_colaborador AS codigo_interno_colaborador,
        resultado_select.ativo AS ativo,
        resultado_select.data_admissao AS data_admissao,
        resultado_select.data_inativacao AS data_inativacao,
        resultado_select.nome_completo_gerente AS nome_completo_gerente,
        resultado_select.codigo_gerente AS codigo_gerente,
        resultado_select.cod_status_apontamento_periodo AS cod_status_apontamento_periodo,
        tsag.descricao AS descricao_status_apontamento,
        resultado_select.soma_horas AS soma_horas,
        resultado_select.mes AS mes,
        resultado_select.ano AS ano,
        resultado_select.tb_org_id AS tb_org_id,
        CASE 
            WHEN tpo.projeto IS NULL OR tpo.projeto = '-' THEN tpo.cod_projeto 
            ELSE CONCAT(tpo.cod_projeto, ' - ', tpo.projeto) 
        END AS projeto,
        tpo.cod_projeto,
		resultado_select.codigo_interno_colaborador_alteracao,
        resultado_select.codigo_interno_colaborador_criacao
    FROM
    (
        SELECT 
            tc.nome_completo AS nome_completo,
            tc.codigo_interno_colaborador AS codigo_interno_colaborador,
            tco.ativo,
            tco.data_admissao,
            tco.data_inativacao,
            tcg.nome_completo AS nome_completo_gerente,
            tch.cod_colaborador_superior AS codigo_gerente,
            (
                CASE
                    WHEN (tsa.tb_cod_status_grupo IS NULL) THEN STATUS_NAO_APONTADO
                    WHEN (SUM((CASE WHEN (tsa.tb_cod_status_grupo = STATUS_REPROVADO) THEN 1 ELSE 0 END)) > 0) THEN STATUS_REPROVADO
                    WHEN (SUM((CASE WHEN (tsa.tb_cod_status_grupo = STATUS_APROVADO) THEN 1 ELSE 0 END)) = COUNT(0)) THEN STATUS_APROVADO
                    ELSE STATUS_PENDENTE
                END
            ) AS cod_status_apontamento_periodo,
            COALESCE(SUM(tca.horas), 0) AS soma_horas,
            tco.tb_org_id AS tb_org_id,
            COALESCE(tv.mes, p_mes) AS mes,
            COALESCE(tv.ano, p_ano) AS ano,
            tca.codigo_interno_colaborador_alteracao,
            tca.codigo_interno_colaborador_criacao,
            tca.tb_projeto_org_cod_projeto
        FROM
            tb_colaborador tc
            JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
            LEFT JOIN tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo AND tch.tb_org_id = tco.tb_org_id
            LEFT JOIN tb_colaborador_org tcog ON tcog.cod_colaborador_externo = tch.cod_colaborador_superior AND tco.tb_org_id = tcog.tb_org_id
            LEFT JOIN tb_colaborador tcg ON tcg.codigo_interno_colaborador = tcog.codigo_interno_colaborador
            LEFT JOIN tb_colaborador_apontamento tca ON tc.codigo_interno_colaborador = tca.codigo_interno_colaborador AND tco.tb_org_id = tca.tb_org_id AND tca.tb_vigencia_id = (SELECT id FROM tb_vigencia WHERE mes = p_mes AND ano = p_ano)
            LEFT JOIN tb_status_apontamento tsa ON tca.tb_status_apontamento_id = tsa.id
            LEFT JOIN tb_vigencia tv ON tca.tb_vigencia_id = tv.id
        WHERE 
            tco.tb_org_id = p_tb_org_id
            AND (COALESCE(p_cod_gerente, 0) = 0 OR tch.cod_colaborador_superior = p_cod_gerente)
            AND (COALESCE(p_nome, '') = '' OR tc.nome_completo LIKE CONCAT('%', p_nome, '%'))
        GROUP BY 
            tc.nome_completo,
            tc.codigo_interno_colaborador,
            tcg.nome_completo,
            tch.cod_colaborador_superior,
            tco.tb_org_id,
            tv.mes,
            tv.ano,
            tsa.cod_status_apontamento,
            tca.tb_projeto_org_cod_projeto,
            COALESCE(tca.codigo_interno_colaborador_alteracao, tca.codigo_interno_colaborador_criacao)
        ORDER BY 
            nome_completo
    ) resultado_select
    LEFT JOIN tb_status_apontamento_grupo tsag ON resultado_select.cod_status_apontamento_periodo = tsag.cod_status_grupo
    LEFT JOIN tb_projeto_org tpo ON resultado_select.tb_projeto_org_cod_projeto = tpo.cod_projeto AND resultado_select.tb_org_id = tpo.tb_org_id
    WHERE
        (COALESCE(p_cod_status_apontamento, 0) = 0 OR resultado_select.cod_status_apontamento_periodo = p_cod_status_apontamento)
        AND (COALESCE(p_cod_projeto, '0') = '0' OR tpo.cod_projeto = p_cod_projeto);

    -- Verifica se o parâmetro `p_cod_colaborador_externo_aprovador` foi passado
    IF p_cod_colaborador_externo_aprovador IS NOT NULL THEN
        -- Criação da tabela temporária com joins para preenchimento de aprovador e código interno do aprovador
        CREATE TEMPORARY TABLE temp_result_filtrada AS
        SELECT 
			DISTINCT
            tr.colaborador_nome,
			tr.codigo_interno_colaborador,
			tr.ativo,
			tr.data_admissao,
			tr.data_inativacao,
			tr.nome_completo_gerente,
			tr.codigo_gerente,
			tr.cod_status_apontamento_periodo,
			tr.soma_horas,
			tr.tb_org_id,
			tr.mes,
			tr.ano,
			-- tr.codigo_interno_colaborador_alteracao,
			-- tr.codigo_interno_colaborador_criacao,
			tr.cod_projeto,
			tr.projeto,
            CASE 
                WHEN tr.cod_status_apontamento_periodo IN (2, 3) 
                    THEN tc_aprovador.codigo_interno_colaborador
                WHEN tr.cod_status_apontamento_periodo = 1 
                    THEN tco_ger.codigo_interno_colaborador
                ELSE NULL 
            END AS codigo_interno_aprovador
        FROM temp_result_para_filtro tr
        LEFT JOIN tb_projeto_gerente tpg ON tr.cod_projeto = tpg.cod_projeto 
            AND tr.tb_org_id = tpg.tb_org_id
        LEFT JOIN tb_colaborador_org tco_ger ON tpg.cod_colaborador_gerente = tco_ger.cod_colaborador_externo AND tr.tb_org_id = tco_ger.tb_org_id
		-- LEFT JOIN tb_colaborador tc_ger ON tco_ger.codigo_interno_colaborador = tc_ger.codigo_interno_colaborador 
        LEFT JOIN tb_colaborador_org tc_aprovador ON (tr.cod_status_apontamento_periodo = STATUS_APROVADO 
                OR tr.cod_status_apontamento_periodo = STATUS_REPROVADO) 
            AND (COALESCE(tr.codigo_interno_colaborador_alteracao, tr.codigo_interno_colaborador_criacao) = tc_aprovador.codigo_interno_colaborador)
            AND tr.tb_org_id = tc_aprovador.tb_org_id
		WHERE
			 (
            CASE 
                WHEN tr.cod_status_apontamento_periodo IN (2, 3) THEN tc_aprovador.codigo_interno_colaborador
                WHEN tr.cod_status_apontamento_periodo = 1 THEN tco_ger.codigo_interno_colaborador
                ELSE NULL 
            END
			) = (SELECT codigo_interno_colaborador 
             FROM tb_colaborador_org 
             WHERE cod_colaborador_externo = p_cod_colaborador_externo_aprovador  
             AND tb_org_id = p_tb_org_id);
    ELSE
        -- Criação da tabela temporária com o resultado de temp_result_para_filtro
        CREATE TEMPORARY TABLE temp_result_filtrada AS
        SELECT 
			tr.colaborador_nome,
			tr.codigo_interno_colaborador,
			tr.ativo,
			tr.data_admissao,
			tr.data_inativacao,
			tr.nome_completo_gerente,
			tr.codigo_gerente,
			tr.cod_status_apontamento_periodo,
			COALESCE(SUM(tr.soma_horas), 0) AS soma_horas,
			tr.tb_org_id,
			tr.mes,
			tr.ano,
			-- tr.codigo_interno_colaborador_alteracao,
			-- tr.codigo_interno_colaborador_criacao,
			tr.cod_projeto,
            tr.projeto,
            NULL AS codigo_interno_aprovador
        FROM
			temp_result_para_filtro tr
        GROUP BY
 			tr.colaborador_nome,
 			tr.codigo_interno_colaborador,
 			tr.ativo,
 			tr.data_admissao,
 			tr.data_inativacao,
 			tr.nome_completo_gerente,
 			tr.codigo_gerente,
 			tr.cod_status_apontamento_periodo,
 			tr.tb_org_id,
 			tr.mes,
 			tr.ano,
 			tr.cod_projeto
;
    END IF;
  
SET @quantidade_total_colaboradores_nao_apontados_filtro = (
   SELECT COUNT(codigo_interno_colaborador) FROM temp_result_filtrada where cod_status_apontamento_periodo = STATUS_NAO_APONTADO -- não apontado
);

SET @quantidade_total_colaboradores_filtro = (
   SELECT COUNT(DISTINCT codigo_interno_colaborador) FROM temp_result_filtrada
);

SET @quantidade_total_colaboradores_apontados_filtro = (
   SELECT COUNT(DISTINCT codigo_interno_colaborador) FROM temp_result_filtrada where cod_status_apontamento_periodo <> STATUS_NAO_APONTADO -- não apontado
);

-- caso tenha filtro de projeto e este projeto não permite apontamento global, precisa contar quantos colaboradores tem associados ao projeto
IF p_cod_projeto IS NOT NULL AND p_cod_projeto <> '' THEN
	-- Verificar se permite apontamento sem alocação
	SET @permite_apontamento = (
		SELECT permite_apont_sem_alocacao
		FROM tb_projeto_org
		WHERE tb_org_id = p_tb_org_id and cod_projeto = p_cod_projeto
	);
    IF @permite_apontamento = 1 THEN
        -- Se o código do projeto é informado e permite apontamento sem alocação
        SET @quantidade_total_colaboradores = @quantidade_total_colaboradores_filtro;
    ELSE
        -- Se o código do projeto é informado e não permite apontamento sem alocação
        SET @quantidade_total_colaboradores = (
            SELECT 
				COUNT(DISTINCT cod_colaborador)
            FROM
				tb_colaborador_projeto_org
            WHERE cod_projeto = p_cod_projeto AND tb_org_id = p_tb_org_id
        );
    END IF;
	ELSE
		-- Se o código do projeto não é informado
		SET @quantidade_total_colaboradores = @quantidade_total_colaboradores_filtro;
	END IF;
	
	SET @quantidade_total_nao_apontado = CONCAT(
		(@quantidade_total_colaboradores_nao_apontados_filtro),
		'/',
		@quantidade_total_colaboradores
	);

-- Consultas finais

	-- Seleção dos resultados principais com paginação
    -- temp_result_filtrada
	SELECT *
	FROM temp_result_filtrada
	ORDER BY 
		colaborador_nome
	LIMIT 
		p_limite 
	OFFSET 
		p_offset;
        
	-- #temp_aprovadores_filtrado
	SELECT 
		taf.*,
		tc.nome_completo as nome_aprovador
	FROM
		temp_aprovadores_filtrado taf
    JOIN
		tb_colaborador tc on taf.codigo_interno_aprovador = tc.codigo_interno_colaborador;
	
    -- #temp_result_filtrada_count_2
	SELECT COUNT(DISTINCT codigo_interno_colaborador) AS total, SUM(soma_horas) AS soma_horas 
	FROM temp_result_filtrada;
	
	-- Atualizar as somas de horas usando as variáveis nomeadas
    -- #temp_result_filtrada_count_2
	SELECT
		@quantidade_total_colaboradores AS quantidade_total_colaboradores,
		SUM(CASE WHEN cod_status_apontamento_periodo = STATUS_PENDENTE 
				OR cod_status_apontamento_periodo = STATUS_APROVADO THEN soma_horas ELSE 0 END) AS soma_horas_lancadas,
		SUM(CASE WHEN cod_status_apontamento_periodo = STATUS_APROVADO THEN soma_horas ELSE 0 END) AS soma_horas_aprovadas,
		SUM(CASE WHEN cod_status_apontamento_periodo = STATUS_PENDENTE THEN soma_horas ELSE 0 END) AS soma_horas_pendentes,
		SUM(CASE WHEN cod_status_apontamento_periodo = STATUS_REPROVADO THEN soma_horas ELSE 0 END) AS soma_horas_reprovadas,
		@quantidade_total_nao_apontado AS quantidade_total_nao_apontado
	FROM temp_result_filtrada;
	
	
	 
	-- Limpeza da tabela temporária
	DROP TEMPORARY TABLE IF EXISTS temp_result;
	DROP TEMPORARY TABLE IF EXISTS temp_aprovadores_para_filtro;
    DROP TEMPORARY TABLE IF EXISTS temp_aprovadores_filtrado;
	DROP TEMPORARY TABLE IF EXISTS temp_result_para_filtro;
    DROP TEMPORARY TABLE IF EXISTS temp_result_filtrada;
END//
DELIMITER ;
