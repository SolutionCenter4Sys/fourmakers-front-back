DROP PROCEDURE IF EXISTS spr_rpt_relatorio_apontamento_bi;

DELIMITER $$
$$
CREATE DEFINER=`admin`@`%` PROCEDURE `spr_rpt_relatorio_apontamento_bi`(
    IN org_id INT, -- Id da org
    IN dateFrom DATE -- Data inicial do relatório
)
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; -- previne que não vai travar as tabelas do DB
    -- Verifica se o cpf_gerente foi passado
	-- Consulta padrão quando o cpf_gerente não é passado
	SELECT 
		co.cod_colaborador_externo as CodColaborador,
		c.nome_completo as Colaborador,
		po.cod_cliente as CodCliente,
		po.cliente as Cliente,
		po.cod_projeto as CodProjeto,
		po.projeto as Projeto,
		po.status as StatusProjeto,
	    co.departamento as Departamento,
	    atv.descricao as Atividade,
	    sa.descricao as StatusAprovacao,
 		CASE WHEN sa.tb_cod_status_grupo IN (1, 4) THEN NULL ELSE c.nome_completo END AS NomeModificadorStatus,
	    ca.justificativa as Justificativa,
	    ROUND((ca.horas / 60), 2) as Horas,
	    CONCAT(YEAR(ca.data), '-', (LPAD(WEEK(ca.data), 2, '0') + 1)) as Semana,
	    DATE_FORMAT(ca.data, "%d/%m/%Y") as Data,
	    ca.observacao as Observacao
	FROM
	    tb_colaborador_apontamento ca
	    JOIN tb_colaborador c ON ca.codigo_interno_colaborador = c.codigo_interno_colaborador
	    JOIN tb_colaborador_org co ON ca.codigo_interno_colaborador = co.codigo_interno_colaborador AND ca.tb_org_id = co.tb_org_id 
	    JOIN tb_projeto_org po ON ca.tb_projeto_org_cod_projeto = po.cod_projeto AND ca.tb_org_id = po.tb_org_id
	    JOIN tb_atividade atv ON ca.tb_atividade_id = atv.id 
	    JOIN tb_status_apontamento sa ON ca.tb_status_apontamento_id = sa.id
	WHERE 
	    ca.tb_org_id = org_id
	    AND ca.data > dateFrom
	ORDER BY ca.data ASC;
COMMIT;
END$$
DELIMITER ;
