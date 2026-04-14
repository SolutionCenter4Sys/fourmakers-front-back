-- Script de criação de tabelas baseado no MER
-- Data: 2025-07-17
-- Versão: 11136

-- Tabela tb_fila
CREATE TABLE tb_fila (
    id VARCHAR(36) NOT NULL PRIMARY KEY,
    descricao VARCHAR(255) NOT NULL
);

-- Tabela tb_lote
CREATE TABLE tb_lote (
    id VARCHAR(36) NOT NULL PRIMARY KEY,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    aprovado_para_processamento TINYINT NOT NULL DEFAULT 0,
    sumario JSON NULL,
    data_finalizacao DATETIME NULL,
    quantidade_paginas INT NOT NULL,
    file_path VARCHAR(255) NULL,
    tb_org_id INT NOT NULL,
    tb_usuario_id BIGINT NOT NULL,
    tb_fila_id VARCHAR(36) NOT NULL,
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    FOREIGN KEY (tb_usuario_id) REFERENCES tb_usuario(id),
    FOREIGN KEY (tb_fila_id) REFERENCES tb_fila(id)
);

-- Tabela tb_item_lote
CREATE TABLE tb_item_lote (
    id VARCHAR(36) NOT NULL PRIMARY KEY,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    file_path VARCHAR(255) NULL,
    data_finalizacao DATETIME NULL,
    sucesso TINYINT NULL,
    retorno JSON NULL,
    tb_lote_id VARCHAR(36) NOT NULL,
    FOREIGN KEY (tb_lote_id) REFERENCES tb_lote(id)
);

-- Tabela tb_folhaponto_colaborador
CREATE TABLE tb_folhaponto_colaborador (
    tb_item_lote_id VARCHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    objeto_folhaponto JSON NOT NULL,
    competencia VARCHAR(7) NOT NULL,
    cnpj VARCHAR(14) NOT NULL,
    PRIMARY KEY (tb_item_lote_id, codigo_interno_colaborador),
    FOREIGN KEY (tb_item_lote_id) REFERENCES tb_item_lote(id),
    FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador)
);

-- Tabela tb_holerite_colaborador
CREATE TABLE tb_holerite_colaborador (
    tb_item_lote_id VARCHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    objeto_holerite JSON NOT NULL,
    competencia VARCHAR(7) NOT NULL,
    cnpj VARCHAR(14) NOT NULL,
    PRIMARY KEY (tb_item_lote_id, codigo_interno_colaborador),
    FOREIGN KEY (tb_item_lote_id) REFERENCES tb_item_lote(id),
    FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador)
);

-- Índices para melhorar performance
CREATE INDEX IX_tb_lote_tb_org_id ON tb_lote(tb_org_id);
CREATE INDEX IX_tb_lote_tb_usuario_id ON tb_lote(tb_usuario_id);
CREATE INDEX IX_tb_lote_tb_fila_id ON tb_lote(tb_fila_id);
CREATE INDEX IX_tb_item_lote_tb_lote_id ON tb_item_lote(tb_lote_id); 