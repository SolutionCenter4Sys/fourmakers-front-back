-- Tabela tb_gestor_area_atuacao
CREATE TABLE tb_gestor_externo_area_atuacao (
    tb_org_id INT NOT NULL,
    cod_gestor_externo VARCHAR(36) NOT NULL,
    tb_area_atuacao_id VARCHAR(36) NOT NULL,
    tb_permanencia_id VARCHAR(36) NOT NULL,
    
    -- Definindo a chave primária composta
    PRIMARY KEY (tb_org_id, cod_gestor_externo, tb_area_atuacao_id),

    -- Chaves estrangeiras
    FOREIGN KEY (tb_org_id, cod_gestor_externo) 
        REFERENCES tb_gestor_externo (tb_org_id, cod_gestor_externo),
    FOREIGN KEY (tb_area_atuacao_id) 
        REFERENCES tb_area_atuacao (id),
    FOREIGN KEY (tb_permanencia_id) 
        REFERENCES tb_permanencia (id)
);
