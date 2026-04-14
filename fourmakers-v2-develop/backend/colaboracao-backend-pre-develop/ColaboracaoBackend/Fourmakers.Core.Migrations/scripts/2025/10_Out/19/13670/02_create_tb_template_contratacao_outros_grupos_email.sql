CREATE TABLE IF NOT EXISTS tb_template_contratacao_outros_grupos_email (
    tb_template_contratacao_id CHAR(36) NOT NULL COMMENT 'ID do template de contratação (FK)',
    email_grupo VARCHAR(255) NOT NULL COMMENT 'Email do grupo',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Data de criação do registro',
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Data da última alteração do registro',
    ativo TINYINT(1) DEFAULT 1 COMMENT 'Indica se o registro está ativo (1) ou inativo (0)',
    FOREIGN KEY (tb_template_contratacao_id) REFERENCES tb_template_contratacao(id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Tabela auxiliar para armazenar lista de emails dos outros grupos de um template de contratação';
