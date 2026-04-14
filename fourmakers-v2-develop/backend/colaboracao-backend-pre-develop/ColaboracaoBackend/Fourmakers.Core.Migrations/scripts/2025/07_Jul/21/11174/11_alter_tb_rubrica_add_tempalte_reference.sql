-- Adicionar coluna com Foreign Key
ALTER TABLE tb_rubrica
ADD COLUMN tb_rubrica_template_id INT,
ADD CONSTRAINT fk_rubrica_template 
FOREIGN KEY (tb_rubrica_template_id) 
REFERENCES tb_rubrica_template(id);