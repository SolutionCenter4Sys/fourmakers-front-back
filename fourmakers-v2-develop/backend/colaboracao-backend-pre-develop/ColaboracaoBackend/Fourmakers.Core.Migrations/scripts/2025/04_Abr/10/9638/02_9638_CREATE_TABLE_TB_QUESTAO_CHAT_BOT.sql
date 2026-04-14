CREATE TABLE `tb_questao_chat_bot` (
  `id` int NOT NULL AUTO_INCREMENT,
  `chat_id` int NOT NULL,
  `questao` text NOT NULL,
  `resposta` text NOT NULL,
  `data_criacao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `chat_id` (`chat_id`),
  CONSTRAINT `tb_questao_chat_bot_ibfk_1` FOREIGN KEY (`chat_id`) REFERENCES `tb_chat_bot` (`id`)
);