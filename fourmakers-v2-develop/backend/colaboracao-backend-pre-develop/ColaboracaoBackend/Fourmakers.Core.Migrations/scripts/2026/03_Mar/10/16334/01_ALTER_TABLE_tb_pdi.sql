ALTER TABLE tb_pdi
    ADD COLUMN dead_line DATETIME DEFAULT NULL,
    ADD COLUMN tb_org_id INT DEFAULT NULL,
ADD CONSTRAINT fk_tb_org_to_org
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id);