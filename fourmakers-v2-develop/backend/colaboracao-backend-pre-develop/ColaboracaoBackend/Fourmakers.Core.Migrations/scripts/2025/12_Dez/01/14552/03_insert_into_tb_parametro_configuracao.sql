INSERT INTO tb_parametro_configuracao (
    id,
    tb_org_id,
    codigo_parametro,
    valor_parametro,
    tb_parametro_nivel_id
)VALUES(
   UUID(),
   2,
   'REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO',
   'true',
   3
);