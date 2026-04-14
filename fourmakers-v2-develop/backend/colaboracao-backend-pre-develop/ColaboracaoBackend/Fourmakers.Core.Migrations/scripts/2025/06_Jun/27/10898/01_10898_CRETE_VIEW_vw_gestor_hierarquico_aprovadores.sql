CREATE VIEW vw_gestor_hierarquico_aprovadores AS
SELECT
    tco_gestor.codigo_interno_colaborador AS cod_interno_gestor,
    tco_colaborador.codigo_interno_colaborador AS cod_interno_colaborador,
    tco_gestor.tb_org_id,
    tch.cod_colaborador_superior as cod_externo_gerente,
    tch.cod_colaborador_externo as cod_externo_colaborador
FROM tb_colaborador_org tco_gestor
         INNER JOIN tb_colaborador_hierarquia tch
                    ON tch.cod_colaborador_superior = tco_gestor.cod_colaborador_externo
                        AND tch.tb_org_id = tco_gestor.tb_org_id
         INNER JOIN tb_colaborador_org tco_colaborador
                    ON tco_colaborador.cod_colaborador_externo = tch.cod_colaborador_externo
                        AND tco_colaborador.tb_org_id = tco_gestor.tb_org_id
         INNER JOIN tb_projeto_gerente tpg
                    ON tpg.cod_colaborador_gerente = tco_colaborador.cod_colaborador_externo
                        AND tpg.tb_org_id = tco_colaborador.tb_org_id
GROUP BY
    tco_gestor.codigo_interno_colaborador,
    tco_colaborador.codigo_interno_colaborador,
    tco_gestor.tb_org_id,
    tch.cod_colaborador_superior,
    tch.cod_colaborador_externo;