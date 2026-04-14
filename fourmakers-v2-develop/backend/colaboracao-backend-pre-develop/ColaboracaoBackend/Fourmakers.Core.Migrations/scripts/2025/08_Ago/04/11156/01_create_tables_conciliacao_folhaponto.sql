-- Criação da tabela tb_conciliacao_folhaponto
CREATE TABLE tb_conciliacao_folhaponto (
    id VARCHAR(36) NOT NULL,
    competencia VARCHAR(7) NOT NULL,
    cnpj VARCHAR(14) NOT NULL,
    tb_lote_id VARCHAR(36) NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (tb_lote_id) REFERENCES tb_lote(id)
);

-- Criação da tabela tb_conciliacao_folhaponto_colaborador
CREATE TABLE tb_conciliacao_folhaponto_colaborador (
    id VARCHAR(36) NOT NULL,
    houve_divergencia TINYINT NOT NULL,
    numero_divergencias INT NOT NULL,
    tb_item_lote_id VARCHAR(36) NOT NULL,
    tb_conciliacao_folhaponto_id VARCHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (tb_item_lote_id) REFERENCES tb_item_lote(id),
    FOREIGN KEY (tb_conciliacao_folhaponto_id) REFERENCES tb_conciliacao_folhaponto(id),
    FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador)
);

-- Criação da tabela tb_conciliacao_folhaponto_divergencia
CREATE TABLE tb_conciliacao_folhaponto_divergencia (
    id VARCHAR(36) NOT NULL,
    campo_divergencia VARCHAR(255) NOT NULL,
    mensagem TEXT NOT NULL,
    valor_esperado VARCHAR(255) NULL,
    valor_contabilidade VARCHAR(255) NULL,
    tb_conciliacao_folhaponto_colaborador_id VARCHAR(36) NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (tb_conciliacao_folhaponto_colaborador_id) REFERENCES tb_conciliacao_folhaponto_colaborador(id)
); 


CREATE TABLE tb_projeto_regime_he (
    cod_projeto VARCHAR(255) NOT NULL,
    tb_org_id INT NOT NULL,
    modalidade_pagamento_horasextras varchar(255) NOT NULL,
    PRIMARY KEY (cod_projeto, tb_org_id),
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    FOREIGN KEY (cod_projeto) REFERENCES tb_projeto_org(cod_projeto)
);