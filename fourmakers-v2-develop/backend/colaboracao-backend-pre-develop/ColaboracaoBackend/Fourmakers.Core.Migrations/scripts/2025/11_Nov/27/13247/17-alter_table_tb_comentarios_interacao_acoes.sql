ALTER TABLE tb_comentarios_interacao_acoes
ADD CONSTRAINT fk_tb_comentarios_interacao_acoes_id FOREIGN KEY (tb_interacao_acoes_id)
REFERENCES tb_interacao_acoes (id) 
ON DELETE CASCADE ON UPDATE CASCADE;
