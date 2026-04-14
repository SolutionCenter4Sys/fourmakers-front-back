CREATE TABLE `tb_template_email_rotina` (
  `id` char(36) NOT NULL,
  `emails` text NOT NULL,
  `tb_org_id` int NOT NULL,
  `corpo_email_parametrizado` text NOT NULL,
  `numero_tentativas` int NOT NULL DEFAULT '0',
  `mensagem_erro` varchar(400) DEFAULT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_disparo` timestamp NULL DEFAULT NULL,
  `data_alteracao` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_tb_org_id_idx` (`tb_org_id`),
  CONSTRAINT `fk_tb_org_id1` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`)
)