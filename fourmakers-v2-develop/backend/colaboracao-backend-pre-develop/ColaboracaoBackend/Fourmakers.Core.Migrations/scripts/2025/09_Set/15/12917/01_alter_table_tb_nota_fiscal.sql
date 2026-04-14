ALTER TABLE tb_nota_fiscal
    ADD COLUMN tb_org_id INT NOT NULL;

ALTER TABLE tb_nota_fiscal
ADD CONSTRAINT fk_tb_nota_fiscal_tb_org
FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
ON DELETE RESTRICT;
