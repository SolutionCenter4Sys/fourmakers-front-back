CREATE TABLE `tb_recurso_menu_funcionalidade_sistema` (
  `codigo_recurso_menu` VARCHAR(50) NOT NULL,
  `tb_funcionalidade_sistema_id` INT NOT NULL,
  PRIMARY KEY (`tb_funcionalidade_sistema_id`, `codigo_recurso_menu`),
  KEY `fk_recmenu_funcionalidade_menu` (`codigo_recurso_menu`),
  CONSTRAINT `fk_recmenu_funcionalidade_funcionalidade` 
      FOREIGN KEY (`tb_funcionalidade_sistema_id`) 
      REFERENCES `tb_funcionalidade_sistema` (`id`) 
      ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_recmenu_funcionalidade_menu` 
      FOREIGN KEY (`codigo_recurso_menu`) 
      REFERENCES `tb_recurso_menu` (`codigo_recurso_menu`) 
      ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;