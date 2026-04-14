-- =============================================================================
-- tb_admissao_cargo_log — auditoria em tb_admissao_cargo
-- Dependências: tb_admissao_cargo (03)
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_cargo_log (
  id CHAR(36) NOT NULL,
  tb_admissao_cargo_id CHAR(36) NOT NULL COMMENT 'ID do cargo (tb_admissao_cargo)',
  acao VARCHAR(20) NOT NULL COMMENT 'CREATE | UPDATE',
  alterador_cpf VARCHAR(100) NOT NULL COMMENT 'CPF do usuário',
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  objeto LONGTEXT NOT NULL COMMENT 'JSON estado anterior (UPDATE) ou criado (CREATE)',
  alteracoes LONGTEXT NOT NULL COMMENT 'JSON novo estado',
  PRIMARY KEY (id),
  KEY idx_tb_admissao_cargo_log_cargo (tb_admissao_cargo_id),
  KEY idx_tb_admissao_cargo_log_data (data_alteracao),
  CONSTRAINT fk_tb_admissao_cargo_log_cargo
    FOREIGN KEY (tb_admissao_cargo_id) REFERENCES tb_admissao_cargo (id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Log — tb_admissao_cargo';
