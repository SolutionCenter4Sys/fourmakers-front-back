CREATE TABLE tb_colaboradores_alocados_snapshot (
    id CHAR(36) NOT NULL PRIMARY KEY,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    colaborador VARCHAR(255) NOT NULL,
    tb_gestor_externo_perfil_id CHAR(36) NULL,
    perfil VARCHAR(255) NULL,
    cod_gestor_cliente VARCHAR(100) NULL,
    gestor_cliente VARCHAR(255) NULL,
    codigo_interno_gestor_adm VARCHAR(36) NULL,
    gestor_adm VARCHAR(255) NULL,
    cod_gestor_operacional VARCHAR(36) NULL,
    gestor_projeto VARCHAR(255) NULL,
    tb_org_id INT NOT NULL,
    cod_cliente VARCHAR(100) NULL,
    retorno_match JSON NULL,
    match_score DOUBLE NULL,
    data_snapshot DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_snapshot_org_data (tb_org_id, data_snapshot),
    INDEX idx_snapshot_colaborador (codigo_interno_colaborador),
    INDEX idx_snapshot_cliente (cod_cliente)
);

