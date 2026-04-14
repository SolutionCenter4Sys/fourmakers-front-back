ALTER TABLE tb_nota_fiscal_rubrica
ADD COLUMN tb_org_id INT NOT NULL;

ALTER TABLE tb_nota_fiscal_rubrica
ADD CONSTRAINT fk_tb_nota_fiscal_rubrica_tb_org
FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
ON DELETE RESTRICT;