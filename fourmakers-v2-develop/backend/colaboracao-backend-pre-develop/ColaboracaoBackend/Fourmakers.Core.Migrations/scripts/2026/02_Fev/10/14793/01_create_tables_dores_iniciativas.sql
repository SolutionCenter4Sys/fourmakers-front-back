-- ============================================================
-- DDL VCX - Tabelas de dores, iniciativas, impactos, urgencias,
--           status, temas e logs
-- ============================================================

-- gcolb_hml.tb_vcx_impactos definition
CREATE TABLE `tb_vcx_impactos` (
  `id` char(36) NOT NULL,
  `descricao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- gcolb_hml.tb_vcx_urgencias definition
CREATE TABLE `tb_vcx_urgencias` (
  `id` char(36) NOT NULL,
  `descricao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- gcolb_hml.tb_vcx_status definition
CREATE TABLE `tb_vcx_status` (
  `id` char(36) NOT NULL,
  `descricao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- gcolb_hml.tb_vcx_temas definition
CREATE TABLE `tb_vcx_temas` (
  `id` char(36) NOT NULL,
  `descricao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `tb_org_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_tb_vcx_temas_tb_org_idx` (`tb_org_id`),
  CONSTRAINT `fk_tb_vcx_temas_tb_org` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- gcolb_hml.tb_vcx_dores definition
CREATE TABLE `tb_vcx_dores` (
  `id` char(36) NOT NULL,
  `tb_organograma_posicao_id` char(36) NOT NULL,
  `titulo` varchar(255) NOT NULL,
  `descricao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `tb_vcx_impactos_id` char(36) DEFAULT NULL,
  `tb_vcx_urgencias_id` char(36) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_tb_vcx_dores_tb_organograma_posicao_idx` (`tb_organograma_posicao_id`),
  KEY `fk_tb_vcx_dores_tb_vcx_impactos_idx` (`tb_vcx_impactos_id`),
  KEY `fk_tb_vcx_dores_tb_vcx_urgencias_idx` (`tb_vcx_urgencias_id`),
  CONSTRAINT `fk_tb_vcx_dores_tb_organograma_posicao` FOREIGN KEY (`tb_organograma_posicao_id`) REFERENCES `tb_organograma_posicao` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_vcx_dores_tb_vcx_impactos` FOREIGN KEY (`tb_vcx_impactos_id`) REFERENCES `tb_vcx_impactos` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_vcx_dores_tb_vcx_urgencias` FOREIGN KEY (`tb_vcx_urgencias_id`) REFERENCES `tb_vcx_urgencias` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- gcolb_hml.tb_vcx_iniciativas definition
CREATE TABLE `tb_vcx_iniciativas` (
  `id` char(36) NOT NULL,
  `tb_organograma_posicao_id` char(36) NOT NULL,
  `titulo` varchar(255) NOT NULL,
  `descricao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `tb_vcx_status_id` char(36) DEFAULT NULL,
  `tb_vcx_temas_id` char(36) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_tb_vcx_iniciativas_tb_organograma_posicao_idx` (`tb_organograma_posicao_id`),
  KEY `fk_tb_vcx_iniciativas_tb_vcx_status_idx` (`tb_vcx_status_id`),
  KEY `fk_tb_vcx_iniciativas_tb_vcx_temas_idx` (`tb_vcx_temas_id`),
  CONSTRAINT `fk_tb_vcx_iniciativas_tb_organograma_posicao` FOREIGN KEY (`tb_organograma_posicao_id`) REFERENCES `tb_organograma_posicao` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_vcx_iniciativas_tb_vcx_status` FOREIGN KEY (`tb_vcx_status_id`) REFERENCES `tb_vcx_status` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_vcx_iniciativas_tb_vcx_temas` FOREIGN KEY (`tb_vcx_temas_id`) REFERENCES `tb_vcx_temas` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- gcolb_hml.tb_vcx_dores_log definition
CREATE TABLE `tb_vcx_dores_log` (
  `id` char(36) NOT NULL,
  `tb_colaborador_codigo_interno_colaborador_alterador` char(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `tb_organograma_posicao_id` char(36) NOT NULL,
  `acao` varchar(20) NOT NULL,
  `objeto` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'Valor antigo (JSON)',
  `alteracao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'Valor novo (JSON)',
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_tb_vcx_dores_log_tb_colaborador_alterador_idx` (`tb_colaborador_codigo_interno_colaborador_alterador`),
  KEY `fk_tb_vcx_dores_log_tb_organograma_posicao_idx` (`tb_organograma_posicao_id`),
  KEY `idx_tb_vcx_dores_log_data_alteracao` (`data_alteracao`),
  CONSTRAINT `fk_tb_vcx_dores_log_tb_colaborador_alterador` FOREIGN KEY (`tb_colaborador_codigo_interno_colaborador_alterador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_vcx_dores_log_tb_organograma_posicao` FOREIGN KEY (`tb_organograma_posicao_id`) REFERENCES `tb_organograma_posicao` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- gcolb_hml.tb_vcx_iniciativas_log definition
CREATE TABLE `tb_vcx_iniciativas_log` (
  `id` varchar(36) NOT NULL,
  `tb_colaborador_codigo_interno_colaborador_alterador` char(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `tb_organograma_posicao_id` varchar(36) NOT NULL,
  `acao` varchar(20) NOT NULL,
  `objeto` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'Valor antigo (JSON)',
  `alteracao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'Valor novo (JSON)',
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_tb_vcx_iniciativas_log_tb_colaborador_alterador_idx` (`tb_colaborador_codigo_interno_colaborador_alterador`),
  KEY `fk_tb_vcx_iniciativas_log_tb_organograma_posicao_idx` (`tb_organograma_posicao_id`),
  KEY `idx_tb_vcx_iniciativas_log_data_alteracao` (`data_alteracao`),
  CONSTRAINT `fk_tb_vcx_iniciativas_log_tb_colaborador_alterador` FOREIGN KEY (`tb_colaborador_codigo_interno_colaborador_alterador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_vcx_iniciativas_log_tb_organograma_posicao` FOREIGN KEY (`tb_organograma_posicao_id`) REFERENCES `tb_organograma_posicao` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;