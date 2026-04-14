UPDATE tb_parametro_configuracao tpc
SET tpc.valor_parametro = false
WHERE tpc.codigo_parametro = 'MOSTRAR_REEMBOLSO'
  AND tpc.tb_org_id IN (2, 4);