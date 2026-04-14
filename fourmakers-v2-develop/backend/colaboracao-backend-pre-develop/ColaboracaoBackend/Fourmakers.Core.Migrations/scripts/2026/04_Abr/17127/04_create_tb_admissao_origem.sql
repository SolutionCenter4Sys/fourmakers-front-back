-- =============================================================================
-- tb_admissao_origem — vinculo entre admissao e vaga/candidatura
-- Dependencias: tb_admissao (01)
-- Nota: tb_vaga_id e tb_candidato_vaga_id sao referencias logicas sem FK,
--       para manter o modulo de admissao independente do de recrutamento.
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_admissao_origem (
    id                   CHAR(36)                   NOT NULL,
    tb_admissao_id       CHAR(36)                   NOT NULL COMMENT 'Processo de admissao vinculado',
    origem_tipo          ENUM('VAGA','CANDIDATURA')  NOT NULL COMMENT 'Tipo da origem: vaga ou candidatura',
    tb_vaga_id           VARCHAR(36)                NULL     COMMENT 'ID da vaga (tb_vaga.id) — sem FK, ref. logica',
    tb_candidato_vaga_id VARCHAR(36)                NULL     COMMENT 'ID da candidatura (tb_candidato_vaga.id) — sem FK, ref. logica',
    data_criacao         DATETIME                   NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_admissao_origem_admissao  (tb_admissao_id),
    KEY        ix_tb_admissao_origem_vaga       (tb_vaga_id),
    KEY        ix_tb_admissao_origem_candidatura(tb_candidato_vaga_id),

    CONSTRAINT fk_tb_admissao_origem_admissao
        FOREIGN KEY (tb_admissao_id) REFERENCES tb_admissao (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Vinculo entre admissao e vaga/candidatura do modulo de recrutamento';
