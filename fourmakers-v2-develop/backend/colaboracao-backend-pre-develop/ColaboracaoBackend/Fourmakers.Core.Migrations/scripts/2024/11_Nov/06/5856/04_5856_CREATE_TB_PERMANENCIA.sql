-- Tabela tb_permanencia
CREATE TABLE tb_permanencia (
    id VARCHAR(36) NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    ativo TINYINT NOT NULL DEFAULT 1,
    PRIMARY KEY (id)
);
