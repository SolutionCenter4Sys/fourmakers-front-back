CREATE TABLE tb_banco_talentos (
    codigo_interno_colaborador VARCHAR(255) NOT NULL,
    tipo_cadastro VARCHAR(255) NOT NULL,
    tb_org_id INT NOT NULL,
    CONSTRAINT fk_tb_org_id FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
);
