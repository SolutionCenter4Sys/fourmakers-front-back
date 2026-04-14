ALTER TABLE tb_interacao_acoes
		ADD COLUMN tb_status_acoes_id int default 2;

ALTER TABLE tb_interacao_acoes
		ADD COLUMN comentario_acao TEXT CHARACTER SET utf8mb4  COLLATE utf8mb4_unicode_ci  NULL;
