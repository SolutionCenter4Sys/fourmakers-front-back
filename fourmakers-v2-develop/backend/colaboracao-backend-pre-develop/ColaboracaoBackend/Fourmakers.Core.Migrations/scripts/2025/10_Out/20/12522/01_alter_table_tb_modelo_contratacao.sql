ALTER TABLE tb_modelo_contratacao_org
    ADD COLUMN oculta_busca_timesheet tinyint(1) DEFAULT 0;

UPDATE tb_modelo_contratacao_org
SET oculta_busca_timesheet = 1
WHERE codigo_modelo_contratacao IN ('Parceiro Terceiro (PJ)', 'CLT', 'Parceiro (PJ)', 'Estagiário', '-')
  AND tb_org_id = 6;