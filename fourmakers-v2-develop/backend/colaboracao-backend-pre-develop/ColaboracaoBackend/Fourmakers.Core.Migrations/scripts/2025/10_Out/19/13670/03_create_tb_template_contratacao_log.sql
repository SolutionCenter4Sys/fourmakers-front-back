CREATE TABLE IF NOT EXISTS tb_template_contratacao_log (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único do log (GUID)',
    tb_template_contratacao_id CHAR(36) NOT NULL COMMENT 'ID do template de contratação relacionado',
    acao VARCHAR(50) NOT NULL COMMENT 'Ação realizada (CREATE, UPDATE, DELETE)',
    tb_colaborador_codigo_interno_colaborador_alterador VARCHAR(36) NOT NULL COMMENT 'Código interno do colaborador que realizou a ação',
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Data e hora da alteração',
    objeto TEXT NULL COMMENT 'JSON com o objeto completo do template no momento da alteração',
    alteracoes TEXT NULL COMMENT 'JSON com as alterações específicas realizadas',
    
    FOREIGN KEY (tb_template_contratacao_id) REFERENCES tb_template_contratacao(id) ON DELETE CASCADE,
    FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_alterador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Tabela de log para rastrear alterações nos templates de contratação';
