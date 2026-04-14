-- Tabela tb_area_atuacao
CREATE TABLE tb_area_atuacao (
    id VARCHAR(36) NOT NULL,
    descricao VARCHAR(255),
    ativo TINYINT NOT NULL DEFAULT 1,
    
    PRIMARY KEY (id)
);
