ALTER TABLE tb_historico_sugestao
		ADD CONSTRAINT k_tb_historico_sugestao FOREIGN KEY (tb_status_sugestao_id) 
											  REFERENCES tb_status_sugestao(id);

