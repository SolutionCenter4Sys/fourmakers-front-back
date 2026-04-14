-- Adicionar colunas de parcela a deduzir na tabela tb_inss
ALTER TABLE tb_inss
ADD COLUMN faixa_1_parcela_deduzir DECIMAL(10,2) DEFAULT 0.00 COMMENT 'Parcela a deduzir da primeira faixa',
ADD COLUMN faixa_2_parcela_deduzir DECIMAL(10,2) DEFAULT 0.00 COMMENT 'Parcela a deduzir da segunda faixa',
ADD COLUMN faixa_3_parcela_deduzir DECIMAL(10,2) DEFAULT 0.00 COMMENT 'Parcela a deduzir da terceira faixa',
ADD COLUMN faixa_4_parcela_deduzir DECIMAL(10,2) DEFAULT 0.00 COMMENT 'Parcela a deduzir da quarta faixa';

-- Atualizar os valores das parcelas a deduzir conforme tabela INSS atual
-- Faixa 1: 0 (sem parcela a deduzir)
-- Faixa 2: R$ 24,32
-- Faixa 3: R$ 111,40
-- Faixa 4: R$ 198,49
UPDATE tb_inss
SET 
    faixa_1_parcela_deduzir = 0.00,
    faixa_2_parcela_deduzir = 24.32,
    faixa_3_parcela_deduzir = 111.40,
    faixa_4_parcela_deduzir = 198.49
WHERE ativo = 1;

