-- Converte id de INT para CHAR(36) (GUID) em ambientes que já tinham tb_cbo com id INT
-- Se a tabela foi criada pela 01 com id CHAR(36), este script não altera a estrutura (id já é CHAR(36)).

-- Funcionalidades alinhadas ao enum FuncionalidadeSistemaEnum (73–75 neste bloco)
INSERT INTO tb_funcionalidade_sistema (id, descricao, data_criacao, data_alteracao, ativo)
VALUES
(73, 'COMUNICACAO_USUARIO_COMUM', NOW(), NOW(), 1),
(74, 'ADMISSAO_CBO_LISTAR', NOW(), NOW(), 1),
(75, 'ADMISSAO_CBO_CRIAR_EDITAR', NOW(), NOW(), 1)
ON DUPLICATE KEY UPDATE descricao = VALUES(descricao);
