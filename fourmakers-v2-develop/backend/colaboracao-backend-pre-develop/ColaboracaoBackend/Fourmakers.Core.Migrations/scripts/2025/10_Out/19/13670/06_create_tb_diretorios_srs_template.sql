CREATE TABLE IF NOT EXISTS tb_diretorios_srs_template (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único do diretório (GUID)',
    descricao VARCHAR(255) NOT NULL COMMENT 'Descrição do diretório',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Data de criação do registro',
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Data da última alteração do registro',
    ativo TINYINT(1) DEFAULT 1 COMMENT 'Indica se o registro está ativo (1) ou inativo (0)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Tabela para armazenar diretórios para templates SRS';

INSERT INTO tb_diretorios_srs_template (id, descricao, data_criacao, data_alteracao, ativo) VALUES
(uuid(), 'Diretório PMO - UN 1 (Bradesco) Diretório de Propostas', NOW(), NOW(), 1),
(uuid(), 'Diretório Faturamento_Bradesco', NOW(), NOW(), 1),
(uuid(), 'Diretório Com - UN 2, 3 e 4 (Executivos de Conta)', NOW(), NOW(), 1),
(uuid(), 'Diretorio de Pré Vendas - UN 2, 3 e 4 (gerentes de projetos e executivos de contas)', NOW(), NOW(), 1),
(uuid(), 'Diretorio de Marketing', NOW(), NOW(), 1),
(uuid(), 'Diretorio de RH', NOW(), NOW(), 1),
(uuid(), 'Diretório de Recrutamento e Seleção (RS)', NOW(), NOW(), 1),
(uuid(), 'Diretório de Infra', NOW(), NOW(), 1),
(uuid(), 'Diretório de Manutenção Predial', NOW(), NOW(), 1);
