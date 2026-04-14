CREATE TABLE `tb_perfil_corporativo_alocacao` (
  `id` varchar(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `tb_perfil_corporativo_id` char(36) NOT NULL,
  `codigo_interno_colaborador` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `data_inicio` datetime NOT NULL,
  `data_fim` datetime DEFAULT NULL,
  `ativo` tinyint DEFAULT '1',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_pfcorp_aloc_org` (`tb_org_id`),
  KEY `idx_pfcorp_aloc_perfil` (`tb_perfil_corporativo_id`),
  KEY `idx_pfcorp_aloc_colab` (`codigo_interno_colaborador`),
  KEY `idx_pfcorp_aloc_vigencia` (`data_fim`),
  CONSTRAINT `fk_pfcorp_aloc_colab` FOREIGN KEY (`codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`),
  CONSTRAINT `fk_pfcorp_aloc_org` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`),
  CONSTRAINT `fk_pfcorp_aloc_perfil` FOREIGN KEY (`tb_perfil_corporativo_id`) REFERENCES `tb_perfil_corporativo` (`id`)
);