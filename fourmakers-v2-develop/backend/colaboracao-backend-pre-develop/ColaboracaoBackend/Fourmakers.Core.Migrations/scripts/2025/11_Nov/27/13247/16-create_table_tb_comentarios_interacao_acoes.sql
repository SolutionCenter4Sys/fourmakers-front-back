CREATE TABLE tb_comentarios_interacao_acoes (
  id  					BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  tb_interacao_acoes_id INT NOT NULL,
  tb_colaborador_codigo_interno_colaborador  varchar(36) DEFAULT NULL,
  data                  TIMESTAMP  DEFAULT CURRENT_TIMESTAMP,
  comentario            TEXT CHARACTER SET utf8mb4  COLLATE utf8mb4_unicode_ci  NULL,
  PRIMARY KEY (id)
);
