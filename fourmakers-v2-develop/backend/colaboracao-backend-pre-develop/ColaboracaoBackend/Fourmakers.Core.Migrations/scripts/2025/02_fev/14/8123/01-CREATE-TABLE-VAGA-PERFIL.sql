-- tb_gestor_externo_perfil definição

CREATE TABLE `tb_vaga_gestor_externo_perfil` (
  `id` char(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT 1,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `codigo_interno_colaborador_criacao` varchar(36) NOT NULL,
  `codigo_vaga` int NOT NULL,
  `tb_gestor_externo_perfil_id` char(36) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `tb_org_id` (`tb_org_id`),
  CONSTRAINT `tb_vaga_gestor_externo_perfil_ibfk_0` FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`)  ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `tb_vaga_gestor_externo_perfil_ibfk_1` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `tb_vaga_gestor_externo_perfil_ibfk_2` FOREIGN KEY (`tb_gestor_externo_perfil_id`) REFERENCES `tb_gestor_externo_perfil` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;


CREATE TABLE `tb_log_vaga` (
  `id` char(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `codigo_interno_colaborador_criacao` varchar(36) NOT NULL,
  `codigo_vaga` int NOT NULL,
  `objeto_vaga` TEXT NULL,
  `acao` int not null,
  PRIMARY KEY (`id`),
  KEY `tb_org_id` (`tb_org_id`),
  CONSTRAINT `tb_log_vaga_ibfk_0` FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`)  ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `tb_log_vaga_ibfk_1` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;


