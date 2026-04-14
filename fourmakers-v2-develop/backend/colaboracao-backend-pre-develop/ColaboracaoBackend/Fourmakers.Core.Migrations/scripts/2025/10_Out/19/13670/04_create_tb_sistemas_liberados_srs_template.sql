CREATE TABLE IF NOT EXISTS tb_sistemas_liberados_srs_template (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único do sistema liberado (GUID)',
    descricao VARCHAR(255) NOT NULL COMMENT 'Descrição do sistema liberado',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Data de criação do registro',
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Data da última alteração do registro',
    ativo TINYINT(1) DEFAULT 1 COMMENT 'Indica se o registro está ativo (1) ou inativo (0)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Tabela para armazenar sistemas liberados para templates SRS';

INSERT INTO tb_sistemas_liberados_srs_template (id, descricao, data_criacao, data_alteracao, ativo) VALUES
(uuid(), 'Portal de Projetos', NOW(), NOW(), 1),
(uuid(), 'Mapa de Alocação', NOW(), NOW(), 1),
(uuid(), 'SCC - Custo profissional', NOW(), NOW(), 1),
(uuid(), 'SCC - Necessário autorização diretoria', NOW(), NOW(), 1),
(uuid(), 'CRM', NOW(), NOW(), 1),
(uuid(), 'CCH - administrador', NOW(), NOW(), 1),
(uuid(), 'Controle de Gadget', NOW(), NOW(), 1),
(uuid(), 'Recrutamento e Seleção - Trabalhar as Vagas', NOW(), NOW(), 1),
(uuid(), 'Recrutamento e Seleção - Criação de Vagas', NOW(), NOW(), 1),
(uuid(), 'Recrutamento e Seleção - Aprovador de Vagas', NOW(), NOW(), 1);
