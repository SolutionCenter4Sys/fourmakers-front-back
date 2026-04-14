ALTER TABLE tb_cnab_org CHANGE diretoria cod_diretoria varchar(255) NULL;
ALTER TABLE tb_cnab_org MODIFY COLUMN cod_diretoria varchar(255) NULL;

ALTER TABLE tb_cnab_remessa CHANGE codigo_diretoria cod_diretoria varchar(255) NOT NULL;
