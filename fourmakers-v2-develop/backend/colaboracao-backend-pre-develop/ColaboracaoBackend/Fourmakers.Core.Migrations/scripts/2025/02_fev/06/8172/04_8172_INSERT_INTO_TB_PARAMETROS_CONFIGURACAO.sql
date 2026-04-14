INSERT INTO tb_parametro_configuracao (
	id,
	tb_org_id,
	codigo_parametro,
	valor_parametro,
	tb_parametro_nivel_id
)VALUES(
	UUID(),
	7,
	'CAMPOS_NAO_OBRIGATORIOS_CADASTRO_COLABORADOR',
	'Email;Gestor;Diretoria;Departamento',
	3
);