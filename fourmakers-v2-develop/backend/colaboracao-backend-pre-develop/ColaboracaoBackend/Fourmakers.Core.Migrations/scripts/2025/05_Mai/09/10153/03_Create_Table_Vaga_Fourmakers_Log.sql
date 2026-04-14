DROP TABLE IF EXISTS tb_vaga_fourmakers_log;
DROP TABLE IF EXISTS tb_vaga_skill;
DROP TABLE IF EXISTS tb_vaga;

CREATE TABLE `tb_vaga` (
  `id` varchar(36) NOT NULL,
  `codigo` bigint NOT NULL,
  `titulo` varchar(255) NOT NULL,
  `numero_de_vagas` int DEFAULT NULL,
  `custo_profissional` DECIMAL(18,2) DEFAULT NULL,
  `rate_card` DECIMAL(18,2) DEFAULT NULL,
  `descricao` text,
  `cargo` text,
  `data_criacao` datetime DEFAULT NULL,
  `localizacao` varchar(50) DEFAULT NULL,
  `estado` varchar(50) DEFAULT NULL,
  `cidade` varchar(100) DEFAULT NULL,
  `frequencia` varchar(36) NULL,
  `modelo_trabalho_cod` int NOT NULL,
  `tb_usuario_criador_cpf` varchar(36) NOT NULL,
  `tb_usuario_aprovador_cpf` varchar(36) NOT NULL,
  `tb_gestor_cod` varchar(36) NOT NULL,
  `tb_status_vaga_cod` varchar(36) NOT NULL,
  `tb_origem_vaga_cod` varchar(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `tb_gestor_externo_perfil_id` varchar(36) NULL,
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

CREATE TABLE `tb_vaga_skill` (
  `id` varchar(36) NOT NULL	,
  `skill_id` int NOT NULL,
  `skill_nivel_id` int NOT NULL,
  `tb_vaga_id` varchar(36) NOT NULL,
  `ativo` tinyint NOT NULL DEFAULT '1',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `tb_item_perfil_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_tb_vaga_idx` (`tb_vaga_id`),
  CONSTRAINT `fk_tb_vaga` FOREIGN KEY (`tb_vaga_id`) REFERENCES `tb_vaga` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3
