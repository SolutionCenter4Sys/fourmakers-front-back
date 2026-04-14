CREATE TABLE tb_curso
(
              id                                 VARCHAR(36) NOT NULL,
              id_externo                         VARCHAR(255) NOT NULL,
              tb_org_id                          INT NOT NULL,
              descricao                          VARCHAR(255) NOT NULL,
              codigo_interno_colaborador_criacao VARCHAR(36) NOT NULL,
              ativo                              TINYINT DEFAULT '1',
              data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
              PRIMARY KEY (id),
              UNIQUE KEY tb_cursoid_externo_idx (id_externo,tb_org_id) using btree,
              KEY tb_curso_tb_org_fk (tb_org_id),
              KEY tb_curso_tb_colaborador_fk (codigo_interno_colaborador_criacao),
              CONSTRAINT tb_curso_tb_colaborador_fk FOREIGN KEY (codigo_interno_colaborador_criacao) REFERENCES tb_colaborador (codigo_interno_colaborador) ON
DELETE CASCADE
ON
UPDATE CASCADE,
       CONSTRAINT tb_curso_tb_org_fk FOREIGN KEY (tb_org_id) REFERENCES tb_org (id)
ON
DELETE CASCADE
ON
UPDATE CASCADE
)