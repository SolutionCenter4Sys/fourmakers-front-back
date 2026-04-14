CREATE TABLE tb_gestor_externo (
    cod_gestor_externo VARCHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,
    cod_interno_colaborador VARCHAR(36) NULL,
    nome VARCHAR(255) NOT NULL,
    email VARCHAR(255) NULL,
    telefone VARCHAR(20) NULL,
    
    PRIMARY KEY (cod_gestor_externo,tb_org_id),
    
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    FOREIGN KEY (cod_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador)
);
