CREATE TABLE tb_organograma_posicao (
  id varchar(36) NOT NULL,
  tb_org_id int  NOT NULL,
  tb_organograma_departamento_id varchar(36) DEFAULT NULL,
  tb_gestor_externo_perfil_id char(36) CHARACTER SET utf8mb3 NOT NULL,
  tb_organograma_posicao_id_superior varchar(36) DEFAULT NULL,
  ativo tinyint DEFAULT 1,
  data_criacao timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY idx_org_pos_org (tb_org_id),
  KEY idx_org_pos_superior (tb_organograma_posicao_id_superior),
  KEY idx_org_pos_perfil (tb_gestor_externo_perfil_id),
  KEY idx_org_pos_dept (tb_organograma_departamento_id),
  CONSTRAINT fk_org_pos_org FOREIGN KEY (tb_org_id) REFERENCES tb_org (id),
  CONSTRAINT fk_org_pos_perfil FOREIGN KEY (tb_gestor_externo_perfil_id) REFERENCES tb_gestor_externo_perfil (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
