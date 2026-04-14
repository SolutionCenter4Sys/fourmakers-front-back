CREATE TABLE IF NOT EXISTS tb_status_vaga_ordem_log (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único do log (GUID)',
    tb_status_vaga_codigo INT NOT NULL COMMENT 'Código do status da vaga',
    ordem_anterior INT NOT NULL COMMENT 'Ordem anterior do status',
    ordem_nova INT NOT NULL COMMENT 'Nova ordem do status',
    tb_colaborador_codigo_interno_colaborador_alterador VARCHAR(36) NOT NULL COMMENT 'Código interno do colaborador que realizou a alteração',
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Data e hora da alteração',
    
    FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_alterador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Tabela de log para rastrear alterações na ordem dos status de vaga de recrutamento';

