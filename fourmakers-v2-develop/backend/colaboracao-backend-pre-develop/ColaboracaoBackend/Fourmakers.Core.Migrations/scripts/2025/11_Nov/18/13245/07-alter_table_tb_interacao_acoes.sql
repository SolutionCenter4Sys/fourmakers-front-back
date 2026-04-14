ALTER TABLE tb_interacao_acoes
ADD CONSTRAINT fk_tb_status_acoes_id FOREIGN KEY (tb_status_acoes_id)
REFERENCES tb_status_acoes (id);
