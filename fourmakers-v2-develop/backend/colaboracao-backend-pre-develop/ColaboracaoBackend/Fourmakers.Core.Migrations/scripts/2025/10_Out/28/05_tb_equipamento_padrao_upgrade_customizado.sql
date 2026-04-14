-- ####################################################################
-- CRIAÇÃO DA TABELA tb_equipamento_padrao_upgrade_customizado
-- Dicionário de Upgrades Customizados e Configurações Extras
-- ####################################################################

CREATE TABLE IF NOT EXISTS tb_equipamento_padrao_upgrade_customizado (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único (UUID)',
    descricao_upgrade TEXT NOT NULL COMMENT 'Detalhes da configuração extra.',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB COMMENT='Dicionário de Upgrades Customizados e Configurações Extras.';
