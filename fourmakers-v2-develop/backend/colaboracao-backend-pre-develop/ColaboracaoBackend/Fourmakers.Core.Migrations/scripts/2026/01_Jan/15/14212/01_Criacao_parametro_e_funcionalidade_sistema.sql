INSERT INTO tb_parametro
(
    id,
    nome_parametro,
    descricao_parametro,
    codigo_parametro,
    codigo_modulo_sistema,
    data_criacao,
    data_alteracao,
    ativo,
    tipo_parametro,
    tb_usuario_id_criacao,
    tb_usuario_id_alteracao
)
VALUES
(
    UUID(),
    'Habilitar remessa de reembolso via CNAB',
    'Permite habilitar a geração e envio de remessa de reembolso no formato CNAB.',
    'HABILITAR_GESTAO_DESEMPENHO',
    'GESTAOPESSOA',
    NOW(),
    NOW(),
    1,
    'BACKEND',
    NULL,
    NULL
);


INSERT INTO tb_parametro_configuracao (id, tb_org_id,codigo_parametro,valor_parametro,tb_parametro_nivel_id)
	VALUES (uuid(), 2,'HABILITAR_GESTAO_DESEMPENHO','true',3);
	
	
	
INSERT INTO tb_funcionalidade_sistema (id,descricao)
	VALUES (54,'GESTAO_DESEMPENHO_RH');
	
INSERT INTO
    tb_grupo_acesso_funcionalidade_sistema
(
    tb_grupo_acesso_id,
    tb_funcionalidade_sistema_id,
    data_criacao,
    ativo
)
SELECT
    ga.id,
    54,
    NOW(),
    1
FROM
    tb_grupo_acesso ga
WHERE
    ga.tb_org_id = 2
    AND ga.descricao IN (
        'PEOPLE PARTNER',
        'GESTÃO PRODUTO'
    );