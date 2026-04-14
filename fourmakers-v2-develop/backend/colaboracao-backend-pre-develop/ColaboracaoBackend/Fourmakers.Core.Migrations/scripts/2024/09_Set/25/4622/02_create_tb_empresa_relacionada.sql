CREATE TABLE `tb_empresa_relacionada_sugestao` (
  `nome` varchar(255) NOT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `codigo_interno_colaborador_criacao` varchar(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  PRIMARY KEY (`nome`, `tb_org_id`),
  KEY `fk_tb_empresa_relacionada_tb_org1_idx` (`tb_org_id`),
  KEY `fk_tb_empresa_relacionada_tb_colaborador1_idx` (`codigo_interno_colaborador_criacao`),
  CONSTRAINT `fk_tb_empresa_relacionada_tb_colaborador1` FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_empresa_relacionada_tb_org1` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;