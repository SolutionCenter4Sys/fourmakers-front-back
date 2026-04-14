INSERT INTO tb_grupo_acesso (descricao, ativo, nivel, tb_org_id)
VALUES ('GESTOR PRESTADOR', 1, 0, 9);

INSERT INTO tb_funcionalidade_sistema (id, descricao, ativo)
VALUES (47, 'GESTAO_PRESTADOR', 1);

INSERT INTO tb_grupo_acesso_funcionalidade_sistema (tb_grupo_acesso_id, tb_funcionalidade_sistema_id, ativo)
VALUES (
    (SELECT id FROM tb_grupo_acesso WHERE descricao = 'GESTOR PRESTADOR' AND tb_org_id = 9),
    47,
    1
);