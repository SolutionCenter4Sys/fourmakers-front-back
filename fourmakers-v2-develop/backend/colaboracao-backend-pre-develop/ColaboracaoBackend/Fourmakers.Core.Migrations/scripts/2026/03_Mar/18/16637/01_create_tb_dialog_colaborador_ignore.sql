CREATE TABLE tb_colaborador_modais_ignorados (
    id int NOT NULL AUTO_INCREMENT,
    codigo_interno_colaborador varchar(36) NOT NULL,
    tb_org_id int NOT NULL,
    tag varchar(100) NOT NULL,
    data_criacao timestamp NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uk_modais_ignorados_colab_org_tag (codigo_interno_colaborador, tb_org_id, tag),
    CONSTRAINT fk_modais_ignorados_colaborador_org
    FOREIGN KEY (tb_org_id, codigo_interno_colaborador)
    REFERENCES tb_colaborador_org (tb_org_id, codigo_interno_colaborador)
);