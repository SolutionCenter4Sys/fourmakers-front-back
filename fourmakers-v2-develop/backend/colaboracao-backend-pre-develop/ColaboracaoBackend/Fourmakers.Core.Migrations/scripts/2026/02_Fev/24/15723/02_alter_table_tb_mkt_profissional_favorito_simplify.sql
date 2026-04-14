-- ------------------------------------------------------------------------------
-- Se a tabela tb_mkt_profissional_favorito já existir com a estrutura antiga
-- (id, tb_usuario_id, data_criacao, data_alteracao), executar este script
-- para substituir pela estrutura simplificada.
-- ------------------------------------------------------------------------------

DROP TABLE IF EXISTS tb_mkt_profissional_favorito;

CREATE TABLE tb_mkt_profissional_favorito (
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    codigo_interno_colaborador_favoritado VARCHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,
    PRIMARY KEY (codigo_interno_colaborador, codigo_interno_colaborador_favoritado, tb_org_id),
    KEY ix_tb_mkt_profissional_favorito_org (tb_org_id),
    CONSTRAINT fk_tb_mkt_profissional_favorito_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
) ENGINE=InnoDB;
