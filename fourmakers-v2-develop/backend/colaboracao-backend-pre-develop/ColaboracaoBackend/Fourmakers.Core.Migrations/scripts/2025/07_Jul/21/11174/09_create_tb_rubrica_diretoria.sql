-- Criar tabela tb_rubrica_diretoria (substitui tb_rubrica_unidade)
CREATE TABLE tb_rubrica_diretoria (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tb_rubrica_id VARCHAR(36) NOT NULL,
    cod_diretoria VARCHAR(50) NOT NULL COMMENT 'Código da diretoria',
    ativo BOOLEAN DEFAULT TRUE,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_alteracao DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Índices para performance
    INDEX idx_rubrica (tb_rubrica_id),
    INDEX idx_cod_diretoria (cod_diretoria),
    INDEX idx_ativo (ativo),
    
    -- Constraint única para evitar duplicatas
    UNIQUE KEY uk_rubrica_diretoria (tb_rubrica_id, cod_diretoria)
);

-- Inserir diretorias para a rubrica ID 1
INSERT INTO tb_rubrica_diretoria (tb_rubrica_id, cod_diretoria) VALUES
('1', '14014761000298'),
('1', '16845496000134'),
('1', '27752891000270'),
('1', '14014761000107'),
('1', '27752891000199');