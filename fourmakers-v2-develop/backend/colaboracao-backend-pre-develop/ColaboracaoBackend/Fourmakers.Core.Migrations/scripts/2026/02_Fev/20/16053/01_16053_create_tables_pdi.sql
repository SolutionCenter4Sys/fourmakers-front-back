-- ------------------------------------------------------------------------------
-- PDI - Plano de Desenvolvimento Individual (Evolução 16053)
-- Data: 2026-02-20
-- Pasta: scripts/2026/02_Fev/20/16053
-- Referência: Documento Técnico Funcional – Evolução PDI (Ajustes Pdi Skill E Servicos)
-- Observação: Estrutura adaptada para MySQL (UUID como CHAR(36), BYTEA->LONGBLOB, JSONB->JSON).
--             colaborador_id referencia tb_colaborador(codigo_interno_colaborador).
-- ------------------------------------------------------------------------------

-- 2.1. Tabela: pdi
CREATE TABLE IF NOT EXISTS pdi (
    id CHAR(36) NOT NULL,
    colaborador_id CHAR(36) NOT NULL,
    titulo VARCHAR(150) NOT NULL,
    descricao TEXT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    status VARCHAR(30) NOT NULL,
    PRIMARY KEY (id),
    KEY idx_pdi_colaborador (colaborador_id),
    CONSTRAINT fk_pdi_colaborador FOREIGN KEY (colaborador_id)
        REFERENCES tb_colaborador (codigo_interno_colaborador)
) ENGINE=InnoDB;

-- 2.2. Tabela: pdi_skill (sem nivelAtual/nivelDesejado; codigo_skill obrigatório)
CREATE TABLE IF NOT EXISTS pdi_skill (
    id CHAR(36) NOT NULL,
    pdi_id CHAR(36) NOT NULL,
    nome_skill VARCHAR(150) NOT NULL,
    codigo_skill VARCHAR(50) NOT NULL,
    PRIMARY KEY (id),
    KEY idx_pdi_skill_pdi (pdi_id),
    UNIQUE KEY uk_pdi_skill_codigo (codigo_skill),
    CONSTRAINT fk_pdi_skill_pdi FOREIGN KEY (pdi_id)
        REFERENCES pdi (id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 2.3. Tabela: action_plan
CREATE TABLE IF NOT EXISTS action_plan (
    id CHAR(36) NOT NULL,
    pdi_id CHAR(36) NOT NULL,
    description TEXT NOT NULL,
    deadline DATE NULL,
    concluido_em DATETIME NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    KEY idx_action_plan_pdi (pdi_id),
    CONSTRAINT fk_action_plan_pdi FOREIGN KEY (pdi_id)
        REFERENCES pdi (id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 2.4. Tabela: pdi_asset (Evidências)
CREATE TABLE IF NOT EXISTS pdi_asset (
    id CHAR(36) NOT NULL,
    pdi_id CHAR(36) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    doc LONGBLOB NULL,
    doc_access VARCHAR(255) NULL,
    doc_path VARCHAR(500) NULL,
    doc_name VARCHAR(255) NOT NULL,
    doc_type VARCHAR(100) NULL,
    doc_size BIGINT NULL,
    doc_mime VARCHAR(150) NULL,
    doc_meta JSON NULL,
    tipo VARCHAR(50) NULL,
    link VARCHAR(500) NULL,
    PRIMARY KEY (id),
    KEY idx_pdi_asset_pdi (pdi_id),
    CONSTRAINT fk_pdi_asset_pdi FOREIGN KEY (pdi_id)
        REFERENCES pdi (id) ON DELETE CASCADE
) ENGINE=InnoDB;
