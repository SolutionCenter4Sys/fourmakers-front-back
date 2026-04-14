INSERT INTO tb_verba_tipo(descricao, label, operacao, tb_org_id, tb_verba_tipo_custo_id)
VALUES
    ("Variavel", "Valor limite por despesa", "+", 2, 1),
    ("Variavel", "Valor limite por despesa", "+", 4, 1),
    ("Variavel", "Valor limite por despesa", "+", 5, 1),
    ("Fixo", "Valor", "-", 2, 2),
    ("Fixo", "Valor", "-", 4, 2),
    ("Fixo", "Valor", "-", 5, 2),
    ("Variável (+)", "Valor limite por receita", "+", 8, 1),
    ("Variável (-)", "Valor limite por receita", "-", 8, 1),
    ("Fixo (+)", "Vlr receita por qtd", "+", 8, 2),
    ("Fixo (-)", "Vlr receita por qtd", "-", 8, 2);
