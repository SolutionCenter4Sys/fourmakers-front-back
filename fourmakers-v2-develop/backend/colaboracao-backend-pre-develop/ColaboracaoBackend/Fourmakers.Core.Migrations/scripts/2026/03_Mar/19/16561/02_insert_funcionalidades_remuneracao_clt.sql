-- Funcionalidades para Remuneração CLT (enum: 76, 77)
INSERT INTO tb_funcionalidade_sistema (id, descricao, data_criacao, data_alteracao, ativo)
VALUES
(76, 'ADMISSAO_REMUNERACAO_CLT_LISTAR', NOW(), NOW(), 1),
(77, 'ADMISSAO_REMUNERACAO_CLT_CRIAR_EDITAR', NOW(), NOW(), 1)
ON DUPLICATE KEY UPDATE descricao = VALUES(descricao);
