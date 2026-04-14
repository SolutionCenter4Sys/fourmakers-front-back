-- =============================================================================
-- Pacote 16694 — Admissão (CBO, cargo, remuneração CLT + logs)
-- tb_admissao_cbo — CBO (Classificação Brasileira de Ocupações)
-- Dependências: nenhuma
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_cbo (
  id CHAR(36) NOT NULL,
  codigo VARCHAR(20) NOT NULL COMMENT 'Código CBO (ex: 2124-05)',
  titulo VARCHAR(500) NOT NULL COMMENT 'Título da ocupação',
  descricao TEXT NULL COMMENT 'Descrição detalhada (ex.: CBO / site do governo)',
  ativo TINYINT NOT NULL DEFAULT 1,
  data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uk_admissao_cbo_codigo (codigo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='CBO (Admissão) — Classificação Brasileira de Ocupações';
