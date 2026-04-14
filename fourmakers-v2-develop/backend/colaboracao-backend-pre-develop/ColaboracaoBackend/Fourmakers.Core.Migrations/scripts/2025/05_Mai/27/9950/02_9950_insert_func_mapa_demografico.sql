INSERT INTO tb_funcionalidade_sistema (id,descricao) VALUES (38,'MAPA_DEMOGRAFICO');

INSERT INTO tb_grupo_acesso_funcionalidade_sistema(tb_grupo_acesso_id, tb_funcionalidade_sistema_id, ativo) VALUES
((SELECT id from tb_grupo_acesso where descricao = 'GESTÃO PRODUTO' AND tb_org_id = 2), 38, 1)