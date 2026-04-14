-- =============================================================================
-- tb_admissao_cbo_log — auditoria em tb_admissao_cbo
-- Dependências: tb_admissao_cbo (00)
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_cbo_log (
  id CHAR(36) NOT NULL,
  tb_admissao_cbo_id CHAR(36) NOT NULL COMMENT 'ID do CBO (tb_admissao_cbo)',
  acao VARCHAR(20) NOT NULL COMMENT 'CREATE | UPDATE',
  alterador_cpf VARCHAR(100) NOT NULL COMMENT 'CPF do usuário',
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  objeto LONGTEXT NOT NULL COMMENT 'JSON estado anterior (UPDATE) ou criado (CREATE)',
  alteracoes LONGTEXT NOT NULL COMMENT 'JSON alterações / novo estado',
  PRIMARY KEY (id),
  KEY idx_tb_admissao_cbo_log_tb_id (tb_admissao_cbo_id),
  KEY idx_tb_admissao_cbo_log_data (data_alteracao),
  CONSTRAINT fk_tb_admissao_cbo_log_cbo
    FOREIGN KEY (tb_admissao_cbo_id) REFERENCES tb_admissao_cbo (id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Log de alterações — tb_admissao_cbo';
