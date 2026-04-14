ALTER TABLE tb_organograma_departamento
ADD CONSTRAINT fk_org_dept_lider
FOREIGN KEY (tb_organograma_posicao_id_lider)
REFERENCES tb_organograma_posicao (id)
ON DELETE RESTRICT;
