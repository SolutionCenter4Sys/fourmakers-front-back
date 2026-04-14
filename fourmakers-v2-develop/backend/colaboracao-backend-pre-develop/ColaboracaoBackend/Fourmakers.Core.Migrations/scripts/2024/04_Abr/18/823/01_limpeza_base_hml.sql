-- Criar tabela temporária
CREATE TEMPORARY TABLE IF NOT EXISTS temp_tb_colaborador_cpf
SELECT tb_colaborador_cpf 
FROM tb_colaborador_org
WHERE tb_org_id = 4 AND tb_colaborador_cpf <> '22452744077';

-- Deletar alocações em projetos
DELETE FROM tb_colaborador_projeto_org WHERE tb_org_id = 4;

-- Deletar registros de tb_periodo_alocacao
DELETE tpa
FROM tb_colaborador_alocado tca
JOIN tb_periodo_alocacao tpa ON tca.id = tpa.tb_colaborador_alocado_id 
WHERE tca.tb_org_id = 4;

-- Deletar registros da tabela tb_colaborador_alocado
DELETE FROM tb_colaborador_alocado WHERE tb_org_id = 4;

-- Deletar registros de apontamentos da org
DELETE FROM tb_colaborador_apontamento WHERE tb_org_id = 4;
DELETE FROM tb_colaborador_apontamento_log WHERE tb_org_id = 4;

-- Deletar registro da tabela hierarquia
DELETE FROM tb_colaborador_hierarquia WHERE tb_org_id = 4;

-- Excluir registros da tabela tb_colaborador_sobre
DELETE FROM tb_colaborador_sobre
WHERE cpf IN (SELECT tb_colaborador_cpf FROM temp_tb_colaborador_cpf);

-- Excluir registros da tabela tb_colaborador_passaporte
DELETE FROM tb_colaborador_passaporte
WHERE colaborador_cpf IN (SELECT tb_colaborador_cpf FROM temp_tb_colaborador_cpf);

-- Excluir registros da tabela tb_colaborador_competencia
DELETE FROM tb_colaborador_competencia
WHERE colaborador_cpf IN (SELECT tb_colaborador_cpf FROM temp_tb_colaborador_cpf);

-- Excluir registros da tabela tb_colaborador
DELETE FROM tb_colaborador
WHERE cpf IN (SELECT tb_colaborador_cpf FROM temp_tb_colaborador_cpf);

-- Excluir registros da tabela tb_usuario
DELETE FROM tb_usuario
WHERE cpf IN (SELECT tb_colaborador_cpf FROM temp_tb_colaborador_cpf);

-- Excluir registros da tabela tb_colaborador_org
DELETE FROM tb_colaborador_org WHERE tb_org_id = 4;

-- Deletar registros da tabela de gerentes de projetos
DELETE FROM tb_projeto_gerente WHERE tb_org_id = 4;

-- Deletar registros da projetos x atividades
DELETE FROM tb_projeto_org_atividade WHERE tb_projeto_tb_org_id = 4;

-- Deletar registros da tabela de projetos
DELETE FROM tb_projeto_org WHERE tb_org_id = 4;

-- Remover tabela temporária
DROP TEMPORARY TABLE temp_tb_colaborador_cpf;
