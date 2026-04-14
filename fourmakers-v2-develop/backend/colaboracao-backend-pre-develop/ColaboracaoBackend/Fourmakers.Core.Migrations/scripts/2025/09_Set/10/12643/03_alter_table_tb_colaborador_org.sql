ALTER TABLE tb_colaborador_org
    ADD COLUMN codigo_modelo_contratacao VARCHAR(36) DEFAULT NULL;

ALTER TABLE tb_colaborador_org
    ADD CONSTRAINT fk_colaborador_org_modelo_contratacao
        FOREIGN KEY (codigo_modelo_contratacao, tb_org_id)
            REFERENCES tb_modelo_contratacao_org (codigo_modelo_contratacao, tb_org_id);