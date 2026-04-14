CREATE  PROCEDURE `spr_estatisticas_etnia`(IN p_org_id INT)
BEGIN
	DECLARE v_total_colaboradores INT;

	-- Criar tabela temporária para armazenar os resultados da parte dos gestores
    CREATE TEMPORARY TABLE IF NOT EXISTS tb_temp_gerentes (
        cpf VARCHAR(255),
        org_id INT
    );
    
    -- Criar tabela temporária para as estatísticas
    CREATE TEMPORARY TABLE IF NOT EXISTS tb_temp_estatistica (
        descricao_retorno_estatistica VARCHAR(255),
		chave_estatistica VARCHAR(255)
    );
    
    -- Criar tabela temporária para armazenar os resultados finais
	CREATE TEMPORARY TABLE IF NOT EXISTS tb_temp_resultados_estatistica (
        descricao VARCHAR(255),
        quantidade_total INT,
        quantidade_gestores INT,
        quantidade_nao_gestores INT,
		percentual DECIMAL(10,2),
        org_id INT
    );
   
    -- Dropar os dados antigos se a tabela já existir
	TRUNCATE TABLE tb_temp_gerentes;
	TRUNCATE TABLE tb_temp_estatistica;
    TRUNCATE TABLE tb_temp_resultados_estatistica;

    -- Preencher tabela temporária com a informação sobre os gestores
    INSERT INTO tb_temp_gerentes
    SELECT 
        cpf,
        org_id
    FROM 
        vw_gestores_estatisticas_org wgeo
	WHERE 
		wgeo.org_id = p_org_id;
     
	-- Preencher tabela temporária com as estatisticas
    INSERT INTO tb_temp_estatistica (descricao_retorno_estatistica, chave_estatistica) VALUES
        ('Preta','Preta'),
        ('Branca','Branca'),
        ('Amarela','Amarela'),
        ('Parda','Parda'),
        ('Prefiro não responder','Prefiro não responder'),
        ('Indígena','Indígena'),
		('Sem resposta', 'Sem resposta');

    -- Calcular a quantidade de colaboradores por estatistica e a quantidade de gestores
    INSERT INTO tb_temp_resultados_estatistica
    SELECT 
        tte.descricao_retorno_estatistica,
        COUNT(tco.tb_colaborador_cpf) AS quantidade_total,
        COUNT(tg.cpf) AS quantidade_gestores,
        COUNT(tco.tb_colaborador_cpf) - COUNT(tg.cpf) AS quantidade_nao_gestores,
		0 AS percentual,
        torg.id AS org_id 
    FROM
        tb_temp_estatistica AS tte
    JOIN tb_org AS torg ON torg.id = p_org_id
    LEFT JOIN tb_colaborador tc ON tte.chave_estatistica = tc.etnia 
										AND tc.candidato = 0 AND tc.ativo = 1
    LEFT JOIN tb_colaborador_org tco ON tc.cpf = tco.tb_colaborador_cpf AND torg.id = tco.tb_org_id AND tco.ativo = 1
    LEFT JOIN tb_temp_gerentes tg ON tc.cpf = tg.cpf AND tg.org_id = torg.id
    WHERE tte.chave_estatistica <> 'Sem resposta'
    GROUP BY tte.descricao_retorno_estatistica, torg.id;

    -- Calcular a quantidade de colaboradores sem resposta e a quantidade de gestores sem resposta
    INSERT INTO tb_temp_resultados_estatistica
	SELECT 
		tte.descricao_retorno_estatistica,
		COUNT(tco.tb_colaborador_cpf) AS quantidade_total,
		COUNT(tg.cpf) AS quantidade_gestores,
		COUNT(tco.tb_colaborador_cpf) - COUNT(tg.cpf) AS quantidade_nao_gestores,
		0 AS percentual,
		torg.id AS org_id 
	FROM
		tb_temp_estatistica AS tte
	JOIN tb_org AS torg ON torg.id = p_org_id
	LEFT JOIN tb_colaborador tc ON (tc.etnia IS NULL OR tc.etnia = '' OR tc.etnia = 'null' OR tc.etnia = 'Sem informação') 
										AND tc.candidato = 0 AND tc.ativo = 1
	LEFT JOIN tb_colaborador_org tco ON tc.cpf = tco.tb_colaborador_cpf AND torg.id = tco.tb_org_id AND tco.ativo = 1
	LEFT JOIN tb_temp_gerentes tg ON tc.cpf = tg.cpf AND tg.org_id = torg.id
	WHERE tte.chave_estatistica = 'Sem resposta'
	GROUP BY tte.descricao_retorno_estatistica, torg.id;
    
    -- Calcular quantidade total de colaboradores por org
	SELECT SUM(quantidade_total) INTO v_total_colaboradores
    FROM tb_temp_resultados_estatistica
    WHERE org_id = p_org_id;

    -- Atualizar o percentual para cada linha
    UPDATE tb_temp_resultados_estatistica
    SET percentual = fn_calcular_percentual(quantidade_total, v_total_colaboradores)
    WHERE org_id = p_org_id;

    -- Selecionar os resultados
    SELECT * FROM tb_temp_resultados_estatistica
    ORDER BY descricao, org_id;
    
    -- Remover tabelas temporárias
    DROP TEMPORARY TABLE IF EXISTS tb_temp_gerentes;
    DROP TEMPORARY TABLE IF EXISTS tb_temp_resultados_estatistica;
    DROP TEMPORARY TABLE IF EXISTS tb_temp_estatistica;
END