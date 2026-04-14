CREATE TABLE tb_fourmakers_recomendacao_avaliacao (
      id BIGINT AUTO_INCREMENT PRIMARY KEY,
      tb_org_id INT NOT NULL,
      codigo_interno_colaborador VARCHAR(100) NOT NULL,
      servico_rate TINYINT NOT NULL,
      recomendacao_rate TINYINT NOT NULL,
      experiencia_descricao TEXT NULL,
      aspecto_descricao TEXT NOT NULL,
      data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
      CONSTRAINT chk_servico_rate CHECK (servico_rate BETWEEN 1 AND 5),
      CONSTRAINT chk_recomendacao_rate CHECK (recomendacao_rate BETWEEN 1 AND 5),
      CONSTRAINT fk_avaliacao_org
          FOREIGN KEY (tb_org_id)
              REFERENCES tb_org(id)
              ON DELETE RESTRICT
              ON UPDATE CASCADE
);