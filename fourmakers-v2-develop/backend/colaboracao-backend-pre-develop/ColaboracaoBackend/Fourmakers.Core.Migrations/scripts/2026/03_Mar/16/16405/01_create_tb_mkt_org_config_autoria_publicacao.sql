-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Configuracao da org para autoria de publicacao
-- Data: 2026-03-16
-- Pasta: scripts/2026/03_Mar/16/16405
-- Tabela de configuracao por org: nome exibido em Autor.NomeAutorAlternativo
-- quando a publicacao tem autoria_tipo = 'alternativo'. O criador nao e alterado.
-- ------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS tb_mkt_org_config (
    tb_org_id INT NOT NULL,
    nome_autor_alternativo VARCHAR(255) NULL
        COMMENT 'Nome exibido em autor.nomeAutorAlternativo quando publicacao tem autoria_tipo=alternativo (ex.: Foursys)',
    url_foto_alternativa VARCHAR(500) NULL
        COMMENT 'URL da foto exibida em autor.urlFotoAlternativa quando publicacao tem autoria_tipo=alternativo',
    PRIMARY KEY (tb_org_id),
    CONSTRAINT fk_tb_mkt_org_config_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
) ENGINE=InnoDB;
