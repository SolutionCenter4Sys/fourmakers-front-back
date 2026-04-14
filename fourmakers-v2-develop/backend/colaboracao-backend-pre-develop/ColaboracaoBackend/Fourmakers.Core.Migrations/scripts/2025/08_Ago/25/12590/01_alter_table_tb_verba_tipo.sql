ALTER TABLE tb_verba_tipo
    ADD COLUMN  exigir_comprovante tinyint(1) NOT NULL DEFAULT 1;

UPDATE tb_verba_tipo
SET exigir_comprovante = 0
WHERE descricao = 'Crédito'
   OR descricao = 'Debito';