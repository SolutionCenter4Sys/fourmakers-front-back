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
    'Configuração validacao cadastro de colaborador',
    'Permitir configurar niveis de validacao no cadastro do colaborador',
    'CAMPOS_NAO_OBRIGATORIOS_CADASTRO_COLABORADOR',
    'GESTAO',
    1,
    'BACKEND'
);
 