-- =============================================================================
-- 16743 — tb_vaga.tb_nivel_vaga_cod → tb_nivel_vaga.codigo (domínio numérico 1–4)
-- Requer coluna tb_admissao_cargo_id (16695) para posicionar AFTER; se não existir,
-- remova "AFTER tb_admissao_cargo_id" ou use AFTER cargo.
-- =============================================================================
ALTER TABLE tb_vaga
  ADD COLUMN tb_nivel_vaga_cod TINYINT NULL
    COMMENT 'Nível da vaga (tb_nivel_vaga.codigo)' AFTER tb_admissao_cargo_id;

ALTER TABLE tb_vaga
  ADD CONSTRAINT fk_tb_vaga_tb_nivel_vaga
    FOREIGN KEY (tb_nivel_vaga_cod) REFERENCES tb_nivel_vaga (codigo)
    ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_tb_vaga_tb_nivel_vaga_cod ON tb_vaga (tb_nivel_vaga_cod);
