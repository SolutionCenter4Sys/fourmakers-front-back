-- Alinha descrição em tb_funcionalidade_sistema ao prefixo ADMISSAO_ (ids 74–77).
-- Necessário em bancos que já rodaram inserts com CBO_* / REMUNERACAO_CLT_*.
-- Permissões por grupo continuam válidas (FK por id numérico).

UPDATE tb_funcionalidade_sistema SET descricao = 'ADMISSAO_CBO_LISTAR', data_alteracao = NOW() WHERE id = 74;
UPDATE tb_funcionalidade_sistema SET descricao = 'ADMISSAO_CBO_CRIAR_EDITAR', data_alteracao = NOW() WHERE id = 75;
UPDATE tb_funcionalidade_sistema SET descricao = 'ADMISSAO_REMUNERACAO_CLT_LISTAR', data_alteracao = NOW() WHERE id = 76;
UPDATE tb_funcionalidade_sistema SET descricao = 'ADMISSAO_REMUNERACAO_CLT_CRIAR_EDITAR', data_alteracao = NOW() WHERE id = 77;
