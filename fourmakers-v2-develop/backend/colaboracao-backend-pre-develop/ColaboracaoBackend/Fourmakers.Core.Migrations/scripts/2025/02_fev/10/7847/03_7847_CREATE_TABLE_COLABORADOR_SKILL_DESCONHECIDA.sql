DROP TABLE `tb_colaborador_skill_desconhecida`;
CREATE TABLE `tb_colaborador_skill_desconhecida` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `codigo_interno_colaborador` varchar(36) DEFAULT NULL,
  `skill_desconhecida_id` bigint NOT NULL,
  `ativo` tinyint NOT NULL DEFAULT '1',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `tb_nivel_id` bigint DEFAULT NULL,
  PRIMARY KEY (`id`)
);
