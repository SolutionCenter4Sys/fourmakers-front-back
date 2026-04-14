CREATE TABLE `tb_projeto_org_atividade` (
  `id` CHAR(36) PRIMARY KEY NOT NULL,
  `tb_projeto_org_cod_projeto` varchar(255),
  `tb_projeto_tb_org_id` integer,
  `tb_atividade_id` CHAR(36),
  FOREIGN KEY (`tb_projeto_org_cod_projeto`, `tb_projeto_tb_org_id`) REFERENCES `tb_projeto_org` (`cod_projeto`, `tb_org_id`),
  FOREIGN KEY (`tb_atividade_id`) REFERENCES `tb_atividade` (`id`)
);