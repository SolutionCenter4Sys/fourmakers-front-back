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
   'Configuração criação de reembolso sem projetos',
   'Permitir configurar a criação de reembolsos sem projeto',
   'REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO',
   'REEMBOLSO',
   1,
   'FRONTEND'
);