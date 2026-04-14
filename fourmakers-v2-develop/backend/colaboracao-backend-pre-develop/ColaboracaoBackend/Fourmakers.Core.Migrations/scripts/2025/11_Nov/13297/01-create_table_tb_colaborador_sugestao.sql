CREATE TABLE tb_colaborador_sugestao (
  id          varchar(36) NOT NULL,
  codigo_interno_colaborador varchar(36) DEFAULT NULL,
  codigo_interno_colaborador_sugeriu varchar(36) DEFAULT NULL,
  codigo_gestor_adm varchar(36) DEFAULT NULL,
  codigo_gestor_oper varchar(36) DEFAULT NULL,
  codigo_cliente varchar(36) DEFAULT NULL,
  tipo_id     int,
  skill_id    int,
  perfil_id   int,
  senioridade_id bigint DEFAULT NULL,
  data        timestamp  DEFAULT CURRENT_TIMESTAMP,
  ativo       tinyint NOT NULL DEFAULT 1,
  PRIMARY KEY (id)
);
