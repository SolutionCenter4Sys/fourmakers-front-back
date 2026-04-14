DROP PROCEDURE spr_get_alocacao_calculo_mensal_filtro;
DELIMITER $$
CREATE PROCEDURE `spr_get_alocacao_calculo_mensal_filtro`(
    IN mes_inicial INT, 
    IN ano_inicial INT, 
    IN mes_final INT, 
    IN ano_final INT, 
	IN eh_tbd INT,
    IN status VARCHAR(50),
    IN idioma VARCHAR(100),
    IN hard_skill VARCHAR(100),
	IN colaborador_ou_tbd VARCHAR (100), 
    IN gestor VARCHAR(100),  
    IN cod_diretoria VARCHAR(100),  
    IN nome_diretoria VARCHAR(100), 
    IN cursor_int INT, 
    IN limite_int INT,
    IN org_id INT
)
BEGIN
    DECLARE ano_atual INT DEFAULT ano_inicial;
    DECLARE mes_atual INT DEFAULT mes_inicial;

    DROP TEMPORARY TABLE IF EXISTS meses;
    CREATE TEMPORARY TABLE meses (
        mes INT,
        ano INT
    );
    
    main_loop: WHILE ano_atual <= ano_final DO
        WHILE mes_atual <= 12 DO
            IF (ano_atual = ano_final AND mes_atual > mes_final) THEN
                LEAVE main_loop;
            END IF;
            INSERT INTO meses (mes, ano) VALUES (mes_atual, ano_atual);
            SET mes_atual = mes_atual + 1;
        END WHILE;
        SET mes_atual = 1;
        SET ano_atual = ano_atual + 1;
    END WHILE;

    -- Criar tabela temporária de alocacoes_colaboradores
    DROP TEMPORARY TABLE IF EXISTS alocacoes_colaboradores;
    CREATE TEMPORARY TABLE alocacoes_colaboradores AS
    SELECT 
        tco.cod_colaborador_externo AS codigo_colaborador,
        tco.tb_colaborador_cpf as colaborador_cpf,
        tc.nome_completo AS nome,
        tco.ativo,
        tco.tb_org_id,
        tco.cod_diretoria,
        tco.diretoria,
        0 AS eh_tbd,
        m.mes,
        m.ano,
        IFNULL(tcpacm_colab.horas, 0) AS horas,
        CAST(NULL AS CHAR(11)) AS tb_colaborador_cpf_gestor_tbd,
        COALESCE(tcpacm_colab.status_colaborador_periodo_alocacao, 'HorasPendentes') AS status_colaborador_periodo_alocacao
    FROM
        tb_colaborador tc
    JOIN
        tb_colaborador_org tco ON tco.tb_colaborador_cpf = tc.cpf and tco.tb_org_id = org_id and tco.tb_org_id = org_id
    JOIN
        meses m  ON (m.ano > ano_inicial OR (m.ano = ano_inicial AND m.mes >= mes_inicial))
				 AND (m.ano < ano_final OR (m.ano = ano_final AND m.mes <= mes_final))
    LEFT JOIN 
        tb_colaborador_periodo_alocacao_calculo_mensal tcpacm_colab ON tcpacm_colab.codigo_colaborador = tco.cod_colaborador_externo AND tco.tb_org_id = tcpacm_colab.tb_org_id
        AND tcpacm_colab.mes = m.mes AND tcpacm_colab.ano = m.ano
    WHERE
         tco.tb_org_id = org_id;

    -- Inserir dados da tabela tb_tbd_alocado na tabela temporária de alocacoes_colaboradores
    INSERT INTO alocacoes_colaboradores
    SELECT 
        tta.cod_tbd_alocado AS codigo_colaborador,
        NULL AS colaborador_cpf,
        tta.descricao AS nome_completo,
        1 AS ativo,
        tta.tb_org_id AS tb_org_id,
        tta.cod_diretoria,
        tta.diretoria,
        1 AS eh_tbd,
        m.mes,
        m.ano,
        IFNULL(tcpacm_tbd.horas, 0) AS horas,
        tta.tb_colaborador_cpf_gestor AS tb_colaborador_cpf_gestor_tbd,
        COALESCE(tcpacm_tbd.status_colaborador_periodo_alocacao, 'HorasPendentes') AS status_colaborador_periodo_alocacao
    FROM
        tb_tbd_alocado tta
    JOIN
        meses m ON (m.ano > ano_inicial OR (m.ano = ano_inicial AND m.mes >= mes_inicial))
				 AND (m.ano < ano_final OR (m.ano = ano_final AND m.mes <= mes_final)) and tta.tb_org_id = org_id
    LEFT JOIN 
        tb_colaborador_periodo_alocacao_calculo_mensal tcpacm_tbd ON tcpacm_tbd.cod_tbd_alocado = tta.cod_tbd_alocado AND tta.tb_org_id = tcpacm_tbd.tb_org_id
        AND tcpacm_tbd.mes = m.mes AND tcpacm_tbd.ano = m.ano
    WHERE
        tta.tb_org_id = org_id;

    -- Criar tabela temporária de tb_tbd_hierarquia
    DROP TEMPORARY TABLE IF EXISTS colaborador_tbd_hierarquia;
    CREATE TEMPORARY TABLE colaborador_tbd_hierarquia AS
    SELECT 
        tta.cod_tbd_alocado AS cod_colaborador_externo,
        tco.cod_colaborador_externo AS cod_colaborador_superior,
        1 as eh_tbd,
        tta.tb_org_id
    FROM tb_tbd_alocado tta 
    JOIN tb_colaborador_org tco ON tta.tb_colaborador_cpf_gestor = tco.tb_colaborador_cpf AND tta.tb_org_id = tco.tb_org_id
    WHERE tta.tb_colaborador_cpf_gestor IS NOT NULL and tta.tb_org_id = org_id;
    
    INSERT INTO colaborador_tbd_hierarquia
    SELECT 
        tch.cod_colaborador_externo,
        tch.cod_colaborador_superior,
        0 as eh_tbd,
        tb_org_id
    FROM tb_colaborador_hierarquia tch 
    WHERE tch.tb_org_id = org_id;
    
    -- Criar tabela temporária de filtrados_com_status
    DROP TEMPORARY TABLE IF EXISTS filtrados_com_status;
    CREATE TEMPORARY TABLE filtrados_com_status AS
    SELECT DISTINCT 
        ac.codigo_colaborador, 
        ac.nome,
        ac.colaborador_cpf,
        ac.eh_tbd,
        GROUP_CONCAT(DISTINCT ti.descricao ORDER BY ti.descricao SEPARATOR ' | ') AS idiomas,
        GROUP_CONCAT(DISTINCT tc.descricao ORDER BY tc.descricao SEPARATOR ' | ') AS hard_skills,
        GROUP_CONCAT(DISTINCT tcth.cod_colaborador_superior ORDER BY tcth.cod_colaborador_superior SEPARATOR ' | ') AS gestores,
        GROUP_CONCAT(DISTINCT tc_h.nome_completo ORDER BY tc_h.nome_completo SEPARATOR ' | ') AS gestores_nome
    FROM alocacoes_colaboradores ac
    LEFT JOIN tb_colaborador_idioma tci ON tci.colaborador_cpf = ac.colaborador_cpf
    LEFT JOIN tb_idioma ti ON tci.idioma_id = ti.id
    LEFT JOIN tb_colaborador_competencia tcc ON tcc.colaborador_cpf = ac.colaborador_cpf
    LEFT JOIN tb_competencia tc ON tcc.competencia_id = tc.id
    LEFT JOIN colaborador_tbd_hierarquia tcth ON (tcth.cod_colaborador_externo = ac.codigo_colaborador AND tcth.tb_org_id = ac.tb_org_id AND ac.eh_tbd = tcth.eh_tbd) 
    LEFT JOIN tb_colaborador_org tco_h ON tcth.cod_colaborador_superior = tco_h.cod_colaborador_externo AND ac.tb_org_id = tco_h.tb_org_id
    LEFT JOIN tb_colaborador tc_h ON tco_h.tb_colaborador_cpf = tc_h.cpf
    WHERE (status IS NULL OR status = '' OR status = 'Todos' OR ac.status_colaborador_periodo_alocacao = status)
    AND (cod_diretoria IS NULL OR cod_diretoria = '' OR cod_diretoria = '0' OR ac.cod_diretoria = cod_diretoria)
    AND (nome_diretoria IS NULL OR nome_diretoria = '' OR ac.diretoria LIKE CONCAT('%', nome_diretoria, '%'))
    AND (idioma IS NULL OR idioma = '' OR EXISTS (
            SELECT 1 FROM tb_colaborador_idioma tci JOIN tb_idioma ti ON tci.idioma_id = ti.id
            WHERE tci.colaborador_cpf = ac.colaborador_cpf AND ti.descricao LIKE CONCAT('%', idioma, '%')
        ))
    AND (hard_skill IS NULL OR hard_skill = '' OR EXISTS (
            SELECT 1 FROM tb_colaborador_competencia tcc JOIN tb_competencia tc ON tcc.competencia_id = tc.id
            WHERE tcc.colaborador_cpf = ac.colaborador_cpf AND tc.descricao LIKE CONCAT('%', hard_skill, '%')
        ))
    AND (gestor IS NULL OR gestor = '' 
            OR (
                EXISTS 
                (
                    SELECT 1 FROM tb_colaborador_hierarquia tch_gestor -- filtro do gestor do colaborador
                    JOIN tb_colaborador_org tco_gestor ON tch_gestor.cod_colaborador_superior = tco_gestor.cod_colaborador_externo AND tch_gestor.tb_org_id = tco_gestor.tb_org_id
                    JOIN tb_colaborador tc_gestor ON tco_gestor.tb_colaborador_cpf = tc_gestor.cpf
                    WHERE tch_gestor.cod_colaborador_externo = ac.codigo_colaborador AND tch_gestor.tb_org_id = ac.tb_org_id AND ac.eh_tbd = 0
                    AND (tc_gestor.nome_completo LIKE CONCAT('%', gestor, '%') OR gestor = tco_gestor.cod_colaborador_externo)
                )
                 OR (
                 EXISTS
                 (
                    SELECT 1 FROM tb_tbd_alocado tch_tbd_gestor -- filtro do gestor do tbd
                    JOIN tb_colaborador_org tco_tbd_gestor ON tch_tbd_gestor.tb_colaborador_cpf_gestor = tco_tbd_gestor.tb_colaborador_cpf AND tch_tbd_gestor.tb_org_id = tco_tbd_gestor.tb_org_id
                    JOIN tb_colaborador tc_tbd_gestor ON tco_tbd_gestor.tb_colaborador_cpf = tc_tbd_gestor.cpf
                    WHERE tch_tbd_gestor.cod_tbd_alocado = ac.codigo_colaborador AND tch_tbd_gestor.tb_org_id = ac.tb_org_id AND ac.eh_tbd = 1
                    AND (tc_tbd_gestor.nome_completo LIKE CONCAT('%', gestor, '%') OR gestor = tco_tbd_gestor.cod_colaborador_externo)
                 )
               )
            )
        )
    AND (colaborador_ou_tbd IS NULL OR colaborador_ou_tbd = '' OR ac.codigo_colaborador = colaborador_ou_tbd OR ac.nome LIKE CONCAT('%', colaborador_ou_tbd, '%'))
    AND (eh_tbd IS NULL OR eh_tbd = '' OR ac.eh_tbd = eh_tbd)
    GROUP BY ac.codigo_colaborador, ac.nome;

    -- Criar tabela temporária de colaboradores_limitados
    DROP TEMPORARY TABLE IF EXISTS colaboradores_limitados;
    CREATE TEMPORARY TABLE colaboradores_limitados AS
    SELECT
        fcs.codigo_colaborador,
        fcs.colaborador_cpf,
		fcs.eh_tbd,
        fcs.nome,
        fcs.idiomas,
        fcs.hard_skills,
        fcs.gestores,
        fcs.gestores_nome,
        ROW_NUMBER() OVER (ORDER BY fcs.nome) AS rn
    FROM filtrados_com_status fcs;

    -- Criar tabela temporária de colaboradores_paginados
    DROP TEMPORARY TABLE IF EXISTS colaboradores_paginados;
    CREATE TEMPORARY TABLE colaboradores_paginados AS
    SELECT cl.codigo_colaborador, cl.eh_tbd
    FROM colaboradores_limitados cl
    WHERE rn BETWEEN cursor_int + 1 AND cursor_int + limite_int;

    -- Selecionar os dados finais
    SELECT cl.codigo_colaborador, cl.colaborador_cpf, cl.eh_tbd, cl.nome, cl.idiomas, cl.hard_skills, cl.gestores, cl.gestores_nome,
           ac.ativo, ac.tb_org_id, ac.cod_diretoria,
           ac.diretoria, ac.eh_tbd, ac.mes, ac.ano,
           ac.horas, ac.status_colaborador_periodo_alocacao
    FROM colaboradores_limitados cl
    JOIN alocacoes_colaboradores ac ON cl.codigo_colaborador = ac.codigo_colaborador and cl.eh_tbd = ac.eh_tbd
    JOIN colaboradores_paginados cp ON cl.codigo_colaborador = cp.codigo_colaborador  and cl.eh_tbd = cp.eh_tbd
    ORDER BY cl.nome,cl.codigo_colaborador;

    -- Limpar as tabelas temporárias
	DROP TEMPORARY TABLE IF EXISTS meses;
	DROP TEMPORARY TABLE IF EXISTS alocacoes_colaboradores;
	DROP TEMPORARY TABLE IF EXISTS colaborador_tbd_hierarquia;
	DROP TEMPORARY TABLE IF EXISTS filtrados_com_status;
	DROP TEMPORARY TABLE IF EXISTS colaboradores_limitados;
	DROP TEMPORARY TABLE IF EXISTS colaboradores_paginados;


END$$
DELIMITER ;