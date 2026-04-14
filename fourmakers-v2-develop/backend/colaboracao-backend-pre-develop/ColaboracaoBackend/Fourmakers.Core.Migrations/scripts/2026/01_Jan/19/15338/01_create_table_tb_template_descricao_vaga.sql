CREATE TABLE tb_template_descricao_vaga (
    id CHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,
    descricao LONGTEXT NOT NULL,
    CONSTRAINT pk_tb_template_descricao_vaga
        PRIMARY KEY (id),
    CONSTRAINT fk_tb_template_descricao_vaga_tb_org
        FOREIGN KEY (tb_org_id)
        REFERENCES tb_org (id)
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci;