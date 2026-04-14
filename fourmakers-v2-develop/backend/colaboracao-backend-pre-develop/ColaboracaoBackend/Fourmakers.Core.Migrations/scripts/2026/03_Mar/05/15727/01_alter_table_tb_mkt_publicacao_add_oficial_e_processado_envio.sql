-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Adiciona publicacao_oficial e processado_envio_notificacao_email em tb_mkt_publicacao
-- Data: 2026-03-05
-- Pasta: scripts/2026/03_Mar/05/15727
-- publicacao_oficial: 1 quando a publicacao nao esta vinculada a nenhuma comunidade (sem registro em tb_mkt_comunidade_publicacao).
-- processado_envio_notificacao_email: 1 apos a rotina ter processado envio de notificacao e email para esta publicacao.
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_publicacao
    ADD COLUMN publicacao_oficial TINYINT(1) NOT NULL DEFAULT 0
        COMMENT '1 = publicacao oficial (sem comunidade vinculada), 0 = publicacao em comunidade'
        AFTER permite_download,
    ADD COLUMN processado_envio_notificacao_email TINYINT(1) NOT NULL DEFAULT 0
        COMMENT '1 = rotina ja enviou notificacao e email para esta publicacao'
        AFTER publicacao_oficial;
