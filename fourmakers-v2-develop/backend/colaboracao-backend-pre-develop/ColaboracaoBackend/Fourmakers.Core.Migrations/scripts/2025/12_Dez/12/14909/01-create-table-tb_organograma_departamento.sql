CREATE TABLE tb_organograma_departamento (
  id varchar(36) NOT NULL,
  tb_org_id int  NOT NULL,
  nome varchar(150) NOT NULL,
  codigo_cliente varchar(45) CHARACTER SET utf8mb3 NOT NULL,
  tb_organograma_posicao_id_lider varchar(36) NOT NULL, 
  ativo tinyint DEFAULT 1,
  data_criacao timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY idx_id_data (id, data_criacao),
  KEY idx_org_dept_org (tb_org_id),
  KEY idx_org_dept_lider (tb_organograma_posicao_id_lider),
  CONSTRAINT fk_org_dept_org FOREIGN KEY (tb_org_id) REFERENCES tb_org (id),
  CONSTRAINT fk_cliente_org FOREIGN KEY (codigo_cliente) REFERENCES tb_cliente_org (codigo_cliente)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

