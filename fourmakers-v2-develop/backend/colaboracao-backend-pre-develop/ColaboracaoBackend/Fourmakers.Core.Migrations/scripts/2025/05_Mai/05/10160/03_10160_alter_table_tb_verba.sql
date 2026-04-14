ALTER TABLE tb_verba
    ADD CONSTRAINT fk_verba_tipo_custo
        FOREIGN KEY (tipo_custo) REFERENCES tb_verba_tipo(id);