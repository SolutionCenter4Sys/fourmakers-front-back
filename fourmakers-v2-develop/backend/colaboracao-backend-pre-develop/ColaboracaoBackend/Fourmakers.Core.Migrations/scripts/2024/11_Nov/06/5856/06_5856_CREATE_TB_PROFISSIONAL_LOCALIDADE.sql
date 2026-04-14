
-- Tabela tb_profissional_localidade
CREATE TABLE tb_profissional_localidade (
    id VARCHAR(36) NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    ativo TINYINT NOT NULL DEFAULT 1,
    PRIMARY KEY (id)
);
