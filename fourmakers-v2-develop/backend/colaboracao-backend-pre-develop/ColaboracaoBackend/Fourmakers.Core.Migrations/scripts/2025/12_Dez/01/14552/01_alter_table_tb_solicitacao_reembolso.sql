ALTER TABLE tb_solicitacao_reembolso
DROP FOREIGN KEY fk_reembolso_projeto;

ALTER TABLE tb_solicitacao_reembolso
DROP FOREIGN KEY fk_reembolso_cliente;

ALTER TABLE tb_solicitacao_reembolso
    MODIFY COLUMN tb_projeto_id VARCHAR(255) NULL;

ALTER TABLE tb_solicitacao_reembolso
    MODIFY COLUMN tb_cliente_id VARCHAR(45) NULL;

ALTER TABLE tb_solicitacao_reembolso
    ADD CONSTRAINT fk_reembolso_projeto
        FOREIGN KEY (tb_projeto_id)
            REFERENCES tb_projeto_org(cod_projeto);

ALTER TABLE tb_solicitacao_reembolso
    ADD CONSTRAINT fk_reembolso_cliente
        FOREIGN KEY (tb_cliente_id)
            REFERENCES tb_cliente_org(codigo_cliente);