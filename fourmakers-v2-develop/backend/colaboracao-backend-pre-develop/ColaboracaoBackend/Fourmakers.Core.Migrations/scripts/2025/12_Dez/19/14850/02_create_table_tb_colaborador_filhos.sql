CREATE TABLE `tb_colaborador_filhos` (
  `id` varchar(36) NOT NULL COMMENT 'GUID do filho',
  `codigo_interno_colaborador` varchar(36) NOT NULL COMMENT 'Chave estrangeira para tb_colaborador',
  `data_nascimento` date NOT NULL COMMENT 'Data de Nascimento do Filho',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_tb_colaborador_filhos_tb_colaborador_idx` (`codigo_interno_colaborador`),
  CONSTRAINT `fk_tb_colaborador_filhos_tb_colaborador` FOREIGN KEY (`codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Filhos do colaborador';
