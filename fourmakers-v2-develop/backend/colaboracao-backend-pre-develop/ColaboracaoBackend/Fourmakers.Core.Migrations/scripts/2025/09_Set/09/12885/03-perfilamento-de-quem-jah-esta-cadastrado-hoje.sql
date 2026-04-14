INSERT INTO tb_usuario_grupo_acesso (tb_usuario_id, tb_grupo_acesso_id, ativo)
SELECT 
    tu.id,
    (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9),
    1
FROM tb_colaborador_org tco
JOIN tb_usuario tu 
    ON tco.codigo_interno_colaborador = tu.codigo_interno_colaborador
WHERE tco.modelo_contratacao = 'CLT'
  AND NOT EXISTS (
      SELECT 1 
      FROM tb_usuario_grupo_acesso uga
      WHERE uga.tb_usuario_id = tu.id
        AND uga.tb_grupo_acesso_id = (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9)
  );

INSERT INTO tb_usuario_grupo_acesso (tb_usuario_id, tb_grupo_acesso_id, ativo)
SELECT 
	distinct
    tu.id,
    (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9),
    1
FROM tb_colaborador_org tco
JOIN tb_usuario tu 
    ON tco.codigo_interno_colaborador = tu.codigo_interno_colaborador
WHERE tco.modelo_contratacao = 'Parceiro (PJ)'
  AND NOT EXISTS (
      SELECT 1 
      FROM tb_usuario_grupo_acesso uga
      WHERE uga.tb_usuario_id = tu.id
        AND uga.tb_grupo_acesso_id = (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9)
  );

INSERT INTO tb_usuario_grupo_acesso (tb_usuario_id, tb_grupo_acesso_id, ativo)
SELECT 
	distinct
    tu.id,
    (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9),
    1
FROM tb_colaborador_org tco
JOIN tb_usuario tu 
    ON tco.codigo_interno_colaborador = tu.codigo_interno_colaborador
WHERE tco.modelo_contratacao = 'Parceiro Terceiro (PJ)'
  AND NOT EXISTS (
      SELECT 1 
      FROM tb_usuario_grupo_acesso uga
      WHERE uga.tb_usuario_id = tu.id
        AND uga.tb_grupo_acesso_id = (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9)
  );
