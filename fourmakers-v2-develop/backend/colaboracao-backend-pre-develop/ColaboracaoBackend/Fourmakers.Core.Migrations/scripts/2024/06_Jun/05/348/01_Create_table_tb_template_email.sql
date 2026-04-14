CREATE TABLE `tb_template_email` (
  `id` char(36) NOT NULL,
  `codigo` varchar(45) NOT NULL,
  `descricao` varchar(200) DEFAULT NULL,
  `template` text,
  `tb_org_id` int NOT NULL,
  `ativo` tinyint DEFAULT '1',
  `data_alteracao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `data_criacao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_tb_org_id_idx` (`tb_org_id`),
  CONSTRAINT `fk_tb_org_id` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`)
)