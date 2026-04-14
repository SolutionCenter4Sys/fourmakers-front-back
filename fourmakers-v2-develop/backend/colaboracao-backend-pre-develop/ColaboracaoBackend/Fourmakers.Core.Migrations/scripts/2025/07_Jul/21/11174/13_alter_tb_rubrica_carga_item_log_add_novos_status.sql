-- Alterar ENUM status_processamento para incluir novos status
ALTER TABLE tb_rubrica_carga_item_log 
MODIFY COLUMN status_processamento ENUM(
    'sucesso', 
    'erro_colaborador_nao_encontrado', 
    'erro_salvamento',
    'erro_colaborador_diretoria_diferente',
    'erro_colaborador_nao_existe_na_org',
    'erro_tipo_identificacao_desconhecido',
    'erro_processamento_geral'
) NOT NULL;

-- Alterar ENUM tipo_identificacao para incluir novos tipos
ALTER TABLE tb_rubrica_carga_item_log 
MODIFY COLUMN tipo_identificacao ENUM(
    'codigo_alternativo', 
    'cpf_colaborador', 
    'nao_encontrado',
    'colaborador_diretoria_diferente',
    'colaborador_nao_existe_na_org',
    'erro_geral'
) NULL COMMENT 'Como o colaborador foi identificado ou tipo de erro';