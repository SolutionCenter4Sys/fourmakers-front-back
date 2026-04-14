DROP VIEW IF EXISTS vw_perfis;

DELIMITER //
CREATE OR REPLACE VIEW vw_perfis AS
select
    tp.nome_perfil as perfil,
    tp.tb_org_id as tb_org_id,
    concat('1|', tp.id) as id,
    tpo.cod_cliente as codigo_cliente,
    1 as ativo
from tb_perfil tp
         join tb_projeto_org tpo
              on tpo.tb_org_id = tp.tb_org_id
                  and tpo.cod_projeto = tp.codigo_projeto

union

select
    concat(tgep.nome_perfil, ' / ', tge.nome) as perfil,
    tgep.tb_org_id as tb_org_id,
    concat('2|', tgep.id) as id,
    tge.codigo_cliente as codigo_cliente,
    tgep.ativo as ativo
from tb_gestor_externo_perfil tgep
         join tb_gestor_externo tge
              on tge.tb_org_id = tgep.tb_org_id
                  and tge.cod_gestor_externo = tgep.cod_gestor_externo;
//
DELIMITER ;