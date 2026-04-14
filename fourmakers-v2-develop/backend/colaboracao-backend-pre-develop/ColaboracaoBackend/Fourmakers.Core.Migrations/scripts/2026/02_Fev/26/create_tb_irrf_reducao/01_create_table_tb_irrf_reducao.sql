-- Criar tabela de redução do IRRF
CREATE TABLE IF NOT EXISTS tb_irrf_reducao (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único (UUID)',
    ano INT NOT NULL COMMENT 'Ano de vigência da tabela',
    faixa_1_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da primeira faixa (até este valor aplica desconto fixo)',
    faixa_1_desconto_maximo DECIMAL(10,2) NOT NULL COMMENT 'Desconto máximo para a primeira faixa (zerando o imposto)',
    faixa_2_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da segunda faixa',
    faixa_2_max DECIMAL(10,2) NOT NULL COMMENT 'Valor máximo da segunda faixa',
    faixa_2_valor_base DECIMAL(10,2) NOT NULL COMMENT 'Valor base para cálculo da redução na segunda faixa',
    faixa_2_coeficiente DECIMAL(10,8) NOT NULL COMMENT 'Coeficiente para cálculo da redução na segunda faixa',
    faixa_3_min DECIMAL(10,2) NOT NULL COMMENT 'Valor mínimo da terceira faixa (a partir deste valor não há redução)',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    ativo TINYINT(1) DEFAULT 1,
    CONSTRAINT unq_irrf_reducao_ano UNIQUE (ano),
    INDEX idx_irrf_reducao_ano_ativo (ano, ativo)
) ENGINE=InnoDB COMMENT='Tabela de redução do IRRF por ano de vigência';

-- Inserir dados da tabela de redução IRRF 2024
-- Faixa 1: Até R$ 5.000,00 - desconto de R$ 312,89
-- Faixa 2: Entre R$ 5.000,01 e R$ 7.350,00 - desconto = R$ 978,62 - (0,133145 × salário bruto CLT)
-- Faixa 3: A partir de R$ 7.350,01 - sem redução
INSERT INTO tb_irrf_reducao (
    id, ano,
    faixa_1_max,
    faixa_1_desconto_maximo,
    faixa_2_min,
    faixa_2_max,
    faixa_2_valor_base,
    faixa_2_coeficiente,
    faixa_3_min
) VALUES (
    UUID(), 2026,
    5000.00,
    312.89,
    5000.01,
    7350.00,
    978.62,
    0.133145,
    7350.01
);

