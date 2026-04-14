-- =============================================================================
-- 16743 — Domínio: nível da vaga (Júnior, Pleno, Sênior, Especialista)
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_nivel_vaga (
  id CHAR(36) NOT NULL COMMENT 'GUID',
  descricao VARCHAR(50) NOT NULL,
  codigo TINYINT NOT NULL COMMENT '1=Júnior, 2=Pleno, 3=Sênior, 4=Especialista',
  PRIMARY KEY (id),
  UNIQUE KEY uk_tb_nivel_vaga_codigo (codigo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Nível da vaga';
