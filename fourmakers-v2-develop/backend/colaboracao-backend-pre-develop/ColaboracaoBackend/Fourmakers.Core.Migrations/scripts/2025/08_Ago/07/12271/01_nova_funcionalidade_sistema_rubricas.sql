INSERT INTO tb_funcionalidade_sistema (id, descricao)
VALUES(42, "RUBRICAS");

INSERT INTO tb_grupo_acesso_funcionalidade_sistema(tb_funcionalidade_sistema_id, tb_grupo_acesso_id)
VALUES(42, (
    SELECT id
    FROM tb_grupo_acesso tga
    WHERE tga.descricao = "GESTOR ROYAL"
      AND tga.tb_org_id = 9
));

