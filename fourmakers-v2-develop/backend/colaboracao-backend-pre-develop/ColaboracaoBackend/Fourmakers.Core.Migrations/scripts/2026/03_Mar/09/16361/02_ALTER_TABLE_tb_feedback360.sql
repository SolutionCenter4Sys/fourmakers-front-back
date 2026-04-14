-- Passo 1: Avaliação opcional (permite gravar NULL quando o front enviar null)
ALTER TABLE `tb_feedback360`
  MODIFY COLUMN `tb_feedback360_avaliacao_id` int DEFAULT NULL COMMENT 'Avaliação (1 a 5). Opcional.';

-- Passo 2: Campo prévia (texto gerado por IA a partir do prompt do usuário)
ALTER TABLE `tb_feedback360`
  ADD COLUMN `previa` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Prévia gerada por IA a partir do prompt do usuário' AFTER `resultado`;