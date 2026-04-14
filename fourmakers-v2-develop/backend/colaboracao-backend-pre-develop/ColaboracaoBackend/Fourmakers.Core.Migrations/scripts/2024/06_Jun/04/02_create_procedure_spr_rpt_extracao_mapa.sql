DELIMITER //

CREATE PROCEDURE spr_rpt_extracao_mapa(
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
        LEFT JOIN tb_colaborador as c ON ca.tb_colaborador_cpf = c.cpf
        LEFT JOIN tb_colaborador_org as co ON co.tb_colaborador_cpf = c.cpf AND ca.tb_org_id = co.tb_org_id
    WHERE
        ca.tb_org_id = org_id AND ca.ativo = 1 AND pa.ativo = 1
    ORDER BY
        pa.data_criacao DESC;
END //

DELIMITER ;
