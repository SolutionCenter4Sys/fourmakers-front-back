CREATE TABLE tb_rubrica (
    id CHAR(36) NOT NULL,
    ativo TINYINT(1) NOT NULL DEFAULT '1',
    codigo_interno_colaborador_alteracao VARCHAR(36) NOT NULL,
    codigo_interno_colaborador_criacao VARCHAR(36) NOT NULL,
    descricao VARCHAR(100) NOT NULL,
    codigo_rubrica_tipo VARCHAR(50) NOT NULL,
    tb_org_id INT NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE (descricao, tb_org_id),
    FOREIGN KEY (codigo_rubrica_tipo) REFERENCES tb_rubrica_tipo(codigo_rubrica_tipo),
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
);


SELECT * FROM tb_rubrica