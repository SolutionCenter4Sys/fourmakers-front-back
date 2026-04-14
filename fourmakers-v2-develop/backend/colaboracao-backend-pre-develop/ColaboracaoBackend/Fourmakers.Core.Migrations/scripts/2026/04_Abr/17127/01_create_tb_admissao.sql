-- =============================================================================
-- tb_admissao — instância do processo de admissão
-- Dependências: tb_org, tb_admissao_pipeline, tb_admissao_status, tb_colaborador
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao (
    id                                        CHAR(36)                    NOT NULL,
    tb_org_id                                 INT                         NOT NULL,
    tb_admissao_pipeline_id                   CHAR(36)                    NOT NULL,
    tb_admissao_status_id                     CHAR(36)                    NOT NULL,
    tb_colaborador_codigo_interno_colaborador VARCHAR(36) CHARACTER SET utf8mb3 NULL COMMENT 'Colaborador alvo da admissão',
    data_inicio                               DATETIME                    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_fim                                  DATETIME                    NULL,
    observacao                                VARCHAR(500)                NULL,
    ativo                                     TINYINT                     NOT NULL DEFAULT 1,
    data_criacao                              DATETIME                    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao                          DATETIME                    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    KEY ix_tb_admissao_org        (tb_org_id),
    KEY ix_tb_admissao_pipeline   (tb_admissao_pipeline_id),
    KEY ix_tb_admissao_status     (tb_admissao_status_id),
    KEY ix_tb_admissao_colaborador(tb_colaborador_codigo_interno_colaborador),

    CONSTRAINT fk_tb_admissao_org
        FOREIGN KEY (tb_org_id) REFERENCES tb_org (id),
    CONSTRAINT fk_tb_admissao_pipeline
        FOREIGN KEY (tb_admissao_pipeline_id) REFERENCES tb_admissao_pipeline (id),
    CONSTRAINT fk_tb_admissao_status
        FOREIGN KEY (tb_admissao_status_id) REFERENCES tb_admissao_status (id),
    CONSTRAINT fk_tb_admissao_colaborador
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador) REFERENCES tb_colaborador (codigo_interno_colaborador)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Processo de admissao';
