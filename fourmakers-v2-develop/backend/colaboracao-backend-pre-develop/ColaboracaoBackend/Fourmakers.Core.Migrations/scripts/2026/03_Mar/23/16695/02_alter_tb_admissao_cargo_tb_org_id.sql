-- =============================================================================
-- 16695 — Multi-tenant: tb_org_id em tb_admissao_cargo + UK (org, descricao)
-- =============================================================================
ALTER TABLE tb_admissao_cargo
  ADD COLUMN tb_org_id INT NULL COMMENT 'Organização (multi-tenant)' AFTER data_alteracao;

UPDATE tb_admissao_cargo SET tb_org_id = 1 WHERE tb_org_id IS NULL;

ALTER TABLE tb_admissao_cargo
  MODIFY COLUMN tb_org_id INT NOT NULL;

ALTER TABLE tb_admissao_cargo DROP INDEX uk_admissao_cargo_descricao;

ALTER TABLE tb_admissao_cargo
  ADD UNIQUE KEY uk_admissao_cargo_org_descricao (tb_org_id, descricao);

ALTER TABLE tb_admissao_cargo
  ADD CONSTRAINT fk_admissao_cargo_org
    FOREIGN KEY (tb_org_id) REFERENCES tb_org (id)
    ON DELETE RESTRICT ON UPDATE CASCADE;
