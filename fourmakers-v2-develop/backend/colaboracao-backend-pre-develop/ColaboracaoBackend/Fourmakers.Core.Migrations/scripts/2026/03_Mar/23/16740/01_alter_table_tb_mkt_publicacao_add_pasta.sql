-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Adiciona pasta em tb_mkt_publicacao (16740)
-- Data: 2026-03-23
-- Pasta: scripts/2026/03_Mar/23/16740
-- Texto livre para agrupamento logico no feed (pastas / filtros).
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_publicacao
    ADD COLUMN pasta VARCHAR(255) NULL
        COMMENT 'Pasta logica para agrupamento no feed'
        AFTER ocultar_no_feed;
