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
   'Configurar filtro no relatório de apontamento simplificado',
   'Permite configurar o filtro para considerar apenas colaboradores ativos no relatório simplificado de apontamentos',
   'CONSIDERAR_APENAS_ATIVOS_RELATORIO_APONTAMENTO_SIMPLIFICADO',
   'TIMESHEET',
   1,
   'BACKEND'
);