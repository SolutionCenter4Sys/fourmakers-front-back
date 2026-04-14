CREATE TABLE `tb_colaborador_dados_demograficos_outros_custos` (
  `id` varchar(36) NOT NULL COMMENT 'GUID do outro custo',
  `codigo_interno_colaborador` varchar(36) NOT NULL COMMENT 'Chave estrangeira para tb_colaborador',
  `tb_colaborador_dados_demograficos_id` varchar(36) NOT NULL COMMENT 'Chave estrangeira para tb_colaborador_dados_demograficos',
  `descricao` varchar(255) NOT NULL COMMENT 'Descrição do custo',
  `valor` decimal(10,2) NOT NULL COMMENT 'Valor do custo (R$)',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_outros_custos_tb_colaborador_idx` (`codigo_interno_colaborador`),
  KEY `fk_outros_custos_dados_demograficos_idx` (`tb_colaborador_dados_demograficos_id`),
  CONSTRAINT `fk_outros_custos_tb_colaborador` FOREIGN KEY (`codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_outros_custos_dados_demograficos` FOREIGN KEY (`tb_colaborador_dados_demograficos_id`) REFERENCES `tb_colaborador_dados_demograficos` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Outros custos variáveis dos dados demográficos do colaborador';

