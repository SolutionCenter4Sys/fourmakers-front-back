-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Adiciona ocultar_no_feed em tb_mkt_publicacao (16503)
-- Data: 2026-03-17
-- Pasta: scripts/2026/03_Mar/17/16503
-- 1 = publicacao nao aparece no feed; 0 = aparece no feed.
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_publicacao
    ADD COLUMN ocultar_no_feed TINYINT(1) NOT NULL DEFAULT 0
        COMMENT '1 = ocultar no feed; 0 = exibir no feed'
        AFTER autoria_tipo;
