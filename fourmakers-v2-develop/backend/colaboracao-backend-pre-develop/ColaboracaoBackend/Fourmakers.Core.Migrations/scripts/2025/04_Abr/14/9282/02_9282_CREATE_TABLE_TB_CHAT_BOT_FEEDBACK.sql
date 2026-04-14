CREATE TABLE `tb_chat_bot_feedback` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `questao_id` INT NOT NULL,
  `questao` TEXT NOT NULL,
  `resposta` TEXT NOT NULL,
  `comentario` TEXT,
  `feedback` TINYINT(1) NOT NULL,
  `codigo_interno_colaborador` VARCHAR(36) NOT NULL,
  `data_criacao` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `questao_id` (`questao_id`),
  KEY `codigo_interno_colaborador` (`codigo_interno_colaborador`),
  CONSTRAINT `tb_chat_bot_feedback_ibfk_1` FOREIGN KEY (`questao_id`) REFERENCES `tb_questao_chat_bot` (`id`),
  CONSTRAINT `tb_chat_bot_feedback_ibfk_2` FOREIGN KEY (`codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`)
);
 