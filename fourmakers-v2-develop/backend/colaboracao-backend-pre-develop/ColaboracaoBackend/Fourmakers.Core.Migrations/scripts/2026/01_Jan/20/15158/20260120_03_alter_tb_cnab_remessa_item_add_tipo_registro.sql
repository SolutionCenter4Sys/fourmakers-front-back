-- Adicionar coluna tipo_registro na tb_cnab_remessa_item para identificar cada linha

ALTER TABLE tb_cnab_remessa_item
ADD COLUMN tipo_registro VARCHAR(50) NULL COMMENT 'Tipo do registro: HEADER_ARQUIVO, HEADER_LOTE, DETALHE_SEGMENTO_A, DETALHE_SEGMENTO_B, TRAILER_LOTE, TRAILER_ARQUIVO';

-- Adicionar índice para buscar por tipo
CREATE INDEX idx_remessa_item_tipo ON tb_cnab_remessa_item(tipo_registro);
