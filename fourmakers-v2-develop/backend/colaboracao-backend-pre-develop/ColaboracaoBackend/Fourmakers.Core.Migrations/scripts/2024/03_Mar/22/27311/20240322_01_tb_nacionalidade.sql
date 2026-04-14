CREATE TABLE `tb_nacionalidade` (
  `id` int NOT NULL AUTO_INCREMENT,
  `descricao` varchar(200) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `Descricao_UNIQUE` (`descricao`)
);
