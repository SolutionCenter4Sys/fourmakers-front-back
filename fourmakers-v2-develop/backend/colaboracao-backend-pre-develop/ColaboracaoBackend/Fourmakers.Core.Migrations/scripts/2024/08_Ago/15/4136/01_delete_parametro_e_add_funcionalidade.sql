DELETE FROM tb_parametro_configuracao where codigo_parametro = 'HABILITAR_RELATORIO_APONTAMENTO_SIMPLIFICADO';
DELETE FROM tb_parametro where codigo_parametro = 'HABILITAR_RELATORIO_APONTAMENTO_SIMPLIFICADO';

INSERT INTO `tb_funcionalidade_sistema` (`id`, `descricao`, `ativo`) VALUES ('28', 'RELATORIO_DE_APONTAMENTO_SIMPLIFICADO', '1');
INSERT INTO `tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`) VALUES ((select id from tb_grupo_acesso where tb_org_id = 6 and descricao = 'ADM GERAL'), (select id from tb_funcionalidade_sistema where descricao = 'RELATORIO_DE_APONTAMENTO_SIMPLIFICADO'), '1');

-- somente em hml
INSERT INTO `tb_usuario_grupo_acesso` (`tb_usuario_id`, `tb_grupo_acesso_id`, `ativo`) VALUES ((select id from tb_usuario where email = 'usuario_qa@foursys.com.br'),(select id from tb_grupo_acesso where tb_org_id = 6 and descricao = 'ADM GERAL'), '1');



