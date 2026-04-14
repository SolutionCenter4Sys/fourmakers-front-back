-- ============================================================
-- Feedback 360 - DDL
-- Tabelas: nível de relacionamento, avaliações, feedback, log
-- ============================================================

-- 1) Nível de relacionamento (gestor, colega, etc.)
CREATE TABLE `tb_feedback360_relacionamento` (
  `id` int NOT NULL AUTO_INCREMENT,
  `descricao` varchar(100) NOT NULL,
  `ativo` tinyint NOT NULL DEFAULT '1',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Nível de relacionamento entre quem envia e quem recebe o feedback (gestor, colega, etc.)';

-- 2) Opções de avaliação (as 5 carinhas) - utf8mb4 para emojis e acentos
CREATE TABLE `tb_feedback360_avaliacoes` (
  `id` int NOT NULL AUTO_INCREMENT,
  `codigo` int NOT NULL,
  `descricao` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ativo` tinyint NOT NULL DEFAULT '1',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_feedback360_avaliacoes_codigo` (`codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Opções de avaliação do feedback 360 (suporta emojis e acentos)';

-- 3) Registro do feedback (enviar feedback) - utf8mb4 para titulo/descricao com emojis e acentos
CREATE TABLE `tb_feedback360` (
  `id` CHAR(36) NOT NULL,
  `codigo_interno_colaborador_remetente` CHAR(36) CHARACTER SET utf8mb3 NOT NULL COMMENT 'Quem envia o feedback',
  `codigo_interno_colaborador_destinatario` CHAR(36) CHARACTER SET utf8mb3 NOT NULL COMMENT 'Quem recebe o feedback',
  `tb_org_id` int NOT NULL,
  `titulo` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Título (campo aberto obrigatório)',
  `descricao` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Descrição (campo aberto obrigatório)',
  `data_interacao` date NOT NULL COMMENT 'Data da interação (hoje ou passado)',
  `tb_feedback360_relacionamento_id` int NOT NULL COMMENT 'Gestor, colega, etc.',
  `tb_feedback360_avaliacao_id` int NOT NULL COMMENT 'Avaliação (1 a 5)',
  `data_criacao` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `editado` tinyint NOT NULL DEFAULT '0' COMMENT '1 se foi editado (destinatário vê que editou, mas não o quê)',
  PRIMARY KEY (`id`),
  KEY `fk_feedback360_remetente` (`codigo_interno_colaborador_remetente`),
  KEY `fk_feedback360_destinatario` (`codigo_interno_colaborador_destinatario`),
  KEY `fk_feedback360_org` (`tb_org_id`),
  KEY `fk_feedback360_relacionamento` (`tb_feedback360_relacionamento_id`),
  KEY `fk_feedback360_avaliacao` (`tb_feedback360_avaliacao_id`),
  KEY `idx_feedback360_destinatario_org` (`codigo_interno_colaborador_destinatario`, `tb_org_id`),
  KEY `idx_feedback360_remetente_org` (`codigo_interno_colaborador_remetente`, `tb_org_id`),
  CONSTRAINT `fk_feedback360_remetente` FOREIGN KEY (`codigo_interno_colaborador_remetente`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON UPDATE CASCADE,
  CONSTRAINT `fk_feedback360_destinatario` FOREIGN KEY (`codigo_interno_colaborador_destinatario`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON UPDATE CASCADE,
  CONSTRAINT `fk_feedback360_org` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `fk_feedback360_relacionamento` FOREIGN KEY (`tb_feedback360_relacionamento_id`) REFERENCES `tb_feedback360_relacionamento` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `fk_feedback360_avaliacao` FOREIGN KEY (`tb_feedback360_avaliacao_id`) REFERENCES `tb_feedback360_avaliacoes` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Feedback 360 enviado de um colaborador para outro (titulo/descricao suportam emojis e acentos)';

-- 4) Log de edições (objeto antigo, objeto novo, ação) - padrão tb_vcx_dores_log
CREATE TABLE `tb_feedback360_log` (
  `id` char(36) NOT NULL,
  `tb_feedback360_id` char(36) NOT NULL,
  `tb_colaborador_codigo_interno_colaborador_alterador` char(36) CHARACTER SET utf8mb3 NOT NULL,
  `acao` varchar(20) NOT NULL COMMENT 'Ex: INSERT, UPDATE',
  `objeto` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Valor antigo (JSON)',
  `alteracao` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Valor novo (JSON)',
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_tb_feedback360_log_feedback_idx` (`tb_feedback360_id`),
  KEY `fk_tb_feedback360_log_colaborador_alterador_idx` (`tb_colaborador_codigo_interno_colaborador_alterador`),
  KEY `idx_tb_feedback360_log_data_alteracao` (`data_alteracao`),
  CONSTRAINT `fk_tb_feedback360_log_feedback` FOREIGN KEY (`tb_feedback360_id`) REFERENCES `tb_feedback360` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_feedback360_log_colaborador_alterador` FOREIGN KEY (`tb_colaborador_codigo_interno_colaborador_alterador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Log de edições do feedback 360 (objeto antigo, novo e ação)';

-- Dados iniciais: níveis de relacionamento (ajustar conforme necessidade)
INSERT INTO `tb_feedback360_relacionamento` (`descricao`) VALUES
('Gestor'),
('Colega'),
('Liderado'),
('Outro');

-- Dados iniciais: opções de avaliação (as 5 opções, com emojis)
INSERT INTO `tb_feedback360_avaliacoes` (`codigo`, `descricao`) VALUES
(1, '😞 Precisa evoluir bastante'),
(2, '🙁 Abaixo do esperado'),
(3, '😐 Dentro do esperado'),
(4, '😊 Acima do esperado'),
(5, '🤩 Referência para os demais');