-- Adiciona coluna ativo em tb_mkt_comunidade.
-- Comunidades inativas (ativo = 0) não aparecem no GET geral (listagem).
-- Valor padrão 1 (ativo) para registros existentes e novos.

ALTER TABLE tb_mkt_comunidade
    ADD COLUMN ativo TINYINT(1) NOT NULL DEFAULT 1
    AFTER publicacao_permite_like_habilitado;

-- Índice para filtrar listagem por ativo = 1
ALTER TABLE tb_mkt_comunidade
    ADD KEY ix_tb_mkt_comunidade_ativo (ativo);
