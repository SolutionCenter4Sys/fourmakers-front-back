DROP PROCEDURE `spr_rpt_relatorio_apontamento`;
DELIMITER //
CREATE PROCEDURE `spr_rpt_relatorio_apontamento`(
    IN mes INT,
    IN ano INT,
    IN org_id INT,
    IN codigo_interno_colaborador_gerente VARCHAR(64) -- Adicionando o parâmetro cpf_gerente
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
                                                            CodigoAprovador VARCHAR(255),
															Aprovador VARCHAR(120),
															Justificativa VARCHAR(500),
															Horas VARCHAR(25),
															Semana VARCHAR(28),
															Data VARCHAR(10),
															`Resumo das atividades` VARCHAR(500),
                                                            ModeloContratacao VARCHAR(255),
                                                            Empresa VARCHAR(255),
                                                            ContatoPrincipal VARCHAR(17)
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
        sag.descricao AS __StatusAprovacao,
        tco_modificador.cod_colaborador_externo AS __CodigoAprovador,
		CASE WHEN sa.tb_cod_status_grupo IN (1, 4) THEN (SELECT GROUP_CONCAT(tc.nome_completo SEPARATOR ', ') FROM tb_projeto_gerente tpg JOIN tb_colaborador_org tco ON tco.cod_colaborador_externo = tpg.cod_colaborador_gerente AND tco.tb_org_id = tpg.tb_org_id JOIN tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador WHERE tpg.cod_projeto = po.cod_projeto AND tpg.tb_org_id = ca.tb_org_id) ELSE c_modificador.nome_completo END AS __Aprovador,
        ca.justificativa AS __Justificativa,
        REPLACE(CONVERT(ROUND((ca.horas / 60), 2), CHAR), '.', ',') AS __Horas,
		CONCAT(YEAR(ca.data), '-', (LPAD(WEEK(ca.data), 2, '0') + 1)) AS __Semana,
        DATE_FORMAT(ca.data, '%d/%m/%Y') AS __Data,
        ca.observacao AS "__Resumo das atividades",
        co.modelo_contratacao AS __ModeloContratacao,
        co.empresa_relacionada AS __EmpresaRelacionada,
        c.contato_principal as __ContatoPrincipal
    FROM
        tb_colaborador_apontamento ca
        JOIN tb_colaborador c ON ca.codigo_interno_colaborador = c.codigo_interno_colaborador
        JOIN tb_colaborador_org co ON ca.codigo_interno_colaborador = co.codigo_interno_colaborador AND ca.tb_org_id = co.tb_org_id 
        JOIN tb_projeto_org po ON ca.tb_projeto_org_cod_projeto = po.cod_projeto AND ca.tb_org_id = po.tb_org_id
        JOIN tb_projeto_gerente tpg ON po.cod_projeto = tpg.cod_projeto AND po.tb_org_id = tpg.tb_org_id
        JOIN tb_colaborador_org tpog on tpg.cod_colaborador_gerente = tpog.cod_colaborador_externo AND ca.tb_org_id = tpog.tb_org_id
        JOIN tb_atividade atv ON ca.tb_atividade_id = atv.id
        JOIN tb_status_apontamento sa ON ca.tb_status_apontamento_id = sa.id
        JOIN tb_status_apontamento_grupo sag ON sag.cod_status_grupo = sa.tb_cod_status_grupo
        LEFT JOIN tb_colaborador c_modificador ON ca.codigo_interno_colaborador_alteracao = c_modificador.codigo_interno_colaborador
        LEFT JOIN tb_colaborador_org tco_modificador ON tco_modificador.codigo_interno_colaborador = c_modificador.codigo_interno_colaborador AND tco_modificador.tb_org_id = ca.tb_org_id
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
            sag.descricao AS __StatusAprovacao,
            tco_modificador.cod_colaborador_externo AS __CodigoAprovador,
			CASE WHEN sa.tb_cod_status_grupo IN (1, 4) THEN (SELECT GROUP_CONCAT(tc.nome_completo SEPARATOR ', ') FROM tb_projeto_gerente tpg JOIN tb_colaborador_org tco ON tco.cod_colaborador_externo = tpg.cod_colaborador_gerente AND tco.tb_org_id = tpg.tb_org_id JOIN tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador WHERE tpg.cod_projeto = po.cod_projeto AND tpg.tb_org_id = ca.tb_org_id) ELSE c_modificador.nome_completo END AS __Aprovador,
            ca.justificativa AS __Justificativa,
            REPLACE(CONVERT(ROUND((ca.horas / 60), 2), CHAR), '.', ',') AS __Horas,
            CONCAT(YEAR(ca.data), '-', (LPAD(WEEK(ca.data), 2, '0') + 1)) AS __Semana,
            DATE_FORMAT(ca.data, '%d/%m/%Y') AS __Data,
            ca.observacao AS "__Resumo das atividades",
			co.modelo_contratacao AS __ModeloContratacao,
			co.empresa_relacionada AS __EmpresaRelacionada,
			c.contato_principal as __ContatoPrincipal
        FROM
            tb_colaborador_apontamento ca
            JOIN tb_colaborador c ON ca.codigo_interno_colaborador = c.codigo_interno_colaborador
            JOIN tb_colaborador_org co ON ca.codigo_interno_colaborador = co.codigo_interno_colaborador AND ca.tb_org_id = co.tb_org_id 
            JOIN tb_projeto_org po ON ca.tb_projeto_org_cod_projeto = po.cod_projeto AND ca.tb_org_id = po.tb_org_id
            JOIN tb_atividade atv ON ca.tb_atividade_id = atv.id 
            JOIN tb_status_apontamento sa ON ca.tb_status_apontamento_id = sa.id
            JOIN tb_status_apontamento_grupo sag ON sag.cod_status_grupo = sa.tb_cod_status_grupo
            LEFT JOIN tb_colaborador c_modificador ON ca.codigo_interno_colaborador_alteracao = c_modificador.codigo_interno_colaborador
            LEFT JOIN tb_colaborador_org tco_modificador ON tco_modificador.codigo_interno_colaborador = c_modificador.codigo_interno_colaborador AND tco_modificador.tb_org_id = ca.tb_org_id
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
END//
DELIMITER ;