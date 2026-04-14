-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Flags de publicação oficial (compatibilidade com informativo)
-- Data: 2026-03-31
-- Objetivo:
--  - Adicionar colunas *_oficial em tb_mkt_grupo
--  - Sem backfill automático: publicação oficial passa a exigir bit próprio (não herda do informativo)
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_grupo
    ADD COLUMN permite_criar_publicacao_oficial TINYINT(1) NOT NULL DEFAULT 0
        AFTER tb_org_id,
    ADD COLUMN publicacao_oficial_requer_aprovacao TINYINT(1) NOT NULL DEFAULT 0
        AFTER permite_criar_publicacao_oficial,
    ADD COLUMN aprova_publicacao_oficial TINYINT(1) NOT NULL DEFAULT 0
        AFTER publicacao_oficial_requer_aprovacao;
