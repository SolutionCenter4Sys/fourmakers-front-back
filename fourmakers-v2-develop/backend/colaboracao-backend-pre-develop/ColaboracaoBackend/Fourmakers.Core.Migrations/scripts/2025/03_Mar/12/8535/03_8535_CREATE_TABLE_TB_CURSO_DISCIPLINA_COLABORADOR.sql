CREATE TABLE tb_curso_disciplina_colaborador
(
            tb_curso_id      VARCHAR(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
            tb_disciplina_id VARCHAR(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
            tb_org_id        INT NOT NULL,
            data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
            data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
            codigo_interno_colaborador VARCHAR(36) NOT NULL,
            semestre                   INT NOT NULL,
            ano                        INT DEFAULT NULL,
            PRIMARY KEY (tb_curso_id,tb_disciplina_id,tb_org_id,codigo_interno_colaborador),
            KEY tb_curso_disciplina_colaborador_tb_colaborador_fk (codigo_interno_colaborador),
            KEY tb_curso_disciplina_colaborador_tb_disciplina_fk (tb_disciplina_id),
            KEY tb_curso_disciplina_colaborador_tb_org_fk (tb_org_id),
            CONSTRAINT tb_curso_disciplina_colaborador_tb_colaborador_fk FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador (codigo_interno_colaborador) ON
DELETE CASCADE
ON
UPDATE CASCADE,
    CONSTRAINT tb_curso_disciplina_colaborador_tb_curso_fk FOREIGN KEY (tb_curso_id) REFERENCES tb_curso (id)
ON
DELETE CASCADE
ON
UPDATE CASCADE,
    CONSTRAINT tb_curso_disciplina_colaborador_tb_disciplina_fk FOREIGN KEY (tb_disciplina_id) REFERENCES tb_disciplina (id)
ON
DELETE CASCADE
ON
UPDATE CASCADE,
    CONSTRAINT tb_curso_disciplina_colaborador_tb_org_fk FOREIGN KEY (tb_org_id) REFERENCES tb_org (id)
ON
DELETE CASCADE
ON
UPDATE CASCADE
);