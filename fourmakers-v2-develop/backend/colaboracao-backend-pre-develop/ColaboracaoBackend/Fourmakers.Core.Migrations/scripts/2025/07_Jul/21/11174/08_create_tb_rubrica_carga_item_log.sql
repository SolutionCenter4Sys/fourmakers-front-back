-- Criar tabela tb_rubrica_carga_item_log para log detalhado de cada item processado
CREATE TABLE tb_rubrica_carga_item_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigo_carga_rubrica VARCHAR(36) NOT NULL,
    json_item_tentativa JSON NOT NULL COMMENT 'JSON do item que foi tentado inserir',
    status_processamento ENUM('sucesso', 'erro_colaborador_nao_encontrado', 'erro_salvamento') NOT NULL,
    tipo_identificacao ENUM('codigo_alternativo', 'cpf_colaborador', 'nao_encontrado') NULL COMMENT 'Como o colaborador foi identificado',
    codigo_interno_colaborador VARCHAR(255) NULL COMMENT 'Colaborador encontrado (se houver)',
    tb_rubrica_colaborador_id VARCHAR(36) NULL COMMENT 'ID do registro salvo em tb_rubrica_colaborador (se sucesso)',
    mensagem_erro TEXT NULL COMMENT 'Detalhes do erro (se houver)',
    data_processamento DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    -- Índices para performance
    INDEX idx_codigo_carga (codigo_carga_rubrica),
    INDEX idx_status (status_processamento),
    INDEX idx_colaborador (codigo_interno_colaborador),
    INDEX idx_data (data_processamento),
    
    -- Foreign key para a carga principal
    FOREIGN KEY (codigo_carga_rubrica) REFERENCES tb_rubrica_carga_log(codigo_carga_rubrica) ON DELETE CASCADE
);