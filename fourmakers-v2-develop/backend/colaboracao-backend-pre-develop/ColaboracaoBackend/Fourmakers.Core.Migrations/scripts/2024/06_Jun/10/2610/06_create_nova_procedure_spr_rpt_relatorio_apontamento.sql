DELIMITER //
CREATE PROCEDURE `spr_rpt_relatorio_apontamento`(
    IN mes INT,
    IN ano INT,
    IN org_id INT,
    IN cpf_gerente VARCHAR(20) -- Adicionando o parâmetro cpf_gerente
)
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; -- previne que não vai travar as tabelas do DB
    -- Verifica se o cpf_gerente foi passado
    IF cpf_gerente IS NOT NULL THEN
        -- Consulta quando o cpf_gerente é passado
        SELECT 
        co.cod_colaborador_externo as CodColaborador,
        c.nome_completo as Colaborador,
		po.cod_cliente as CodCliente,
		po.cliente as Cliente,
		po.cod_projeto as CodProjeto,
		po.projeto as Projeto,
        co.departamento as Departamento,
        atv.descricao as Atividade,
        sa.descricao as StatusAprovacao,
		CASE WHEN sa.tb_cod_status_grupo IN (1, 4) THEN NULL ELSE ca.tb_colaborador_cpf_alteracao END AS CpfModificadorStatus,
		CASE WHEN sa.tb_cod_status_grupo IN (1, 4) THEN NULL ELSE c_modificador.nome_completo END AS NomeModificadorStatus,
        ca.justificativa as Justificativa,
        CONVERT(ROUND((ca.horas / 60), 2),CHAR)as Horas,
		CONCAT(YEAR(ca.data), '-', (LPAD(WEEK(ca.data), 2, '0') + 1)) as Semana,
        ca.data as Data,
        ca.observacao as Observacao
    FROM
        tb_colaborador_apontamento ca
        JOIN tb_colaborador c ON ca.tb_colaborador_org_tb_colaborador_cpf = c.cpf
        JOIN tb_colaborador_org co ON ca.tb_colaborador_org_tb_colaborador_cpf = co.tb_colaborador_cpf AND ca.tb_org_id = co.tb_org_id 
        JOIN tb_projeto_org po ON ca.tb_projeto_org_cod_projeto = po.cod_projeto AND ca.tb_org_id = po.tb_org_id
        JOIN tb_projeto_gerente tpg ON po.cod_projeto = tpg.cod_projeto AND po.tb_org_id = tpg.tb_org_id
        JOIN tb_colaborador_org tpog on tpg.cod_colaborador_gerente = tpog.cod_colaborador_externo
        JOIN tb_atividade atv ON ca.tb_atividade_id = atv.id
        JOIN tb_status_apontamento sa ON ca.tb_status_apontamento_id = sa.id
        LEFT JOIN tb_colaborador c_modificador ON ca.tb_colaborador_cpf_alteracao = c_modificador.cpf
    WHERE 
        ca.tb_org_id = org_id
        AND MONTH(ca.data) = mes
        AND YEAR(ca.data) = ano
        AND tpog.tb_colaborador_cpf = cpf_gerente
    ORDER BY ca.horas DESC;
    ELSE
        -- Consulta padrão quando o cpf_gerente não é passado
        SELECT 
			co.cod_colaborador_externo as CodColaborador,
			c.nome_completo as Colaborador,
			po.cod_cliente as CodCliente,
			po.cliente as Cliente,
			po.cod_projeto as CodProjeto,
			po.projeto as Projeto,
            co.departamento as Departamento,
            atv.descricao as Atividade,
            sa.descricao as StatusAprovacao,
            CASE WHEN sa.tb_cod_status_grupo IN (1, 4) THEN NULL ELSE ca.tb_colaborador_cpf_alteracao END AS CpfModificadorStatus,
			CASE WHEN sa.tb_cod_status_grupo IN (1, 4) THEN NULL ELSE c_modificador.nome_completo END AS NomeModificadorStatus,
            ca.justificativa as Justificativa,
            ROUND((ca.horas / 60), 2) as Horas,
            CONCAT(YEAR(ca.data), '-', (LPAD(WEEK(ca.data), 2, '0') + 1)) as Semana,
            ca.data as Data,
            ca.observacao as Observacao
        FROM
            tb_colaborador_apontamento ca
            JOIN tb_colaborador c ON ca.tb_colaborador_org_tb_colaborador_cpf = c.cpf
            JOIN tb_colaborador_org co ON ca.tb_colaborador_org_tb_colaborador_cpf = co.tb_colaborador_cpf AND ca.tb_org_id = co.tb_org_id 
            JOIN tb_projeto_org po ON ca.tb_projeto_org_cod_projeto = po.cod_projeto AND ca.tb_org_id = po.tb_org_id
            JOIN tb_atividade atv ON ca.tb_atividade_id = atv.id 
            JOIN tb_status_apontamento sa ON ca.tb_status_apontamento_id = sa.id
            LEFT JOIN tb_colaborador c_modificador ON ca.tb_colaborador_cpf_alteracao = c_modificador.cpf
        WHERE 
            ca.tb_org_id = org_id
            AND MONTH(ca.data) = mes
            AND YEAR(ca.data) = ano
        ORDER BY ca.horas DESC;
    END IF;
COMMIT;
END //

DELIMITER ;
