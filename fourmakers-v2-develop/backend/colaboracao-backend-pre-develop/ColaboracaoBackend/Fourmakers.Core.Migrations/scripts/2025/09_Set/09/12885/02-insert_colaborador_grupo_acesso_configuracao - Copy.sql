INSERT INTO tb_colaborador_grupo_acesso_configuracao (
    tabela,
    coluna,
    condicao,
    chave,
    tb_grupo_acesso_id,
    tb_org_id
) VALUES (
    'tb_colaborador_org',
    'modelo_contratacao',
    'tb_org_id,codigo_interno_colaborador',
    'CLT',
    (select id from tb_grupo_acesso where descricao = 'COLABORADOR CLT' and tb_org_id = 9),
    9
);