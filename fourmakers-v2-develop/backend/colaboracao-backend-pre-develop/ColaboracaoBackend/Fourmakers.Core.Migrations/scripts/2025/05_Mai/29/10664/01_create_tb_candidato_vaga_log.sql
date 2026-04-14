CREATE TABLE `tb_candidato_vaga_log` (
  `id` varchar(36) NOT NULL,
  `tb_candidato_vaga_id` varchar(36) NOT NULL,
  `tb_candidato_status_id` int NOT NULL,
  `tb_colaborador_codigo_interno_colaborador` varchar(36) NOT NULL,
  `data_alteracao` datetime DEFAULT CURRENT_TIMESTAMP,
  `objeto` text,
  PRIMARY KEY (`id`),
  KEY `fk_tb_candidato_vaga_id` (`tb_candidato_vaga_id`),
  KEY `fk_tb_colaborador_codigo_interno_colaborador` (`tb_colaborador_codigo_interno_colaborador`),
  CONSTRAINT `fk_tb_candidato_vaga_log_candidato_vaga_id` FOREIGN KEY (`tb_candidato_vaga_id`) REFERENCES `tb_candidato_vaga` (`id`),
  CONSTRAINT `fk_tb_candidato_vaga_log_colaborador` FOREIGN KEY (`tb_colaborador_codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;