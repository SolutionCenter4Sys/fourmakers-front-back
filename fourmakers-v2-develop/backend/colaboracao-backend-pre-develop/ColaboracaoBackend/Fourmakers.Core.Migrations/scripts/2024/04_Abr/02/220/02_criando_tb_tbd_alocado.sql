CREATE TABLE `tb_tbd_alocado` (
  `cod_tbd_alocado` int NOT NULL,
  `descricao` varchar(255) DEFAULT NULL,
  `tb_colaborador_cpf_gestor` varchar(11) DEFAULT NULL,
  `cod_diretoria` int DEFAULT NULL,
  `diretoria` varchar(255) DEFAULT NULL,
  `tb_org_id` int DEFAULT NULL,
  `data_criacao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY `tbd_unico` (`cod_tbd_alocado`,`tb_org_id`),
  KEY `tb_org_id` (`tb_org_id`),
  KEY `tb_colaborador_cpf_gestor` (`tb_colaborador_cpf_gestor`),
  KEY `idx_tb_tbd_alocado_cod_tbd_alocado` (`cod_tbd_alocado`),
  CONSTRAINT `tb_tbd_alocado_ibfk_1` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`),
  CONSTRAINT `tb_tbd_alocado_ibfk_2` FOREIGN KEY (`tb_colaborador_cpf_gestor`) REFERENCES `tb_colaborador` (`cpf`)
);
