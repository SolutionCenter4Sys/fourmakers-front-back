CREATE TABLE tb_interacoes_categoria_sub (
id  						BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
tb_interacoes_id 			INT NOT NULL,
tb_categoria_assunto_id 	INT NOT NULL,
tb_subcategoria_assunto_id 	INT NOT NULL,
data_criacao 				TIMESTAMP  DEFAULT CURRENT_TIMESTAMP,
ativo 						tinyint NOT NULL DEFAULT '1',
PRIMARY KEY (id),
UNIQUE KEY idx_interacao_categoria_sub (
        tb_interacoes_id,
        tb_categoria_assunto_id,
        tb_subcategoria_assunto_id
        ),
CONSTRAINT fk_tb_interacoes_id
  FOREIGN KEY (tb_interacoes_id)
  REFERENCES tb_interacoes(id),
CONSTRAINT fk_tb_categoria_assunto_id
  FOREIGN KEY (tb_categoria_assunto_id)
  REFERENCES tb_categoria_assunto(id),
CONSTRAINT fk_tb_subcategoria_assunto_id
FOREIGN KEY (tb_categoria_assunto_id, tb_subcategoria_assunto_id)
REFERENCES tb_subcategoria_assunto(tb_categoria_assunto_id, id)
);

