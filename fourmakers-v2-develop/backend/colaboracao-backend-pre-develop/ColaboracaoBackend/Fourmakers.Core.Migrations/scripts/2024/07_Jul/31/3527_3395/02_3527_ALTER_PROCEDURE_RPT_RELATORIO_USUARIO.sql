CREATE DEFINER=`admin`@`%` PROCEDURE `spr_rpt_relatorio_colaboradores`(
    IN orgId INT
)
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
    Cpf VARCHAR(11),
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
        tco.tb_colaborador_cpf as Cpf, 
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
        tb_colaborador tc ON tc.cpf = tco.tb_colaborador_cpf
    JOIN
        tb_usuario tu ON tco.tb_colaborador_cpf = tu.cpf
    LEFT JOIN 
        tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo AND tco.tb_org_id = tch.tb_org_id
    LEFT JOIN 
        tb_colaborador_org tcog ON tch.cod_colaborador_superior = tcog.cod_colaborador_externo AND tco.tb_org_id = tcog.tb_org_id
    LEFT JOIN 
        tb_colaborador tcg ON tcg.cpf = tcog.tb_colaborador_cpf
    WHERE
        tco.tb_org_id = orgId;

    -- Seleciona os dados da tabela temporária
    SELECT * FROM tmp_result;

    -- Remove a tabela temporária
    DROP TEMPORARY TABLE tmp_result;

    COMMIT;
END