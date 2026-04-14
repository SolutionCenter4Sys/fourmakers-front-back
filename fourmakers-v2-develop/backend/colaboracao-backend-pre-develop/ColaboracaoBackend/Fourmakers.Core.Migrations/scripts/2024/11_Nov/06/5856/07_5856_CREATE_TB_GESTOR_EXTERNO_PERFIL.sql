CREATE TABLE `tb_gestor_externo_perfil` (
  `id` varchar(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `cod_gestor_externo` varchar(36) NOT NULL,
  `nome_perfil` varchar(255) NOT NULL,
  `custo_perfil` decimal(10,2) NOT NULL,
  `ratecard_perfil` decimal(10,2) NOT NULL,
  `informacoes_relevantes` text,
  `tb_permanencia_id` varchar(36) DEFAULT NULL,
  `tb_modelo_trabalho_id` varchar(36) DEFAULT NULL,
  `tb_profissional_localidade_id` varchar(36) DEFAULT NULL,
  `ativo` tinyint(1) DEFAULT NULL,
  `data_criacao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `codigo_interno_colaborador_criacao` varchar(36) DEFAULT NULL,
  `codigo_interno_colaborador_alteracao` varchar(36) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `tb_org_id` (`tb_org_id`),
  KEY `cod_gestor_externo` (`cod_gestor_externo`, `tb_org_id`),
  KEY `tb_permanencia_id` (`tb_permanencia_id`),
  KEY `tb_modelo_trabalho_id` (`tb_modelo_trabalho_id`),
  KEY `tb_profissional_localidade_id` (`tb_profissional_localidade_id`),
  KEY `fk_codigo_interno_colaborador_criacao` (`codigo_interno_colaborador_criacao`),
  KEY `fk_codigo_interno_colaborador_alteracao` (`codigo_interno_colaborador_alteracao`),
  CONSTRAINT `fk_codigo_interno_colaborador_criacao` 
    FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`),
  CONSTRAINT `fk_codigo_interno_colaborador_alteracao` 
    FOREIGN KEY (`codigo_interno_colaborador_alteracao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`),
  CONSTRAINT `tb_gestor_externo_perfil_ibfk_1` 
    FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`),
  CONSTRAINT `tb_gestor_externo_perfil_ibfk_2` 
    FOREIGN KEY (`cod_gestor_externo`, `tb_org_id`) REFERENCES `tb_gestor_externo` (`cod_gestor_externo`, `tb_org_id`),
  CONSTRAINT `tb_gestor_externo_perfil_ibfk_3` 
    FOREIGN KEY (`tb_permanencia_id`) REFERENCES `tb_permanencia` (`id`),
  CONSTRAINT `tb_gestor_externo_perfil_ibfk_4` 
    FOREIGN KEY (`tb_modelo_trabalho_id`) REFERENCES `tb_modelo_trabalho` (`id`),
  CONSTRAINT `tb_gestor_externo_perfil_ibfk_5` 
    FOREIGN KEY (`tb_profissional_localidade_id`) REFERENCES `tb_profissional_localidade` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
