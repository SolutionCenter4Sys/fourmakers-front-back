CREATE TABLE `tb_skill_desconhecida` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `descricao` varchar(100) NOT NULL,
  `ativo` tinyint NOT NULL DEFAULT '1',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `usuario_criacao_id` bigint DEFAULT NULL,
  `confirmada` tinyint NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
);