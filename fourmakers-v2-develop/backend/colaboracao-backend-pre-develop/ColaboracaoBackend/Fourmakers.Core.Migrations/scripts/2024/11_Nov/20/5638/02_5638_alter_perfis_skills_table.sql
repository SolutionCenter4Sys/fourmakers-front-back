alter table tb_gestor_externo_perfil_skill
	add column `data_criacao` datetime DEFAULT CURRENT_TIMESTAMP,
	add column `codigo_interno_colaborador_criacao` char(36) DEFAULT NULL,
    ADD FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_usuario` (`codigo_interno_colaborador`);