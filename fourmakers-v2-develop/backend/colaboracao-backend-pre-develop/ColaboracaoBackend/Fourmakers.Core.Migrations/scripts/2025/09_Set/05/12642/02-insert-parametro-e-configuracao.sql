INSERT INTO tb_parametro (id, nome_parametro,descricao_parametro,codigo_parametro,codigo_modulo_sistema,ativo,tipo_parametro)
	VALUES (uuid(),'Associar Grupo Acesso por Configuração', 'Habilita a associação automática do usuário quando for criado ou editado. Pra determinar quais grupos de acesso será consultada a tabela tb_colaborador_grupo_acesso_configuracao','DEVE_ASSOCIAR_GRUPO_ACESSO_POR_CONFIGURACAO','USUARIO',1,'BACKEND');

INSERT INTO tb_parametro_configuracao (id,tb_org_id,codigo_parametro,valor_parametro,tb_parametro_nivel_id)
	VALUES (uuid(),9,'DEVE_ASSOCIAR_GRUPO_ACESSO_POR_CONFIGURACAO','true',3);

