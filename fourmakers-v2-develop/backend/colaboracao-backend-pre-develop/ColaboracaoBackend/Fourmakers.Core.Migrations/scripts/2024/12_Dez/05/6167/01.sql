

CREATE TABLE `tb_perfil` (
  `id` char(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `nome_perfil` varchar(255) NOT NULL,
  `codigo_projeto` varchar(255) NULL,
  `data_criacao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `codigo_interno_colaborador_criacao` varchar(36) DEFAULT NULL,
  `codigo_interno_colaborador_alteracao` varchar(36) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `tb_org_id` (`tb_org_id`),
  CONSTRAINT `fk_tb_perfil_codigo_projeto` FOREIGN KEY (`codigo_projeto`) REFERENCES `tb_projeto_org` (`cod_projeto`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_perfil_codigo_interno_colaborador_alteracao` FOREIGN KEY (`codigo_interno_colaborador_alteracao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`) ,
  CONSTRAINT `fk_tb_perfil_codigo_interno_colaborador_criacao` FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;


CREATE TABLE `tb_perfil_alocacao` (
  `id` varchar(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `tb_colaborador_periodo_alocacao_id` bigint NOT NULL,
  `tb_gestor_externo_perfil_id` varchar(36) NULL,
  `tb_perfil_id` varchar(36) NULL,
  PRIMARY KEY (`id`),
  KEY `tb_org_id` (`tb_org_id`),
  CONSTRAINT `tb_perfil_alocacao_tb_org_fk` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`)  ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `tb_perfil_alocacao_tb_colaborador_periodo_alocacao_fk` FOREIGN KEY (`tb_colaborador_periodo_alocacao_id`) REFERENCES `tb_colaborador_periodo_alocacao` (`id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `tb_perfil_alocacao_tb_gestor_externo_perfil_fk` FOREIGN KEY (`tb_gestor_externo_perfil_id`) REFERENCES `tb_gestor_externo_perfil` (`id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `tb_perfil_alocacao_tb_perfi_fk` FOREIGN KEY (`tb_perfil_id`) REFERENCES `tb_perfil` (`id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;


CREATE TABLE `tb_perfil_skill` (
  `tb_perfil_id` char(36) NOT NULL,
  `tb_item_perfil_id` bigint NOT NULL,
  `skill_id` bigint NOT NULL,
  `tb_nivel_id` bigint NULL,
  `data_criacao` datetime DEFAULT CURRENT_TIMESTAMP,
  `codigo_interno_colaborador_criacao` char(36) DEFAULT NULL,
  PRIMARY KEY (`tb_perfil_id`,`tb_item_perfil_id`,`skill_id`),
  KEY `tb_item_perfil_id` (`tb_item_perfil_id`),
  KEY `tb_nivel_id` (`tb_nivel_id`),
  KEY `codigo_interno_colaborador_criacao` (`codigo_interno_colaborador_criacao`),
  CONSTRAINT `tb_perfil_skill_ibfk_1` FOREIGN KEY (`tb_perfil_id`) REFERENCES `tb_perfil` (`id`)  ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `tb_perfil_skill_ibfk_2` FOREIGN KEY (`tb_item_perfil_id`) REFERENCES `tb_item_perfil` (`id`),
  CONSTRAINT `tb_perfil_skill_ibfk_3` FOREIGN KEY (`tb_nivel_id`) REFERENCES `tb_nivel` (`id`),
  CONSTRAINT `tb_perfil_skill_ibfk_4` FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

CREATE TABLE `tb_colaborador_alocado_skill` (
  `tb_colaborador_periodo_alocacao_id` bigint NOT NULL,
  `tb_item_perfil_id` bigint NOT NULL,
  `skill_id` bigint NOT NULL,
  `tb_nivel_id` bigint NULL,
  `data_criacao` datetime DEFAULT CURRENT_TIMESTAMP,
  `codigo_interno_colaborador_criacao` char(36) DEFAULT NULL,
  PRIMARY KEY (`tb_colaborador_periodo_alocacao_id`,`tb_item_perfil_id`,`skill_id`),
  KEY `tb_item_perfil_id` (`tb_item_perfil_id`),
  KEY `tb_nivel_id` (`tb_nivel_id`),
  KEY `codigo_interno_colaborador_criacao` (`codigo_interno_colaborador_criacao`),
  CONSTRAINT `tb_colaborador_alocado_skill_ibfk_1` FOREIGN KEY (`tb_colaborador_periodo_alocacao_id`) REFERENCES `tb_colaborador_periodo_alocacao` (`id`)  ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `tb_colaborador_alocado_skill_ibfk_2` FOREIGN KEY (`tb_item_perfil_id`) REFERENCES `tb_item_perfil` (`id`),
  CONSTRAINT `tb_colaborador_alocado_skill_ibfk_3` FOREIGN KEY (`tb_nivel_id`) REFERENCES `tb_nivel` (`id`),
  CONSTRAINT `tb_colaborador_alocado_skill_ibfk_4` FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

ALTER TABLE tb_colaborador_periodo_alocacao
ADD retroalimenta_cv tinyint default 0;

create view vw_skills as
select tc.descricao, tc.id, (select tip.id from tb_item_perfil tip where tip.descricao = 'COMPETENCIA') as tipo_id  from tb_competencia tc
union
select ts.descricao, ts.id, (select tip.id from tb_item_perfil tip where tip.descricao = 'SOFTSKILL') as tipo_id  from tb_softskill ts
union
select tm.descricao, tm.id, (select tip.id from tb_item_perfil tip where tip.descricao = 'METODOLOGIA') as tipo_id  from tb_metodologia tm
union
select td.descricao, td.id, (select tip.id from tb_item_perfil tip where tip.descricao = 'DOMINIONEGOCIO') as tipo_id  from tb_dominionegocio td
union
select ti.descricao, ti.id, (select tip.id from tb_item_perfil tip where tip.descricao = 'IDIOMA') as tipo_id  from tb_idioma ti;


create view vw_perfis as
select tp.nome_perfil as perfil, tp.tb_org_id, CONCAT('1|', tp.id) as id, tpo.cod_cliente as codigo_cliente  from tb_perfil tp inner join tb_projeto_org tpo on tpo.tb_org_id = tp.tb_org_id and tpo.cod_projeto = tp.codigo_projeto
union
select CONCAT(tgep.nome_perfil, ' / ', tge.nome) as perfil, tgep.tb_org_id, CONCAT('2|', tgep.id), tge.codigo_cliente as id from tb_gestor_externo_perfil tgep inner join tb_gestor_externo tge on tge.tb_org_id = tgep.tb_org_id and tge.cod_gestor_externo = tgep.cod_gestor_externo;


            