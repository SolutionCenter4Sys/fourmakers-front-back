-- ####################################################################
-- CRIAÇÃO DA TABELA tb_equipamento_padrao_cargo_funcao
-- Dicionário de Cargos e Funções para a padronização de equipamentos
-- ####################################################################

CREATE TABLE IF NOT EXISTS tb_equipamento_padrao_cargo_funcao (
    id CHAR(36) NOT NULL PRIMARY KEY COMMENT 'ID único (UUID)',
    nome_cargo_funcao VARCHAR(100) NOT NULL UNIQUE COMMENT 'Nome da Função ou Stack',
    tb_equipamento_padrao_grupo_area_id CHAR(36) NOT NULL COMMENT 'FK para o grupo de área (Admin/Dev)',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_cargo_funcao_grupo_area
        FOREIGN KEY (tb_equipamento_padrao_grupo_area_id) 
        REFERENCES tb_equipamento_padrao_grupo_area(id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB COMMENT='Dicionário de Cargos e Funções para a padronização de equipamentos.';
