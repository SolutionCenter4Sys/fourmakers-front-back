-- tb_admissao_pipeline + log (padrão tb_admissao_cbo / tb_admissao_cbo_log).
-- Dependência: tb_org (PK id INT).

CREATE TABLE IF NOT EXISTS tb_admissao_pipeline (
    id CHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,
    nome VARCHAR(120) NOT NULL,
    descricao VARCHAR(255) NULL,
    ativo TINYINT NOT NULL DEFAULT 1,
    versao INT NOT NULL DEFAULT 1,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uk_tb_admissao_pipeline_org_nome (tb_org_id, nome),
    KEY ix_tb_admissao_pipeline_org (tb_org_id),
    CONSTRAINT fk_tb_admissao_pipeline_org FOREIGN KEY (tb_org_id) REFERENCES tb_org (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS tb_admissao_pipeline_log (
    id CHAR(36) NOT NULL,
    tb_admissao_pipeline_id CHAR(36) NOT NULL,
    acao VARCHAR(20) NOT NULL,
    alterador_cpf VARCHAR(100) NOT NULL,
    data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    objeto LONGTEXT NOT NULL,
    alteracoes LONGTEXT NOT NULL,
    PRIMARY KEY (id),
    KEY idx_tb_admissao_pipeline_log_tb_id (tb_admissao_pipeline_id),
    KEY idx_tb_admissao_pipeline_log_data (data_alteracao),
    CONSTRAINT fk_tb_admissao_pipeline_log_pipeline
        FOREIGN KEY (tb_admissao_pipeline_id) REFERENCES tb_admissao_pipeline (id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Vínculo pipeline ↔ status (ordem e regras por etapa). Depende de tb_admissao_pipeline e tb_admissao_status.
CREATE TABLE IF NOT EXISTS tb_admissao_pipeline_status (
    id CHAR(36) NOT NULL,
    tb_admissao_pipeline_id CHAR(36) NOT NULL,
    tb_admissao_status_id CHAR(36) NOT NULL,
    ordem INT NOT NULL,
    obrigatorio TINYINT NOT NULL DEFAULT 1,
    permite_retroceder TINYINT NULL DEFAULT NULL,
    status_inicial TINYINT NOT NULL DEFAULT 0,
    status_final TINYINT NOT NULL DEFAULT 0,
    ativo TINYINT NOT NULL DEFAULT 1,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uk_tb_admissao_pipeline_status_pipeline_status (tb_admissao_pipeline_id, tb_admissao_status_id),
    UNIQUE KEY uk_tb_admissao_pipeline_status_pipeline_ordem (tb_admissao_pipeline_id, ordem),
    KEY ix_tb_admissao_pipeline_status_pipeline (tb_admissao_pipeline_id),
    KEY ix_tb_admissao_pipeline_status_status (tb_admissao_status_id),
    CONSTRAINT fk_tb_admissao_pipeline_status_pipeline
        FOREIGN KEY (tb_admissao_pipeline_id) REFERENCES tb_admissao_pipeline (id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_tb_admissao_pipeline_status_status
        FOREIGN KEY (tb_admissao_status_id) REFERENCES tb_admissao_status (id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT ck_tb_admissao_pipeline_status_ordem CHECK (ordem > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;