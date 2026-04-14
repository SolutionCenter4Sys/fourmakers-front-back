INSERT INTO tb_grupo_acesso_funcionalidade_sistema (tb_funcionalidade_sistema_id, tb_grupo_acesso_id )
VALUES (34,(SELECT tga.id from tb_grupo_acesso tga WHERE tga.descricao = "Gestores"));