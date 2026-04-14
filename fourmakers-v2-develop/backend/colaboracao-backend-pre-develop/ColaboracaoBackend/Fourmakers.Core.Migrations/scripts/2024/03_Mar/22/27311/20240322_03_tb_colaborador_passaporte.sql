CREATE TABLE `tb_colaborador_passaporte` (
  `id` int NOT NULL AUTO_INCREMENT,
  `colaborador_cpf` varchar(11) NOT NULL,
  `tb_nacionalidade_id` int NOT NULL,
  `validade` date DEFAULT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_tb_nacionalidade_has_tb_colaborador_passaporte1_idx` (`tb_nacionalidade_id`),
  KEY `fk_tb_colaborador_has_tb_colaborador_passaporte1_idx` (`colaborador_cpf`),
  CONSTRAINT `fk_tb_nacionalidade_has_tb_colaborador_passaporte1` FOREIGN KEY (`tb_nacionalidade_id`) REFERENCES `tb_nacionalidade` (`id`),
  CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_passaporte1` FOREIGN KEY (`colaborador_cpf`) REFERENCES `tb_colaborador` (`cpf`) ON UPDATE CASCADE
);
