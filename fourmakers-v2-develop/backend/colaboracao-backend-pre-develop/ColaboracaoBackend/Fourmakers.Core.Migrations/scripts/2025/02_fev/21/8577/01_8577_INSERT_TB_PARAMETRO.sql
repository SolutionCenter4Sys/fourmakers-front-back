INSERT INTO tb_parametro (
	id,
    nome_parametro,
    descricao_parametro,
    codigo_parametro,
    codigo_modulo_sistema,
    ativo,
    tipo_parametro
) 

VALUES (
	UUID(),
    'Configuração para listagem de aderencia para as orgs',
    'Permitir configurar quais orgs listar na aderencia',
    'CONFIGURACAO_ADERENCIA_ORG',
    'GESTAO',
    1,
    'BACKEND'
);