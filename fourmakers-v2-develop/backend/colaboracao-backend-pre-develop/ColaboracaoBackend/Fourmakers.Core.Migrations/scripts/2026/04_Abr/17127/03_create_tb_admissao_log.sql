-- =============================================================================
-- tb_admissao_log — auditoria em tb_admissao
-- Dependências: tb_admissao (01), tb_colaborador
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_log (
    id                                                  CHAR(36)     NOT NULL,
    tb_admissao_id                                      CHAR(36)     NOT NULL COMMENT 'ID do processo de admissao (tb_admissao)',
    acao                                                VARCHAR(20)  NOT NULL COMMENT 'CREATE | UPDATE | STATUS_UPDATE | DELETE',
    tb_colaborador_codigo_interno_colaborador_alterador VARCHAR(100) CHARACTER SET utf8mb3 NOT NULL COMMENT 'Codigo interno do colaborador que realizou a alteracao',
    data_alteracao                                      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    objeto                                              LONGTEXT     NOT NULL COMMENT 'JSON estado anterior (UPDATE/DELETE) ou registro criado (CREATE)',
    alteracoes                                          LONGTEXT     NOT NULL COMMENT 'JSON novo estado apos a alteracao',

    PRIMARY KEY (id),
    KEY idx_tb_admissao_log_admissao (tb_admissao_id),
    KEY idx_tb_admissao_log_data     (data_alteracao),

    CONSTRAINT fk_tb_admissao_log_admissao
        FOREIGN KEY (tb_admissao_id) REFERENCES tb_admissao (id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_tb_admissao_log_colaborador_alterador
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_alterador) REFERENCES tb_colaborador (codigo_interno_colaborador)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Log de auditoria da admissao';
