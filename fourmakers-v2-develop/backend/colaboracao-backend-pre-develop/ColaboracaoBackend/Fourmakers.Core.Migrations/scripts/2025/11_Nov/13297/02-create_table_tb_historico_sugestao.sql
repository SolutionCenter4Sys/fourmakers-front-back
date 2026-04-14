CREATE  TABLE tb_historico_sugestao(
  id          varchar(36) NOT NULL,
  codigo_interno_colaborador_avaliador varchar(36),
  tb_colaborador_sugestao_id varchar(36),
  aprovado tinyint,
  tb_status_sugestao_id int NOT NULL,
  codigo_interno_colaborador varchar(36),
  perfil_id int,
  observacao TEXT CHARACTER SET utf8mb4  COLLATE utf8mb4_unicode_ci  NULL,
  data timestamp  DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
);
