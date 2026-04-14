-- ------------------------------------------------------------------------------
-- PBI 15728: Marketing / Comunicacao - Permissão de acesso a Analytics por grupo
-- Data: 2026-03-05
-- Adiciona coluna permite_acessar_analytics em tb_mkt_grupo.
-- Usuários que pertençam a um grupo com este flag terão PermiteAcessarAnalytics = true em PermissoesUsuarioLogado.
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_grupo
    ADD COLUMN permite_acessar_analytics TINYINT(1) NOT NULL DEFAULT 0
    AFTER permite_criar_comunidade;
