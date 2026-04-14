CREATE TABLE tb_solicitacao_documento (
      id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
      tipo VARCHAR(5) NOT NULL,
      url VARCHAR(300) NOT NULL,
      relative_path varchar(300) NOT NULL,
      tb_solicitacao_reembolso_id INT NOT NULL,

      CONSTRAINT fk_documento_reembolso FOREIGN KEY (tb_solicitacao_reembolso_id) REFERENCES tb_solicitacao_reembolso(id)
);