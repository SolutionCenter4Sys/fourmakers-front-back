-- =============================================================================
-- 16695 — Multi-tenant: tb_org_id em tb_admissao_cbo + UK (org, codigo)
-- Pré-requisito: tb_org
-- Backfill: org 1 (Fourmakers) para cargas legadas sem org
-- =============================================================================
ALTER TABLE tb_admissao_cbo
  ADD COLUMN tb_org_id INT NULL COMMENT 'Organização (multi-tenant)' AFTER data_alteracao;

UPDATE tb_admissao_cbo SET tb_org_id = 1 WHERE tb_org_id IS NULL;

ALTER TABLE tb_admissao_cbo
  MODIFY COLUMN tb_org_id INT NOT NULL;

ALTER TABLE tb_admissao_cbo DROP INDEX uk_admissao_cbo_codigo;

ALTER TABLE tb_admissao_cbo
  ADD UNIQUE KEY uk_admissao_cbo_org_codigo (tb_org_id, codigo);

ALTER TABLE tb_admissao_cbo
  ADD CONSTRAINT fk_admissao_cbo_org
    FOREIGN KEY (tb_org_id) REFERENCES tb_org (id)
    ON DELETE RESTRICT ON UPDATE CASCADE;
