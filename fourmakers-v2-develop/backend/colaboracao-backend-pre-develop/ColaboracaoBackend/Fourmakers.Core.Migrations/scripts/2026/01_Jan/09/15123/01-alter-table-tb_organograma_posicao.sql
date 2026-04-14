ALTER TABLE tb_organograma_posicao
	ADD COLUMN  data_alteracao 	timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ;

ALTER TABLE tb_organograma_posicao
	ADD COLUMN  c_level tinyint DEFAULT 0;

ALTER TABLE tb_organograma_posicao
	ADD COLUMN  profissional_externo tinyint DEFAULT 0;


