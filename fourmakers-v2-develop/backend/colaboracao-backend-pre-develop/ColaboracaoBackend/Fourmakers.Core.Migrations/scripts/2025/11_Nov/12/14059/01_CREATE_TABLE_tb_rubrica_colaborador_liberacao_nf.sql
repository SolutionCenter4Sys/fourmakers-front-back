CREATE TABLE tb_rubrica_colaborador_liberacao_nf (
     id CHAR(36) NOT NULL DEFAULT (UUID()) PRIMARY KEY,
     tb_rubrica_colaborador_id CHAR(36) NOT NULL,
     vigencia_mes INT NOT NULL,
     vigencia_ano INT NOT NULL,
     tb_org_id INT NOT NULL,
     CONSTRAINT fk_rubrica_colaborador_liberacao_nf_rubrica_colaborador
         FOREIGN KEY (tb_rubrica_colaborador_id)
             REFERENCES tb_rubrica_colaborador(id)
             ON DELETE CASCADE
             ON UPDATE CASCADE,
     CONSTRAINT fk_rubrica_colaborador_liberacao_nf_org
         FOREIGN KEY (tb_org_id)
             REFERENCES tb_org(id)
             ON DELETE CASCADE
             ON UPDATE CASCADE,
     INDEX idx_tb_rubrica_colaborador_id (tb_rubrica_colaborador_id),
     INDEX idx_tb_org_id (tb_org_id)
)