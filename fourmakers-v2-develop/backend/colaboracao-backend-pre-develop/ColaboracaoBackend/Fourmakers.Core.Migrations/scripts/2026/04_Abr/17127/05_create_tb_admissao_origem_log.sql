-- =============================================================================
-- tb_admissao_origem_log — auditoria em tb_admissao_origem
-- Dependencias: tb_admissao_origem (04), tb_colaborador
-- Acoes possiveis:
--   CREATE  — primeiro vinculo registrado
--   REPLACE — vinculo substituido (o anterior foi removido e um novo foi criado)
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_origem_log (
    id                                                  CHAR(36)     NOT NULL,
    tb_admissao_origem_id                               CHAR(36)     NOT NULL COMMENT 'ID do vinculo (tb_admissao_origem)',
    tb_admissao_id                                      CHAR(36)     NOT NULL COMMENT 'ID do processo de admissao — facilita consultas sem JOIN',
    acao                                                VARCHAR(20)  NOT NULL COMMENT 'CREATE | REPLACE',
    tb_colaborador_codigo_interno_colaborador_alterador VARCHAR(100) CHARACTER SET utf8mb3 NOT NULL COMMENT 'Codigo interno do colaborador que realizou a operacao',
    data_alteracao                                      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    objeto                                              LONGTEXT     NOT NULL COMMENT 'JSON estado anterior (vazio em CREATE)',
    alteracoes                                          LONGTEXT     NOT NULL COMMENT 'JSON do novo vinculo criado',

    PRIMARY KEY (id),
    KEY idx_tb_admissao_origem_log_origem   (tb_admissao_origem_id),
    KEY idx_tb_admissao_origem_log_admissao (tb_admissao_id),
    KEY idx_tb_admissao_origem_log_data     (data_alteracao),

    -- tb_admissao_origem_id NAO tem FK: o registro de origem pode ser deletado num REPLACE
    CONSTRAINT fk_tb_admissao_origem_log_admissao
        FOREIGN KEY (tb_admissao_id) REFERENCES tb_admissao (id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Log de auditoria dos vinculos de origem da admissao';
