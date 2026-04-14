DROP PROCEDURE `spr_rpt_extracao_mapa_alocacao`;
delimiter //
CREATE PROCEDURE `spr_rpt_extracao_mapa_alocacao`(
    IN org_id INT
)
BEGIN
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT
        COALESCE(co.cod_colaborador_externo, tbd.cod_tbd_alocado) as CodigoColaborador,
        COALESCE(c.nome_completo, tbd.descricao) as Recurso,
        CASE WHEN tbd.descricao IS NULL THEN 'Não' ELSE 'Sim' END AS TBD,
        po.cod_cliente as CodigoCliente, 
        tclo.nome_cliente as Cliente, 
        po.status as Status, 
        po.cod_projeto as CodigoProjeto, 
        po.projeto as Projeto, 
        co.departamento as Departamento,
        pa.data_inicio as DataInicio, 
        pa.data_fim as DataTermino,
        pa.observacao as Observacao,
        pa.percentual as Percentual,
        pa.quantidade_horas as QuantidadeHoras,
        CASE WHEN pa.inclui_fimdesemana = 1 THEN 'Sim' ELSE 'Não' END AS IncluiFimDeSemana,
        pa.oportunidade as Oportunidade,
        CASE WHEN pa.prioritario = 1 THEN 'Sim' ELSE 'Não' END AS Prioritario,
        CASE 
            WHEN co.ativo = 1 THEN 'Ativo' 
            WHEN co.ativo IS NULL AND tbd.descricao IS NOT NULL THEN 'Ativo' 
            ELSE 'Inativo'
        END AS StatusColaborador
    FROM 
        tb_colaborador_periodo_alocacao as pa
        JOIN tb_projeto_org as po ON pa.codigo_projeto = po.cod_projeto AND pa.tb_org_id = po.tb_org_id
		LEFT JOIN tb_cliente_org tclo ON po.cod_cliente = tclo.codigo_cliente AND po.tb_org_id = tclo.tb_org_id			
        LEFT JOIN tb_tbd_alocado tbd on pa.cod_tbd_alocado = tbd.cod_tbd_alocado AND tbd.tb_org_id = pa.tb_org_id
        LEFT JOIN tb_colaborador as c ON pa.codigo_interno_colaborador = c.codigo_interno_colaborador
        LEFT JOIN tb_colaborador_org as co ON co.codigo_interno_colaborador = c.codigo_interno_colaborador AND pa.tb_org_id = co.tb_org_id
    WHERE
        pa.tb_org_id = org_id AND pa.ativo = 1
    ORDER BY
        pa.data_criacao DESC;
        
COMMIT;
END //
delimiter ;