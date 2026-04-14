CREATE TABLE `tb_candidatos_vaga` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `id_vaga` bigint DEFAULT NULL,
  `nome` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `status` varchar(50) DEFAULT NULL,
  `candidato_id` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`)
);
