CREATE TABLE IF NOT EXISTS tb_template_contratacao_diretorios (
    tb_template_contratacao_id CHAR(36) NOT NULL COMMENT 'ID do template de contratação (FK)',
    tb_diretorios_srs_template_id CHAR(36) NOT NULL COMMENT 'ID do diretório (FK)',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Data de criação do registro',
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Data da última alteração do registro',
    ativo TINYINT(1) DEFAULT 1 COMMENT 'Indica se o registro está ativo (1) ou inativo (0)',
    leitura BOOLEAN NOT NULL DEFAULT TRUE COMMENT 'Permissão de leitura no diretório',
    escrita BOOLEAN NOT NULL DEFAULT FALSE COMMENT 'Permissão de escrita no diretório',
    
    FOREIGN KEY (tb_template_contratacao_id) REFERENCES tb_template_contratacao(id) ON DELETE CASCADE,
    FOREIGN KEY (tb_diretorios_srs_template_id) REFERENCES tb_diretorios_srs_template(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Tabela de relacionamento entre templates de contratação e diretórios';
