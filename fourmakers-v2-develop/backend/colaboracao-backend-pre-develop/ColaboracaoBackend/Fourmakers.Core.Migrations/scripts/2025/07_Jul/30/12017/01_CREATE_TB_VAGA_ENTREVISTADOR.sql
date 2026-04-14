CREATE TABLE tb_vaga_entrevistador (
    tb_vaga_id VARCHAR(36) NOT NULL,
    tb_colaborador_codigo_interno_colaborador_entrevistador VARCHAR(36) NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (tb_vaga_id, tb_colaborador_codigo_interno_colaborador_entrevistador),
    CONSTRAINT fk_vaga_entrevistador_vaga FOREIGN KEY (tb_vaga_id) REFERENCES tb_vaga(id),
    CONSTRAINT fk_vaga_entrevistador_entrevistador FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_entrevistador) REFERENCES tb_colaborador(codigo_interno_colaborador)
); 