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

    -- Criação da tabela temporária
    CREATE TEMPORARY TABLE temp_result AS
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
        CASE WHEN
            tpo.projeto IS NULL OR tpo.projeto = '-'
        THEN
            tpo.cod_projeto 
        ELSE
            CONCAT(tpo.cod_projeto, ' - ', tpo.projeto)
        END AS projeto,
        tc_aprovador.nome_completo as aprovador
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
            AND (COALESCE(p_nome,'') = '' OR tc.nome_completo LIKE CONCAT('%',p_nome,'%'))
        GROUP BY 
            tc.nome_completo,
            tc.codigo_interno_colaborador,
            tcg.nome_completo,
            tch.cod_colaborador_superior,
            tco.tb_org_id,
            tv.mes,
            tv.ano,
            tsa.cod_status_apontamento,
            tca.tb_projeto_org_cod_projeto
        ORDER BY 
            nome_completo
    ) resultado_select
    LEFT JOIN tb_status_apontamento_grupo tsag ON resultado_select.cod_status_apontamento_periodo = tsag.cod_status_grupo
    LEFT JOIN tb_projeto_org tpo ON resultado_select.tb_projeto_org_cod_projeto = tpo.cod_projeto AND resultado_select.tb_org_id = tpo.tb_org_id
    LEFT JOIN tb_colaborador tc_aprovador ON (resultado_select.cod_status_apontamento_periodo = STATUS_APROVADO 
		   		   OR resultado_select.cod_status_apontamento_periodo = STATUS_REPROVADO) 
                   AND (COALESCE(resultado_select.codigo_interno_colaborador_alteracao,resultado_select.codigo_interno_colaborador_criacao ) = tc_aprovador.codigo_interno_colaborador)
    WHERE 
        (COALESCE(p_cod_status_apontamento, 0) = 0 OR resultado_select.cod_status_apontamento_periodo = p_cod_status_apontamento)
        AND (COALESCE(p_cod_projeto, '0') = '0' OR resultado_select.tb_projeto_org_cod_projeto = p_cod_projeto)
        AND (COALESCE(p_cod_colaborador_externo_aprovador, '0') = '0' OR (resultado_select.cod_status_apontamento_periodo <> STATUS_PENDENTE 
				AND resultado_select.codigo_interno_colaborador_alteracao = (select codigo_interno_colaborador from tb_colaborador_org
																			 where cod_colaborador_externo = p_cod_colaborador_externo_aprovador
                                                                            and tb_org_id = p_tb_org_id)));
  
SET @quantidade_total_colaboradores_nao_apontados_filtro = (
   SELECT COUNT(codigo_interno_colaborador) FROM temp_result where cod_status_apontamento_periodo = STATUS_NAO_APONTADO -- não apontado
);

SET @quantidade_total_colaboradores_filtro = (
   SELECT COUNT(DISTINCT codigo_interno_colaborador) FROM temp_result
);

SET @quantidade_total_colaboradores_apontados_filtro = (
   SELECT COUNT(DISTINCT codigo_interno_colaborador) FROM temp_result where cod_status_apontamento_periodo <> STATUS_NAO_APONTADO -- não apontado
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
    SELECT *
    FROM temp_result
    ORDER BY 
        colaborador_nome
    LIMIT 
        p_limite 
    OFFSET 
        p_offset;

  SELECT COUNT(DISTINCT codigo_interno_colaborador) as total, SUM(soma_horas) as soma_horas 
    FROM temp_result;
    
-- Atualizar as somas de horas usando as variáveis nomeadas
SELECT
    @quantidade_total_colaboradores AS quantidade_total_colaboradores,
    SUM(CASE WHEN cod_status_apontamento_periodo = STATUS_PENDENTE 
            OR cod_status_apontamento_periodo = STATUS_APROVADO THEN soma_horas ELSE 0 END) AS soma_horas_lancadas,
    SUM(CASE WHEN cod_status_apontamento_periodo = STATUS_APROVADO THEN soma_horas ELSE 0 END) AS soma_horas_aprovadas,
    SUM(CASE WHEN cod_status_apontamento_periodo = STATUS_PENDENTE THEN soma_horas ELSE 0 END) AS soma_horas_pendentes,
    SUM(CASE WHEN cod_status_apontamento_periodo = STATUS_REPROVADO THEN soma_horas ELSE 0 END) AS soma_horas_reprovadas,
    @quantidade_total_nao_apontado as quantidade_total_nao_apontado
FROM temp_result;

-- Limpeza da tabela temporária
DROP TEMPORARY TABLE temp_result;
END