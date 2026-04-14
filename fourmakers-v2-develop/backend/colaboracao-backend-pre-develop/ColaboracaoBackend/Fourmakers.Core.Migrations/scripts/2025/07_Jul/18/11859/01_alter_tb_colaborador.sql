ALTER TABLE tb_colaborador 
ADD COLUMN qualificado BOOLEAN NULL;

CREATE TABLE tb_colaborador_qualificacao_log (
    tb_colaborador_codigo_interno_colaborador_qualificado VARCHAR(255) NOT NULL,
    tb_colaborador_codigo_interno_colaborador_qualificador VARCHAR(255) NOT NULL,
    qualificado BOOLEAN NOT NULL,
    data_criacao DATETIME NOT NULL
);