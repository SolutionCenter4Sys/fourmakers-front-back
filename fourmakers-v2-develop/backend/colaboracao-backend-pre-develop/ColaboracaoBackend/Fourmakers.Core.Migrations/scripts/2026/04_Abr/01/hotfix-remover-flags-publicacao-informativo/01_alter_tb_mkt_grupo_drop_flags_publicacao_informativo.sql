-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Remover flags de publicação informativo
-- Data: 2026-04-01
-- Objetivo:
--  - Remover colunas *_informativo de tb_mkt_grupo
--  - A partir daqui, permissões passam a existir somente para *_oficial
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_grupo
    DROP COLUMN permite_criar_publicacao_informativo,
    DROP COLUMN publicacao_informativo_requer_aprovacao,
    DROP COLUMN aprova_publicacao_informativo;

