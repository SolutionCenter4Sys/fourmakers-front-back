CREATE TABLE tb_curso_disciplina
(
              tb_curso_id      VARCHAR(36) NOT NULL,
              tb_disciplina_id VARCHAR(36) NOT NULL,
              tb_org_id        INT NOT NULL,
              data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              PRIMARY KEY (tb_curso_id,tb_disciplina_id,tb_org_id),
              KEY tb_curso_disciplina_tb_disciplina_fk (tb_disciplina_id),
              KEY tb_curso_disciplina_tb_org_fk (tb_org_id),
              CONSTRAINT tb_curso_disciplina_tb_curso_fk FOREIGN KEY (tb_curso_id) REFERENCES tb_curso (id) ON
DELETE CASCADE
ON
UPDATE CASCADE,
       CONSTRAINT tb_curso_disciplina_tb_disciplina_fk FOREIGN KEY (tb_disciplina_id) REFERENCES tb_disciplina (id)
ON
DELETE CASCADE
ON
UPDATE CASCADE,
       CONSTRAINT tb_curso_disciplina_tb_org_fk FOREIGN KEY (tb_org_id) REFERENCES tb_org (id)
ON
DELETE CASCADE
ON
UPDATE CASCADE
)