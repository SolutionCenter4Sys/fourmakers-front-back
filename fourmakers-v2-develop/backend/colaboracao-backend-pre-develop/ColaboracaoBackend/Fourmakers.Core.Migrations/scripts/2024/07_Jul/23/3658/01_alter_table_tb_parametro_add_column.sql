ALTER TABLE tb_parametro ADD COLUMN tipo_parametro varchar(100)

update tb_parametro set tipo_parametro = 'BACKEND';

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Mostrar coluna aprovadores no timesheet', 'Na aba aprovação existe uma coluna de aprovadores que apresentar os gerentes que aprovaram, essa flag vai definir se deve ser apresentada ou não.', 'MOSTRAR_COLUNA_APROVADORES_TIMESHEET', 'APONTAMENTOS', '1', 'FRONTEND');

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Valor da label Colaboradores no Timesheet', 'Onde estiver uma ocorrência da palavra \'Colaboradores\' será apresentado o conteúdo da label.', 'LABEL_COLABORADORES_TIMESHEET', 'APONTAMENTOS', '1', 'FRONTEND');

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Valor da label Colaboradores no Timesheet', 'Onde estiver uma ocorrência da palavra \'Colaborador\' será apresentado o conteúdo da label.', 'LABEL_COLABORADOR_TIMESHEET', 'APONTAMENTOS', '1', 'FRONTEND');

-- INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
-- VALUES (UUID(),'URL Default Login da ORG', 'URL padrão para redirecionar login na ORG.', 'URL_LOGIN_DEFAULT_ORG', 'FOURMAKERS', '1', 'FRONTEND');


INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Mostrar Aba Perfil', 'Indica se deve mostrar a aba Perfil dentro da aba Dados Pessoais.', 'MOSTRA_ABA_PERFIL', 'DADOS_CADASTRAIS', '1', 'FRONTEND');

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Mostrar Aba Passaporte/Vistos', 'Indica se deve mostrar a aba Passaporte/Vistos dentro da aba Dados Pessoais.', 'MOSTRA_PASSAPORTE_VISTOS', 'DADOS_CADASTRAIS', '1', 'FRONTEND');

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Mostrar Aba Contato', 'Indica se deve mostrar a aba Contato dentro da aba Dados Pessoais.', 'MOSTRA_ABA_CONTATO', 'DADOS_CADASTRAIS', '1', 'FRONTEND');

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Mostrar Aba Endereço', 'Indica se deve mostrar a aba Endereço dentro da aba Dados Pessoais.', 'MOSTRA_ABA_ENDERECO', 'DADOS_CADASTRAIS', '1', 'FRONTEND');

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Mostrar Aba Saúde', 'Indica se deve mostrar a aba Saúde dentro da aba Dados Pessoais.', 'MOSTRA_ABA_SAUDE', 'DADOS_CADASTRAIS', '1', 'FRONTEND');

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Mostrar Aba Dependentes', 'Indica se deve mostrar a aba Dependentes dentro da aba Dados Pessoais.', 'MOSTRA_ABA_DEPENDENTES', 'DADOS_CADASTRAIS', '1', 'FRONTEND');

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Mostrar Aba Compartilhamento', 'Indica se deve mostrar a aba Compartilhamento dentro da aba Dados Pessoais.', 'MOSTRA_ABA_COMPARTILHAMENTO', 'DADOS_CADASTRAIS', '1', 'FRONTEND');

INSERT INTO `tb_parametro` (`id`,`nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`) 
VALUES (UUID(),'Mostrar Aba Alterar Senha', 'Indica se deve mostrar a aba Alterar Senha dentro da aba Dados Pessoais.', 'MOSTRA_ABA_ALTERAR_SENHA', 'DADOS_CADASTRAIS', '1', 'FRONTEND');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'MOSTRAR_COLUNA_APROVADORES_TIMESHEET', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`, `tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (uuid(), '2', 'MOSTRAR_COLUNA_APROVADORES_TIMESHEET', 'true', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'LABEL_COLABORADORES_TIMESHEET', 'Cooperados', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'LABEL_COLABORADOR_TIMESHEET', 'Cooperado(a)', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'MOSTRA_ABA_PERFIL', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'MOSTRA_PASSAPORTE_VISTOS', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'MOSTRA_ABA_CONTATO', 'true', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'MOSTRA_ABA_ENDERECO', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'MOSTRA_ABA_SAUDE', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'MOSTRA_ABA_DEPENDENTES', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'MOSTRA_ABA_COMPARTILHAMENTO', 'true', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'6', 'MOSTRA_ABA_ALTERAR_SENHA', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'4', 'MOSTRA_ABA_ENDERECO', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'4', 'MOSTRA_ABA_SAUDE', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`,`tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(),'4', 'MOSTRA_ABA_DEPENDENTES', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`, `tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (uuid(), '6', 'MOSTRAR_COLUNA_APROVADORES_TIMESHEET', 'false', '3');

INSERT INTO `tb_parametro_configuracao` (`id`, `tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (uuid(), '4', 'MOSTRAR_COLUNA_APROVADORES_TIMESHEET', 'true', '3');

INSERT INTO `tb_parametro_configuracao` (`id`, `tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (uuid(), '2', 'MOSTRAR_COLUNA_APROVADORES_TIMESHEET', 'true', '3');
 

ALTER TABLE `tb_parametro`
ADD COLUMN `tb_usuario_id_criacao` BIGINT,
ADD COLUMN `tb_usuario_id_alteracao` BIGINT,
ADD CONSTRAINT `fk_tb_parametro_id_criacao`
  FOREIGN KEY (`tb_usuario_id_criacao`) REFERENCES `tb_usuario`(`id`),
ADD CONSTRAINT `fk_tb_parametro_id_alteracao`
  FOREIGN KEY (`tb_usuario_id_alteracao`) REFERENCES `tb_usuario`(`id`);

ALTER TABLE `tb_parametro_configuracao`
ADD COLUMN `tb_usuario_id_criacao` BIGINT,
ADD COLUMN `tb_usuario_id_alteracao` BIGINT,
ADD CONSTRAINT `fk_tb_parametro_configuracao_id_criacao`
  FOREIGN KEY (`tb_usuario_id_criacao`) REFERENCES `tb_usuario`(`id`),
ADD CONSTRAINT `fk_tb_parametro_configuracao_id_alteracao`
  FOREIGN KEY (`tb_usuario_id_alteracao`) REFERENCES `tb_usuario`(`id`);
  
ALTER TABLE `tb_parametro` 
ADD UNIQUE INDEX `codigo_parametro_UNIQUE` (`codigo_parametro` ASC) VISIBLE;
;

INSERT INTO `tb_grupo_acesso` (`descricao`, `ativo`, `nivel`, `tb_org_id`) VALUES ('Gestor Parâmetros', '1', '0', '2');
INSERT INTO `tb_grupo_acesso` (`descricao`, `ativo`, `nivel`, `tb_org_id`) VALUES ('Gestor Parâmetros', '1', '0', '4');
INSERT INTO `tb_grupo_acesso` (`descricao`, `ativo`, `nivel`, `tb_org_id`) VALUES ('Gestor Parâmetros', '1', '0', '6');

INSERT INTO `tb_funcionalidade_sistema` (`id`, `descricao`, `ativo`) VALUES ('25', 'CADASTRO_PARAMETRO', '1');
INSERT INTO `tb_funcionalidade_sistema` (`id`, `descricao`, `ativo`) VALUES ('26', 'CADASTRO_PARAMETRO_CONFIGURACAO', '1');
INSERT INTO `tb_funcionalidade_sistema` (`id`, `descricao`, `ativo`) VALUES ('27', 'CADASTRO_PARAMETRO_CONFIGURACAO_OUTRAS_ORG', '1');

INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((SELECT id FROM tb_grupo_acesso where descricao = 'Gestor Parâmetros' AND tb_org_id = 2), '25', '1');
INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((SELECT id FROM tb_grupo_acesso where descricao = 'Gestor Parâmetros' AND tb_org_id = 2), '26', '1');
INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((SELECT id FROM tb_grupo_acesso where descricao = 'Gestor Parâmetros' AND tb_org_id = 2), '27', '1');

INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((SELECT id FROM tb_grupo_acesso where descricao = 'Gestor Parâmetros' AND tb_org_id = 4), '25', '1');
INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((SELECT id FROM tb_grupo_acesso where descricao = 'Gestor Parâmetros' AND tb_org_id = 4), '26', '1');
INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((SELECT id FROM tb_grupo_acesso where descricao = 'Gestor Parâmetros' AND tb_org_id = 4), '27', '1');

INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((SELECT id FROM tb_grupo_acesso where descricao = 'Gestor Parâmetros' AND tb_org_id = 6), '25', '1');
INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((SELECT id FROM tb_grupo_acesso where descricao = 'Gestor Parâmetros' AND tb_org_id = 6), '26', '1');
INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((SELECT id FROM tb_grupo_acesso where descricao = 'Gestor Parâmetros' AND tb_org_id = 6), '27', '1');
