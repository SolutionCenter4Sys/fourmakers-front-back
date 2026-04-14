INSERT INTO `tb_grupo_acesso` (`descricao`, `ativo`, `nivel`, `tb_org_id`) VALUES ('GESTORES', '1', '0', '7');
INSERT INTO `tb_grupo_acesso` (`descricao`, `ativo`, `nivel`, `tb_org_id`) VALUES ('RESIDENTES', '1', '0', '7');

INSERT INTO  tb_grupo_acesso_funcionalidade_sistema(tb_grupo_acesso_id, tb_funcionalidade_sistema_id, ativo) VALUES ((select id from tb_grupo_acesso where descricao = 'GESTORES'),9,1);
INSERT INTO  tb_grupo_acesso_funcionalidade_sistema(tb_grupo_acesso_id, tb_funcionalidade_sistema_id, ativo) VALUES ((select id from tb_grupo_acesso where descricao = 'GESTORES'),15,1);