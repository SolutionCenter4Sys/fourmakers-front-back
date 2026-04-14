-- =============================================================================
-- tb_admissao_cargo — cargo de admissão (GUID), vinculado a um CBO
-- Dependências: tb_admissao_cbo (00)
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_cargo (
  id CHAR(36) NOT NULL,
  descricao VARCHAR(100) NOT NULL,
  tb_cbo_id CHAR(36) NULL COMMENT 'CBO obrigatório na API (tb_admissao_cbo); NULL só legado',
  ativo TINYINT NOT NULL DEFAULT 1,
  data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uk_admissao_cargo_descricao (descricao),
  KEY idx_admissao_cargo_cbo (tb_cbo_id),
  CONSTRAINT fk_admissao_cargo_cbo
    FOREIGN KEY (tb_cbo_id) REFERENCES tb_admissao_cbo (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Cargo de admissão (GUID)';
