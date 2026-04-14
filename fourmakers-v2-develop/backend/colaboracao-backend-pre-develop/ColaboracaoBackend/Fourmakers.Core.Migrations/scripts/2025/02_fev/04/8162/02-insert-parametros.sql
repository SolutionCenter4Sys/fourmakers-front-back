INSERT INTO tb_parametro (id,nome_parametro,descricao_parametro,codigo_parametro,codigo_modulo_sistema,ativo,tipo_parametro,tb_usuario_id_criacao)
VALUES (UUID(),'URL Home Default','Define a URL padrão de redirecionamento para o portal.','URL_HOME_DEFAULT','GERAL',1,'FRONTEND',1011);

INSERT INTO tb_parametro (id,nome_parametro,descricao_parametro,codigo_parametro,codigo_modulo_sistema,ativo,tipo_parametro,tb_usuario_id_criacao)
VALUES (UUID(),'Mostrar Vagas','Habilita a exibição de vagas no portal.','MOSTRA_VAGAS','VAGAS',1,'FRONTEND',1011);

INSERT INTO tb_parametro (id,nome_parametro,descricao_parametro,codigo_parametro,codigo_modulo_sistema,ativo,tipo_parametro,tb_usuario_id_criacao)
VALUES (UUID(),'Mostrar Mapa Demográfico','Habilita a exibição de mapa demográfico.','MOSTRA_MAPA_DEMOGRAFICO','GESTAO',1,'FRONTEND',1011);

INSERT INTO tb_parametro (id,nome_parametro,descricao_parametro,codigo_parametro,codigo_modulo_sistema,ativo,tipo_parametro,tb_usuario_id_criacao)
VALUES (UUID(),'Pode Candidatar','Habilita a candidatura do usuário ao banco de talentos.','PODE_CANDIDATAR','VAGAS',1,'FRONTEND',1011);