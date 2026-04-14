-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Bit por org: indica se o feed deve priorizar
-- somente comunicados oficiais (o front repassa ao ObterListaPublicacaoGeral).
-- Data: 2026-03-20
-- Pasta: scripts/2026/03_Mar/20/16670
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_org_config
    ADD COLUMN somente_publicacao_oficial_no_feed TINYINT(1) NOT NULL DEFAULT 0
        COMMENT '1 = UI pode filtrar feed por publicacao_oficial; exposto em GET Grupo/PermissoesUsuarioLogado'
        AFTER url_foto_alternativa;


UPDATE tb_mkt_org_config
	SET somente_publicacao_oficial_no_feed=1
	WHERE tb_org_id=2;