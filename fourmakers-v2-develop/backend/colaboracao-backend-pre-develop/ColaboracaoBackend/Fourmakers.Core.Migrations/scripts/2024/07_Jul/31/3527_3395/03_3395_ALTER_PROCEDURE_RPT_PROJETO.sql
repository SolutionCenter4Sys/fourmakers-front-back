CREATE PROCEDURE `spr_rpt_relatorio_projeto`(
   IN org_id INT
)
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; -- previne que não vai travar as tabelas do DB
	SELECT 
        tpo.cod_projeto as CodigoProjeto, 
        tpo.projeto as NomeProjeto, 
        tpg.cod_colaborador_gerente as CodAprovador,
        tcg.nome_completo as Aprovador,
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
		tb_colaborador tcg ON tcog.tb_colaborador_cpf = tcg.cpf
    LEFT JOIN 
        tb_projeto_org_atividade tpoa ON tpo.cod_projeto = tpoa.tb_projeto_org_cod_projeto AND tpoa.tb_projeto_tb_org_id = tpo.tb_org_id
    LEFT JOIN 
        tb_atividade ta ON tpoa.tb_atividade_id = ta.id AND ta.tb_org_id = tpo.tb_org_id
    WHERE
        tpo.tb_org_id = org_id;
COMMIT;
END