CREATE TABLE `tb_parametro_configuracao` (
  `id` char(36) NOT NULL,
  `tb_org_id` int DEFAULT NULL,
  `tb_colaborador_org_cpf` varchar(11) DEFAULT NULL,
  `tb_grupo_acesso_id` int DEFAULT NULL,
  `codigo_parametro` varchar(255) DEFAULT NULL,
  `valor_parametro` varchar(1000) DEFAULT NULL,
  `tb_parametro_nivel_id` int DEFAULT NULL,
  `data_criacao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`),
  FOREIGN KEY (`codigo_parametro`) REFERENCES `tb_parametro` (`codigo_parametro`),
  FOREIGN KEY (`tb_org_id`, `tb_colaborador_org_cpf`) REFERENCES `tb_colaborador_org`(`tb_org_id`, `tb_colaborador_cpf`),
  FOREIGN KEY (`tb_grupo_acesso_id`) REFERENCES `tb_grupo_acesso` (`id`),
  FOREIGN KEY (`tb_parametro_nivel_id`) REFERENCES `tb_parametro_nivel` (`id`)
);