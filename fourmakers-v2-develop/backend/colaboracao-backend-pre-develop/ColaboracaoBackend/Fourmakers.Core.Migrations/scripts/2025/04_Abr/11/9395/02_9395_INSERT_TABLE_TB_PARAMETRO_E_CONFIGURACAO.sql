INSERT INTO tb_parametro (
id,
nome_parametro,
descricao_parametro,
codigo_parametro,
codigo_modulo_sistema,
data_criacao,
data_alteracao,
ativo,
tipo_parametro)
	VALUES (
uuid(),
'Clientes novos são criados com visibilidade configurada para a gestão de alocados',
'Quando um cliente é criado através do CADASTRO DE PROJETOS, deve vir setado com valor configurado para a coluna ''visivel_na_gestao_de_alocados'' da tabela tb_cliente_org',
'CADASTRA_CLIENTE_OCULTO_NA_GESTAO_ALOCADOS',
'PROJETO',
CURRENT_DATE(),
CURRENT_DATE(),
1,
'BACKEND');

INSERT INTO tb_parametro_configuracao (id,tb_org_id,codigo_parametro,valor_parametro,tb_parametro_nivel_id)
	VALUES (uuid(),2,'CADASTRA_CLIENTE_OCULTO_NA_GESTAO_ALOCADOS','true',3);