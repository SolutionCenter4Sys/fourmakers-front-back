DROP PROCEDURE `spr_rpt_relatorio_apontamento_bi`;
DELIMITER //
CREATE PROCEDURE `spr_rpt_relatorio_apontamento_bi`(
    IN org_id INT, -- Id da org
    IN dateFrom DATE -- Data inicial do relatório
)
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; -- previne que não vai travar as tabelas do DB
	SELECT 
		co.cod_colaborador_externo as CodColaborador,
		c.nome_completo as Colaborador,
		po.cod_cliente as CodCliente,
		tclo.nome_cliente as Cliente,
		po.cod_projeto as CodProjeto,
		po.projeto as Projeto,
		po.status as StatusProjeto,
	    co.departamento as Departamento,
	    atv.descricao as Atividade,
	    sag.descricao as StatusAprovacao,
        tco_modificador.cod_colaborador_externo AS CodigoAprovador,
		CASE WHEN sa.tb_cod_status_grupo IN (1, 4)
		THEN (SELECT GROUP_CONCAT(tc.nome_completo SEPARATOR ', ') FROM tb_projeto_gerente tpg JOIN tb_colaborador_org tco ON tco.cod_colaborador_externo = tpg.cod_colaborador_gerente AND tco.tb_org_id = tpg.tb_org_id JOIN tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador WHERE tpg.cod_projeto = po.cod_projeto AND tpg.tb_org_id = ca.tb_org_id)
		ELSE c_modificador.nome_completo END as Aprovador,
	    ca.justificativa as Justificativa,
	    ROUND((ca.horas / 60), 2) as Horas,
	    CONCAT(YEAR(ca.data), '-', (LPAD(WEEK(ca.data), 2, '0') + 1)) as Semana,
	    DATE_FORMAT(ca.data, "%d/%m/%Y") as Data,
	    ca.observacao as Observacao,
		co.modelo_contratacao AS ModeloContratacao,
		co.empresa_relacionada AS EmpresaRelacionada,
		c.contato_principal as ContatoPrincipal
	FROM
	    tb_colaborador_apontamento ca
	    JOIN tb_colaborador c ON ca.codigo_interno_colaborador = c.codigo_interno_colaborador
	    JOIN tb_colaborador_org co ON ca.codigo_interno_colaborador = co.codigo_interno_colaborador AND ca.tb_org_id = co.tb_org_id 
	    JOIN tb_projeto_org po ON ca.tb_projeto_org_cod_projeto = po.cod_projeto AND ca.tb_org_id = po.tb_org_id
		LEFT JOIN tb_cliente_org tclo ON po.cod_cliente = tclo.codigo_cliente AND po.tb_org_id = tclo.tb_org_id			
	    JOIN tb_atividade atv ON ca.tb_atividade_id = atv.id 
	    JOIN tb_status_apontamento sa ON ca.tb_status_apontamento_id = sa.id
        JOIN tb_status_apontamento_grupo sag ON sag.cod_status_grupo = sa.tb_cod_status_grupo
        LEFT JOIN tb_colaborador c_modificador ON ca.codigo_interno_colaborador_alteracao = c_modificador.codigo_interno_colaborador
        LEFT JOIN tb_colaborador_org tco_modificador ON tco_modificador.codigo_interno_colaborador = c_modificador.codigo_interno_colaborador AND tco_modificador.tb_org_id = ca.tb_org_id
	WHERE 
	    ca.tb_org_id = org_id
	    AND ca.data > dateFrom
	ORDER BY ca.data ASC;
COMMIT;
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ; -- Restaura o nível de isolamento padrão
END
//
DELIMITER ;