-- gcolb_prd.tb_formacao definition




CREATE TABLE `tb_origem_historico_cv` (
  `id` varchar(36) NOT NULL,
  `descricao` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=815 DEFAULT CHARSET=utf8mb3;

insert into tb_origem_historico_cv (id, descricao) values 
(uuid(), 'MAPA_ALOCACAO'),
(uuid(), 'MANUAL'),
(uuid(), 'LINKEDIN');

CREATE TABLE `tb_tipo_historico_cv` (
  `id` varchar(36) NOT NULL,
  `descricao` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=815 DEFAULT CHARSET=utf8mb3;

insert into tb_tipo_historico_cv (id, descricao) values 
(uuid(), 'INSERT'),
(uuid(), 'UPDATE'),
(uuid(), 'DELETE');

insert into tb_item_perfil (id, descricao, ativo) values 
(10, 'SOBRE', 1),
(11, 'EXPERIENCIA', 1),
(12, 'CERTIFICACAO', 1),
(13, 'ESCOLARIDADE', 1);


CREATE TABLE `tb_historico_cv` (
  `id` varchar(36) NOT NULL,
  `codigo_interno_colaborador` varchar(36) NOT NULL,
  `tb_item_perfil_id` bigint NOT NULL,
  `tb_origem_historico_cv_id` varchar(36) NOT NULL,
  `tb_tipo_historico_cv_id` varchar(36) NOT NULL,
  `tb_skill_id` bigint NULL,
  `tb_nivel_id` bigint NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_tb_historico_cv_tb_nivel_id` FOREIGN KEY (`tb_nivel_id`) REFERENCES `tb_nivel` (`id`) ON DELETE CASCADE,
  KEY `fk_tb_historico_cv_codigo_interno_colaborador_idx` (`codigo_interno_colaborador`),
  CONSTRAINT `fk_tb_historico_cv_codigo_interno_colaborador` FOREIGN KEY (`codigo_interno_colaborador`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`) ON DELETE CASCADE,
  KEY `fk_tb_historico_cv_tb_item_perfil_id_idx` (`tb_item_perfil_id`),
  CONSTRAINT `fk_tb_historico_cv_tb_item_perfil_id` FOREIGN KEY (`tb_item_perfil_id`) REFERENCES `tb_item_perfil` (`id`) ON DELETE CASCADE,
  KEY `fk_tb_historico_cv_tb_origem_historico_cv_id_idx` (`tb_origem_historico_cv_id`),
  CONSTRAINT `fk_tb_historico_cv_tb_origem_historico_cv_id` FOREIGN KEY (`tb_origem_historico_cv_id`) REFERENCES `tb_origem_historico_cv` (`id`) ON DELETE CASCADE,
  KEY `fk_tb_historico_cv_tb_tipo_historico_cv_id_idx` (`tb_tipo_historico_cv_id`),
  CONSTRAINT `fk_tb_historico_cv_tb_tipo_historico_cv_id` FOREIGN KEY (`tb_tipo_historico_cv_id`) REFERENCES `tb_tipo_historico_cv` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=815 DEFAULT CHARSET=utf8mb3;