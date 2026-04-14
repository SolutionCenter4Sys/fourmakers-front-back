ALTER TABLE tb_nota_fiscal
DROP FOREIGN KEY tb_nota_fiscal_ibfk_1,
DROP FOREIGN KEY tb_nota_fiscal_ibfk_2;

ALTER TABLE tb_nota_fiscal
    MODIFY COLUMN codigo_interno_colaborador_criacao VARCHAR(36) DEFAULT NULL,
    MODIFY COLUMN codigo_interno_colaborador_alteracao VARCHAR(36) DEFAULT NULL;

ALTER TABLE tb_nota_fiscal
    ADD CONSTRAINT fk_nota_criacao_colaborador
        FOREIGN KEY (codigo_interno_colaborador_criacao)
            REFERENCES tb_colaborador (codigo_interno_colaborador)
            ON UPDATE CASCADE
            ON DELETE SET NULL,
ADD CONSTRAINT fk_nota_alteracao_colaborador
    FOREIGN KEY (codigo_interno_colaborador_alteracao)
    REFERENCES tb_colaborador (codigo_interno_colaborador)
    ON UPDATE CASCADE
       ON DELETE SET NULL;