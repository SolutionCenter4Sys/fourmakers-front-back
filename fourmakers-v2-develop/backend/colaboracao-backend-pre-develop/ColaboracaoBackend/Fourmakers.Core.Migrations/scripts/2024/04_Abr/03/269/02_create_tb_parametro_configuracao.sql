create TABLE tb_org_parametro_configuracao (
    id char(36) NOT NULL PRIMARY KEY,
    tb_org_id INT,
    codigo_parametro VARCHAR(255), -- Chave estrangeira para tb_parametro
    valor_parametro VARCHAR(1000),
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    FOREIGN KEY (codigo_parametro) REFERENCES tb_parametro(codigo_parametro)
);