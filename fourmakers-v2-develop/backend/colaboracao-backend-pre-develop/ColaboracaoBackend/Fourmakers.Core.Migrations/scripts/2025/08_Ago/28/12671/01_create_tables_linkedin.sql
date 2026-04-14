CREATE TABLE tb_tipos_emprego_linkedin (
    id CHAR(36) NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE tb_niveis_experiencia_linkedin (
    id CHAR(36) NOT NULL,
    descricao VARCHAR(100) NOT NULL,
    PRIMARY KEY (id)
);