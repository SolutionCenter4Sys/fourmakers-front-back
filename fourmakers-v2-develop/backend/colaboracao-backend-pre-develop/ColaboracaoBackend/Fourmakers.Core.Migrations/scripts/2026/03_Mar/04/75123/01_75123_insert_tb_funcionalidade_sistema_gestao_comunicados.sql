-- PBI 75123: Funcionalidade GESTAO_COMUNICADOS para Gestão de acessos e menu
-- Inserir em tb_funcionalidade_sistema para controle de acesso e exibição no menu

INSERT INTO tb_funcionalidade_sistema
(id, descricao, data_criacao, data_alteracao, ativo)
VALUES
(72, 'GESTAO_COMUNICADOS', NOW(), NOW(), 1);
