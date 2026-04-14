-- Adicionar colunas na tb_cnab_retorno_item para controle de ocorrências e tipo de registro

ALTER TABLE tb_cnab_retorno_item
ADD COLUMN codigo_ocorrencia VARCHAR(2) NULL COMMENT 'Código da ocorrência retornada (posição 230-231 do Segmento A)',
ADD COLUMN tipo_registro VARCHAR(50) NULL COMMENT 'Tipo do registro: HEADER_ARQUIVO, HEADER_LOTE, DETALHE_SEGMENTO_A, DETALHE_SEGMENTO_B, TRAILER_LOTE, TRAILER_ARQUIVO';

-- Adicionar índice para buscar por ocorrência
CREATE INDEX idx_retorno_item_ocorrencia ON tb_cnab_retorno_item(codigo_ocorrencia);

-- Comentários nas colunas
ALTER TABLE tb_cnab_retorno_item
MODIFY COLUMN codigo_ocorrencia VARCHAR(2) NULL COMMENT 'Código da ocorrência retornada (ex: 00=PAGO, BD=AGENDADO)',
MODIFY COLUMN tipo_registro VARCHAR(50) NULL COMMENT 'Tipo: HEADER_ARQUIVO, HEADER_LOTE, DETALHE_SEGMENTO_A, DETALHE_SEGMENTO_B, TRAILER_LOTE, TRAILER_ARQUIVO';
