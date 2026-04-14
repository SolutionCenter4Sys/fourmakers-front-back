CREATE TABLE tb_colaborador_curso
(
            tb_curso_id VARCHAR(36) NOT NULL,
            tb_org_id   INT NOT NULL,
            data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
            data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
            codigo_interno_colaborador VARCHAR(36) NOT NULL,
            PRIMARY KEY (tb_org_id,tb_curso_id,codigo_interno_colaborador),
            KEY tb_colaborador_curso_tb_colaborador_fk (codigo_interno_colaborador),
            KEY tb_colaborador_curso_tb_curso_fk (tb_curso_id),
            CONSTRAINT tb_colaborador_curso_tb_colaborador_fk FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador (codigo_interno_colaborador) ON
DELETE CASCADE
ON
UPDATE CASCADE,
    CONSTRAINT tb_colaborador_curso_tb_curso_fk FOREIGN KEY (tb_curso_id) REFERENCES tb_curso (id)
ON
DELETE CASCADE
ON
UPDATE CASCADE,
    CONSTRAINT tb_colaborador_curso_tb_org_fk FOREIGN KEY (tb_org_id) REFERENCES tb_org (id)
ON
DELETE CASCADE
ON
UPDATE CASCADE
);