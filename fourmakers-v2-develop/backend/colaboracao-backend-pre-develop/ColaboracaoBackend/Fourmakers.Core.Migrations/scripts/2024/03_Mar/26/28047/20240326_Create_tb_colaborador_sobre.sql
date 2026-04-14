CREATE TABLE `tb_colaborador_sobre` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `descricao` text,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `cpf` varchar(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_colaborador_cpf1_idx` (`cpf`),
  CONSTRAINT `fk_colaborador_cpf1` FOREIGN KEY (`cpf`) REFERENCES `tb_colaborador` (`cpf`)
)