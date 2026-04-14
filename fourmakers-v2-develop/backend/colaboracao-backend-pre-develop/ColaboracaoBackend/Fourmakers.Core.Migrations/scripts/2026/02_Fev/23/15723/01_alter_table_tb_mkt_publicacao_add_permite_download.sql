-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Adiciona permite_download em tb_mkt_publicacao
-- Data: 2026-02-23
-- Pasta: scripts/2026/02_Fev/23/marketing-comunicacao
-- Usado quando tipo = 'documento'; controla se o colaborador pode baixar anexos.
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_publicacao
    ADD COLUMN permite_download TINYINT(1) NOT NULL DEFAULT 0
        COMMENT '1 = permite download dos anexos (quando tipo = documento), 0 = nao permite'
        AFTER publicacao_fixada_pelo_responsavel;
