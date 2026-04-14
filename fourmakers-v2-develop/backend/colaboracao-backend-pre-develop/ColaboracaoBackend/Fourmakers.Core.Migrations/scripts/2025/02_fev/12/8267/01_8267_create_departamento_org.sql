CREATE TABLE tb_departamento_org (
    id CHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,
    ativo TINYINT(1) NOT NULL DEFAULT '1',
    codigo_interno_colaborador_criacao VARCHAR(36) NOT NULL,
    departamento VARCHAR(255) NOT NULL,
    cod_departamento VARCHAR(255) NOT NULL, -- Tamanho não especificado, assumindo VARCHAR(255)
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE (tb_org_id, cod_departamento), -- Restrição UNIQUE composta
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
);