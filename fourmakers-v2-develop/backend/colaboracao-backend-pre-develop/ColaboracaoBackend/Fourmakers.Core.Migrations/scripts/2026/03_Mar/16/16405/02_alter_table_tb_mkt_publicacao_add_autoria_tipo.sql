-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Autoria da publicacao (pessoal vs alternativo)
-- Data: 2026-03-16
-- Pasta: scripts/2026/03_Mar/16/16405
-- pessoal = exibe criador; alternativo = exibe nome em Autor.NomeAutorAlternativo (tb_mkt_org_config.nome_autor_alternativo).
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_publicacao
    ADD COLUMN autoria_tipo ENUM('pessoal','alternativo') NOT NULL DEFAULT 'pessoal'
        COMMENT 'pessoal = exibe criador; alternativo = exibe nome em Autor.NomeAutorAlternativo (config org)'
        AFTER codigo_interno_colaborador_criacao;
