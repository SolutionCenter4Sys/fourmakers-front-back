-- Criar tabela tb_rubrica_unidade
CREATE TABLE tb_rubrica_unidade (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tb_rubrica_id INT NOT NULL,
    unidade VARCHAR(255) NOT NULL,
    ativo BOOLEAN DEFAULT TRUE,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_alteracao DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_rubrica_unidade (tb_rubrica_id, unidade)
);
