CREATE TABLE tb_parametro (
    id CHAR(36) NOT NULL PRIMARY KEY,
    nome_parametro VARCHAR(255) NOT NULL,
    descricao_parametro TEXT NOT NULL,
    codigo_parametro VARCHAR(255) NOT NULL,
    codigo_modulo_sistema VARCHAR(255) NOT NULL,
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    ativo BOOL,
    INDEX idx_codigo_parametro (codigo_parametro) -- Adiciona um índice para a coluna codigo_parametro
);
