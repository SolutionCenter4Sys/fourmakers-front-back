-- Adicionar novos campos na tb_rubrica_colaborador
ALTER TABLE tb_rubrica_colaborador 
ADD COLUMN codigo_carga_rubrica VARCHAR(36),
ADD COLUMN descricao VARCHAR(500),
ADD INDEX idx_codigo_carga (codigo_carga_rubrica);