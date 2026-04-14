ALTER TABLE `tb_grupo_acesso`
ADD COLUMN `acesso_todos_clientes` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Indica se o grupo de acesso tem acesso a todos os clientes da organização';

