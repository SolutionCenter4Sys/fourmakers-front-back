INSERT INTO tb_parametro_configuracao(id, tb_org_id, codigo_parametro, valor_parametro)
SELECT id, tb_org_id, codigo_parametro, valor_parametro FROM tb_org_parametro_configuracao;

UPDATE tb_parametro_configuracao 
SET tb_parametro_nivel_id = 3
WHERE tb_org_id IS NOT NULL 
AND tb_colaborador_org_cpf IS NULL 
AND tb_grupo_acesso_id IS NULL 
AND tb_parametro_nivel_id IS NULL;