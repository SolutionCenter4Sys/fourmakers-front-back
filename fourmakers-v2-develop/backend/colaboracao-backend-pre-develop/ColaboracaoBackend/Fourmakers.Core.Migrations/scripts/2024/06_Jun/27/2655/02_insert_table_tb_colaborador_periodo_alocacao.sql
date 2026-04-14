INSERT INTO tb_colaborador_periodo_alocacao
(
    id, data_inicio, data_fim, quantidade_horas, inclui_fimdesemana, ativo, data_criacao, 
    data_alteracao, observacao, oportunidade, prioritario, percentual, 
    codigo_colaborador, codigo_projeto, tb_colaborador_cpf, tb_org_id, cod_tbd_alocado, tb_atividade_id
)
SELECT 
    tpa.id, 
    tpa.data_inicio, 
    tpa.data_fim, 
    tpa.quantidade_horas, 
    tpa.inclui_fimdesemana, 
    tpa.ativo, 
    tpa.data_criacao, 
    tpa.data_alteracao, 
    tpa.observacao, 
    tpa.oportunidade, 
    tpa.prioritario, 
    tpa.percentual,
    tca.codigo_colaborador, 
    tca.codigo_projeto, 
    tca.tb_colaborador_cpf, 
    tca.tb_org_id, 
    tca.cod_tbd_alocado,
    null -- , tca.tb_atividade_id
FROM 
    tb_periodo_alocacao tpa
JOIN 
    tb_colaborador_alocado tca ON tpa.tb_colaborador_alocado_id = tca.id
JOIN 
	tb_colaborador tc ON tc.cpf = tca.tb_colaborador_cpf
JOIN
	tb_colaborador_org tco ON tca.tb_org_id = tco.tb_org_id and tc.cpf = tco.tb_colaborador_cpf
							   and tca.codigo_colaborador = tco.cod_colaborador_externo
WHERE
    tca.tb_colaborador_cpf = tc.cpf and tpa.ativo = 1;