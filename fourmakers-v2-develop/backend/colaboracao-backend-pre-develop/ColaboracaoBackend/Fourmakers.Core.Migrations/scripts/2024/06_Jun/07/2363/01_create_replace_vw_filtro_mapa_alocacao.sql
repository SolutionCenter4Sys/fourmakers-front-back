DELIMITER //

CREATE OR REPLACE VIEW vw_filtro_mapaalocacao AS
    SELECT 
        c.cpf AS cpf,
        c.nome_completo AS nome_completo,
        co.ativo AS ativo,
        co.tb_org_id AS tb_org_id,
        co.cod_diretoria,
        co.diretoria,
        (SELECT 
                CONCAT('||',
                            GROUP_CONCAT(DISTINCT co.descricao
                                SEPARATOR '||'),
                            '||')
            FROM
                tb_colaborador_competencia cc
                JOIN tb_competencia co ON (co.id = cc.competencia_id)
            WHERE
                cc.colaborador_cpf = c.cpf
            GROUP BY cc.colaborador_cpf) AS hardskills,
        (SELECT 
                CONCAT('||',
                            GROUP_CONCAT(DISTINCT so.descricao
                                SEPARATOR '||'),
                            '||')
            FROM
                tb_colaborador_idioma cs
                JOIN tb_idioma so ON (so.id = cs.idioma_id)
            WHERE
                cs.colaborador_cpf = c.cpf
            GROUP BY cs.colaborador_cpf) AS idiomas
    FROM
        tb_colaborador c
        JOIN tb_colaborador_org co ON co.tb_colaborador_cpf = c.cpf
    GROUP BY
        c.cpf , co.tb_org_id;

//

DELIMITER ;
