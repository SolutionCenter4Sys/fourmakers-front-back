-- Criar tabela tb_rubrica_template
CREATE TABLE tb_rubrica_template (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome_template VARCHAR(255) NOT NULL,
    mapeamento_campos JSON NOT NULL,
    link_modelo_s3 VARCHAR(500),
    ativo BOOLEAN DEFAULT TRUE,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_alteracao DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);