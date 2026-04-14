CREATE TABLE tb_projeto_gestor_externo (
    cod_projeto VARCHAR(255) NOT NULL,
    tb_org_id INT NOT NULL,
    cod_proposta VARCHAR(20) NOT NULL,
    cod_gestor_externo VARCHAR(36) NOT NULL,
    PRIMARY KEY (cod_projeto, tb_org_id, cod_proposta, cod_gestor_externo),
    FOREIGN KEY (cod_projeto, tb_org_id) REFERENCES tb_projeto_org (cod_projeto, tb_org_id),
    FOREIGN KEY (cod_proposta, tb_org_id, cod_projeto) REFERENCES tb_projeto_proposta (cod_proposta, tb_org_id, cod_projeto),
    FOREIGN KEY (cod_gestor_externo, tb_org_id) REFERENCES tb_gestor_externo (cod_gestor_externo, tb_org_id)
);