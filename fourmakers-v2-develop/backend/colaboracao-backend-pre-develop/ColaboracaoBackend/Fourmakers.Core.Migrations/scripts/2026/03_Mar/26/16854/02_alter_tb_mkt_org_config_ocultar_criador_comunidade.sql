-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Bit por org: ocultar quem criou a comunidade na UI.
-- Data: 2026-03-26
-- Pasta: scripts/2026/03_Mar/26/16854
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_org_config
    ADD COLUMN ocultar_criador_comunidade TINYINT(1) NOT NULL DEFAULT 0
        COMMENT '1 = UI nao exibe criador da comunidade; exposto em GET Grupo/PermissoesUsuarioLogado'
        AFTER enviar_email_publicacao_oficial_sempre;


UPDATE tb_mkt_org_config
SET ocultar_criador_comunidade = 1
WHERE tb_org_id = 2;
