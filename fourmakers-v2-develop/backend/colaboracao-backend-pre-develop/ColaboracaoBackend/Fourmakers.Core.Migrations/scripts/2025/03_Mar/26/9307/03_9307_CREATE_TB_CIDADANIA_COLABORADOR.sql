CREATE TABLE tb_cidadania_colaborador (
  id INT PRIMARY KEY AUTO_INCREMENT,
  codigo_interno_colaborador VARCHAR(36) NOT NULL,
  tb_cidadania_id INT NOT NULL,
  tb_cidadania_status_id INT NOT NULL,
  ativo TINYINT(1) NOT NULL DEFAULT 1,
  data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (tb_cidadania_id) REFERENCES tb_cidadania(id),
  FOREIGN KEY (tb_cidadania_status_id) REFERENCES tb_cidadania_status(id),
  FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador)
);