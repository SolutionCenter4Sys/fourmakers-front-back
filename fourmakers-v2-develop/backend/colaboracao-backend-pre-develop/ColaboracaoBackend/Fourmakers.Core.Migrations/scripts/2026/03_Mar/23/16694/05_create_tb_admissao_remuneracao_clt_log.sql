-- =============================================================================
-- tb_admissao_remuneracao_clt_log — auditoria em tb_admissao_remuneracao_clt
-- Dependências: tb_admissao_remuneracao_clt (04)
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_remuneracao_clt_log (
  id CHAR(36) NOT NULL,
  tb_admissao_remuneracao_clt_id CHAR(36) NOT NULL COMMENT 'ID em tb_admissao_remuneracao_clt',
  acao VARCHAR(20) NOT NULL COMMENT 'CREATE | UPDATE',
  alterador_cpf VARCHAR(100) NOT NULL COMMENT 'CPF do usuário',
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  objeto LONGTEXT NOT NULL COMMENT 'JSON estado anterior (UPDATE) ou criado (CREATE)',
  alteracoes LONGTEXT NOT NULL COMMENT 'JSON novo estado / diff',
  PRIMARY KEY (id),
  KEY idx_tb_admissao_remuneracao_clt_log_tb_id (tb_admissao_remuneracao_clt_id),
  KEY idx_tb_admissao_remuneracao_clt_log_data (data_alteracao),
  CONSTRAINT fk_tb_admissao_remuneracao_clt_log_admissao_remuneracao
    FOREIGN KEY (tb_admissao_remuneracao_clt_id) REFERENCES tb_admissao_remuneracao_clt (id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Log — tb_admissao_remuneracao_clt';
