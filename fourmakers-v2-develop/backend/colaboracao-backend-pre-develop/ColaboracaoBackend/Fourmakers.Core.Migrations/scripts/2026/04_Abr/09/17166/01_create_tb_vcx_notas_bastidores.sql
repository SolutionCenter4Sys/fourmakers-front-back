-- Notas de bastidores por posição (VCX) + log dedicado — apenas descricao (max 2000 caracteres na aplicacao)

CREATE TABLE `tb_vcx_notas_bastidores` (
  `id` char(36) NOT NULL,
  `tb_organograma_posicao_id` char(36) NOT NULL,
  `descricao` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_tb_vcx_notas_bastidores_tb_organograma_posicao_idx` (`tb_organograma_posicao_id`),
  CONSTRAINT `fk_tb_vcx_notas_bastidores_tb_organograma_posicao` FOREIGN KEY (`tb_organograma_posicao_id`) REFERENCES `tb_organograma_posicao` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `tb_vcx_notas_bastidores_log` (
  `id` char(36) NOT NULL,
  `tb_colaborador_codigo_interno_colaborador_alterador` char(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `tb_organograma_posicao_id` char(36) NOT NULL,
  `acao` varchar(20) NOT NULL,
  `objeto` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'Valor antigo (JSON)',
  `alteracao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'Valor novo (JSON)',
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_tb_vcx_notas_bastidores_log_tb_colaborador_alterador_idx` (`tb_colaborador_codigo_interno_colaborador_alterador`),
  KEY `fk_tb_vcx_notas_bastidores_log_tb_organograma_posicao_idx` (`tb_organograma_posicao_id`),
  KEY `idx_tb_vcx_notas_bastidores_log_data_alteracao` (`data_alteracao`),
  CONSTRAINT `fk_tb_vcx_notas_bastidores_log_tb_colaborador_alterador` FOREIGN KEY (`tb_colaborador_codigo_interno_colaborador_alterador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_vcx_notas_bastidores_log_tb_organograma_posicao` FOREIGN KEY (`tb_organograma_posicao_id`) REFERENCES `tb_organograma_posicao` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
