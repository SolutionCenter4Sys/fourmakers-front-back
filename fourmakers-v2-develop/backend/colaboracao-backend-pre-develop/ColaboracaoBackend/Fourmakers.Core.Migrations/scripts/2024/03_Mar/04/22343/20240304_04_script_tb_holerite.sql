CREATE TABLE `tb_holerite` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `competencia_mes` varchar(2) NOT NULL,
  `competencia_ano` varchar(4) NOT NULL,
  `status` int NOT NULL,
  `lg_id_tarefa` varchar(50) DEFAULT NULL,
  `lg_blob_url_arquivo` varchar(500) DEFAULT NULL,
  `s3_url_arquivo` varchar(500) DEFAULT NULL,
  `data_emissao` timestamp NULL DEFAULT NULL,
  `data_criacao` timestamp NOT NULL,
  `data_alteracao` timestamp NOT NULL,
  `tb_colaborador_lg_id` bigint NOT NULL,
  `processo` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `tb_colaborador_lg_id` (`tb_colaborador_lg_id`),
  CONSTRAINT `tb_holerite_ibfk_1` FOREIGN KEY (`tb_colaborador_lg_id`) REFERENCES `tb_colaborador_lg` (`id`)
);
