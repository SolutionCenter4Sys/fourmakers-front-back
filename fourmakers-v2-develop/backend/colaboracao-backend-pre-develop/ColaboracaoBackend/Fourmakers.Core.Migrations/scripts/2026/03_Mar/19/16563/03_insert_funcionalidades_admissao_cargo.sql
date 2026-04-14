-- Funcionalidades para Cargo de Admissão (CRUD sem exclusão via API; enum: 78, 79)
INSERT INTO tb_funcionalidade_sistema (id, descricao, data_criacao, data_alteracao, ativo)
VALUES
(78, 'ADMISSAO_CARGO_LISTAR', NOW(), NOW(), 1),
(79, 'ADMISSAO_CARGO_CRIAR_EDITAR', NOW(), NOW(), 1)
ON DUPLICATE KEY UPDATE descricao = VALUES(descricao);
