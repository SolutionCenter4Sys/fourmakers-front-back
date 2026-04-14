-- ------------------------------------------------------------------------------
-- Notificacao in-app: URL customizada por registro (precedência sobre tb_funcionalidade_rota).
-- Valor: URL absoluta (http/https) ou path relativo (ex.: comunicacao/post?id=...).
-- Data: 2026-03-30
-- Pasta: scripts/2026/03_Mar/30/16999
-- ------------------------------------------------------------------------------

ALTER TABLE tb_notificacao
    ADD COLUMN url_customizada VARCHAR(2048) NULL
        COMMENT 'URL completa ou path relativo ao URL_BASE; se NULL, usa rota de tb_funcionalidade_rota.'
        AFTER tb_funcionalidade_sistema_id;
