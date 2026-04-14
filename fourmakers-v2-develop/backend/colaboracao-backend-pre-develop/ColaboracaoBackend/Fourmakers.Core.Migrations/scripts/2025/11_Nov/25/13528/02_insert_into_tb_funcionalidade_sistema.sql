INSERT INTO tb_funcionalidade_sistema 
(id, descricao, data_criacao , data_alteracao, ativo)
VALUES
(53, "ALIANCAS_PARCERIAS", NOW(), NOW(), 1);

INSERT INTO tb_funcionalidade_rota 
(id, rota, tb_funcionalidade_sistema_id, tb_org_id)
VALUES
(UUID(), "gestao/parcerias", 53, 2);