DROP TABLE IF EXISTS tb_candidato_vaga;

CREATE TABLE `tb_candidato_vaga` (
  `id` varchar(36) NOT NULL,
  `tb_usuario_cpf` varchar(36) NOT NULL,
  `tb_vaga_id` varchar(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `tb_candidato_id` int NOT NULL,
  `tb_status_id` int NOT NULL DEFAULT '1',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3

