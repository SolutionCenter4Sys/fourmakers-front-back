INSERT INTO tb_funcionalidade_sistema (id, descricao) 
VALUES 
    (32, 'VISUALIZAR_INDICACOES'),
    (33, 'EDITAR_INDICACOES');

INSERT INTO tb_grupo_acesso_funcionalidade_sistema (tb_funcionalidade_sistema_id, tb_grupo_acesso_id)
VALUES 
    (32, 16),
    (33, 16);