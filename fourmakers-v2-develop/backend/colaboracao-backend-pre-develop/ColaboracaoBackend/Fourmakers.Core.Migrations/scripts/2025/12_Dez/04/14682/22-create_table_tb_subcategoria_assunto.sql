CREATE TABLE tb_subcategoria_assunto (
  id  		INT NOT NULL,
  descricao VARCHAR(150) NOT NULL,
  tb_categoria_assunto_id INT NOT NULL,
  data_criacao TIMESTAMP  DEFAULT CURRENT_TIMESTAMP,
  ativo tinyint NOT NULL DEFAULT '1',
  PRIMARY KEY (tb_categoria_assunto_id, id),
  CONSTRAINT fk_subcategoria_assunto
  FOREIGN KEY (tb_categoria_assunto_id)
  REFERENCES tb_categoria_assunto(id)
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

