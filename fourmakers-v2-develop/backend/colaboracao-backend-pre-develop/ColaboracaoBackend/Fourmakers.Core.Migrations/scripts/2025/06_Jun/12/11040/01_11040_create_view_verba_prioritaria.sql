CREATE OR REPLACE VIEW vw_verba_personalizada_prioritaria AS
SELECT
    tb_verba_personalizada.id AS id,
    tb_verba_personalizada.tb_verba_id AS tb_verba_id,
    tb_verba_personalizada.tb_org_id AS tb_org_id,
    tb_verba_personalizada.cliente_id AS cliente_id,
    tb_verba_personalizada.projeto_id AS projeto_id,
    tb_verba_personalizada.codigo_interno_colaborador AS codigo_interno_colaborador,
    tb_verba_personalizada.valor_customizado AS valor_customizado,
    tb_verba_personalizada.custo_cliente AS custo_cliente,
    tb_verba_personalizada.ativo AS ativo,
    tb_verba_personalizada.deletado AS deletado,
    tb_verba_personalizada.desativado_em AS desativado_em,
    tb_verba_personalizada.data_criacao AS data_criacao,
    CASE
        WHEN (tb_verba_personalizada.projeto_id IS NOT NULL AND
              tb_verba_personalizada.codigo_interno_colaborador IS NOT NULL AND
              tb_verba_personalizada.cliente_id IS NULL) THEN 1
        WHEN (tb_verba_personalizada.projeto_id IS NOT NULL AND
              tb_verba_personalizada.codigo_interno_colaborador IS NULL AND
              tb_verba_personalizada.cliente_id IS NULL) THEN 2
        WHEN (tb_verba_personalizada.cliente_id IS NOT NULL AND
              tb_verba_personalizada.codigo_interno_colaborador IS NOT NULL AND
              tb_verba_personalizada.projeto_id IS NULL) THEN 3
        WHEN (tb_verba_personalizada.cliente_id IS NOT NULL AND
              tb_verba_personalizada.codigo_interno_colaborador IS NULL AND
              tb_verba_personalizada.projeto_id IS NULL) THEN 4
        WHEN (tb_verba_personalizada.codigo_interno_colaborador IS NOT NULL AND
              tb_verba_personalizada.cliente_id IS NULL AND
              tb_verba_personalizada.projeto_id IS NULL) THEN 5
        WHEN (tb_verba_personalizada.codigo_interno_colaborador IS NULL AND
              tb_verba_personalizada.cliente_id IS NULL AND
              tb_verba_personalizada.projeto_id IS NULL) THEN 6
        ELSE 7
    END AS prioridade
FROM
    tb_verba_personalizada
WHERE
    tb_verba_personalizada.ativo = 1
    AND tb_verba_personalizada.deletado = 0;