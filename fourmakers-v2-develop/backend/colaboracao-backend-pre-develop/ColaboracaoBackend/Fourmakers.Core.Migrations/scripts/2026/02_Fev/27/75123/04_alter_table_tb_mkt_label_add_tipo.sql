-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Tipo da label (documento ou informativo)
-- Data: 2026-02-27
-- Adiciona coluna tipo em tb_mkt_label.
-- Valores permitidos: 'informativo', 'documento'.
-- Indice unico passa a ser (nome + tipo + tb_org_id).
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_label
    ADD COLUMN tipo VARCHAR(20) NOT NULL DEFAULT 'informativo'
    AFTER nome;

-- Remove indice unico antigo (nome + org)
ALTER TABLE tb_mkt_label
    DROP INDEX uq_tb_mkt_label_nome_org;

-- Novo indice unico: nome + tipo + org (permite mesmo nome com tipos diferentes na mesma org)
ALTER TABLE tb_mkt_label
    ADD UNIQUE INDEX uq_tb_mkt_label_nome_tipo_org (nome, tipo, tb_org_id);
