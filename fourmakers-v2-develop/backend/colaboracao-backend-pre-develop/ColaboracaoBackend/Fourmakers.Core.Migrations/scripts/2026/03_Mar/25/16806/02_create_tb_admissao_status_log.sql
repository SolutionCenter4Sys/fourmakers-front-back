-- =============================================================================
-- tb_admissao_status_log — auditoria em tb_admissao_status
-- Dependências: tb_admissao_status (01)
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_status_log (
  id CHAR(36) NOT NULL,
  tb_admissao_status_id CHAR(36) NOT NULL COMMENT 'ID do status (tb_admissao_status)',
  acao VARCHAR(20) NOT NULL COMMENT 'CREATE | UPDATE | DELETE',
  alterador_cpf VARCHAR(100) NOT NULL COMMENT 'CPF do usuário',
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  objeto LONGTEXT NOT NULL COMMENT 'JSON estado anterior (UPDATE/DELETE) ou vazio (CREATE)',
  alteracoes LONGTEXT NOT NULL COMMENT 'JSON novo estado ou registro excluído logicamente',
  PRIMARY KEY (id),
  KEY idx_tb_admissao_status_log_status (tb_admissao_status_id),
  KEY idx_tb_admissao_status_log_data (data_alteracao),
  CONSTRAINT fk_tb_admissao_status_log_status
    FOREIGN KEY (tb_admissao_status_id) REFERENCES tb_admissao_status (id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Log — tb_admissao_status';
