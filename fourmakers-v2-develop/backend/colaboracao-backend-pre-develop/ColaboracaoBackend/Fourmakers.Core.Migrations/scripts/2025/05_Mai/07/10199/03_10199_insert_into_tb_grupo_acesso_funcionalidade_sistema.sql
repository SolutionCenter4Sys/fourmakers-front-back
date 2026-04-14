INSERT INTO tb_grupo_acesso_funcionalidade_sistema (tb_funcionalidade_sistema_id, tb_grupo_acesso_id, ativo)
SELECT
    36,
    tga.id,
    1
FROM tb_grupo_acesso tga
WHERE tga.descricao = 'REEMBOLSO'
  AND tga.tb_org_id = 2;