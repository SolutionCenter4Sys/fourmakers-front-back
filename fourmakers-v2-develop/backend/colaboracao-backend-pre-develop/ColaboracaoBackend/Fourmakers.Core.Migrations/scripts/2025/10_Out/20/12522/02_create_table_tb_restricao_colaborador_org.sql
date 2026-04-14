CREATE TABLE tb_restricao_acesso_colaborador (
     id BIGINT PRIMARY KEY AUTO_INCREMENT,
     codigo_interno_colaborador VARCHAR(36) NOT NULL,
     restricao_tipo VARCHAR(50) NOT NULL,
     valor VARCHAR(255) NOT NULL,
     tb_org_id int NOT NULL,
     data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
     data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

     CONSTRAINT fk_colab_org FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador_org(codigo_interno_colaborador),
     CONSTRAINT fk_colab_org_to_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
);