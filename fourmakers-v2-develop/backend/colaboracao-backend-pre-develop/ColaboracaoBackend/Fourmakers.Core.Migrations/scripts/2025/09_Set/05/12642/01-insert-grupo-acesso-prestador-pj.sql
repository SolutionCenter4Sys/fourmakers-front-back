INSERT INTO tb_grupo_acesso (descricao, ativo, nivel, tb_org_id) VALUES ('PRESTADOR', 1, 0, 9);
INSERT INTO tb_funcionalidade_sistema(id, descricao, ativo) VALUES (46,'PRESTADOR_PJ',1);
INSERT INTO tb_grupo_acesso_funcionalidade_sistema(tb_grupo_acesso_id,tb_funcionalidade_sistema_id, ativo) VALUES ((select id from tb_grupo_acesso where descricao = 'PRESTADOR' and tb_org_id = 9),46,1);