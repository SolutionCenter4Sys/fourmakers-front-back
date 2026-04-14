DELIMITER //

CREATE PROCEDURE `spr_rpt_relatorio_colaboradores`(
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
            tco.tb_org_id AS OrgId,
            tc_gerente.nome_completo AS gestor_hierarquico,
            '' AS data_rescisao
        FROM 
            tb_colaborador tc
        JOIN 
            tb_colaborador_org tco ON tc.cpf = tco.tb_colaborador_cpf
        JOIN 
            tb_usuario tu ON tc.cpf = tu.cpf
        JOIN 
            tb_colaborador_competencia tcc ON tc.cpf = tcc.colaborador_cpf
        JOIN 
            tb_competencia tcpt ON tcc.competencia_id = tcpt.id
        JOIN 
            tb_nivel tnv ON tcc.tb_nivel_id = tnv.id 
		LEFT JOIN 
			tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo AND tco.tb_org_id = tch.tb_org_id
		LEFT JOIN 
			tb_colaborador_org tco_gerente ON tch.cod_colaborador_superior = tco_gerente.cod_colaborador_externo AND tch.tb_org_id = tco_gerente.tb_org_id
		LEFT JOIN 
			tb_colaborador tc_gerente ON tco_gerente.tb_colaborador_cpf = tc_gerente.cpf
        WHERE 
            tco.tb_org_id = orgId;
    ELSE 
        SELECT
            tc.nome_completo AS NomeColaborador, 
            tu.email AS Email,
            tco.cod_diretoria AS CodDiretoria,
            tco.diretoria AS Diretoria,
            tco.ativo AS Ativo,
            tco.tb_org_id AS OrgId,
            tc_gerente.nome_completo AS gestor_hierarquico,
            '' AS data_rescisao   
        FROM 
            tb_colaborador tc
        JOIN 
            tb_colaborador_org tco ON tc.cpf = tco.tb_colaborador_cpf
        JOIN 
            tb_usuario tu ON tc.cpf = tu.cpf
		LEFT JOIN 
			tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo AND tco.tb_org_id = tch.tb_org_id
		LEFT JOIN 
			tb_colaborador_org tco_gerente ON tch.cod_colaborador_superior = tco_gerente.cod_colaborador_externo AND tch.tb_org_id = tco_gerente.tb_org_id
		LEFT JOIN 
			tb_colaborador tc_gerente ON tco_gerente.tb_colaborador_cpf = tc_gerente.cpf
        WHERE 
            tco.tb_org_id = orgId;
    END IF;

    COMMIT;
END //

DELIMITER ;
