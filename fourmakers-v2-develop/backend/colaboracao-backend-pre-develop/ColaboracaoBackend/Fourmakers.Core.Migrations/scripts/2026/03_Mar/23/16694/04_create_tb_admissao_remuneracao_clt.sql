-- =============================================================================
-- tb_admissao_remuneracao_clt — faixas, CBO opcional e piso por cargo de admissão
-- Dependências: tb_admissao_cargo (03), tb_admissao_cbo (00)
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_remuneracao_clt (
  id CHAR(36) NOT NULL,
  tb_admissao_cargo_id CHAR(36) NOT NULL COMMENT 'Cargo (tb_admissao_cargo)',
  faixa1_inicio DECIMAL(15,2) NULL,
  faixa1_final DECIMAL(15,2) NULL,
  faixa2_inicio DECIMAL(15,2) NULL,
  faixa2_final DECIMAL(15,2) NULL,
  faixa3_inicio DECIMAL(15,2) NULL,
  faixa3_final DECIMAL(15,2) NULL,
  faixa4_inicio DECIMAL(15,2) NULL,
  faixa4_final DECIMAL(15,2) NULL,
  tb_cbo_id CHAR(36) NULL COMMENT 'CBO (tb_admissao_cbo)',
  piso DECIMAL(15,2) NULL,
  ativo TINYINT NOT NULL DEFAULT 1,
  data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uk_admissao_remuneracao_cargo (tb_admissao_cargo_id),
  KEY idx_admissao_remuneracao_cbo (tb_cbo_id),
  CONSTRAINT fk_admissao_remuneracao_cargo
    FOREIGN KEY (tb_admissao_cargo_id) REFERENCES tb_admissao_cargo (id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_admissao_remuneracao_cbo
    FOREIGN KEY (tb_cbo_id) REFERENCES tb_admissao_cbo (id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Remuneração CLT por cargo de admissão (GUID)';
