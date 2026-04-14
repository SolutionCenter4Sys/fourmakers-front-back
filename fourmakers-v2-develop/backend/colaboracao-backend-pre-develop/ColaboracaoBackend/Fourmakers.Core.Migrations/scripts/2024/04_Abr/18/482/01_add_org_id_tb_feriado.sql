ALTER TABLE tb_feriado
ADD tb_org_id INT NOT NULL;

ALTER TABLE tb_feriado
ADD CONSTRAINT fk_feriado_tb_org_id FOREIGN KEY (tb_org_id) REFERENCES tb_org(id);