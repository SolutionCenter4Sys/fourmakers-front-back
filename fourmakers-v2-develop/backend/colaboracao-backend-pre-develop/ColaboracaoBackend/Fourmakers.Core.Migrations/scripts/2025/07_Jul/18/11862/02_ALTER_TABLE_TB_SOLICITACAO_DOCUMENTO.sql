ALTER TABLE tb_solicitacao_documento
DROP FOREIGN KEY fk_documento_reembolso;

ALTER TABLE tb_solicitacao_documento
    ADD CONSTRAINT fk_documento_reembolso
        FOREIGN KEY (tb_solicitacao_reembolso_id)
            REFERENCES tb_solicitacao_reembolso(id)
            ON DELETE CASCADE;