DROP TABLE tb_labs_log_rank_candidates_ids;
CREATE TABLE tb_labs_log_rank_candidates_ids (
  id CHAR(36) NOT NULL,
  tb_org_id INT NOT NULL,
  data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  id_vaga VARCHAR(36) NULL,
  codigo_interno_colaborador VARCHAR(36) NULL,
  objeto_request LONGTEXT NULL,
  objeto_response LONGTEXT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;