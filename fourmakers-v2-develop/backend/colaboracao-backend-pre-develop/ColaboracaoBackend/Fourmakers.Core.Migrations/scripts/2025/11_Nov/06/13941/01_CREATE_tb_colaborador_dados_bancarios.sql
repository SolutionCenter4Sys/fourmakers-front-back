CREATE TABLE tb_colaborador_dados_bancarios (
    id CHAR(36) NOT NULL PRIMARY KEY,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,

-- Dados bancários para TED
    codigo_banco_ted CHAR(3) DEFAULT NULL,
    agencia_ted CHAR(5) DEFAULT NULL,
    agencia_dv_ted CHAR(1) DEFAULT NULL,
    conta_ted CHAR(12) DEFAULT NULL,
    conta_dv_ted CHAR(1) DEFAULT NULL,

-- Dados para PIX
    chave_pix VARCHAR(100) DEFAULT NULL,
    tipo_chave_pix CHAR(1) DEFAULT NULL, -- 'C'=CPF, 'J'=CNPJ, 'E'=e-mail, 'T'=telefone, 'R'=EVP

    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

-- 🔒 UNIQUE por colaborador + org
    UNIQUE KEY uk_colaborador_org (codigo_interno_colaborador, tb_org_id),

-- 🔗 FKs
    CONSTRAINT fk_dados_bancarios_colaborador
        FOREIGN KEY (codigo_interno_colaborador)
            REFERENCES tb_colaborador_org (codigo_interno_colaborador)
            ON DELETE CASCADE
            ON UPDATE CASCADE,

    CONSTRAINT fk_dados_bancarios_org
        FOREIGN KEY (tb_org_id)
            REFERENCES tb_org (id)
            ON DELETE CASCADE
            ON UPDATE CASCADE
);