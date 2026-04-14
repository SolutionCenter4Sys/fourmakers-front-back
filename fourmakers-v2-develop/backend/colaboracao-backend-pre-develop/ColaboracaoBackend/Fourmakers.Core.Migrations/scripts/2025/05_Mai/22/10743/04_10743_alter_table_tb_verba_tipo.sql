ALTER TABLE tb_verba_tipo
    ADD COLUMN ativo tinyint(1) DEFAULT 1;

ALTER TABLE tb_verba_tipo
    ADD COLUMN tb_org_id int NOT NULL;

ALTER TABLE tb_verba_tipo
    ADD COLUMN tb_verba_tipo_custo_id INT NOT NULL,
ADD CONSTRAINT fk_tb_verba_tipo_tb_verba_tipo_custo
    FOREIGN KEY (tb_verba_tipo_custo_id)
    REFERENCES tb_verba_tipo_custo(id);
