CREATE TABLE `tb_pais` (
  `id` int NOT NULL AUTO_INCREMENT,
  `descricao` varchar(200) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `Descricao_UNIQUE` (`descricao`)
);
