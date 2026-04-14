-- Tabela tb_modelo_trabalho
CREATE TABLE tb_modelo_trabalho (
    id VARCHAR(36) NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    ativo TINYINT NOT NULL DEFAULT 1,
    PRIMARY KEY (id)
);