CREATE TABLE `tb_recurso` (
  `codigo_recurso` varchar(50) NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  `codigo_interno_colaborador_criacao` char(36) NOT NULL,
  `codigo_interno_colaborador_alteracao` char(36) DEFAULT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`codigo_recurso`),
  UNIQUE KEY `codigo_recurso` (`codigo_recurso`)
);