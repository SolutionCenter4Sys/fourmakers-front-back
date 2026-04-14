DELIMITER $$
CREATE PROCEDURE `spr_rpt_relatorio_colaboradores`(IN orgId INT)
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
		SELECT
			tco.cod_colaborador_externo as ColaboradorID,
			tc.nome_completo as NomeColaborador, 
			tco.tb_colaborador_cpf as Cpf, 
			tco.diretoria as Diretoria, 
			tco.departamento as Departamento, 
			tch.cod_colaborador_superior as CodigoGestorHierarquico,
			tcg.nome_completo as NomeGestorHierarquico,
			tu.email as EmailCorporativo,
			tco.ativo AS Ativo,
			tco.data_admissao as DataAdmissao,
			'' AS DataRescisao
		FROM 
            tb_colaborador_org tco
        JOIN 
            tb_colaborador tc ON tc.cpf = tco.tb_colaborador_cpf
        JOIN
            tb_usuario tu ON tco.tb_colaborador_cpf = tu.cpf
         LEFT JOIN 
            tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo and tco.tb_org_id = tch.tb_org_id
         LEFT JOIN 
            tb_colaborador_org tcog ON tch.cod_colaborador_superior = tcog.cod_colaborador_externo and tco.tb_org_id = tcog.tb_org_id
         LEFT JOIN 
			tb_colaborador tcg ON tcg.cpf = tcog.tb_colaborador_cpf
         WHERE
            tco.tb_org_id = orgId;
COMMIT;
END $$
DELIMITER ;