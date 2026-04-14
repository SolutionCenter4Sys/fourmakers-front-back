CREATE TABLE tb_perfil_corporativo_log (
  id 				varchar(36) NOT NULL,
  tb_colaborador_codigo_interno_colaborador varchar(36) NOT NULL,
  acao           	varchar(20) NOT NULL,
  tb_colaborador_codigo_interno_colaborador_alterador varchar(36) NOT NULL ,
  data_alteracao 	timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ,
  objeto         	longtext NOT NULL ,
  alteracoes     	longtext NOT NULL ,
  PRIMARY KEY (id),
  KEY idx_perfil_corporativo (id, data_alteracao),
  CONSTRAINT fk_pfcorp_log_tb_colaborador_criador FOREIGN KEY (tb_colaborador_codigo_interno_colaborador) REFERENCES tb_colaborador (codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_pfcorp_log_tb_colaborador_alterador FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_alterador) REFERENCES tb_colaborador (codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ;
