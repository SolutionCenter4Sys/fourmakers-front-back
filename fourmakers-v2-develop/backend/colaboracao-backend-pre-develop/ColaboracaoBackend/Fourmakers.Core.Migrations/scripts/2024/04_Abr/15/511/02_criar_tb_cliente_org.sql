CREATE TABLE tb_cliente_org (
    id CHAR(36) PRIMARY KEY NOT NULL,
    codigo_cliente INT,
    nome_cliente VARCHAR(255),
    tb_org_id INT,
    ativo BOOLEAN,
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org`(`id`),
    UNIQUE (codigo_cliente, tb_org_id)
);