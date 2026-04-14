-- ####################################################################
-- CRIAÇÃO DA TABELA tb_equipamento_padrao_grupo_area
-- Dicionário de Grupos de Área/Departamento
-- ####################################################################

CREATE TABLE IF NOT EXISTS tb_equipamento_padrao_grupo_area (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único (UUID)',
    nome_grupo VARCHAR(100) NOT NULL UNIQUE COMMENT 'Nome do grupo (ex: Administrativo, Desenvolvedor)',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB COMMENT='Dicionário de Grupos de Área/Departamento.';
