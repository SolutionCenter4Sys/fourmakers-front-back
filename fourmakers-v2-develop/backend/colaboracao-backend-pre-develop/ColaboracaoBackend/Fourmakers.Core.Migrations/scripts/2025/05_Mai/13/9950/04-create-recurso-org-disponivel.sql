CREATE TABLE `tb_recurso_org_disponivel` (
  `tb_org_id` int NOT NULL,
  `codigo_recurso` varchar(50) NOT NULL,
  PRIMARY KEY (`tb_org_id`,`codigo_recurso`),
  KEY `fk_tb_recurso_org_disponivel_tb_recurso1_idx` (`codigo_recurso`),
  KEY `fk_tb_recurso_org_disponivel_tb_org_idx` (`tb_org_id`),
  CONSTRAINT `fk_tb_recurso_org_disponivel_tb_org` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`),
  CONSTRAINT `fk_tb_recurso_org_disponivel_tb_recurso1` FOREIGN KEY (`codigo_recurso`) REFERENCES `tb_recurso` (`codigo_recurso`) ON DELETE CASCADE ON UPDATE CASCADE
);