-- ####################################################################
-- CRIAÇÃO DA TABELA tb_equipamento_padrao_categoria_requisito
-- Dicionário de Categorias de Requisitos (Padrão, Secundário, etc.)
-- ####################################################################

CREATE TABLE IF NOT EXISTS tb_equipamento_padrao_categoria_requisito (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único (UUID)',
    categoria_nome VARCHAR(50) NOT NULL UNIQUE COMMENT 'Nome da categoria (ex: PADRAO)',
    descricao VARCHAR(255) NULL COMMENT 'Descrição detalhada da categoria',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB COMMENT='Dicionário de Categorias de Requisitos (Padrão, Secundário, etc.).';
