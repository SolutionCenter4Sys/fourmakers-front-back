CREATE TABLE `tb_grupo_acesso_log` (
  `id` char(36) NOT NULL,
  `tb_grupo_acesso_id` int NOT NULL,
  `acao` varchar(20) NOT NULL COMMENT 'Ação realizada: CREATE, UPDATE, DELETE',
  `tb_colaborador_codigo_interno_colaborador_alterador` varchar(36) NOT NULL COMMENT 'Código interno do colaborador que realizou a alteração',
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Data e hora da alteração',
  `objeto` longtext NOT NULL COMMENT 'JSON com o estado completo do objeto antes da alteração',
  `alteracoes` longtext NOT NULL COMMENT 'JSON com as alterações realizadas',
  PRIMARY KEY (`id`),
  KEY `fk_ga_log_ga_id_idx` (`tb_grupo_acesso_id`),
  KEY `fk_ga_log_colab_alterador_idx` (`tb_colaborador_codigo_interno_colaborador_alterador`),
  CONSTRAINT `fk_ga_log_ga_id` FOREIGN KEY (`tb_grupo_acesso_id`) REFERENCES `tb_grupo_acesso` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_ga_log_colab_alterador` FOREIGN KEY (`tb_colaborador_codigo_interno_colaborador_alterador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Log de alterações na tabela tb_grupo_acesso';

