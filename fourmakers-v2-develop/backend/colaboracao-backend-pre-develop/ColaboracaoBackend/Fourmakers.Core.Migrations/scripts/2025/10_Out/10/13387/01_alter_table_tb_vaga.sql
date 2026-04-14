-- 1? Remover a constraint antiga
ALTER TABLE tb_vaga 
DROP FOREIGN KEY fk_vaga_colaborador_responsavel;

-- 2? Renomear a coluna
ALTER TABLE tb_vaga
CHANGE COLUMN tb_colaborador_codigo_interno_colaborador_responsavel 
tb_colaborador_codigo_interno_colaborador_recrutador VARCHAR(255);

-- 3? Criar novamente a constraint com nome atualizado
ALTER TABLE tb_vaga
ADD CONSTRAINT fk_vaga_colaborador_recrutador
FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_recrutador)
REFERENCES tb_colaborador (codigo_interno_colaborador);