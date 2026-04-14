INSERT INTO tb_verba_tipo (descricao, label, operacao, tb_org_id, ativo, tb_verba_tipo_custo_id)
VALUES
    ('Despesa variável', 'Valor limite de reembolso', '-', 2, 1, 1),
    ('Despesa fixa', 'Valor do reembolso por unidade', '-', 2, 1, 2),
    ('Crédito', 'Valor limite de crédito', '+', 2, 1, 1),
    ('Débito', 'Valor limite de débito', '-', 2, 1, 1);

-- tb_org_id = 5
INSERT INTO tb_verba_tipo (descricao, label, operacao, tb_org_id, ativo, tb_verba_tipo_custo_id)
VALUES
    ('Despesa variável', 'Valor limite de reembolso', '-', 5, 1, 1),
    ('Despesa fixa', 'Valor do reembolso por unidade', '-', 5, 1, 2),
    ('Crédito', 'Valor limite de crédito', '+', 5, 1, 1),
    ('Débito', 'Valor limite de débito', '-', 5, 1, 1);

-- tb_org_id = 8
INSERT INTO tb_verba_tipo (descricao, label, operacao, tb_org_id, ativo, tb_verba_tipo_custo_id)
VALUES
    ('Despesa variável', 'Valor limite de reembolso', '-', 8, 1, 1),
    ('Despesa fixa', 'Valor do reembolso por unidade', '-', 8, 1, 2),
    ('Crédito', 'Valor limite de crédito', '+', 8, 1, 1),
    ('Débito', 'Valor limite de débito', '-', 8, 1, 1);
 