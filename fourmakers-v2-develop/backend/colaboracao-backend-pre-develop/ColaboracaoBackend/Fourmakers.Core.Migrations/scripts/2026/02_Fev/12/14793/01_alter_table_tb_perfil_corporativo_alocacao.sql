ALTER TABLE tb_perfil_corporativo_alocacao
	ADD COLUMN criado_por_mapa_relacionamento TINYINT(1) NOT NULL DEFAULT 0
		COMMENT '1 = criada pelo mapa de relacionamento, 0 = criada por outros fluxos'
		AFTER ativo;