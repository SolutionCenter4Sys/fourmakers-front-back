-- Criar tabela tb_rubrica_codigo_colaborador_alternativo
CREATE TABLE tb_rubrica_codigo_colaborador_alternativo (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigo_alternativo VARCHAR(255) NOT NULL,
    codigo_interno_colaborador VARCHAR(255) NOT NULL,
    tb_rubrica_id INT NOT NULL,
    ativo BOOLEAN DEFAULT TRUE,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_alteracao DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY unique_codigo_alternativo_rubrica (codigo_alternativo, tb_rubrica_id),
    INDEX idx_codigo_interno (codigo_interno_colaborador)
);