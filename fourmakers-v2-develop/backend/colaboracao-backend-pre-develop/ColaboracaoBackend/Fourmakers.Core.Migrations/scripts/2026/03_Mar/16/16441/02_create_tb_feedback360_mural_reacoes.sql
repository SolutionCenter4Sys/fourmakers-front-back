-- ============================================================
-- Feedback 360 - Mural: registro de quem reagiu a cada card com qual emoji
-- Uma linha = um colaborador reagiu a um feedback (card) com um tipo de emoji.
-- Toggle: inserir = curtir; remover = descurtir.
-- ============================================================

CREATE TABLE `tb_feedback360_mural_reacoes` (
  `tb_feedback360_id` char(36) NOT NULL COMMENT 'Card do mural (feedback)',
  `tb_feedback360_mural_reacao_id` int NOT NULL COMMENT 'Tipo de emoji (FK tb_feedback360_mural_emojis_reacao)',
  `codigo_interno_colaborador` char(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL COMMENT 'Quem reagiu',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`tb_feedback360_id`, `tb_feedback360_mural_reacao_id`, `codigo_interno_colaborador`),
  KEY `fk_mural_reacoes_emojis_reacao` (`tb_feedback360_mural_reacao_id`),
  KEY `fk_mural_reacoes_colaborador` (`codigo_interno_colaborador`),
  CONSTRAINT `fk_mural_reacoes_feedback` FOREIGN KEY (`tb_feedback360_id`) REFERENCES `tb_feedback360` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_mural_reacoes_emojis_reacao` FOREIGN KEY (`tb_feedback360_mural_reacao_id`) REFERENCES `tb_feedback360_mural_emojis_reacao` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `fk_mural_reacoes_colaborador` FOREIGN KEY (`codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Reações dos colaboradores aos cards do mural (quem reagiu a qual feedback com qual emoji)';
