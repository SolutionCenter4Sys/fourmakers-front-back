CREATE TABLE `tb_colaborador_lg` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `cpf` varchar(11) NOT NULL,
  `org_id` bigint DEFAULT NULL,
  `lg_pessoa_id` bigint DEFAULT NULL,
  `tb_holerite_id` bigint NOT NULL,
  `lg_matricula` varchar(75) NOT NULL,
  `codigo_empresa` int NOT NULL,
  PRIMARY KEY (`id`)
);