CREATE TABLE IF NOT EXISTS tb_irrf (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único (UUID)',
    ano INT NOT NULL COMMENT 'Ano de vigência da tabela',
    faixa_1_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da primeira faixa',
    faixa_1_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da primeira faixa',
    faixa_1_aliquota DECIMAL(5,2) NOT NULL COMMENT 'Alíquota da primeira faixa (%)',
    faixa_1_deducao DECIMAL(10,2) NOT NULL COMMENT 'Parcela a deduzir da primeira faixa',
    faixa_2_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da segunda faixa',
    faixa_2_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da segunda faixa',
    faixa_2_aliquota DECIMAL(5,2) NOT NULL COMMENT 'Alíquota da segunda faixa (%)',
    faixa_2_deducao DECIMAL(10,2) NOT NULL COMMENT 'Parcela a deduzir da segunda faixa',
    faixa_3_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da terceira faixa',
    faixa_3_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da terceira faixa',
    faixa_3_aliquota DECIMAL(5,2) NOT NULL COMMENT 'Alíquota da terceira faixa (%)',
    faixa_3_deducao DECIMAL(10,2) NOT NULL COMMENT 'Parcela a deduzir da terceira faixa',
    faixa_4_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da quarta faixa',
    faixa_4_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da quarta faixa',
    faixa_4_aliquota DECIMAL(5,2) NOT NULL COMMENT 'Alíquota da quarta faixa (%)',
    faixa_4_deducao DECIMAL(10,2) NOT NULL COMMENT 'Parcela a deduzir da quarta faixa',
    faixa_5_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da quinta faixa',
    faixa_5_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da quinta faixa',
    faixa_5_aliquota DECIMAL(5,2) NOT NULL COMMENT 'Alíquota da quinta faixa (%)',
    faixa_5_deducao DECIMAL(10,2) NOT NULL COMMENT 'Parcela a deduzir da quinta faixa',
    deducao_por_dependente DECIMAL(10,2) NOT NULL COMMENT 'Valor da dedução por dependente',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    ativo TINYINT(1) DEFAULT 1,
    CONSTRAINT unq_irrf_ano UNIQUE (ano),
    INDEX idx_irrf_ano_ativo (ano, ativo)
) ENGINE=InnoDB COMMENT='Tabela de faixas do IRRF por ano de vigência';

-- Inserir dados da tabela IRRF 2024
INSERT INTO tb_irrf (
    id, ano,
    faixa_1_min, faixa_1_max, faixa_1_aliquota, faixa_1_deducao,
    faixa_2_min, faixa_2_max, faixa_2_aliquota, faixa_2_deducao,
    faixa_3_min, faixa_3_max, faixa_3_aliquota, faixa_3_deducao,
    faixa_4_min, faixa_4_max, faixa_4_aliquota, faixa_4_deducao,
    faixa_5_min, faixa_5_max, faixa_5_aliquota, faixa_5_deducao,
    deducao_por_dependente
) VALUES (
    UUID(), 2024,
    0.00, 2112.00, 0.00, 0.00,
    2112.01, 2826.65, 7.50, 158.40,
    2826.66, 3751.05, 15.00, 370.40,
    3751.06, 4664.68, 22.50, 651.73,
    4664.69, 999999.99, 27.50, 884.96,
    189.59
);

-- Inserir dados da tabela IRRF 2024
INSERT INTO tb_irrf (
    id, ano,
    faixa_1_min, faixa_1_max, faixa_1_aliquota, faixa_1_deducao,
    faixa_2_min, faixa_2_max, faixa_2_aliquota, faixa_2_deducao,
    faixa_3_min, faixa_3_max, faixa_3_aliquota, faixa_3_deducao,
    faixa_4_min, faixa_4_max, faixa_4_aliquota, faixa_4_deducao,
    faixa_5_min, faixa_5_max, faixa_5_aliquota, faixa_5_deducao,
    deducao_por_dependente
) VALUES (
    UUID(), 2025,
    0.00, 2428.80, 0.00, 0.00,
    2428.81, 2826.65, 7.50, 182.16,
    2826.66, 3751.05, 15.00, 394.16,
    3751.06, 4664.68, 22.50, 675.49,
    4664.68, 99999999.99, 27.50, 908.73,
    189.59
);