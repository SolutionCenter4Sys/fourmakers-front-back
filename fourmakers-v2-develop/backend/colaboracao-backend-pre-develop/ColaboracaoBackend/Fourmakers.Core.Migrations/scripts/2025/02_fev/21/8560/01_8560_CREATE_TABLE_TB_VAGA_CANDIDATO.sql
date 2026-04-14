CREATE TABLE tb_vaga_candidato (
    id VARCHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    vaga_id BIGINT NOT NULL,
    tb_org_id INT NOT NULL,
    candidato_id INT NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,  
    PRIMARY KEY (id),
    FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador),
    FOREIGN KEY (vaga_id) REFERENCES tb_vagas_srs(id_vaga),
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
);