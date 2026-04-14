-- Feedback 360: vários destinatários por feedback (reconhecimento em lote / time).
-- 1) Tabela de associação
-- 2) Backfill a partir de codigo_interno_colaborador_destinatario
-- 3) Remove coluna e FK antigas em tb_feedback360

CREATE TABLE `tb_feedback360_destinatario` (
  `id` char(36) NOT NULL,
  `tb_feedback360_id` char(36) NOT NULL,
  `codigo_interno_colaborador` char(36) CHARACTER SET utf8mb3 NOT NULL COMMENT 'Colaborador que recebe este feedback',
  `data_criacao` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_fb360_dest_feedback_colab` (`tb_feedback360_id`,`codigo_interno_colaborador`),
  KEY `idx_fb360_dest_colab` (`codigo_interno_colaborador`),
  KEY `idx_fb360_dest_feedback` (`tb_feedback360_id`),
  CONSTRAINT `fk_fb360_dest_feedback` FOREIGN KEY (`tb_feedback360_id`) REFERENCES `tb_feedback360` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_fb360_dest_colab` FOREIGN KEY (`codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Destinatários do feedback 360 (N por registro em tb_feedback360)';

INSERT INTO `tb_feedback360_destinatario` (`id`, `tb_feedback360_id`, `codigo_interno_colaborador`)
SELECT UUID(), f.`id`, f.`codigo_interno_colaborador_destinatario`
FROM `tb_feedback360` f;

ALTER TABLE `tb_feedback360`
  DROP FOREIGN KEY `fk_feedback360_destinatario`;

ALTER TABLE `tb_feedback360`
  DROP INDEX `idx_feedback360_destinatario_org`;

ALTER TABLE `tb_feedback360`
  DROP COLUMN `codigo_interno_colaborador_destinatario`;