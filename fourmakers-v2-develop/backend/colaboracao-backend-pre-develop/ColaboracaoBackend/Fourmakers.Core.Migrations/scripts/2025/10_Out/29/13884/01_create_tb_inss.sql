CREATE TABLE IF NOT EXISTS tb_inss (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único (UUID)',
    ano INT NOT NULL COMMENT 'Ano de vigência da tabela',
    faixa_1_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da primeira faixa',
    faixa_1_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da primeira faixa',
    faixa_1_aliquota DECIMAL(5,2) NOT NULL COMMENT 'Alíquota da primeira faixa (%)',
    faixa_2_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da segunda faixa',
    faixa_2_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da segunda faixa',
    faixa_2_aliquota DECIMAL(5,2) NOT NULL COMMENT 'Alíquota da segunda faixa (%)',
    faixa_3_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da terceira faixa',
    faixa_3_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da terceira faixa',
    faixa_3_aliquota DECIMAL(5,2) NOT NULL COMMENT 'Alíquota da terceira faixa (%)',
    faixa_4_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da quarta faixa',
    faixa_4_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da quarta faixa',
    faixa_4_aliquota DECIMAL(5,2) NOT NULL COMMENT 'Alíquota da quarta faixa (%)',
    teto_inss DECIMAL(10,2) NOT NULL COMMENT 'Teto do INSS',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    ativo TINYINT(1) DEFAULT 1,
    CONSTRAINT unq_inss_ano UNIQUE (ano),
    INDEX idx_inss_ano_ativo (ano, ativo)
) ENGINE=InnoDB COMMENT='Tabela de faixas do INSS por ano de vigência';

-- Inserir dados da tabela INSS 2024
INSERT INTO tb_inss (
    id, ano, 
    faixa_1_min, faixa_1_max, faixa_1_aliquota,
    faixa_2_min, faixa_2_max, faixa_2_aliquota,
    faixa_3_min, faixa_3_max, faixa_3_aliquota,
    faixa_4_min, faixa_4_max, faixa_4_aliquota,
    teto_inss
) VALUES (
    UUID(), 2024,
    0.00, 1412.00, 7.50,
    1412.01, 2666.68, 9.00,
    2666.69, 4000.03, 12.00,
    4000.04, 7786.02, 14.00,
    7786.02
);

-- Inserir dados da tabela INSS 2025
INSERT INTO tb_inss (
    id, ano, 
    faixa_1_min, faixa_1_max, faixa_1_aliquota,
    faixa_2_min, faixa_2_max, faixa_2_aliquota,
    faixa_3_min, faixa_3_max, faixa_3_aliquota,
    faixa_4_min, faixa_4_max, faixa_4_aliquota,
    teto_inss
) VALUES (
    UUID(), 2025,
    0.00, 1518.00, 7.50,
    1518.01, 2793.88, 9.00,
    2793.89, 4190.83, 12.00,
    4190.84, 8157.41, 14.00,
    8157.41
);