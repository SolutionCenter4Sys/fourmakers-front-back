-- Funcionalidades para Status de Admissão (enum: 80, 81)
INSERT INTO tb_funcionalidade_sistema (id, descricao, data_criacao, data_alteracao, ativo)
VALUES
(80, 'ADMISSAO_STATUS_LISTAR', NOW(), NOW(), 1),
(81, 'ADMISSAO_STATUS_CRIAR_EDITAR', NOW(), NOW(), 1)
ON DUPLICATE KEY UPDATE descricao = VALUES(descricao), ativo = VALUES(ativo), data_alteracao = NOW();
