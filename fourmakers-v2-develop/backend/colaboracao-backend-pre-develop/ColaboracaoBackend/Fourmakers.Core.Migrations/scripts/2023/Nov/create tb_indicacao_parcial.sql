CREATE TABLE `tb_indicacao_premiada_parcial` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `id_usuario_indicou` bigint NOT NULL,
  `id_vaga` bigint NOT NULL,
  `link_indicacao` varchar(350) NOT NULL,
  `nome_indicado` varchar(120) NOT NULL,
  `email_indicado` varchar(100) DEFAULT NULL,
  `telefone_indicado` varchar(11) NOT NULL,
  `linkedin` varchar(350) DEFAULT NULL,
  `relacao_indicado` varchar(350) NOT NULL,
  `disponivel` tinyint NOT NULL DEFAULT '0',
  `autorizou` tinyint NOT NULL DEFAULT '0',
  `utilizado` tinyint NOT NULL DEFAULT '0',
  `data_criacao` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3