-- Log de request/response do Match Semântico (best_candidates/hyde)
CREATE TABLE IF NOT EXISTS tb_labs_log_match_semantico (
  id CHAR(36) NOT NULL COMMENT 'ID único (GUID) retornado para o front',
  tb_org_id INT NOT NULL,
  data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  codigo_interno_colaborador VARCHAR(36) NULL,
  objeto_request LONGTEXT NULL,
  objeto_response LONGTEXT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
