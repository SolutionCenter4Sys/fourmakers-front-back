CREATE OR REPLACE VIEW vw_competencias_sugeridas AS
SELECT 
    tc.id AS CompetenciaId,
    tc.descricao AS Descricao,
    tc.data_criacao AS DataCriacao,
    tc.usuario_criacao_id AS UsuarioCriacaoId,
    'HARDSKILL' AS CompetenciaTipo,
    (SELECT COUNT(*) FROM tb_colaborador_competencia tcc WHERE tcc.competencia_id = tc.id AND tcc.ativo = 1) AS qtdUsuariosCompetencia
FROM 
    tb_competencia tc
WHERE 
    tc.ativo = 1 AND tc.confirmada = 0

UNION ALL

SELECT 
    ts.id AS CompetenciaId,
    ts.descricao AS Descricao,
    ts.data_criacao AS DataCriacao,
    NULL AS UsuarioCriacaoId,  -- tb_softskill não tem usuario_criacao_id
    'SOFTSKILL' AS CompetenciaTipo,
    (SELECT COUNT(*) FROM tb_colaborador_softskill tcs WHERE tcs.softskill_id = ts.id AND tcs.ativo = 1) AS qtdUsuariosCompetencia
FROM 
    tb_softskill ts
WHERE 
    ts.ativo = 1 AND ts.confirmada = 0

UNION ALL

SELECT 
    tm.id AS CompetenciaId,
    tm.descricao AS Descricao,
    tm.data_criacao AS DataCriacao,
    NULL AS UsuarioCriacaoId,  -- tb_softskill não tem usuario_criacao_id
    'METODOLOGIA' AS CompetenciaTipo,
    (SELECT COUNT(*) FROM tb_colaborador_metodologia tcm WHERE tcm.metodologia_id = tm.id AND tcm.ativo = 1) AS qtdUsuariosCompetencia
FROM 
    tb_metodologia tm
WHERE 
    tm.ativo = 1 AND tm.confirmada = 0

UNION ALL

SELECT 
    td.id AS CompetenciaId,
    td.descricao AS Descricao,
    td.data_criacao AS DataCriacao,
    NULL AS UsuarioCriacaoId,  -- tb_softskill não tem usuario_criacao_id
    'DOMINIO' AS CompetenciaTipo,
    (SELECT COUNT(*) FROM tb_colaborador_dominionegocio tcd WHERE tcd.dominionegocio_id = td.id AND tcd.ativo = 1) AS qtdUsuariosCompetencia
FROM 
    tb_dominionegocio td
WHERE 
    td.ativo = 1 AND td.confirmada = 0

UNION ALL

SELECT 
    ti.id AS CompetenciaId,
    ti.descricao AS Descricao,
    ti.data_criacao AS DataCriacao,
    NULL AS UsuarioCriacaoId,  -- tb_idioma não tem usuario_criacao_id
    'IDIOMA' AS CompetenciaTipo,
    (SELECT COUNT(*) FROM tb_colaborador_idioma tci WHERE tci.idioma_id = ti.id AND tci.ativo = 1) AS qtdUsuariosCompetencia
FROM 
    tb_idioma ti
WHERE 
    ti.ativo = 1 AND ti.confirmada = 0;
