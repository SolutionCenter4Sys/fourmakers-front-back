DROP PROCEDURE IF EXISTS spr_estatisticas_escolaridade;
DELIMITER $$
$$
CREATE PROCEDURE `spr_estatisticas_escolaridade`(IN p_org_id INT)
BEGIN
	DECLARE v_total_colaboradores INT;

     -- Criar tabela temporária para armazenar os resultados da parte dos gestores
    CREATE TEMPORARY TABLE IF NOT EXISTS tb_temp_gerentes (
        codigo_interno_colaborador VARCHAR(255),
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
        codigo_interno_colaborador,
        org_id
    FROM 
        vw_gestores_estatisticas_org wgeo
	WHERE 
		wgeo.org_id = p_org_id;
    
    -- Preencher tabela temporária com as estatisticas
    INSERT INTO tb_temp_estatistica (descricao_retorno_estatistica, chave_estatistica)
    VALUES
		('Mestrado ou Doutorado incompleto ou cursando', 'Mestrado ou Doutorado cursando'),
		('Mestrado ou Doutorado incompleto ou cursando', 'Mestrado ou Doutorado incompleto'),
		('Mestrado ou Doutorado completo', 'Mestrado ou Doutorado completo'),
		('Ensino fundamental completo ou menos', 'Ensino fundamental completo ou menos'),
		('Ensino médio completo ou menos', 'Ensino médio completo ou menos'),
		('Ensino superior incompleto ou cursando', 'Ensino superior incompleto'),
		('Ensino superior incompleto ou cursando', 'Ensino superior cursando'),
		('Ensino superior completo', 'Ensino superior completo'),
		('Pós-graduação incompleto ou cursando', 'Pós-graduação incompleto'),
		('Pós-graduação incompleto ou cursando', 'Pós-graduação cursando'),
		('Pós-graduação completo', 'Pós-graduação completo'),
		('Nenhum', 'Nenhum'),
        ('Sem resposta', 'Sem resposta');

    -- Calcular a quantidade de colaboradores por estatistica e a quantidade de gestores
    INSERT INTO tb_temp_resultados_estatistica
    SELECT 
        tte.descricao_retorno_estatistica,
        COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
        COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
        COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
        0 AS percentual,
        torg.id AS org_id 
    FROM
        tb_temp_estatistica AS tte
    JOIN tb_org AS torg ON torg.id = p_org_id
    LEFT JOIN tb_colaborador tc ON tte.chave_estatistica = tc.escolaridade 
    									AND tc.candidato = 0 AND tc.ativo = 1
    LEFT JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND torg.id = tco.tb_org_id AND tco.ativo = 1
    LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
    WHERE tte.chave_estatistica <> 'Sem resposta'
    GROUP BY tte.descricao_retorno_estatistica, torg.id;

    -- Calcular a quantidade de colaboradores sem resposta e a quantidade de gestores sem resposta
	INSERT INTO tb_temp_resultados_estatistica
    SELECT 
		tte.descricao_retorno_estatistica,
		COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
		COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
		COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
        0 AS percentual,
		torg.id AS org_id 
	FROM
		  tb_temp_estatistica AS tte
	JOIN tb_org AS torg ON torg.id = p_org_id
	LEFT JOIN tb_colaborador tc ON (tc.escolaridade IS NULL OR tc.escolaridade = '')
										AND tc.candidato = 0 AND tc.ativo = 1
	LEFT JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND torg.id = tco.tb_org_id AND tco.ativo = 1
	LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
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
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_estatisticas_etnia;
DELIMITER $$
$$
CREATE PROCEDURE `spr_estatisticas_etnia`(IN p_org_id INT)
BEGIN
	DECLARE v_total_colaboradores INT;

	-- Criar tabela temporária para armazenar os resultados da parte dos gestores
    CREATE TEMPORARY TABLE IF NOT EXISTS tb_temp_gerentes (
        codigo_interno_colaborador VARCHAR(255),
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
        codigo_interno_colaborador,
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
        COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
        COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
        COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
		0 AS percentual,
        torg.id AS org_id 
    FROM
        tb_temp_estatistica AS tte
    JOIN tb_org AS torg ON torg.id = p_org_id
    LEFT JOIN tb_colaborador tc ON tte.chave_estatistica = tc.etnia 
										AND tc.candidato = 0 AND tc.ativo = 1
    LEFT JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND torg.id = tco.tb_org_id AND tco.ativo = 1
    LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
    WHERE tte.chave_estatistica <> 'Sem resposta'
    GROUP BY tte.descricao_retorno_estatistica, torg.id;

    -- Calcular a quantidade de colaboradores sem resposta e a quantidade de gestores sem resposta
    INSERT INTO tb_temp_resultados_estatistica
	SELECT 
		tte.descricao_retorno_estatistica,
		COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
		COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
		COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
		0 AS percentual,
		torg.id AS org_id 
	FROM
		tb_temp_estatistica AS tte
	JOIN tb_org AS torg ON torg.id = p_org_id
	LEFT JOIN tb_colaborador tc ON (tc.etnia IS NULL OR tc.etnia = '' OR tc.etnia = 'null' OR tc.etnia = 'Sem informação') 
										AND tc.candidato = 0 AND tc.ativo = 1
	LEFT JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND torg.id = tco.tb_org_id AND tco.ativo = 1
	LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
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
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_estatisticas_genero;
DELIMITER $$
$$
CREATE PROCEDURE `spr_estatisticas_genero`(IN p_org_id INT)
BEGIN
	DECLARE v_total_colaboradores INT;

	-- Criar tabela temporária para armazenar os resultados da parte dos gestores
    CREATE TEMPORARY TABLE IF NOT EXISTS tb_temp_gerentes (
        codigo_interno_colaborador VARCHAR(255),
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
        codigo_interno_colaborador,
        org_id
    FROM 
        vw_gestores_estatisticas_org wgeo
	WHERE 
		wgeo.org_id = p_org_id;
     
	-- Preencher tabela temporária com as estatisticas
    INSERT INTO tb_temp_estatistica (descricao_retorno_estatistica, chave_estatistica) VALUES
        ('Feminino','Feminino'),
		('Masculino','Masculino'),
        ('Não binário','Não binário'),
        ('Homem cisgênero','Homem cisgênero'),
        ('Agênero','Agênero'),
        ('Transgênero','Transgênero'),
        ('Mulher cisgênero','Mulher cisgênero'),
        ('Prefiro não responder','Prefiro não responder'),
		('Sem resposta', 'Sem resposta');

    -- Calcular a quantidade de colaboradores por estatistica e a quantidade de gestores
    INSERT INTO tb_temp_resultados_estatistica
    SELECT 
        tte.descricao_retorno_estatistica,
        COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
        COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
        COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
		0 AS percentual,
        torg.id AS org_id 
    FROM
        tb_temp_estatistica AS tte
    JOIN tb_org AS torg ON torg.id = p_org_id
    LEFT JOIN tb_colaborador tc ON tte.chave_estatistica = tc.genero 
										AND tc.candidato = 0 AND tc.ativo = 1
    LEFT JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND torg.id = tco.tb_org_id AND tco.ativo = 1
    LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
    WHERE tte.chave_estatistica <> 'Sem resposta'
    GROUP BY tte.descricao_retorno_estatistica, torg.id;

    -- Calcular a quantidade de colaboradores sem resposta e a quantidade de gestores sem resposta
    INSERT INTO tb_temp_resultados_estatistica
	SELECT 
		tte.descricao_retorno_estatistica,
		COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
		COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
		COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
		0 AS percentual,
		torg.id AS org_id 
	FROM
		tb_temp_estatistica AS tte
	JOIN tb_org AS torg ON torg.id = p_org_id
	LEFT JOIN tb_colaborador tc ON (tc.genero IS NULL OR tc.genero = '') 
										AND tc.candidato = 0 AND tc.ativo = 1
	LEFT JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND torg.id = tco.tb_org_id AND tco.ativo = 1
	LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
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
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_estatisticas_idade;
DELIMITER $$
$$
CREATE PROCEDURE `spr_estatisticas_idade`(IN p_org_id INT)
BEGIN
	DECLARE v_total_colaboradores INT;

	-- Criar tabela temporária para armazenar os resultados da parte dos gestores
    CREATE TEMPORARY TABLE IF NOT EXISTS tb_temp_gerentes (
        codigo_interno_colaborador VARCHAR(255),
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
        codigo_interno_colaborador,
        org_id
    FROM 
        vw_gestores_estatisticas_org wgeo
	WHERE 
		wgeo.org_id = p_org_id;
     
	-- Preencher tabela temporária com as estatisticas
    INSERT INTO tb_temp_estatistica (descricao_retorno_estatistica, chave_estatistica) VALUES
        ('Idade 25 ou menos','idade_25_ou_menos'),
        ('Idade 26 a 34','idade_26_34'),
        ('Idade 35 a 44','idade_35_44'),
        ('Idade 45 a 54','idade_45_54'),
        ('Idade 55 ou mais','idade_55_mais'),
        ('Sem resposta','Sem_Resposta');

    -- Calcular a quantidade de colaboradores por estatistica e a quantidade de gestores
    INSERT INTO tb_temp_resultados_estatistica
    SELECT 
        tte.descricao_retorno_estatistica,
        COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
        COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
        COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
		0 AS percentual,
        torg.id AS org_id 
    FROM
        tb_temp_estatistica AS tte
    JOIN tb_org AS torg ON torg.id = p_org_id
    LEFT JOIN tb_colaborador tc ON tte.chave_estatistica = fn_estatisticas_calcular_idade(tc.data_nascimento) AND tc.candidato = 0 AND tc.ativo = 1
    LEFT JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND torg.id = tco.tb_org_id AND tco.ativo = 1
    LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
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
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_estatisticas_orientacao_sexual;
DELIMITER $$
$$
CREATE PROCEDURE `spr_estatisticas_orientacao_sexual`(IN p_org_id INT)
BEGIN
	DECLARE v_total_colaboradores INT;

	-- Criar tabela temporária para armazenar os resultados da parte dos gestores
    CREATE TEMPORARY TABLE IF NOT EXISTS tb_temp_gerentes (
        codigo_interno_colaborador VARCHAR(255),
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
        codigo_interno_colaborador,
        org_id
    FROM 
        vw_gestores_estatisticas_org wgeo
	WHERE 
		wgeo.org_id = p_org_id;
     
	-- Preencher tabela temporária com as estatisticas
    INSERT INTO tb_temp_estatistica (descricao_retorno_estatistica, chave_estatistica) VALUES
        ('Assexual','Assexual'),
        ('Bissexual','Bissexual'),
        ('Heterossexual','Heterossexual'),
        ('Homossexual','Homossexual'),
        ('Prefiro não responder','Prefiro não responder'),
        ('Sem informação','Sem informação'),
		('Sem resposta', 'Sem resposta');

    -- Calcular a quantidade de colaboradores por estatistica e a quantidade de gestores
    INSERT INTO tb_temp_resultados_estatistica
    SELECT 
        tte.descricao_retorno_estatistica,
        COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
        COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
        COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
		0 AS percentual,
        torg.id AS org_id 
    FROM
        tb_temp_estatistica AS tte
    JOIN tb_org AS torg ON torg.id = p_org_id
    LEFT JOIN tb_colaborador tc ON tte.chave_estatistica = tc.orientacao_sexual 
										AND tc.candidato = 0 AND tc.ativo = 1
    LEFT JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND torg.id = tco.tb_org_id AND tco.ativo = 1
    LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
    WHERE tte.chave_estatistica <> 'Sem resposta'
    GROUP BY tte.descricao_retorno_estatistica, torg.id;

    -- Calcular a quantidade de colaboradores sem resposta e a quantidade de gestores sem resposta
    INSERT INTO tb_temp_resultados_estatistica
	SELECT 
		tte.descricao_retorno_estatistica,
		COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
		COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
		COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
		0 AS percentual,
		torg.id AS org_id 
	FROM
		tb_temp_estatistica AS tte
	JOIN tb_org AS torg ON torg.id = p_org_id
	LEFT JOIN tb_colaborador tc ON (tc.orientacao_sexual IS NULL OR tc.orientacao_sexual = '' OR tc.orientacao_sexual = 'null' OR tc.orientacao_sexual = 'Sem informação') 
										AND tc.candidato = 0 AND tc.ativo = 1
	LEFT JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND torg.id = tco.tb_org_id AND tco.ativo = 1
	LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
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
END$$
DELIMITER ;


DROP PROCEDURE IF EXISTS spr_estatisticas_tempo_servico;
DELIMITER $$
$$
CREATE PROCEDURE `spr_estatisticas_tempo_servico`(IN p_org_id INT)
BEGIN
	DECLARE v_total_colaboradores INT;

	-- Criar tabela temporária para armazenar os resultados da parte dos gestores
    CREATE TEMPORARY TABLE IF NOT EXISTS tb_temp_gerentes (
        codigo_interno_colaborador VARCHAR(255),
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
        codigo_interno_colaborador,
        org_id
    FROM 
        vw_gestores_estatisticas_org wgeo
	WHERE 
		wgeo.org_id = p_org_id;
     
	-- Preencher tabela temporária com as estatisticas
    INSERT INTO tb_temp_estatistica (descricao_retorno_estatistica, chave_estatistica) VALUES
        ('Até 1 ano','ate_1_ano'),
        ('Entre 1 e 2 anos','entre_1_e_2_anos'),
        ('Entre 3 e 5 anos','entre_3_e_5_anos'),
        ('Entre 6 e 10 anos','entre_6_e_10_anos'),
        ('Entre 11 e 15 anos','entre_11_e_15_anos'),
        ('Entre 16 e 20 anos','entre_16_e_20_anos'),
        ('Mais de 20 anos','mais_de_20_anos'),
        ('Sem resposta','sem_resposta');

    -- Calcular a quantidade de colaboradores por estatistica e a quantidade de gestores
    INSERT INTO tb_temp_resultados_estatistica
    SELECT 
        tte.descricao_retorno_estatistica,
        COUNT(tco.codigo_interno_colaborador) AS quantidade_total,
        COUNT(tg.codigo_interno_colaborador) AS quantidade_gestores,
        COUNT(tco.codigo_interno_colaborador) - COUNT(tg.codigo_interno_colaborador) AS quantidade_nao_gestores,
		0 AS percentual,
        torg.id AS org_id 
    FROM
        tb_temp_estatistica AS tte
    JOIN tb_org AS torg ON torg.id = p_org_id
    LEFT JOIN tb_colaborador_org tco ON tte.chave_estatistica = fn_estatisticas_calcular_tempo_servico(tco.data_admissao)
											AND torg.id = tco.tb_org_id AND tco.ativo = 1
    LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador AND tc.candidato = 0 AND tc.ativo = 1
    LEFT JOIN tb_temp_gerentes tg ON tc.codigo_interno_colaborador = tg.codigo_interno_colaborador AND tg.org_id = torg.id
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
END$$
DELIMITER ;


DROP PROCEDURE IF EXISTS spr_extracao_mapa;

-- procedure não deve mais ser criada, nome mudou para 
-- spr_rpt_extracao_mapa e já está sendo criada neste script

DROP PROCEDURE IF EXISTS spr_get_alocacao_calculo_mensal_filtro;
DELIMITER $$
$$
CREATE PROCEDURE `spr_get_alocacao_calculo_mensal_filtro`(
    IN mes_inicial INT, 
    IN ano_inicial INT, 
    IN mes_final INT, 
    IN ano_final INT, 
	IN eh_tbd INT,
    -- IN ativo INT,
    IN status VARCHAR(50),
    IN idioma VARCHAR(100),
    IN hard_skill VARCHAR(100),
	IN colaborador_ou_tbd VARCHAR (100), 
    IN gestor VARCHAR(100),  
    IN cod_diretoria VARCHAR(100),  
    IN nome_diretoria VARCHAR(100), 
    IN cursor_int INT, 
    IN limite_int INT,
    IN org_id INT
)
BEGIN
    DECLARE ano_atual INT DEFAULT ano_inicial;
    DECLARE mes_atual INT DEFAULT mes_inicial;

    DROP TEMPORARY TABLE IF EXISTS meses;
    CREATE TEMPORARY TABLE meses (
        mes INT,
        ano INT,
		INDEX idx_mes_ano (mes, ano)
    );
    
    main_loop: WHILE ano_atual <= ano_final DO
        WHILE mes_atual <= 12 DO
            IF (ano_atual = ano_final AND mes_atual > mes_final) THEN
                LEAVE main_loop;
            END IF;
            INSERT INTO meses (mes, ano) VALUES (mes_atual, ano_atual);
            SET mes_atual = mes_atual + 1;
        END WHILE;
        SET mes_atual = 1;
        SET ano_atual = ano_atual + 1;
    END WHILE;

	DROP TEMPORARY TABLE IF EXISTS temp_status_colaboradores;
	CREATE TEMPORARY TABLE temp_status_colaboradores (
		codigo_colaborador INT,
		cod_tbd_alocado INT,
		status_colaborador_periodo_alocacao VARCHAR(255),
		tb_org_id INT,
        INDEX idx_temp_status (codigo_colaborador, cod_tbd_alocado, status_colaborador_periodo_alocacao, tb_org_id)
	);

	-- Preencher a tabela temporária apenas se status não for nulo, vazio ou "Todos"
	IF (status IS NOT NULL AND status <> '' AND status <> 'Todos') THEN
		INSERT INTO temp_status_colaboradores
		SELECT DISTINCT
			tcpacm.codigo_colaborador,
			tcpacm.cod_tbd_alocado,
			tcpacm.status_colaborador_periodo_alocacao,
			tcpacm.tb_org_id
		FROM
			tb_colaborador_periodo_alocacao_calculo_mensal tcpacm
		WHERE
			((tcpacm.ano > ano_inicial OR (tcpacm.ano = ano_inicial AND tcpacm.mes >= mes_inicial))
			AND (tcpacm.ano < ano_final OR (tcpacm.ano = ano_final AND tcpacm.mes <= mes_final)))
			AND tcpacm.tb_org_id = org_id;
	END IF;
		
    -- Criar tabela temporária de filtrados_com_status
    DROP TEMPORARY TABLE IF EXISTS filtrados_com_status;
    CREATE TEMPORARY TABLE filtrados_com_status AS
    SELECT DISTINCT 
        ac.codigo_colaborador, 
        ac.nome,
        ac.codigo_interno_colaborador,
        ac.eh_tbd,
        ac.tb_org_id,
        ac.ativo,
        ac.cod_diretoria,
        ac.diretoria,
        GROUP_CONCAT(DISTINCT ti.descricao ORDER BY ti.descricao SEPARATOR ' | ') AS idiomas,
        GROUP_CONCAT(DISTINCT tc.descricao ORDER BY tc.descricao SEPARATOR ' | ') AS hard_skills,
        GROUP_CONCAT(DISTINCT vgct.cod_colaborador_superior ORDER BY vgct.cod_colaborador_superior SEPARATOR ' | ') AS gestores,
        GROUP_CONCAT(DISTINCT vgct.nome_gestor_adm ORDER BY vgct.nome_gestor_adm SEPARATOR ' | ') AS gestores_nome
    FROM vw_alocacacao_recurso_calculo_mensal ac
    LEFT JOIN tb_colaborador_idioma tci ON tci.codigo_interno_colaborador = ac.codigo_interno_colaborador
    LEFT JOIN tb_idioma ti ON tci.idioma_id = ti.id
    LEFT JOIN tb_colaborador_competencia tcc ON tcc.codigo_interno_colaborador = ac.codigo_interno_colaborador
    LEFT JOIN tb_competencia tc ON tcc.competencia_id = tc.id
    LEFT JOIN vw_gestores_colaborador_tbd vgct ON (vgct.cod_colaborador_externo = ac.codigo_colaborador AND vgct.tb_org_id = ac.tb_org_id AND ac.eh_tbd = vgct.eh_tbd) 
    WHERE 
     (status IS NULL OR status = '' OR status = 'Todos' OR EXISTS 
        (
            SELECT 1 FROM temp_status_colaboradores temp
            WHERE temp.status_colaborador_periodo_alocacao = status 
                AND ((ac.eh_tbd = 1 AND temp.cod_tbd_alocado = ac.codigo_colaborador)
                OR (ac.eh_tbd = 0 AND temp.codigo_colaborador = ac.codigo_colaborador))
                AND ac.tb_org_id = temp.tb_org_id
        )
    ) AND
	(cod_diretoria IS NULL OR cod_diretoria = '' OR cod_diretoria = '0' OR ac.cod_diretoria = cod_diretoria)
    AND (nome_diretoria IS NULL OR nome_diretoria = '' OR ac.diretoria LIKE CONCAT('%', nome_diretoria, '%'))
    AND (idioma IS NULL OR idioma = '' OR EXISTS (
            SELECT 1 FROM tb_colaborador_idioma tci JOIN tb_idioma ti ON tci.idioma_id = ti.id
            WHERE tci.codigo_interno_colaborador = ac.codigo_interno_colaborador AND ti.descricao LIKE CONCAT('%', idioma, '%')
        ))
    AND (hard_skill IS NULL OR hard_skill = '' OR EXISTS (
            SELECT 1 FROM tb_colaborador_competencia tcc JOIN tb_competencia tc ON tcc.competencia_id = tc.id
            WHERE tcc.codigo_interno_colaborador = ac.codigo_interno_colaborador AND tc.descricao LIKE CONCAT('%', hard_skill, '%')
        ))
    AND (gestor IS NULL OR gestor = '' 
            OR (
                EXISTS 
                (
                    SELECT 1 FROM vw_gestores_colaborador_tbd vgct
                    WHERE vgct.cod_colaborador_externo = ac.codigo_colaborador AND vgct.tb_org_id = ac.tb_org_id AND ac.eh_tbd = 0
                    AND (vgct.nome_gestor_adm LIKE CONCAT('%', gestor, '%') OR gestor = vgct.cod_colaborador_superior)
                )
                 OR (
                 EXISTS
                 (
                    SELECT 1 FROM vw_gestores_colaborador_tbd vgct
                    WHERE vgct.cod_colaborador_externo = ac.codigo_colaborador AND vgct.tb_org_id = ac.tb_org_id AND ac.eh_tbd = 1
                    AND (vgct.nome_gestor_adm LIKE CONCAT('%', gestor, '%') OR gestor = vgct.cod_colaborador_superior)
                 )
               )
            )
        )
    AND (colaborador_ou_tbd IS NULL OR colaborador_ou_tbd = '' OR ac.codigo_colaborador = colaborador_ou_tbd OR ac.nome LIKE CONCAT('%', colaborador_ou_tbd, '%'))
    AND (eh_tbd IS NULL OR eh_tbd = '' OR ac.eh_tbd = eh_tbd)
	-- AND (ativo IS NULL OR ac.ativo = ativo)
    AND ac.ativo = 1
    AND ac.tb_org_id = org_id
    GROUP BY ac.codigo_colaborador, ac.nome,
			 ac.codigo_interno_colaborador,
		 	 ac.eh_tbd,
			 ac.tb_org_id,
			 ac.ativo,
			 ac.cod_diretoria,
			 ac.diretoria 
    ORDER BY ac.nome
    LIMIT limite_int OFFSET cursor_int;

    -- Selecionar os dados finais
    SELECT 	cl.codigo_colaborador,
			cl.codigo_interno_colaborador, 
			cl.eh_tbd,
            cl.nome,
            cl.idiomas,
            cl.hard_skills,
            cl.gestores,
            cl.gestores_nome,
            cl.ativo,
            cl.tb_org_id,
            cl.cod_diretoria,
            cl.diretoria,
            cl.eh_tbd, 
			m.mes,
            m.ano,
			IFNULL(tcpacm.status_colaborador_periodo_alocacao, 'HorasPendentes') AS status_colaborador_periodo_alocacao,
			IFNULL(tcpacm.horas, 0) AS horas
    FROM filtrados_com_status cl
	JOIN
        meses m ON (m.ano > ano_inicial OR (m.ano = ano_inicial AND m.mes >= mes_inicial))
				 AND (m.ano < ano_final OR (m.ano = ano_final AND m.mes <= mes_final))
    LEFT JOIN 
        tb_colaborador_periodo_alocacao_calculo_mensal tcpacm 
			ON ((cl.eh_tbd = 1 AND tcpacm.cod_tbd_alocado = cl.codigo_colaborador)
				OR (cl.eh_tbd = 0 AND tcpacm.codigo_colaborador = cl.codigo_colaborador)) AND cl.tb_org_id = tcpacm.tb_org_id
        AND tcpacm.mes = m.mes AND tcpacm.ano = m.ano
    WHERE
        cl.tb_org_id = org_id
    ORDER BY cl.nome, m.ano, m.mes;

    -- Limpar as tabelas temporárias
	DROP TEMPORARY TABLE IF EXISTS meses;
	DROP TEMPORARY TABLE IF EXISTS filtrados_com_status;
	DROP TEMPORARY TABLE IF EXISTS temp_status_colaboradores;

END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_get_alocacao_colaborador_e_tbd;
DELIMITER $$
$$
CREATE  PROCEDURE `spr_get_alocacao_colaborador_e_tbd`(
    IN p_pesquisa VARCHAR(255),
    IN p_org_id INT,
    IN p_periodo_alocado_id INT,
    IN p_codigo_diretoria VARCHAR(255),
    IN p_codigo_departamento VARCHAR(255),
    IN p_codigo_gestor_adm VARCHAR(255),
    IN p_codigo_colab_ou_tbd VARCHAR(255),
    IN p_filtro_tipo_profissional INT,
    IN p_codigo_gestor_projeto VARCHAR(255),
    IN p_lista_codigo_clientes VARCHAR(1000),
    IN p_apenas_projetos_prioritarios BOOL,
    IN p_lista_codigo_projetos VARCHAR(1000),
    IN p_codigo_status_projeto INT,
    IN p_qtd_gerente_projeto_prioridade INT
)
BEGIN
	DECLARE done INT DEFAULT FALSE;
    DECLARE current_item VARCHAR(255);
    DECLARE split_index INT DEFAULT 1;
    
    CREATE TEMPORARY TABLE temp_clientes (
        cliente_codigo VARCHAR(255)
    );
    
    -- Preencher a tabela temporária com a lista de clientes
    WHILE CHAR_LENGTH(p_lista_codigo_clientes) > 0 DO
        SET split_index = LOCATE(',', p_lista_codigo_clientes);
        
        IF split_index = 0 THEN
            SET current_item = p_lista_codigo_clientes;
            SET p_lista_codigo_clientes = '';
        ELSE
            SET current_item = SUBSTRING(p_lista_codigo_clientes, 1, split_index - 1);
            SET p_lista_codigo_clientes = SUBSTRING(p_lista_codigo_clientes, split_index + 1);
        END IF;
        
        INSERT INTO temp_clientes (cliente_codigo) VALUES (current_item);
    END WHILE;
    
    CREATE TEMPORARY TABLE temp_projetos (
        projeto_codigo VARCHAR(255)
    );
    
    -- Preencher a tabela temporária com a lista de projetos
    WHILE CHAR_LENGTH(p_lista_codigo_projetos) > 0 DO
        SET split_index = LOCATE(',', p_lista_codigo_projetos);
        
        IF split_index = 0 THEN
            SET current_item = p_lista_codigo_projetos;
            SET p_lista_codigo_projetos = '';
        ELSE
            SET current_item = SUBSTRING(p_lista_codigo_projetos, 1, split_index - 1);
            SET p_lista_codigo_projetos = SUBSTRING(p_lista_codigo_projetos, split_index + 1);
        END IF;
        
        INSERT INTO temp_projetos (projeto_codigo) VALUES (current_item);
    END WHILE;

    SELECT 
    * 
    FROM (
        SELECT 
            tcpa.id AS periodo_alocado_id,
                tco.cod_diretoria,
                tco.diretoria,
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
            vgct_gestor.nome_gestor_adm,
            tcpa.codigo_interno_colaborador,
            tcpa.codigo_projeto,
            tpo.projeto,
            tpo.cod_cliente,
            tpo.cliente,
            tpo.status,
            tpg.cod_colaborador_gerente AS cod_gerente,
            tg.nome_completo AS nome_gerente,
                DATE_FORMAT(tcpa.data_inicio, '%d/%m/%Y') AS data_inicio,
                DATE_FORMAT(tcpa.data_fim, '%d/%m/%Y') AS data_fim,
			tcpa.quantidade_horas as quantidade_horas__ND,
			REPLACE(CAST(tcpa.quantidade_horas AS CHAR), '.', ',') AS quantidade_horas,
            tcpa.percentual,
            CASE 
                WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1
                ELSE 0
            END AS tbd,
			ROW_NUMBER() OVER (PARTITION BY tcpa.id ORDER BY tpg.cod_colaborador_gerente) AS gerente_projeto_prioridade__ND
        FROM 
            tb_colaborador_periodo_alocacao tcpa
        LEFT JOIN
            tb_colaborador_org tco ON tcpa.codigo_colaborador = tco.cod_colaborador_externo AND tco.tb_org_id = tcpa.tb_org_id
        LEFT JOIN
            tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador 
		LEFT JOIN
			tb_tbd_alocado ttbd ON tcpa.cod_tbd_alocado = ttbd.cod_tbd_alocado AND tcpa.tb_org_id = ttbd.tb_org_id
		LEFT JOIN
			vw_gestores_colaborador_tbd vgct_gestor ON (((CASE WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1 ELSE 0 END) = vgct_gestor.eh_tbd AND ttbd.cod_tbd_alocado = vgct_gestor.cod_colaborador_externo) 
																	OR ((CASE WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1 ELSE 0 END) = vgct_gestor.eh_tbd AND tco.cod_colaborador_externo = vgct_gestor.cod_colaborador_externo))
                                                                    AND tcpa.tb_org_id = vgct_gestor.tb_org_id
        LEFT JOIN
            tb_projeto_org tpo ON tcpa.codigo_projeto = tpo.cod_projeto AND tpo.tb_org_id = tcpa.tb_org_id
        LEFT JOIN 
            tb_projeto_gerente tpg ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tcpa.tb_org_id
        LEFT JOIN
            tb_colaborador_org tco_g ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo AND tco_g.tb_org_id = tcpa.tb_org_id
        LEFT JOIN
            tb_colaborador tg ON tco_g.codigo_interno_colaborador = tg.codigo_interno_colaborador
        WHERE
            -- Trazendo apenas ativos
            tcpa.ativo = 1 
            AND
            -- Filtrando apenas usuarios ativos
            (COALESCE(tcpa.cod_tbd_alocado,'')<>'' OR tco.ativo = 1)
            AND
            -- Filtrando pelo parâmetro: org Id
            tcpa.tb_org_id = p_org_id 
            AND
            -- Filtrando pelo parâmetro: periodo alocado
            (p_periodo_alocado_id IS NULL OR tcpa.id = p_periodo_alocado_id) 
            AND
            -- Filtrando pelo parâmetro: pesquisa
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
            AND
            -- Filtrando pelo parâmetro: p_codigo_diretoria
            (p_codigo_diretoria IS NULL OR tco.cod_diretoria = p_codigo_diretoria)
            AND
            -- Filtrando pelo parâmetro: p_codigo_departamento
            (p_codigo_departamento IS NULL OR tco.cod_departamento = p_codigo_departamento)
            AND
            -- Filtrando pelo parâmetro: p_codigo_gestor_adm
            (
                p_codigo_gestor_adm IS NULL 
                OR 
                (
                    vgct_gestor.cod_colaborador_superior = p_codigo_gestor_adm
                )
            )
            AND
            -- Filtrando pelo parâmetro: p_codigo_colab_ou_tbd
            (p_codigo_colab_ou_tbd IS NULL OR ((tcpa.cod_tbd_alocado IS NOT NULL AND ttbd.cod_tbd_alocado = p_codigo_colab_ou_tbd) OR (tcpa.cod_tbd_alocado IS NULL AND tco.cod_colaborador_externo = p_codigo_colab_ou_tbd)))
            AND
            -- Filtrando pelo parâmetro: p_filtro_tipo_profissional
            (
                (p_filtro_tipo_profissional = 0 OR p_filtro_tipo_profissional IS NULL)
                OR 
                (p_filtro_tipo_profissional = 1 AND tcpa.cod_tbd_alocado IS NULL)
                OR 
                (p_filtro_tipo_profissional = 2 AND tcpa.cod_tbd_alocado IS NOT NULL)
            )
            AND
            -- Filtrando pelo parâmetro: p_codigo_gestor_projeto
            (p_codigo_gestor_projeto IS NULL OR (tpg.cod_colaborador_gerente = p_codigo_gestor_projeto))
            AND 
            -- Filtrando pelo parâmetro: p_apenas_projetos_prioritarios
            ((p_apenas_projetos_prioritarios IS NULL OR p_apenas_projetos_prioritarios = false) OR (tpo.prioritario = p_apenas_projetos_prioritarios))
            AND
            -- Filtrando pelo parâmetro: p_lista_codigo_clientes
            (p_lista_codigo_clientes IS NULL OR (tpo.cod_cliente IN (SELECT cliente_codigo FROM temp_clientes)))
            AND
            -- Filtrando pelo parâmetro: p_lista_codigo_projetos
            (p_lista_codigo_projetos IS NULL OR (tpo.cod_projeto IN (SELECT projeto_codigo FROM temp_projetos)))
            AND
			-- Filtrando pelo parâmetro: p_codigo_status_projeto
			(p_codigo_status_projeto IS NULL OR tpo.cod_status = p_codigo_status_projeto)
            ORDER BY
                (CASE 
                    WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN ttbd.descricao
                    ELSE tc.nome_completo
                END), tcpa.data_fim
			
     ) AS subquery
	 WHERE
	 COALESCE(p_qtd_gerente_projeto_prioridade,0) = 0 OR subquery.gerente_projeto_prioridade__ND = p_qtd_gerente_projeto_prioridade;
	-- SELECT * FROM temp_clientes;
    -- SELECT * FROM temp_projetos;
	DROP TEMPORARY TABLE temp_clientes;
    DROP TEMPORARY TABLE temp_projetos;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_get_alocacao_colab_e_tbd;
DELIMITER $$
$$
CREATE PROCEDURE `spr_get_alocacao_colab_e_tbd`(
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
        END AS tbd,
        CASE 
            WHEN tca.cod_tbd_alocado IS NOT NULL THEN tc_gestor_tbd.nome_completo
            ELSE vw_gestor_adm.nome_completo_gerente
        END AS nome_gestor_adm
    FROM 
        tb_colaborador_alocado tca
    LEFT JOIN 
        tb_periodo_alocacao tpa ON tca.id = tpa.tb_colaborador_alocado_id
    LEFT JOIN
        tb_colaborador_org tco ON tca.codigo_colaborador = tco.cod_colaborador_externo AND tco.tb_org_id = tca.tb_org_id
    LEFT JOIN
        tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador 
	LEFT JOIN
        tb_tbd_alocado ttbd ON tca.cod_tbd_alocado = ttbd.cod_tbd_alocado and tca.tb_org_id = ttbd.tb_org_id
	LEFT JOIN 
		tb_colaborador_org tco_gestor_tbd ON tco_gestor_tbd.codigo_interno_colaborador = ttbd.codigo_interno_colaborador = ttbd.tb_org_id = tco_gestor_tbd.tb_org_id
	LEFT JOIN
		tb_colaborador tc_gestor_tbd ON ttbd.codigo_interno_colaborador = tc_gestor_tbd.codigo_interno_colaborador
    LEFT JOIN
        tb_projeto_org tpo ON tca.codigo_projeto = tpo.cod_projeto AND tpo.tb_org_id = tca.tb_org_id
    LEFT JOIN 
        tb_projeto_gerente tpg ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tca.tb_org_id
    LEFT JOIN
        tb_colaborador_org tco_g ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo AND tco_g.tb_org_id = tca.tb_org_id
    LEFT JOIN
        tb_colaborador tg ON tco_g.codigo_interno_colaborador = tg.codigo_interno_colaborador
	LEFT JOIN
		vw_colaboradores_gestor vw_gestor_adm ON vw_gestor_adm.cod_colaborador = tco.cod_colaborador_externo AND vw_gestor_adm.tb_org_id = tco.tb_org_id
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
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_get_apontamento_vigencia;
DELIMITER $$
$$
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
        resultado_select.codigo_interno_colaborador AS codigo_interno_colaborador,
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
        tc.codigo_interno_colaborador AS codigo_interno_colaborador,
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
    JOIN tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
    LEFT JOIN tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo AND tch.tb_org_id = tco.tb_org_id
    LEFT JOIN tb_colaborador_org tcog ON tcog.cod_colaborador_externo = tch.cod_colaborador_superior AND tco.tb_org_id = tcog.tb_org_id
    LEFT JOIN tb_colaborador tcg ON tcg.codigo_interno_colaborador = tcog.codigo_interno_colaborador
    LEFT JOIN tb_colaborador_apontamento tca ON tc.codigo_interno_colaborador = tca.codigo_interno_colaborador AND tco.tb_org_id = tca.tb_org_id AND tca.tb_vigencia_id = (SELECT id FROM tb_vigencia WHERE mes = p_mes AND ano = p_ano)
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
        tc.codigo_interno_colaborador,
        tcg.nome_completo,
        tch.cod_colaborador_superior,
        tco.tb_org_id,
        tv.mes,
        tv.ano,
        tsa.cod_status_apontamento,
        tca.observacao
        ORDER BY 
            nome_completo
    ) resultado_select
    LEFT JOIN tb_status_apontamento tsa ON resultado_select.cod_status_apontamento_periodo = tsa.cod_status_apontamento
    WHERE 
        (COALESCE(p_cod_status_apontamento, 0) = 0 OR tsa.tb_cod_status_grupo = p_cod_status_apontamento);
        
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
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_get_projeto_com_atividades_por_cpf;
DELIMITER $$
$$
CREATE PROCEDURE `spr_get_projeto_com_atividades_por_cpf`(
    IN p_codigo_interno_colaborador VARCHAR(36),
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
        tco_cod.codigo_interno_colaborador
    FROM
		tb_atividade ta 
    JOIN
		tb_projeto_org_atividade tpoa ON ta.id = tpoa.tb_atividade_id AND ta.tb_org_id = tpoa.tb_projeto_tb_org_id
    JOIN
		tb_projeto_org tpo ON tpoa.tb_projeto_org_cod_projeto = tpo.cod_projeto AND ta.tb_org_id = tpo.tb_org_id
    LEFT JOIN
		tb_colaborador_org tco_cod ON tco_cod.codigo_interno_colaborador = p_codigo_interno_colaborador AND tpo.tb_org_id = tco_cod.tb_org_id
    LEFT JOIN
		tb_colaborador_projeto_org tcpo ON tpo.cod_projeto = tcpo.cod_projeto AND tpo.tb_org_id = tcpo.tb_org_id 
			AND tcpo.cod_colaborador = tco_cod.cod_colaborador_externo
    WHERE 
        (tcpo.cod_colaborador IS NOT NULL 
        OR (tpo.permite_apont_sem_alocacao = 1 OR (tpo.permite_apont_sem_alocacao_outro_colab = 1 AND p_lancamento_para_outro_colaborador)))
        AND ta.tb_org_id = p_org_id
        AND (tpo.data_fim >= CURDATE() OR tpo.data_fim IS NULL OR tpo.data_fim = '0001-01-01')
    ORDER BY tcpo.cod_colaborador, ta.descricao;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_rpt_extracao_mapa;
DELIMITER $$
$$
CREATE PROCEDURE `spr_rpt_extracao_mapa`(
    IN org_id INT
)
BEGIN
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT
        COALESCE(co.cod_colaborador_externo, tbd.cod_tbd_alocado) as CodigoColaborador,
        COALESCE(c.nome_completo, tbd.descricao) as Recurso,
        CASE WHEN tbd.descricao IS NULL THEN 'Não' ELSE 'Sim' END AS TBD,
        po.cod_cliente as CodigoCliente, 
        po.cliente as Cliente, 
        po.status as Status, 
        po.cod_projeto as CodigoProjeto, 
        po.projeto as Projeto, 
        co.departamento as Departamento,
        pa.data_inicio as DataInicio, 
        pa.data_fim as DataTermino,
        pa.observacao as Observacao,
        pa.percentual as Percentual,
        pa.quantidade_horas as QuantidadeHoras,
        CASE WHEN pa.inclui_fimdesemana = 1 THEN 'Sim' ELSE 'Não' END AS IncluiFimDeSemana,
        pa.oportunidade as Oportunidade,
        CASE WHEN pa.prioritario = 1 THEN 'Sim' ELSE 'Não' END AS Prioritario,
        CASE 
            WHEN co.ativo = 1 THEN 'Ativo' 
            WHEN co.ativo IS NULL AND tbd.descricao IS NOT NULL THEN 'Ativo' 
            ELSE 'Inativo'
        END AS StatusColaborador
    FROM 
        tb_colaborador_alocado as ca
        JOIN tb_periodo_alocacao as pa ON pa.tb_colaborador_alocado_id = ca.id
        JOIN tb_projeto_org as po ON ca.codigo_projeto = po.cod_projeto AND ca.tb_org_id = po.tb_org_id
        LEFT JOIN tb_tbd_alocado tbd on ca.cod_tbd_alocado = tbd.cod_tbd_alocado AND tbd.tb_org_id = ca.tb_org_id
        LEFT JOIN tb_colaborador as c ON ca.codigo_interno_colaborador = c.codigo_interno_colaborador
        LEFT JOIN tb_colaborador_org as co ON co.codigo_interno_colaborador = c.codigo_interno_colaborador AND ca.tb_org_id = co.tb_org_id
    WHERE
        ca.tb_org_id = org_id AND ca.ativo = 1 AND pa.ativo = 1
    ORDER BY
        pa.data_criacao DESC;
        
COMMIT;
END$$
DELIMITER ;


DROP PROCEDURE IF EXISTS spr_rpt_extracao_mapa_alocacao;
DELIMITER $$
$$
CREATE PROCEDURE `spr_rpt_extracao_mapa_alocacao`(
    IN org_id INT
)
BEGIN
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT
        COALESCE(co.cod_colaborador_externo, tbd.cod_tbd_alocado) as CodigoColaborador,
        COALESCE(c.nome_completo, tbd.descricao) as Recurso,
        CASE WHEN tbd.descricao IS NULL THEN 'Não' ELSE 'Sim' END AS TBD,
        po.cod_cliente as CodigoCliente, 
        po.cliente as Cliente, 
        po.status as Status, 
        po.cod_projeto as CodigoProjeto, 
        po.projeto as Projeto, 
        co.departamento as Departamento,
        pa.data_inicio as DataInicio, 
        pa.data_fim as DataTermino,
        pa.observacao as Observacao,
        pa.percentual as Percentual,
        pa.quantidade_horas as QuantidadeHoras,
        CASE WHEN pa.inclui_fimdesemana = 1 THEN 'Sim' ELSE 'Não' END AS IncluiFimDeSemana,
        pa.oportunidade as Oportunidade,
        CASE WHEN pa.prioritario = 1 THEN 'Sim' ELSE 'Não' END AS Prioritario,
        CASE 
            WHEN co.ativo = 1 THEN 'Ativo' 
            WHEN co.ativo IS NULL AND tbd.descricao IS NOT NULL THEN 'Ativo' 
            ELSE 'Inativo'
        END AS StatusColaborador
    FROM 
        tb_colaborador_periodo_alocacao as pa
        JOIN tb_projeto_org as po ON pa.codigo_projeto = po.cod_projeto AND pa.tb_org_id = po.tb_org_id
        LEFT JOIN tb_tbd_alocado tbd on pa.cod_tbd_alocado = tbd.cod_tbd_alocado AND tbd.tb_org_id = pa.tb_org_id
        LEFT JOIN tb_colaborador as c ON pa.codigo_interno_colaborador = c.codigo_interno_colaborador
        LEFT JOIN tb_colaborador_org as co ON co.codigo_interno_colaborador = c.codigo_interno_colaborador AND pa.tb_org_id = co.tb_org_id
    WHERE
        pa.tb_org_id = org_id AND pa.ativo = 1
    ORDER BY
        pa.data_criacao DESC;
        
COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS spr_rpt_relatorio_apontamento;
DELIMITER $$
$$
CREATE PROCEDURE `spr_rpt_relatorio_apontamento`(
    IN mes INT,
    IN ano INT,
    IN org_id INT,
    IN codigo_interno_colaborador_gerente VARCHAR(20) -- Adicionando o parâmetro cpf_gerente
)
BEGIN
DECLARE v_valor_parametro VARCHAR(255);
    DECLARE v_sql TEXT;

    -- Obter o valor do parâmetro
    SELECT REPLACE(REPLACE(valor_parametro ,'(a)',''),' ','')
    INTO v_valor_parametro
    FROM tb_parametro_configuracao 
    WHERE codigo_parametro = 'LABEL_COLABORADOR_TIMESHEET' 
      AND tb_org_id = org_id;

    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DROP TEMPORARY TABLE IF EXISTS tmp_result;

	-- ATENÇÃO: o nome dinâmico das colunas, vem deste local aqui
    SET v_sql = CONCAT('CREATE TEMPORARY TABLE tmp_result (
															Cod', IFNULL(v_valor_parametro,'Colaborador'), ' VARCHAR(255),
															', IFNULL(v_valor_parametro,'Colaborador'), ' VARCHAR(120),
															CodCliente VARCHAR(45),
															Cliente VARCHAR(255),
															CodProjeto VARCHAR(255),
															Projeto VARCHAR(255),
															`Status do Projeto` VARCHAR(255),
															Departamento VARCHAR(255),
															Atividade VARCHAR(255),
															StatusAprovacao VARCHAR(50),
															Aprovador VARCHAR(120),
															Justificativa VARCHAR(500),
															Horas VARCHAR(25),
															Semana VARCHAR(28),
															Data VARCHAR(10),
															`Resumo das atividades` VARCHAR(500)
														)');


    -- Copia o conteúdo da variável local para uma variável de sessão
    SET @sql = v_sql;

    -- Executar a criação da tabela temporária
    PREPARE stmt FROM @sql;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;
    -- Verifica se o cpf_gerente foi passado
    IF codigo_interno_colaborador_gerente IS NOT NULL THEN
        -- Insere os dados na tabela temporária quando o cpf_gerente é passado
        INSERT INTO tmp_result
        SELECT 
        co.cod_colaborador_externo AS __CodColaborador,
        c.nome_completo AS __Colaborador,
		po.cod_cliente AS __CodCliente,
		po.cliente AS __Cliente,
		po.cod_projeto AS __CodProjeto,
		po.projeto AS __Projeto,
        po.status AS "__Status do Projeto",
        co.departamento AS __Departamento,
        atv.descricao AS __Atividade,
        sa.descricao AS __StatusAprovacao,
		CASE WHEN sa.tb_cod_status_grupo IN (1, 4) THEN NULL ELSE c_modificador.nome_completo END AS __Aprovador,
        ca.justificativa AS __Justificativa,
        REPLACE(CONVERT(ROUND((ca.horas / 60), 2), CHAR), '.', ',') AS __Horas,
		CONCAT(YEAR(ca.data), '-', (LPAD(WEEK(ca.data), 2, '0') + 1)) AS __Semana,
        DATE_FORMAT(ca.data, '%d/%m/%Y') AS __Data,
        ca.observacao AS "__Resumo das atividades"
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
    ORDER BY ca.horas DESC;
    ELSE
        -- Insere os dados na tabela temporária quando o cpf_gerente não é passado
        INSERT INTO tmp_result
        SELECT 
			co.cod_colaborador_externo AS __CodColaborador,
			c.nome_completo AS __Colaborador,
			po.cod_cliente AS __CodCliente,
			po.cliente AS __Cliente,
			po.cod_projeto AS __CodProjeto,
			po.projeto AS __Projeto,
            po.status AS "__Status do Projeto",
            co.departamento AS __Departamento,
            atv.descricao AS __Atividade,
            sa.descricao AS __StatusAprovacao,
			CASE WHEN sa.tb_cod_status_grupo IN (1, 4) THEN NULL ELSE c_modificador.nome_completo END AS __Aprovador,
            ca.justificativa AS __Justificativa,
            REPLACE(CONVERT(ROUND((ca.horas / 60), 2), CHAR), '.', ',') AS __Horas,
            CONCAT(YEAR(ca.data), '-', (LPAD(WEEK(ca.data), 2, '0') + 1)) AS __Semana,
            DATE_FORMAT(ca.data, '%d/%m/%Y') AS __Data,
            ca.observacao AS "__Resumo das atividades"
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
        ORDER BY ca.horas DESC;
    END IF;

    -- Seleciona os dados da tabela temporária
    SELECT * FROM tmp_result;

    -- Remove a tabela temporária
    DROP TEMPORARY TABLE tmp_result;

COMMIT;
END$$
DELIMITER ;


DROP PROCEDURE IF EXISTS spr_rpt_relatorio_colaboradores;
DELIMITER $$
$$
CREATE PROCEDURE `spr_rpt_relatorio_colaboradores`(IN orgId INT)
BEGIN
DECLARE v_valor_parametro VARCHAR(255);
    DECLARE v_sql TEXT;

    -- Obter o valor do parâmetro
    SELECT REPLACE(REPLACE(valor_parametro ,'(a)',''),' ','') 
    INTO v_valor_parametro
    FROM tb_parametro_configuracao 
    WHERE codigo_parametro = 'LABEL_COLABORADOR_TIMESHEET' 
      AND tb_org_id = orgId;

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DROP TEMPORARY TABLE IF EXISTS tmp_result;

    SET v_sql = CONCAT('CREATE TEMPORARY TABLE tmp_result (
    ', IFNULL(v_valor_parametro, 'Colaborador'), 'ID VARCHAR(255),
    Nome', IFNULL(v_valor_parametro, 'Colaborador'), ' VARCHAR(120),
    CodigoInternoColaborador VARCHAR(11),
    Diretoria VARCHAR(255),
    Departamento VARCHAR(255),
	CodigoGestorADM VARCHAR(45),
    NomeGestorADM VARCHAR(120),
    EmailCorporativo VARCHAR(255),
    Ativo VARCHAR(7),
    DataAdmissao TIMESTAMP,
    DataRescisao VARCHAR(0)
	)');

    -- Executar a criação da tabela temporária
    SET @sql = v_sql;
    PREPARE stmt FROM @sql;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;

    -- Insere os dados na tabela temporária
    INSERT INTO tmp_result
		SELECT
			tco.cod_colaborador_externo as ColaboradorID,
			tc.nome_completo as NomeColaborador, 
			tc.documento_colaborador as Cpf, 
			tc.codigo_interno_colaborador as CodigoInternoColaborador,
			tco.diretoria as Diretoria, 
			tco.departamento as Departamento, 
			tch.cod_colaborador_superior as CodigoGestorHierarquico,
			tcg.nome_completo as NomeGestorHierarquico,
			tu.email as EmailCorporativo,
            CASE 
				WHEN tco.ativo = 1 THEN "Ativo"
				ELSE "Inativo"
			END AS Ativo,
			tco.data_admissao as DataAdmissao,
			'' AS DataRescisao
		FROM 
            tb_colaborador_org tco
        JOIN 
            tb_colaborador tc ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
        JOIN
            tb_usuario tu ON tco.codigo_interno_colaborador = tu.codigo_interno_colaborador
         LEFT JOIN 
            tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo and tco.tb_org_id = tch.tb_org_id
         LEFT JOIN 
            tb_colaborador_org tcog ON tch.cod_colaborador_superior = tcog.cod_colaborador_externo and tco.tb_org_id = tcog.tb_org_id
         LEFT JOIN 
			tb_colaborador tcg ON tcg.codigo_interno_colaborador = tcog.codigo_interno_colaborador
         WHERE
            tco.tb_org_id = orgId;

    -- Seleciona os dados da tabela temporária
    SELECT * FROM tmp_result;

    -- Remove a tabela temporária
    DROP TEMPORARY TABLE tmp_result;

COMMIT;
END$$
DELIMITER ;


DROP PROCEDURE IF EXISTS spr_rpt_relatorio_colaboradores_skills;
DELIMITER $$
$$
CREATE PROCEDURE `spr_rpt_relatorio_colaboradores_skills`(
    IN orgId INT,
    IN hardskills BOOL)
BEGIN
    -- Set the transaction isolation level before starting the transaction
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    START TRANSACTION;

    IF hardskills = TRUE THEN
        SELECT 
            tc.nome_completo AS NomeColaborador, 
            tu.email AS Email,
            tcpt.descricao AS Hardskill, 
            tnv.descricao AS Senioridade,
            tco.cod_diretoria AS CodDiretoria,
            tco.diretoria AS Diretoria,
            tco.ativo AS Ativo,
            tco.tb_org_id AS OrgId
        FROM 
            tb_colaborador tc
        JOIN 
            tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
        JOIN 
            tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador
        JOIN 
            tb_colaborador_competencia tcc ON tc.codigo_interno_colaborador = tcc.codigo_interno_colaborador
        JOIN 
            tb_competencia tcpt ON tcc.competencia_id = tcpt.id
        JOIN 
            tb_nivel tnv ON tcc.tb_nivel_id = tnv.id
        WHERE 
            tco.tb_org_id = orgId;
    ELSE 
        SELECT
            tc.nome_completo AS NomeColaborador, 
            tu.email AS Email,
            tco.cod_diretoria AS CodDiretoria,
            tco.diretoria AS Diretoria,
            tco.ativo AS Ativo,
            tco.tb_org_id AS OrgId 
        FROM 
            tb_colaborador tc
        JOIN 
            tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
        JOIN 
            tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador
        WHERE 
            tco.tb_org_id = orgId;
    END IF;

    COMMIT;
END$$
DELIMITER ;


DROP PROCEDURE IF EXISTS spr_rpt_relatorio_projeto;
DELIMITER $$
$$
CREATE PROCEDURE `spr_rpt_relatorio_projeto`(
   IN org_id INT
)
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; -- previne que não vai travar as tabelas do DB
	SELECT 
        tpo.cod_projeto as CodigoProjeto, 
        tpo.projeto as NomeProjeto, 
        tpg.cod_colaborador_gerente as CodGestorProjeto,
        tcg.nome_completo as GestorProjeto,
        tpo.cod_cliente as CodigoCliente, 
        tpo.cliente as NomeCliente, 
        tpo.status as StatusProjeto, 
        tpo.data_inicio as DataInicial, 
        tpo.data_fim as DataFinal, 
        tpo.codigo_oportunidade as CodigoOportunidade,
        ta.descricao as Atividades
    FROM 
        tb_projeto_org tpo
    LEFT JOIN 
        tb_projeto_gerente tpg ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tpo.tb_org_id
	LEFT JOIN
		tb_colaborador_org tcog ON tcog.cod_colaborador_externo = tpg.cod_colaborador_gerente AND tcog.tb_org_id = tpo.tb_org_id
    LEFT JOIN
		tb_colaborador tcg ON tcog.codigo_interno_colaborador = tcg.codigo_interno_colaborador
    LEFT JOIN 
        tb_projeto_org_atividade tpoa ON tpo.cod_projeto = tpoa.tb_projeto_org_cod_projeto AND tpoa.tb_projeto_tb_org_id = tpo.tb_org_id
    LEFT JOIN 
        tb_atividade ta ON tpoa.tb_atividade_id = ta.id AND ta.tb_org_id = tpo.tb_org_id
    WHERE
        tpo.tb_org_id = org_id;
COMMIT;
END$$
DELIMITER ;


CREATE TABLE `tb_token_validacao` (
  `id` varchar(36) NOT NULL,
  `codigo_interno_colaborador` varchar(36) NOT NULL,
  `token` varchar(6) NOT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `validade` timestamp NOT NULL,
  `confirmado` tinyint NOT NULL DEFAULT '0',
  `tipo_validacao` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_tb_token_validacao_tb_colaborador1_idx` (`codigo_interno_colaborador`),
  CONSTRAINT `fk_tb_token_validacao_tb_colaborador1` FOREIGN KEY (`codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

INSERT INTO tb_template_email (id,codigo,descricao,template)
	VALUES ('d4841afc-4f85-11ef-837c-0242ac140002','TOKEN_ACESSO_EMAIL','Template para código de acesso ao fourmakers','<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html dir="ltr" lang="en">

  <head>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link
      href="https://fonts.googleapis.com/css2?family=Nunito:ital,wght@0,200..1000;1,200..1000&display=swap"
      rel="stylesheet">
  </head>

  <body
    style="background-color:#f3f3f5;font-family:Nunito,sans-serif">
    <table align="center" width="100%" border="0" cellPadding="0"
      cellSpacing="0" role="presentation"
      style="max-width:100%;width:680px;margin:0 auto;background-color:#ffffff">
      <tbody>
        <tr style="width:100%">
          <td>
            <table align="center" width="100%" border="0" cellPadding="0"
              cellSpacing="0" role="presentation"
              style="border-radius:5px 5px 0 0;display:flex;flex-direction:column; width:100%">
              <tbody style="width:100%">
                <svg style="width: 100%; " width="100%"
                  height="100%" id="svg" viewBox="0 0 1440 490"
                  xmlns="http://www.w3.org/2000/svg"
                  class="transition duration-300 ease-in-out delay-150"><path
                    d="M 0,500 L 0,187 C 122.80000000000001,200.46666666666667 245.60000000000002,213.93333333333334 398,199 C 550.4,184.06666666666666 732.4000000000001,140.73333333333335 911,134 C 1089.6,127.26666666666665 1264.8,157.13333333333333 1440,187 L 1440,500 L 0,500 Z"
                    stroke="none" stroke-width="0" fill="#1c1c38"
                    fill-opacity="1"
                    class="transition-all duration-300 ease-in-out delay-150 path-0"
                    transform="rotate(-180 720 250)"></path></svg>

                <tr
                  style="width: 100%; display: flex; align-items: center; justify-content: space-between; margin-top: -233px; padding: 30px;">
                  <td><img
                      src="https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/ecfp9qwak914/logo-darkmode.png"
                      style="display:block;outline:none;border:none;text-decoration:none;"
                      width="156" /></td>
                  <td><img
                      src="https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/wntmgxmbqzt5/avatar-inclinado.png"
                      style="display:block;outline:none;border:none;text-decoration:none; margin-right: 30px;"
                      width="146" /></td>
                </tr>
              </tbody>
            </table>
            <table align="center" width="100%" border="0" cellPadding="0"
              cellSpacing="0" role="presentation"
              style="padding:30px 30px 0px 30px">
              <tbody>
                <tr>
                  <td>
                    <h2
                      style="margin:0 0 30px;font-weight:bold;font-size:21px;line-height:22px;color:#FF5315">Prezado(a)
                      ${NOME},</h2>
                    <p
                      style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44">
                      Chegou seu código de acesso para o Fourmakers.</p>
					
                    <p
                      style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44">
                      <b>Código de Acesso: ${TOKEN}</p></b>

                
                    
                    <br>
                    <p
                      style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44">Caso tenha qualquer dúvida, não hesite em entrar em contato conosco pelo e-mail: <br>
                      ajuda@fourmakers.io <br>
                      Até breve, Equipe Fourmakers.io</p>

                  </td>
                </tr>
              </tbody>
            </table>
            <table align="center" width="100%" border="0" cellPadding="0"
              cellSpacing="0" role="presentation"
              style="border-radius:5px 5px 0 0;display:flex;flex-direction:column; width:100%">
              <tbody style="width:100%">
                <svg width="100%" height="100%" id="svg" viewBox="0 0 1440 590"
                  xmlns="http://www.w3.org/2000/svg"
                  class="transition duration-300 ease-in-out delay-150"><path
                    d="M 0,600 L 0,225 C 110.34449760765548,178.21531100478467 220.68899521531097,131.43062200956936 307,133 C 393.31100478468903,134.56937799043064 455.5885167464115,184.4928229665072 545,200 C 634.4114832535885,215.5071770334928 750.956937799043,196.59808612440193 846,184 C 941.043062200957,171.40191387559807 1014.5837320574162,165.11483253588517 1110,173 C 1205.4162679425838,180.88516746411483 1322.7081339712918,202.94258373205741 1440,225 L 1440,600 L 0,600 Z"
                    stroke="none" stroke-width="0" fill="#1c1c38"
                    fill-opacity="1"
                    class="transition-all duration-300 ease-in-out delay-150 path-0"></path></svg>

                <tr
                  style="width: 100%; display: flex; align-items: center; justify-content: center; margin-top: -100px;">
                  <td><img
                      src="https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/ecfp9qwak914/logo-darkmode.png"
                      style="display:block;outline:none;border:none;text-decoration:none;"
                      width="156" /></td>

                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table>

  </body>

</html>');