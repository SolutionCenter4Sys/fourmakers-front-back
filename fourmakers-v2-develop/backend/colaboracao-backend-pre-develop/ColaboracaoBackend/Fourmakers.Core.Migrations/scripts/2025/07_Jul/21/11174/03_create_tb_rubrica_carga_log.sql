-- Criar tabela tb_rubrica_carga_log
CREATE TABLE tb_rubrica_carga_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigo_carga_rubrica VARCHAR(36) NOT NULL UNIQUE,
    arquivo_origem VARCHAR(500) NOT NULL,
    unidade VARCHAR(255) NOT NULL,
    tb_rubrica_id INT NOT NULL,
    tb_rubrica_template_id INT NOT NULL,
    status_processamento ENUM('iniciado', 'conversao_png', 'extracao_huggingface', 'processamento', 'sucesso', 'erro') DEFAULT 'iniciado',
    json_retorno_huggingface JSON,
    mensagem_erro TEXT,
    quantidade_registros_processados INT DEFAULT 0,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_processamento_fim DATETIME,
    INDEX idx_codigo_carga (codigo_carga_rubrica),
    INDEX idx_status (status_processamento)
);