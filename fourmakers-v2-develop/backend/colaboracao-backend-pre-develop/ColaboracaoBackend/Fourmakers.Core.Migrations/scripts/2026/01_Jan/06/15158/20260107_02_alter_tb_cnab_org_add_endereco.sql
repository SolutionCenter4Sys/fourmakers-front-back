
ALTER TABLE tb_cnab_org
ADD COLUMN endereco_empresa VARCHAR(30) NOT NULL,
ADD COLUMN numero_local VARCHAR(5) NOT NULL,
ADD COLUMN complemento_endereco VARCHAR(15) NULL,
ADD COLUMN cidade VARCHAR(20) NOT NULL,
ADD COLUMN cep CHAR(8) NOT NULL,
ADD COLUMN estado CHAR(2) NOT NULL;

-- Exemplo de INSERT com os novos campos (baseado no exemplo oficial)
INSERT INTO tb_cnab_org (
    id,
    tb_org_id,
    diretoria,
    codigo_banco,
    agencia,
    agencia_dv,
    conta,
    conta_dv,
    codigo_convenio,
    cnpj_empresa,
    forma_pagamento,
    nome_empresa,
    nome_banco,
    endereco_empresa,
    numero_local,
    complemento_endereco,
    cidade,
    cep,
    estado
) VALUES (
    UUID(),
    2,
    NULL,
    '341',
    '4807',
    NULL,
    '03004',
    '6',
    NULL,
    '03808125000130',
    'PIX',
    'FOURSYS PROJETOS E SISTEMAS EM',
    'BANCO ITAU S/A',
    'AV COPACABANA',
    '00190',
    NULL,
    'BARUERI',
    '00006472',
    'SP'
);
