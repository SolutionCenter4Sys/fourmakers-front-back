-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Adiciona subtitulo (headline) em tb_mkt_publicacao
-- Data: 2026-03-16
-- Pasta: scripts/2026/03_Mar/16/16404
-- Segundo titulo / subtitulo / headline da publicacao (comunicado).
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_publicacao
    ADD COLUMN subtitulo VARCHAR(255) NULL
        COMMENT 'Subtitulo ou headline da publicacao (segundo titulo)'
        AFTER titulo;
