UPDATE tb_colaborador_apontamento a
JOIN tb_colaborador_apontamento_log log ON log.colaborador_apontamento_id = a.id
SET a.tb_colaborador_cpf_criacao = log.tb_colaborador_cpf_criacao
WHERE 
	log.tb_status_apontamento_anterior_id IS NULL
	AND log.tb_colaborador_cpf_criacao IS NOT NULL;