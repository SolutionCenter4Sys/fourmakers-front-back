ALTER TABLE tb_recurso_menu CHANGE codigo_recurso_pai codigo_recurso_menu_pai varchar(50) NULL;

ALTER TABLE tb_recurso_menu CHANGE codigo_recurso_menu codigo_recurso_menu varchar(50) AFTER nome_menu;
ALTER TABLE tb_recurso_menu CHANGE codigo_recurso codigo_recurso varchar(50) NOT NULL AFTER id;


-- Remover a constraint existente
ALTER TABLE tb_recurso_menu DROP FOREIGN KEY `fk_tb_recurso_menu_codigo_recurso_pai`;

-- Adicionar novamente a constraint com o nome correto e as ações desejadas
ALTER TABLE tb_recurso_menu 
ADD CONSTRAINT `fk_tb_recurso_menu_codigo_recurso_pai` 
FOREIGN KEY (`codigo_recurso_menu_pai`) 
REFERENCES `tb_recurso_menu` (`codigo_recurso_menu`) 
ON DELETE CASCADE 
ON UPDATE CASCADE;
