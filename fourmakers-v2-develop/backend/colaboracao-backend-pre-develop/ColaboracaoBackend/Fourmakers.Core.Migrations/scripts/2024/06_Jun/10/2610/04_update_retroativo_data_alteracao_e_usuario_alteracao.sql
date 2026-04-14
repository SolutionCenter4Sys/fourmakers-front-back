UPDATE tb_colaborador_apontamento AS tca
JOIN (
    SELECT 
        tcal.colaborador_apontamento_id,
        tcal.tb_colaborador_cpf_criacao,
        tcal.data_criacao
    FROM 
        tb_colaborador_apontamento_log AS tcal
    WHERE 
        tcal.tb_status_apontamento_anterior_id IS NOT NULL
        AND tcal.tb_colaborador_cpf_criacao IS NOT NULL
        AND tcal.data_criacao = (
            SELECT MAX(data_criacao)
            FROM tb_colaborador_apontamento_log
            WHERE colaborador_apontamento_id = tcal.colaborador_apontamento_id
            AND tb_status_apontamento_anterior_id IS NOT NULL
            AND tb_colaborador_cpf_criacao IS NOT NULL
        )
) AS subquery
ON tca.id = subquery.colaborador_apontamento_id
SET tca.tb_colaborador_cpf_alteracao = subquery.tb_colaborador_cpf_criacao,
	tca.data_alteracao = subquery.data_criacao;