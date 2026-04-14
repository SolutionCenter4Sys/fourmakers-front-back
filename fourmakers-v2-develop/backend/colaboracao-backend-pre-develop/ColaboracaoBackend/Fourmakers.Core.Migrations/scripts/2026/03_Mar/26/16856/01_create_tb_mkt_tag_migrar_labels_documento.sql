-- ------------------------------------------------------------------------------
-- Marketing / Comunicação — Tags para publicações tipo documento
-- Ticket: 16856
-- Cria tb_mkt_tag e tb_mkt_publicacao_tag; migra labels com tipo = documento.
-- Reutiliza os mesmos UUIDs das labels documento nas tags para simplificar vínculos.
-- ------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS tb_mkt_tag (
    id CHAR(36) NOT NULL,
    nome VARCHAR(255) NOT NULL,
    tb_org_id INT NOT NULL,
    data_criacao DATETIME NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_tag_nome_org (nome, tb_org_id),
    KEY ix_tb_mkt_tag_org (tb_org_id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_publicacao_tag (
    id CHAR(36) NOT NULL,
    tb_mkt_tag_id CHAR(36) NOT NULL,
    tb_mkt_publicacao_id CHAR(36) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_publicacao_tag (tb_mkt_tag_id, tb_mkt_publicacao_id),
    KEY ix_tb_mkt_publicacao_tag_publicacao (tb_mkt_publicacao_id),
    CONSTRAINT fk_tb_mkt_publicacao_tag_tag FOREIGN KEY (tb_mkt_tag_id) REFERENCES tb_mkt_tag(id),
    CONSTRAINT fk_tb_mkt_publicacao_tag_publicacao FOREIGN KEY (tb_mkt_publicacao_id) REFERENCES tb_mkt_publicacao(id)
) ENGINE=InnoDB;
