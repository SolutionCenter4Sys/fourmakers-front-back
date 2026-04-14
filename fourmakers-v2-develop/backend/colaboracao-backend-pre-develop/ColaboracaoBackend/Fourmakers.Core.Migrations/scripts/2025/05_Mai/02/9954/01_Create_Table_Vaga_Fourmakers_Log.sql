DROP TABLE tb_vaga_fourmakers_log;

DROP TABLE tb_vaga;

CREATE TABLE `tb_vaga` (
  `id` varchar(36) NOT NULL,
  `codigo` bigint NOT NULL,
  `titulo` varchar(255) NOT NULL,
  `numero_de_vagas` int DEFAULT NULL,
  `taxa_maxima_hora` varchar(255) DEFAULT NULL,
  `descricao` text,
  `funcao` text,
  `data_criacao` datetime DEFAULT NULL,
  `localizacao` varchar(50) DEFAULT NULL,
  `observacao_localizacao` text,
  `estado` varchar(50) DEFAULT NULL,
  `cidade` varchar(100) DEFAULT NULL,
  `tb_usuario_criador_cpf` varchar(36) NOT NULL,
  `tb_usuario_aprovador_cpf` varchar(36) NOT NULL,
  `tb_gestor_cod` varchar(36) NOT NULL,
  `tb_status_vaga_cod` varchar(36) NOT NULL,
  `tb_origem_vaga_cod` varchar(36) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

CREATE TABLE `tb_vaga_fourmakers_log` (
  `id` varchar(36) NOT NULL,
  `tb_vaga_id` varchar(36) NOT NULL,
  `tb_status_vaga_cod` int NOT NULL,
  `tb_usuario_cpf` varchar(36) NOT NULL,
  `data_alteracao` datetime DEFAULT CURRENT_TIMESTAMP,
  `objeto` text,
  PRIMARY KEY (`id`),
  KEY `fk_id_vaga` (`tb_vaga_id`),
  KEY `fk_cpf_usuario` (`tb_usuario_cpf`),
  CONSTRAINT `fk_cpf_usuario` FOREIGN KEY (`tb_usuario_cpf`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`),
  CONSTRAINT `fk_id_vaga` FOREIGN KEY (`tb_vaga_id`) REFERENCES `tb_vaga` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;


