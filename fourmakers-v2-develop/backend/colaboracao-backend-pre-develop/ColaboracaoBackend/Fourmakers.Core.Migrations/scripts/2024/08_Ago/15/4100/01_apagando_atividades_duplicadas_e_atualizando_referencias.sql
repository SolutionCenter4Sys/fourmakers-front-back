-- Criando tabela temporária com atividades únicas por org
CREATE TEMPORARY TABLE tb_atividade_temp AS
SELECT MIN(id) AS id, descricao, tb_org_id
FROM tb_atividade
GROUP BY descricao, tb_org_id;

-- Atualizando referências tb_projeto_org_atividade
UPDATE tb_projeto_org_atividade pa
JOIN tb_atividade a ON pa.tb_atividade_id = a.id
JOIN tb_atividade_temp at ON a.descricao = at.descricao AND a.tb_org_id = at.tb_org_id
SET pa.tb_atividade_id = at.id;

-- Atualizando referências tb_colaborador_apontamento
UPDATE tb_colaborador_apontamento ca
JOIN tb_atividade a ON ca.tb_atividade_id = a.id
JOIN tb_atividade_temp at ON a.descricao = at.descricao AND a.tb_org_id = at.tb_org_id
SET ca.tb_atividade_id = at.id;

-- Atualizando referências tb_colaborador_apontamento_log
UPDATE tb_colaborador_apontamento_log cal
JOIN tb_atividade a ON cal.tb_atividade_id = a.id
JOIN tb_atividade_temp at ON a.descricao = at.descricao AND a.tb_org_id = at.tb_org_id
SET cal.tb_atividade_id = at.id;

-- Atualizando referências tb_colaborador_periodo_alocacao
UPDATE tb_colaborador_periodo_alocacao cpa
JOIN tb_atividade a ON cpa.tb_atividade_id = a.id
JOIN tb_atividade_temp at ON a.descricao = at.descricao AND a.tb_org_id = at.tb_org_id
SET cpa.tb_atividade_id = at.id;

-- Apagando as duplicadas
DELETE FROM tb_atividade
WHERE id NOT IN (SELECT id FROM tb_atividade_temp);

--Apagando tabela temporária
DROP TEMPORARY TABLE tb_atividade_temp;