DELETE FROM tb_recurso;

ALTER TABLE tb_recurso_menu ADD codigo_recurso_menu VARCHAR(50);

ALTER TABLE tb_recurso_org_disponivel
  DROP CONSTRAINT `fk_tb_recurso_org_disponivel_tb_recurso1`;

ALTER TABLE tb_recurso_org_disponivel CHANGE codigo_recurso codigo_recurso_menu varchar(50) NOT NULL;

ALTER TABLE tb_recurso_menu 
ADD UNIQUE INDEX idx_codigo_recurso_menu (codigo_recurso_menu);

ALTER TABLE tb_recurso_org_disponivel
  ADD CONSTRAINT fk_tb_recurso_org_disp_cod_recurso_menu 
  FOREIGN KEY (codigo_recurso_menu) 
  REFERENCES tb_recurso_menu (codigo_recurso_menu) 
  ON DELETE CASCADE 
  ON UPDATE CASCADE;