-- =============================================================================
-- OPCIONAL — documentação para DBA / ambientes já implantados
--
-- Se o banco foi populado com IDs antigos (ex.: CBO 73–74, Remuneração 75–76, Admissão 77–78)
-- e o enum agora é:
--   73 COMUNICACAO_USUARIO_COMUM
--   74 ADMISSAO_CBO_LISTAR, 75 ADMISSAO_CBO_CRIAR_EDITAR
--   76 ADMISSAO_REMUNERACAO_CLT_LISTAR, 77 ADMISSAO_REMUNERACAO_CLT_CRIAR_EDITAR
--   78 ADMISSAO_CARGO_LISTAR, 79 ADMISSAO_CARGO_CRIAR_EDITAR
--
-- É preciso alinhar tb_funcionalidade_sistema e TODAS as tabelas com FK para tb_funcionalidade_sistema.id.
-- Não há script genérico seguro aqui sem conhecer vínculos (usuário, grupo, etc.).
--
-- Checagem sugerida:
--   SELECT id, descricao FROM tb_funcionalidade_sistema WHERE id BETWEEN 73 AND 79 ORDER BY id;
-- =============================================================================

SELECT 1;
