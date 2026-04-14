INSERT INTO
	tb_parametro_configuracao (id, tb_org_id, codigo_parametro, valor_parametro, tb_parametro_nivel_id)
VALUES
    (uuid(), 7, 'URL_HOME_DEFAULT', 'vagas/portal', 3),
    (uuid(), 7, 'MOSTRA_VAGAS', 'true', 3),
    (uuid(), 7, 'MOSTRA_ABA_SAUDE', 'false', 3),
    (uuid(), 7, 'MOSTRA_ABA_DEPENDENTES', 'false', 3),
    (uuid(), 7, 'LABEL_COLABORADORES_TIMESHEET', 'Residentes', 3),
    (uuid(), 7, 'LABEL_COLABORADOR_TIMESHEET', 'Residente', 3);

INSERT INTO
	tb_parametro_configuracao (id, tb_org_id, tb_grupo_acesso_id, codigo_parametro, valor_parametro, tb_parametro_nivel_id)
VALUES
    (uuid(), 7, (select id from tb_grupo_acesso where descricao = 'GESTORES'), 'MOSTRA_MAPA_DEMOGRAFICO', 'true', 2),
    (uuid(), 7, (select id from tb_grupo_acesso where descricao = 'RESIDENTES'), 'PODE_CANDIDATAR', 'true', 2);
	
	
/*INSERT INTO `tb_usuario_grupo_acesso` (`tb_usuario_id`, `tb_grupo_acesso_id`, `ativo`) VALUES ('<informaridususario>', (select id from tb_grupo_acesso where descricao = 'GESTORES'), '1');
INSERT INTO `tb_usuario_grupo_acesso` (`tb_usuario_id`, `tb_grupo_acesso_id`, `ativo`) VALUES ('<informaridususario>', (select id from tb_grupo_acesso where descricao = 'RESIDENTES'),'1');*/
