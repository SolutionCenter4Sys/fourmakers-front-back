-- ####################################################################
-- CRIAÇÃO DA TABELA tb_equipamento_padrao_mapeamento
-- Mapeamento central da padronização de equipamentos por cargo e categoria
-- ####################################################################

CREATE TABLE IF NOT EXISTS tb_equipamento_padrao_mapeamento (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'PK (UUID) da tabela de mapeamento',
    tb_equipamento_padrao_cargo_funcao_id CHAR(36) NOT NULL COMMENT 'FK para tb_equipamento_padrao_cargo_funcao.id',
    tb_equipamento_padrao_categoria_requisito_id CHAR(36) NOT NULL COMMENT 'FK para tb_equipamento_padrao_categoria_requisito.id',
    tb_equipamento_padrao_especificacao_id CHAR(36) NOT NULL COMMENT 'FK para tb_equipamento_padrao_especificacao.id',
    tb_equipamento_padrao_catalogo_modelo_id_aprovado_1 CHAR(36) NULL COMMENT 'FK para o primeiro modelo aprovado',
    tb_equipamento_padrao_catalogo_modelo_id_aprovado_2 CHAR(36) NULL COMMENT 'FK para o segundo modelo aprovado',
    tb_equipamento_padrao_catalogo_modelo_id_aprovado_3 CHAR(36) NULL COMMENT 'FK para o terceiro modelo aprovado',
    tb_equipamento_padrao_catalogo_modelo_id_aprovado_4 CHAR(36) NULL COMMENT 'FK para o quarto modelo aprovado',
    tb_equipamento_padrao_upgrade_customizado_id CHAR(36) NULL COMMENT 'FK para tb_equipamento_padrao_upgrade_customizado.id',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_map_cargo_funcao
        FOREIGN KEY (tb_equipamento_padrao_cargo_funcao_id) 
        REFERENCES tb_equipamento_padrao_cargo_funcao(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_map_categoria_requisito
        FOREIGN KEY (tb_equipamento_padrao_categoria_requisito_id) 
        REFERENCES tb_equipamento_padrao_categoria_requisito(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_map_especificacao
        FOREIGN KEY (tb_equipamento_padrao_especificacao_id) 
        REFERENCES tb_equipamento_padrao_especificacao(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_map_modelo_aprovado_1
        FOREIGN KEY (tb_equipamento_padrao_catalogo_modelo_id_aprovado_1) 
        REFERENCES tb_equipamento_padrao_catalogo_modelo(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_map_modelo_aprovado_2
        FOREIGN KEY (tb_equipamento_padrao_catalogo_modelo_id_aprovado_2) 
        REFERENCES tb_equipamento_padrao_catalogo_modelo(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_map_modelo_aprovado_3
        FOREIGN KEY (tb_equipamento_padrao_catalogo_modelo_id_aprovado_3) 
        REFERENCES tb_equipamento_padrao_catalogo_modelo(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_map_modelo_aprovado_4
        FOREIGN KEY (tb_equipamento_padrao_catalogo_modelo_id_aprovado_4) 
        REFERENCES tb_equipamento_padrao_catalogo_modelo(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_map_upgrade_customizado
        FOREIGN KEY (tb_equipamento_padrao_upgrade_customizado_id) 
        REFERENCES tb_equipamento_padrao_upgrade_customizado(id) ON DELETE RESTRICT ON UPDATE CASCADE
        
) ENGINE=InnoDB COMMENT='Mapeamento central da padronização de equipamentos por cargo e categoria.';
