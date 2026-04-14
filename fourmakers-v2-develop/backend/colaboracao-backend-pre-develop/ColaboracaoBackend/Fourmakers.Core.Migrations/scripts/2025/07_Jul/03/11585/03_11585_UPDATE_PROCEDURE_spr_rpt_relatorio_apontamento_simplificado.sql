DROP PROCEDURE IF EXISTS spr_rpt_relatorio_apontamento_simplificado;

DELIMITER //
          
CREATE PROCEDURE spr_rpt_relatorio_apontamento_simplificado(
    IN mes INT,
    IN ano INT,
    IN org_id INT,
    IN codigo_interno_colaborador_gerente VARCHAR(64) -- Adicionando o parâmetro cpf_gerente
)
BEGIN
DECLARE v_valor_parametro VARCHAR(255);
    DECLARE v_sql TEXT;

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

-- Insere os dados na tabela temporária quando o cpf_gerente não é passado
SELECT
    CONCAT(LOWER(CONVERT(fn_get_dia_semana(tca.data), CHAR)), ' ', CONVERT(DATE_FORMAT(tca.data, '%d/%m'), CHAR)) AS Data,
    tc.nome_completo AS Nome,
    tco.cod_colaborador_externo AS "Matrícula",
    ta.descricao AS Tarefa,
    CASE WHEN
             tpo.projeto IS NULL OR tpo.projeto = '-'
             THEN
             tpo.cod_projeto
         ELSE
             CONCAT(tpo.cod_projeto, ' - ', tpo.projeto)
        END AS Projeto,
    TIME_FORMAT(SEC_TO_TIME(tca.horas * 60), '%H:%i') AS Horas,
    tca.observacao AS "Resumo das atividades",
    tc_aprovador.nome_completo AS Aprovador,
    REPLACE(CONVERT(ROUND((tca.horas / 60), 2), CHAR), '.', ',') AS "Hora (Decimal)",
    tsag.descricao as Status
FROM
    tb_colaborador_apontamento tca
        JOIN tb_colaborador tc ON tca.codigo_interno_colaborador = tc.codigo_interno_colaborador
        JOIN tb_colaborador_org tco ON tca.codigo_interno_colaborador = tco.codigo_interno_colaborador AND tca.tb_org_id = tco.tb_org_id
        JOIN tb_projeto_org tpo ON tca.tb_projeto_org_cod_projeto = tpo.cod_projeto AND tca.tb_org_id = tpo.tb_org_id
        JOIN tb_atividade ta ON tca.tb_atividade_id = ta.id
        JOIN tb_status_apontamento tsa ON tca.tb_status_apontamento_id = tsa.id
        JOIN tb_status_apontamento_grupo tsag ON tsa.tb_cod_status_grupo = tsag.cod_status_grupo
        LEFT JOIN tb_colaborador tc_aprovador ON tca.codigo_interno_colaborador_justificativa = tc_aprovador.codigo_interno_colaborador and tsa.tb_cod_status_grupo = 2 -- aprovado
        LEFT JOIN tb_parametro_configuracao tpc ON tpc.valor_parametro = tco.modelo_contratacao AND tpc.tb_org_id = tco.tb_org_id AND tpc.codigo_parametro = 'OCULTAR_CONSULTAS_TIMESHEET_MODELO_CONTRATACAO'
WHERE
    tca.tb_org_id = org_id -- and tco.ativo = 1
        AND (tsa.tb_cod_status_grupo = 1 OR tsa.tb_cod_status_grupo = 2)-- grupo status aprovacao
        AND MONTH(tca.data) = mes
  AND YEAR(tca.data) = ano
  AND tpc.id is null
ORDER BY
    tca.data, tc.nome_completo;
COMMIT;
END //

DELIMITER ;

