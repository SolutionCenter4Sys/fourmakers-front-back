ALTER TABLE tb_colaborador_sugestao
ADD CONSTRAINT k_tb_nivel_senioridade FOREIGN KEY (senioridade_id) 
											  REFERENCES tb_nivel(id);
