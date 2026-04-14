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
    'REEMBOLSO_HABILITAR_REMESSA_CNAB',
    'REEMBOLSO',
    NOW(),
    NOW(),
    1,
    'BACKEND',
    NULL,
    NULL
);


INSERT INTO tb_parametro_configuracao (id, tb_org_id,codigo_parametro,valor_parametro,tb_parametro_nivel_id)
	VALUES (uuid(), 2,'REEMBOLSO_HABILITAR_REMESSA_CNAB','true',3);