-- =============================================================================
-- 16768 — Remove tb_vaga.tb_admissao_cargo_id e tb_vaga.tb_nivel_vaga_cod
-- Reverte FKs/índices criados em 16695 e 16743 (nomes conforme scripts originais).
-- Ordem: FK de nível primeiro (referencia tb_nivel_vaga), depois FK de cargo.
-- =============================================================================
ALTER TABLE tb_vaga
  DROP FOREIGN KEY fk_tb_vaga_tb_nivel_vaga;

ALTER TABLE tb_vaga
  DROP FOREIGN KEY fk_tb_vaga_tb_admissao_cargo;

DROP INDEX idx_tb_vaga_tb_nivel_vaga_cod ON tb_vaga;
DROP INDEX idx_tb_vaga_tb_admissao_cargo_id ON tb_vaga;

ALTER TABLE tb_vaga
  DROP COLUMN tb_nivel_vaga_cod,
  DROP COLUMN tb_admissao_cargo_id;
