DELIMITER //
          
CREATE OR REPLACE VIEW `vw_skills` AS
select
    tc.descricao as descricao,
    tc.id        as id,
    tip.id       as tipo_id,
    tc.confirmada as confirmada
from tb_competencia tc
         join tb_item_perfil tip on tip.descricao = 'COMPETENCIA'
where tc.ativo = true

union all

select
    ts.descricao as descricao,
    ts.id        as id,
    tip.id       as tipo_id,
    ts.confirmada as confirmada
from tb_softskill ts
         join tb_item_perfil tip on tip.descricao = 'SOFTSKILL'
where ts.ativo = true

union all

select
    tm.descricao as descricao,
    tm.id        as id,
    tip.id       as tipo_id,
    tm.confirmada as confirmada
from tb_metodologia tm
         join tb_item_perfil tip on tip.descricao = 'METODOLOGIA'
where tm.ativo = true

union all

select
    td.descricao as descricao,
    td.id        as id,
    tip.id       as tipo_id,
    td.confirmada as confirmada
from tb_dominionegocio td
         join tb_item_perfil tip on tip.descricao = 'DOMINIONEGOCIO'
where td.ativo = true

union all

select
    ti.descricao as descricao,
    ti.id        as id,
    tip.id       as tipo_id,
    ti.confirmada as confirmada
from tb_idioma ti
         join tb_item_perfil tip on tip.descricao = 'IDIOMA'
where ti.ativo = true

union all

select
    tsd.descricao as descricao,
    tsd.id        as id,
    tip.id        as tipo_id,
    tsd.confirmada as confirmada
from tb_skill_desconhecida tsd
         join tb_item_perfil tip on tip.descricao = 'DESCONHECIDO'
where tsd.ativo = true; //

DELIMITER ;