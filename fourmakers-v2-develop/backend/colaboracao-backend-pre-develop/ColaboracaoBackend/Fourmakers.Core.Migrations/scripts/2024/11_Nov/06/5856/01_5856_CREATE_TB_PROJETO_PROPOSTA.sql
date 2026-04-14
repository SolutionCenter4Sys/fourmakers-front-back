CREATE TABLE tb_projeto_proposta (
    cod_proposta VARCHAR(20) NOT NULL,
    tb_org_id INT NOT NULL,
    cod_projeto VARCHAR(255) NOT NULL,
    
    PRIMARY KEY (cod_proposta, tb_org_id, cod_projeto),
    
    FOREIGN KEY (cod_projeto, tb_org_id) 
        REFERENCES tb_projeto_org (cod_projeto, tb_org_id)
);
