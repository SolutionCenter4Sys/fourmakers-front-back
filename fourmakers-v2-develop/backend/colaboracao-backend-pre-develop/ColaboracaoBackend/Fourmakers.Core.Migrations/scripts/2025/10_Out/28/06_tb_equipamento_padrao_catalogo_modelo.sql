-- ####################################################################
-- CRIAÇÃO DA TABELA tb_equipamento_padrao_catalogo_modelo
-- Catálogo de Modelos de Equipamento Aprovados pela TI
-- ####################################################################

CREATE TABLE IF NOT EXISTS tb_equipamento_padrao_catalogo_modelo (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único (UUID)',
    marca VARCHAR(50) NOT NULL COMMENT 'Marca do equipamento',
    nome_modelo VARCHAR(100) NOT NULL COMMENT 'Nome específico do modelo',
    linha_produto VARCHAR(50) NOT NULL COMMENT 'Linha corporativa (ex: Intel, AMD, Apple)',
    CONSTRAINT unq_modelo UNIQUE (marca, nome_modelo)
) ENGINE=InnoDB COMMENT='Catálogo de Modelos de Equipamento Aprovados pela TI.';
