-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Orgs podem forcar envio de e-mail para publicacao
-- oficial a todos os colaboradores com e-mail, ignorando notifica_email do usuario.
-- Data: 2026-03-26
-- Pasta: scripts/2026/03_Mar/26/16854
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_org_config
    ADD COLUMN enviar_email_publicacao_oficial_sempre TINYINT(1) NOT NULL DEFAULT 0
        COMMENT '1 = rotina envia e-mail de comunicacao oficial a todos ativos com e-mail; 0 = respeita notifica_email em tb_mkt_colaborador_configuracao'
        AFTER somente_publicacao_oficial_no_feed;


UPDATE tb_mkt_org_config
SET enviar_email_publicacao_oficial_sempre = 1
WHERE tb_org_id = 2;
