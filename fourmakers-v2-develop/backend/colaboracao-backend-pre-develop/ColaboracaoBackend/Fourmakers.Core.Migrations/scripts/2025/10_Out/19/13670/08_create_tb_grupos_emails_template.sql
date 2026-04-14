CREATE TABLE IF NOT EXISTS tb_grupos_emails_template (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único do grupo de email (GUID)',
    descricao VARCHAR(255) NOT NULL COMMENT 'Descrição do grupo de email',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Data de criação do registro',
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Data da última alteração do registro',
    ativo TINYINT(1) DEFAULT 1 COMMENT 'Indica se o registro está ativo (1) ou inativo (0)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Tabela para armazenar grupos de emails para templates SRS';

INSERT INTO tb_grupos_emails_template (id, descricao, data_criacao, data_alteracao, ativo) VALUES
(uuid(), 'Todos Foursys', NOW(), NOW(), 1),
(uuid(), 'Paulista', NOW(), NOW(), 1),
(uuid(), 'Alphaville', NOW(), NOW(), 1),
(uuid(), 'Curitiba', NOW(), NOW(), 1);
