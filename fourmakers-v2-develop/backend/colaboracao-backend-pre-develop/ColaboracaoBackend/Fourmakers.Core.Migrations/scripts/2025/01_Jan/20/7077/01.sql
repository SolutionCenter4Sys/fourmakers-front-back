CREATE TABLE `tb_colaborador_skill_desconhecida` (
  `id` varchar(36) NOT NULL,
  `codigo_interno_colaborador` varchar(36) DEFAULT NULL,
  `descricao` varchar(255) NOT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_tb_colaborador_skill_desconhecida_tb_colaborador1_idx` (`codigo_interno_colaborador`)
) ENGINE=InnoDB AUTO_INCREMENT=234 DEFAULT CHARSET=utf8mb3;

INSERT INTO tb_item_perfil
(id, descricao, ativo, data_criacao, data_alteracao)
VALUES(14, 'DESCONHECIDO', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
