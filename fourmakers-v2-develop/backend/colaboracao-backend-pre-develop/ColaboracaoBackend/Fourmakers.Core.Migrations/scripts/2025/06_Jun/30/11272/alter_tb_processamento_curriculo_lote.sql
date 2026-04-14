ALTER TABLE `tb_processamento_curriculo_lote`
ADD COLUMN `identificador_fila` VARCHAR(50) DEFAULT NULL;

UPDATE `tb_processamento_curriculo_lote`
SET `identificador_fila` = 'FILA_CURRICULO_ARQUIVO'
WHERE `identificador_fila` IS NULL;
