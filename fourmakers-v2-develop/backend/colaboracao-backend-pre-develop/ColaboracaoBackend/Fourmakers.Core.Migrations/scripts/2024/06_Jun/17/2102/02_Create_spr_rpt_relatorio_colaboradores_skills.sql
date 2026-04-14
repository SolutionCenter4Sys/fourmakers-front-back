DELIMITER $$
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
            tb_colaborador_org tco ON tc.cpf = tco.tb_colaborador_cpf
        JOIN 
            tb_usuario tu ON tc.cpf = tu.cpf
        JOIN 
            tb_colaborador_competencia tcc ON tc.cpf = tcc.colaborador_cpf
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
            tb_colaborador_org tco ON tc.cpf = tco.tb_colaborador_cpf
        JOIN 
            tb_usuario tu ON tc.cpf = tu.cpf
        WHERE 
            tco.tb_org_id = orgId;
    END IF;

    COMMIT;
END$$

DELIMITER ;
;