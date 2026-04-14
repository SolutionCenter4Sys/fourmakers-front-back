-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Permissão de criação de comunidade por grupo
-- Data: 2026-02-27
-- Adiciona coluna permite_criar_comunidade em tb_mkt_grupo.
-- Somente usuários que pertençam a um grupo com este flag poderão criar/editar/excluir comunidades.
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_grupo
    ADD COLUMN permite_criar_comunidade TINYINT(1) NOT NULL DEFAULT 0
    AFTER tb_org_id;
