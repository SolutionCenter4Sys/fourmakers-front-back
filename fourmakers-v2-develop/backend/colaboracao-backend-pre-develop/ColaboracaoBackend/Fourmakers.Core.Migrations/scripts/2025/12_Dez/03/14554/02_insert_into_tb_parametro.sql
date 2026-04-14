INSERT INTO tb_parametro (
    id,
    nome_parametro,
    descricao_parametro,
    codigo_parametro,
    codigo_modulo_sistema,
    ativo,
    tipo_parametro
)
VALUES (
           UUID(),
           'Configurar workflow de aprovação reembolso',
           'Permitir configurar a etapa de aprovação em reembolso',
           'REEMSOLBO_WORKFLOW_APROVACAO_CFO',
           'REEMBOLSO',
           1,
           'FRONTEND'
       );