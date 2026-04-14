ALTER TABLE `tb_gestor_externo` 
ADD INDEX `idx_uq_email_org_id` (`email` ASC, `tb_org_id` ASC);


INSERT INTO `tb_parametro` (`id`, `nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`)
VALUES (UUID(), 'Deve ignorar gereniciamento cliente associado ao projeto ao criar e editar', 'Indica se deve ignorar, no cadastro do cliente, quando está sendo feita uma edição, nos endpoints EditarProjeto e CadastrarProjeto.', 'DEVE_PULAR_GERENCIA_CLIENTES_PROJETO', 'MAPA_DE_ALOCACAO', '1', 'BACKEND');

INSERT INTO `tb_parametro_configuracao` (`id`, `tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`) VALUES (uuid(), '2', 'DEVE_PULAR_GERENCIA_CLIENTES_PROJETO', 'true', '3');