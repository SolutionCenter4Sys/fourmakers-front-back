-- Script para inserir funcionalidades do módulo de Recrutamento
-- na tabela tb_funcionalidade_sistema

INSERT INTO tb_funcionalidade_sistema 
(id, descricao, data_criacao, data_alteracao, ativo)
VALUES
(56, 'RECRUTAMENTO_LISTAR_VAGAS', NOW(), NOW(), 1),
(57, 'RECRUTAMENTO_CRIAR_VAGA', NOW(), NOW(), 1),
(58, 'RECRUTAMENTO_EDITAR_VAGA', NOW(), NOW(), 1),
(59, 'RECRUTAMENTO_CANCELAR_VAGA', NOW(), NOW(), 1),
(60, 'RECRUTAMENTO_MOVIMENTAR_VAGA', NOW(), NOW(), 1);

