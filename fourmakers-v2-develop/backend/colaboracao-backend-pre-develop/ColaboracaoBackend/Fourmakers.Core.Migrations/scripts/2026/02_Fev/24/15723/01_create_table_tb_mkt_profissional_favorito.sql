-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Favoritar profissional
-- Só existem registros de quem está favoritado; ao desfavoritar, remove o registro.
-- codigo_interno_colaborador = quem favoritou | codigo_interno_colaborador_favoritado = profissional favoritado
-- ------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS tb_mkt_profissional_favorito (
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    codigo_interno_colaborador_favoritado VARCHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,
    PRIMARY KEY (codigo_interno_colaborador, codigo_interno_colaborador_favoritado, tb_org_id),
    KEY ix_tb_mkt_profissional_favorito_org (tb_org_id),
    CONSTRAINT fk_tb_mkt_profissional_favorito_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
) ENGINE=InnoDB;
