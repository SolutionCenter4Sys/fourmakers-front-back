-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Tabela de sugestao de pastas por org (16741)
-- Data: 2026-03-24
-- Populada a partir de publicacoes com publicacao_status = 'ativa' e pasta preenchida.
-- ------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS tb_mkt_publicacao_pasta_sugestao (
    id CHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,
    nome VARCHAR(255) NOT NULL COMMENT 'Nome da pasta (TRIM), distinto por org',
    data_atualizacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_publicacao_pasta_sugestao_org_nome (tb_org_id, nome),
    KEY ix_tb_mkt_publicacao_pasta_sugestao_org (tb_org_id),
    CONSTRAINT fk_tb_mkt_publicacao_pasta_sugestao_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id) ON UPDATE CASCADE
) ENGINE=InnoDB;
