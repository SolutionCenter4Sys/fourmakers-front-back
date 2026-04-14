
-- Alteração da tabela tb_conciliacao_folhaponto
-- Ticket: 12270
-- Data: 2025-09-22
-- Descrição: Adicionar colunas aprovado e data_aprovacao para controlar status de aprovação da conciliação

-- Adicionar coluna aprovado (tinyint com valor padrão 0)
ALTER TABLE tb_conciliacao_folhaponto 
ADD COLUMN aprovado TINYINT(1) NOT NULL DEFAULT 0 
COMMENT 'Indica se a conciliação foi aprovada (1) ou não (0)';

-- Adicionar coluna data_aprovacao (timestamp)
ALTER TABLE tb_conciliacao_folhaponto 
ADD COLUMN data_aprovacao TIMESTAMP NULL 
COMMENT 'Data e hora da aprovação da conciliação';


