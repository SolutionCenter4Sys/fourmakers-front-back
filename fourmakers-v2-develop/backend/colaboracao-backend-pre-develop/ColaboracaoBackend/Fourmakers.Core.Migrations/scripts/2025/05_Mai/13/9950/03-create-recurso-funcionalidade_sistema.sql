CREATE TABLE `tb_recurso_funcionalidade_sistema` (
  `tb_funcionalidade_sistema_id` int NOT NULL,
  `codigo_recurso` varchar(50) NOT NULL,
  PRIMARY KEY (`tb_funcionalidade_sistema_id`,`codigo_recurso`),
  KEY `fk_recurso_funcionalidade_recurso` (`codigo_recurso`),
  CONSTRAINT `fk_recurso_funcionalidade_funcionalidade` FOREIGN KEY (`tb_funcionalidade_sistema_id`) REFERENCES `tb_funcionalidade_sistema` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_recurso_funcionalidade_recurso` FOREIGN KEY (`codigo_recurso`) REFERENCES `tb_recurso` (`codigo_recurso`) ON DELETE CASCADE ON UPDATE CASCADE
);