DROP PROCEDURE `spr_get_alocacao_colaborador_e_tbd`;
DELIMITER //
CREATE PROCEDURE `spr_get_alocacao_colaborador_e_tbd`(
    IN p_pesquisa VARCHAR(255),
    IN p_org_id INT,
    IN p_periodo_alocado_id INT,
    IN p_codigo_diretoria VARCHAR(255),
    IN p_codigo_departamento VARCHAR(255),
    IN p_codigo_gestor_adm VARCHAR(255),
    IN p_lista_codigo_colab_ou_tbd VARCHAR(1000),
    IN p_filtro_tipo_profissional INT,
    IN p_codigo_gestor_projeto VARCHAR(255),
    IN p_lista_codigo_clientes VARCHAR(1000),
    IN p_apenas_projetos_prioritarios BOOL,
    IN p_lista_codigo_projetos VARCHAR(1000),
    IN p_codigo_status_projeto INT,
    IN p_qtd_gerente_projeto_prioridade INT,
    IN p_incluir_inativos BOOL
)
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE current_item VARCHAR(255);
    DECLARE split_index INT DEFAULT 1;

	DROP TEMPORARY TABLE IF EXISTS temp_clientes;
	DROP TEMPORARY TABLE IF EXISTS temp_projetos;    
    DROP TEMPORARY TABLE IF EXISTS temp_colab_ou_tbd;

    -- Criar tabela temporária para clientes
    CREATE TEMPORARY TABLE temp_clientes (
        cliente_codigo VARCHAR(255)
    );
    
    -- Criar tabela temporária para projetos
    CREATE TEMPORARY TABLE temp_projetos (
        projeto_codigo VARCHAR(255)
    );
    
    -- Criar tabela temporária para colaboradores ou TBDs
    CREATE TEMPORARY TABLE temp_colab_ou_tbd (
        colab_ou_tbd_codigo VARCHAR(255)
    );
    
    -- Usar a função para preencher as tabelas temporárias
    CALL spr_fn_inserir_string_com_split(p_lista_codigo_clientes, 'temp_clientes', 'cliente_codigo',',');
    CALL spr_fn_inserir_string_com_split(p_lista_codigo_projetos, 'temp_projetos', 'projeto_codigo',',');
    CALL spr_fn_inserir_string_com_split(p_lista_codigo_colab_ou_tbd, 'temp_colab_ou_tbd', 'colab_ou_tbd_codigo',',');
    

    SELECT 
    * 
    FROM (
        SELECT 
            tcpa.id AS periodo_alocado_id,
            tco.cod_diretoria,
            tco.diretoria,
            CASE 
                WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN ttbd.cod_departamento
                ELSE tco.cod_departamento
            END AS cod_departamento,
            CASE 
                WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN ttbd.departamento
                ELSE tco.departamento
            END AS departamento,
            CASE 
                WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN tcpa.cod_tbd_alocado
                ELSE tcpa.codigo_colaborador
            END AS codigo_colaborador,
            CASE 
                WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN ttbd.descricao
                ELSE tc.nome_completo
            END AS nome_completo,
            vgct_gestor.nome_gestor_adm,
            tcpa.codigo_interno_colaborador,
            tcpa.codigo_projeto,
            tpo.projeto,
            tpo.cod_cliente,
            tpo.cliente,
            tpo.status,
            tpg.cod_colaborador_gerente AS cod_gerente,
            tg.nome_completo AS nome_gerente,
            DATE_FORMAT(tcpa.data_inicio, '%d/%m/%Y') AS data_inicio,
            DATE_FORMAT(tcpa.data_fim, '%d/%m/%Y') AS data_fim,
            tcpa.quantidade_horas as quantidade_horas__ND,
            REPLACE(CAST(tcpa.quantidade_horas AS CHAR), '.', ',') AS quantidade_horas,
            tcpa.percentual,
            CASE 
                WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1
                ELSE 0
            END AS tbd,
            tcpa.observacao,
            tcpa.oportunidade,
            tcpa.prioritario,
            ROW_NUMBER() OVER (PARTITION BY tcpa.id ORDER BY tpg.cod_colaborador_gerente) AS gerente_projeto_prioridade__ND
        FROM 
            tb_colaborador_periodo_alocacao tcpa
        LEFT JOIN
            tb_colaborador_org tco ON tcpa.codigo_colaborador = tco.cod_colaborador_externo AND tco.tb_org_id = tcpa.tb_org_id
        LEFT JOIN
            tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador 
        LEFT JOIN
            tb_tbd_alocado ttbd ON tcpa.cod_tbd_alocado = ttbd.cod_tbd_alocado AND tcpa.tb_org_id = ttbd.tb_org_id
        LEFT JOIN
            vw_gestores_colaborador_tbd vgct_gestor ON (((CASE WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1 ELSE 0 END) = vgct_gestor.eh_tbd AND ttbd.cod_tbd_alocado = vgct_gestor.cod_colaborador_externo) 
                OR ((CASE WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1 ELSE 0 END) = vgct_gestor.eh_tbd AND tco.cod_colaborador_externo = vgct_gestor.cod_colaborador_externo))
                AND tcpa.tb_org_id = vgct_gestor.tb_org_id
        LEFT JOIN
            tb_projeto_org tpo ON tcpa.codigo_projeto = tpo.cod_projeto AND tpo.tb_org_id = tcpa.tb_org_id
        LEFT JOIN 
            tb_projeto_gerente tpg ON tpo.cod_projeto = tpg.cod_projeto AND tpg.tb_org_id = tcpa.tb_org_id
        LEFT JOIN
            tb_colaborador_org tco_g ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo AND tco_g.tb_org_id = tcpa.tb_org_id
        LEFT JOIN
            tb_colaborador tg ON tco_g.codigo_interno_colaborador = tg.codigo_interno_colaborador
        LEFT JOIN 
            temp_colab_ou_tbd temp ON (tcpa.cod_tbd_alocado = temp.colab_ou_tbd_codigo OR tco.cod_colaborador_externo = temp.colab_ou_tbd_codigo)
        WHERE
            -- Trazendo apenas ativos
            tcpa.ativo = 1 
            AND
            -- Filtrando apenas usuarios ativos
            (COALESCE(tcpa.cod_tbd_alocado,'')<>'' OR (tco.ativo = 1 OR (tco.ativo = 0 AND p_incluir_inativos = true)))
            AND
            -- Filtrando pelo parâmetro: org Id
            tcpa.tb_org_id = p_org_id 
            AND
            -- Filtrando pelo parâmetro: periodo alocado
            (p_periodo_alocado_id IS NULL OR tcpa.id = p_periodo_alocado_id) 
            AND
            -- Filtrando pelo parâmetro: pesquisa
            (p_pesquisa IS NULL OR
            (tcpa.cod_tbd_alocado IS NOT NULL AND tcpa.cod_tbd_alocado = p_pesquisa) OR
            (tcpa.cod_tbd_alocado IS NULL AND tcpa.codigo_colaborador = p_pesquisa) OR
            (tcpa.cod_tbd_alocado IS NOT NULL AND ttbd.descricao LIKE CONCAT('%', p_pesquisa, '%')) OR
            (tcpa.cod_tbd_alocado IS NULL AND tc.nome_completo LIKE CONCAT('%', p_pesquisa, '%')) OR
            (tcpa.cod_tbd_alocado IS NULL AND tpo.projeto LIKE CONCAT('%', p_pesquisa, '%')) OR
            tpo.cod_cliente = p_pesquisa OR
            tcpa.codigo_projeto = p_pesquisa OR
            tpo.cliente LIKE CONCAT('%', p_pesquisa, '%') OR
            tpg.cod_colaborador_gerente = p_pesquisa OR
            tg.nome_completo LIKE CONCAT('%', p_pesquisa, '%'))
            AND
            -- Filtrando pelo parâmetro: p_codigo_diretoria
            (p_codigo_diretoria IS NULL OR tco.cod_diretoria = p_codigo_diretoria)
            AND
            -- Filtrando pelo parâmetro: p_codigo_departamento
            (p_codigo_departamento IS NULL OR ((tcpa.cod_tbd_alocado IS NOT NULL AND ttbd.cod_departamento = p_codigo_departamento) OR (tcpa.cod_tbd_alocado IS NULL AND tco.cod_departamento = p_codigo_departamento)))
            AND
			-- Filtrando pelo parâmetro: p_codigo_gestor_adm
            (
                p_codigo_gestor_adm IS NULL 
                OR 
                (
                    vgct_gestor.cod_colaborador_superior = p_codigo_gestor_adm
                )
            )
            AND (
                p_lista_codigo_colab_ou_tbd IS NULL 
                OR (
                    temp.colab_ou_tbd_codigo IS NOT NULL 
                    AND (
                        (tcpa.cod_tbd_alocado IS NOT NULL AND tcpa.cod_tbd_alocado = temp.colab_ou_tbd_codigo) 
                        OR 
                        (tcpa.cod_tbd_alocado IS NULL AND tco.cod_colaborador_externo = temp.colab_ou_tbd_codigo)
                    )
                )
            )
            AND
-- Filtrando pelo parâmetro: p_filtro_tipo_profissional
            (
                (p_filtro_tipo_profissional = 0 OR p_filtro_tipo_profissional IS NULL)
                OR 
                (p_filtro_tipo_profissional = 1 AND tcpa.cod_tbd_alocado IS NULL)
                OR 
                (p_filtro_tipo_profissional = 2 AND tcpa.cod_tbd_alocado IS NOT NULL)
            )
            AND
            -- Filtrando pelo parâmetro: p_codigo_gestor_projeto
            (p_codigo_gestor_projeto IS NULL OR (tpg.cod_colaborador_gerente = p_codigo_gestor_projeto))
            AND 
            -- Filtrando pelo parâmetro: p_apenas_projetos_prioritarios
            ((p_apenas_projetos_prioritarios IS NULL OR p_apenas_projetos_prioritarios = false) OR (tpo.prioritario = p_apenas_projetos_prioritarios))
            AND
            -- Filtrando pelo parâmetro: p_lista_codigo_clientes
            (p_lista_codigo_clientes IS NULL OR (tpo.cod_cliente IN (SELECT cliente_codigo FROM temp_clientes)))
            AND
            -- Filtrando pelo parâmetro: p_lista_codigo_projetos
            (p_lista_codigo_projetos IS NULL OR (tpo.cod_projeto IN (SELECT projeto_codigo FROM temp_projetos)))
            AND
			-- Filtrando pelo parâmetro: p_codigo_status_projeto
            (p_codigo_status_projeto IS NULL OR tpo.cod_status = p_codigo_status_projeto)
			ORDER BY
                (CASE 
                    WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN ttbd.descricao
                    ELSE tc.nome_completo
                END), tcpa.data_fim
			
     ) AS subquery
	 WHERE
	 COALESCE(p_qtd_gerente_projeto_prioridade,0) = 0 OR subquery.gerente_projeto_prioridade__ND = p_qtd_gerente_projeto_prioridade;
	
	-- DROP TEMPORARY TABLE IF EXISTS temp_clientes;
	-- DROP TEMPORARY TABLE IF EXISTS temp_projetos;    
    -- DROP TEMPORARY TABLE IF EXISTS temp_colab_ou_tbd;

END
//
DELIMITER ;