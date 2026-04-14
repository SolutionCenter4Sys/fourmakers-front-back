-- ####################################################################
-- CRIAÇÃO DA TABELA tb_equipamento_padrao_especificacao
-- Dicionário de Especificações Técnicas de Equipamento
-- ####################################################################

CREATE TABLE IF NOT EXISTS tb_equipamento_padrao_especificacao (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único (UUID)',
    tipo_equipamento VARCHAR(50) NOT NULL,
    sistema_operacional VARCHAR(50) NOT NULL,
    cpu_geracao VARCHAR(100) NOT NULL,
    gpu VARCHAR(100) NOT NULL,
    memoria_ram VARCHAR(50) NOT NULL,
    armazenamento_disco VARCHAR(50) NOT NULL,
    CONSTRAINT unq_especificacao UNIQUE (tipo_equipamento, sistema_operacional, cpu_geracao, memoria_ram, armazenamento_disco)
) ENGINE=InnoDB COMMENT='Dicionário de Especificações Técnicas de Equipamento.';
