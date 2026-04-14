ALTER TABLE `tb_gestor_externo`
ADD COLUMN `data_criacao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN `data_alteracao` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
ADD COLUMN `tipo_cadastro` varchar(50) DEFAULT NULL,
ADD COLUMN `codigo_interno_colaborador_criacao` varchar(36) DEFAULT NULL,
ADD COLUMN `codigo_interno_colaborador_alteracao` varchar(36) DEFAULT NULL,
ADD KEY `codigo_interno_colaborador_criacao` (`codigo_interno_colaborador_criacao`),
ADD KEY `codigo_interno_colaborador_alteracao` (`codigo_interno_colaborador_alteracao`),
ADD CONSTRAINT FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`),
ADD CONSTRAINT FOREIGN KEY (`codigo_interno_colaborador_alteracao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`);


ALTER TABLE tb_gestor_externo ADD COLUMN`ativo` tinyint(1) DEFAULT NULL;