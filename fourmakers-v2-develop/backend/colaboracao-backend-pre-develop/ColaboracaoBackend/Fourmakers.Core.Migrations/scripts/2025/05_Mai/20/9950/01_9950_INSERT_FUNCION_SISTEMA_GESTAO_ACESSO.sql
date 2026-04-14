INSERT INTO tb_funcionalidade_sistema (id,descricao,ativo)
	VALUES (37,'GESTAO_ACESSO',1);

INSERT INTO tb_grupo_acesso_funcionalidade_sistema(tb_grupo_acesso_id, tb_funcionalidade_sistema_id, ativo) VALUES
((SELECT id from tb_grupo_acesso where descricao = 'GESTÃO PRODUTO' AND tb_org_id = 2), 37, 1)