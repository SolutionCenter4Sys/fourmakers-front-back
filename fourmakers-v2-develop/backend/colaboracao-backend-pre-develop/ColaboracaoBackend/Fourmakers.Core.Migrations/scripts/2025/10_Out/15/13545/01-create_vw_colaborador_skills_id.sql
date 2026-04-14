CREATE OR REPLACE VIEW vw_colaborador_skills_id AS
select
    hab.id AS id,
    hab.codigo_interno_colaborador AS codigo_interno_colaborador,
    hab.nivel_id AS nivel_id,
    hab.nivel AS nivel,
    hab.tipo_id AS tipo_id,
    hab.tipo AS tipo,
    hab.descricao AS descricao
from
    (
    select
        tc.id AS id,
        c.codigo_interno_colaborador AS codigo_interno_colaborador,
        n.id AS nivel_id,
        n.descricao AS nivel,
        1 AS tipo_id,
        'Competência' AS tipo,
        tc.descricao AS descricao
    from
        ((tb_colaborador_competencia c
    join tb_nivel n on
        ((c.tb_nivel_id = n.id)))
    join tb_competencia tc on
        ((c.competencia_id = tc.id)))
    where
        (c.ativo = 1)
union all
    select
        tm.id AS id,
        m.codigo_interno_colaborador AS codigo_interno_colaborador,
        n.id AS nivel_id,
        n.descricao AS nivel,
        3 AS tipo_id,
        'Metodologia' AS tipo,
        tm.descricao AS descricao
    from
        ((tb_colaborador_metodologia m
    join tb_nivel n on
        ((m.tb_nivel_id = n.id)))
    join tb_metodologia tm on
        ((m.metodologia_id = tm.id)))
    where
        (m.ativo = 1)
union all
    select
        ti.id AS id,
        tci.codigo_interno_colaborador AS codigo_interno_colaborador,
        n.id AS nivel_id,
        n.descricao AS nivel,
        9 AS tipo_id,
        'Idioma' AS tipo,
        ti.descricao AS descricao
    from
        ((tb_colaborador_idioma tci
    join tb_nivel n on
        ((tci.tb_nivel_id = n.id)))
    join tb_idioma ti on
        ((tci.idioma_id = ti.id)))
    where
        (tci.ativo = 1)
union all
    select
        ts.id AS id,
        tcs.codigo_interno_colaborador AS codigo_interno_colaborador,
        n.id AS nivel_id,
        n.descricao AS nivel,
        8 AS tipo_id,
        'Softskill' AS tipo,
        ts.descricao AS descricao
    from
        ((tb_colaborador_softskill tcs
    join tb_nivel n on
        ((tcs.tb_nivel_id = n.id)))
    join tb_softskill ts on
        ((tcs.softskill_id = ts.id)))
    where
        (tcs.ativo = 1)
union all
    select
        ts.id AS id,
        tcs.codigo_interno_colaborador AS codigo_interno_colaborador,
        n.id AS nivel_id,
        n.descricao AS nivel,
        2 AS tipo_id,
        'Formação' AS tipo,
        ts.descricao AS descricao
    from
        ((tb_colaborador_formacao tcs
    join tb_nivel n on
        ((tcs.tb_nivel_id = n.id)))
    join tb_formacao ts on
        ((tcs.formacao_id = ts.id)))
    where
        (tcs.ativo = 1)

 union all
    select
        td.id AS id,
        tcd.codigo_interno_colaborador AS codigo_interno_colaborador,
        n.id AS nivel_id,
        n.descricao AS nivel,
        4 AS tipo_id,
        'Dominio' AS tipo,
        td.descricao AS descricao
    from
        ((tb_colaborador_dominionegocio tcd
    join tb_nivel n on
        ((tcd.tb_nivel_id = n.id)))
    join tb_dominionegocio td on
        ((tcd.dominionegocio_id = td.id)))
    where
        (tcd.ativo = 1)
) hab;

