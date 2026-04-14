-- Inserção na tabela select * from tb_parametro
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
    'Configuração disponibilidade de aderencia',
    'Permite definir valores de disponibilidade entre datas',
    'CONFIGURACAO_PERCENTUAL_MATCH_REVERSO',
    'MAPA_DE_ALOCACAO',
    1, 
    'BACKEND'
);

-- Inserção na tabela tb_parametro_configuracao
INSERT INTO tb_parametro_configuracao (
    id,
    tb_org_id,
    codigo_parametro,
    valor_parametro,
    tb_parametro_nivel_id
)
VALUES (
    UUID(), 
    2, 
    'CONFIGURACAO_PERCENTUAL_MATCH_REVERSO',
    'perc_max_skills_relevantes=44;perc_max_skills_desejaveis=24;perc_max_localidade_colaborador_modelo_trabalho=14;perc_max_custos_perfil_colaborador=18',
    3
);