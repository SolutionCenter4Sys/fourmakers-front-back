INSERT INTO tb_token_sistema (
    sistema,
    token,
    data_criacao,
    data_alteracao,
    ativo,
    tb_org_id
)
SELECT
    'FOURMAKERS-ROYAL',
    'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJvcmdJZCI6OSwiY2xpZW50Ijoicm95YWwiLCJ0aW1lc3RhbXAiOjE3MjQxODc3NTF9.c2lnbmF0dXJlX3RlbXA',
    NOW(),
    NOW(),
    1,
    9
WHERE NOT EXISTS (
    SELECT 1
    FROM tb_token_sistema
    WHERE sistema = 'FOURMAKERS-ROYAL'
);
