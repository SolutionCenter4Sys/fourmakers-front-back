-- =============================================================================
-- tb_admissao_status — status de admissão por organização (análogo a tb_status_vaga + tb_org_id)
-- Dependências: tb_org
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_status (
  id CHAR(36) NOT NULL,
  tb_org_id INT NOT NULL COMMENT 'Organização (tb_org)',
  descricao VARCHAR(100) NOT NULL,
  codigo INT NULL COMMENT 'Código opcional (similar tb_status_vaga.codigo)',
  ordem INT NOT NULL DEFAULT 0,
  ativo TINYINT NOT NULL DEFAULT 1,
  data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uk_admissao_status_org_descricao (tb_org_id, descricao),
  KEY idx_admissao_status_org (tb_org_id),
  KEY idx_admissao_status_org_ordem (tb_org_id, ordem),
  CONSTRAINT fk_admissao_status_org
    FOREIGN KEY (tb_org_id) REFERENCES tb_org (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Status de admissão por organização';
