ALTER TABLE tb_historico_sugestao
	ADD CONSTRAINT fk_tb_historico_sugestao_colaborador_sugestao
	FOREIGN KEY (tb_colaborador_sugestao_id)
	REFERENCES tb_colaborador_sugestao(id)
	ON DELETE CASCADE;

