INSERT INTO tb_funcionalidade_sistema (descricao) 
VALUES ('CRIAR_PROPOSTAS');

SET @funcionalidade_id = LAST_INSERT_ID();

INSERT INTO tb_grupo_acesso (descricao,nivel, tb_org_id) 
VALUES ('GESTÃO DE PROPOSTAS',0 , 2);

SET @grupo_acesso_id = LAST_INSERT_ID();

INSERT INTO tb_grupo_acesso_funcionalidade_sistema (tb_grupo_acesso_id, tb_funcionalidade_sistema_id)
VALUES (@grupo_acesso_id, @funcionalidade_id);