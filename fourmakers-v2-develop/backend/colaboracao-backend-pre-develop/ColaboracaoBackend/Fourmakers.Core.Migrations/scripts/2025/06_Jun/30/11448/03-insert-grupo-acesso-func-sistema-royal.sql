INSERT INTO tb_grupo_acesso_funcionalidade_sistema
    (tb_grupo_acesso_id, tb_funcionalidade_sistema_id, data_criacao, data_alteracao, ativo)
VALUES
    ((select id from tb_grupo_acesso where descricao = 'CONSULTA CV' AND tb_org_id = 9), 8, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'CONSULTA CV' AND tb_org_id = 9), 9, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'CONSULTA CV' AND tb_org_id = 9), 10, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR APONTAMENTO' AND tb_org_id = 9), 11, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR APONTAMENTO' AND tb_org_id = 9), 16, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR DP' AND tb_org_id = 9), 15, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR DP' AND tb_org_id = 9), 18, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR DP' AND tb_org_id = 9), 19, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR GERAL' AND tb_org_id = 9), 38, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR PDI' AND tb_org_id = 9), 24, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR PDI' AND tb_org_id = 9), 39, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR REEMBOLSO' AND tb_org_id = 9), 36, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR RECRUTAMENTO' AND tb_org_id = 9), 32, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR RECRUTAMENTO' AND tb_org_id = 9), 33, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR RECRUTAMENTO' AND tb_org_id = 9), 34, NOW(), NOW(), 1),
    ((select id from tb_grupo_acesso where descricao = 'GESTOR RECRUTAMENTO' AND tb_org_id = 9), 35, NOW(), NOW(), 1);