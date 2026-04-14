-- =============================================================================
-- tb_admissao_historico_status — timeline de movimentacoes de status
-- Dependências: tb_org, tb_admissao, tb_admissao_status, tb_colaborador
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_historico_status (
    id                                        CHAR(36)                    NOT NULL,
    tb_org_id                                 INT                         NOT NULL,
    tb_admissao_id                            CHAR(36)                    NOT NULL,
    tb_admissao_status_origem_id              CHAR(36)                    NULL    COMMENT 'Status anterior (nulo na primeira movimentacao)',
    tb_admissao_status_destino_id             CHAR(36)                    NOT NULL COMMENT 'Status de destino',
    tb_colaborador_codigo_interno_colaborador VARCHAR(50) CHARACTER SET utf8mb3 NOT NULL COMMENT 'Quem realizou a movimentacao',
    data_movimentacao                         DATETIME                    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    observacao                                VARCHAR(500)                NULL,
    data_criacao                              DATETIME                    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao                          DATETIME                    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    KEY ix_tb_admissao_hist_org    (tb_org_id),
    KEY ix_tb_admissao_hist_adm    (tb_admissao_id),
    KEY ix_tb_admissao_hist_orig   (tb_admissao_status_origem_id),
    KEY ix_tb_admissao_hist_dest   (tb_admissao_status_destino_id),
    KEY ix_tb_admissao_hist_colab  (tb_colaborador_codigo_interno_colaborador),
    KEY ix_tb_admissao_hist_data   (data_movimentacao),

    CONSTRAINT fk_tb_admissao_hist_org
        FOREIGN KEY (tb_org_id) REFERENCES tb_org (id),
    CONSTRAINT fk_tb_admissao_hist_admissao
        FOREIGN KEY (tb_admissao_id) REFERENCES tb_admissao (id),
    CONSTRAINT fk_tb_admissao_hist_status_origem
        FOREIGN KEY (tb_admissao_status_origem_id) REFERENCES tb_admissao_status (id),
    CONSTRAINT fk_tb_admissao_hist_status_destino
        FOREIGN KEY (tb_admissao_status_destino_id) REFERENCES tb_admissao_status (id),
    CONSTRAINT fk_tb_admissao_hist_colaborador
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador) REFERENCES tb_colaborador (codigo_interno_colaborador)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Timeline de movimentacoes de status da admissao';
