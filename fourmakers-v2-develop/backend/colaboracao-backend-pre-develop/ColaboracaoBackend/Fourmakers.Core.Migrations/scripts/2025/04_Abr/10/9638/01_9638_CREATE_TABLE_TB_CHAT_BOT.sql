CREATE TABLE `tb_chat_bot` (
  `id` int NOT NULL AUTO_INCREMENT,
  `codigo_interno_colaborador` varchar(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `ativo` tinyint(1) DEFAULT '1',
  `data_criacao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `codigo_interno_colaborador` (`codigo_interno_colaborador`),
  KEY `tb_org_id` (`tb_org_id`),
  CONSTRAINT `tb_chat_bot_ibfk_1` FOREIGN KEY (`codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`),
  CONSTRAINT `tb_chat_bot_ibfk_2` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`)
);