-- Alteração da tabela tb_conciliacao_folhaponto_divergencia
-- Ticket: 11566
-- Data: 2025-09-03
-- Descrição: Atualizar tabela para suportar novos campos do ItemAnaliseDTO

-- 1. Tornar campo_divergencia nullable
ALTER TABLE tb_conciliacao_folhaponto_divergencia 
MODIFY COLUMN campo_divergencia VARCHAR(255) NULL;

-- 2. Adicionar coluna status (enum: DIVERGENTE, VERIFICADO, INFORMATIVO)
ALTER TABLE tb_conciliacao_folhaponto_divergencia 
ADD COLUMN status VARCHAR(20) NOT NULL DEFAULT 'DIVERGENTE' 
COMMENT 'Status do item analisado: DIVERGENTE, VERIFICADO, INFORMATIVO';

-- 3. Adicionar coluna regra
ALTER TABLE tb_conciliacao_folhaponto_divergencia 
ADD COLUMN regra VARCHAR(500) NULL 
COMMENT 'Regra aplicada na análise';

-- 4. Adicionar coluna formula (para cálculo detalhado)
ALTER TABLE tb_conciliacao_folhaponto_divergencia 
ADD COLUMN formula VARCHAR(500) NULL 
COMMENT 'Fórmula utilizada no cálculo';

-- 5. Adicionar coluna passos (JSON para armazenar array de strings)
ALTER TABLE tb_conciliacao_folhaponto_divergencia 
ADD COLUMN passos JSON NULL 
COMMENT 'Lista de passos do cálculo em formato JSON';

-- 6. Adicionar coluna variaveis (JSON para armazenar array de strings no formato "chave: valor")
ALTER TABLE tb_conciliacao_folhaponto_divergencia 
ADD COLUMN variaveis JSON NULL 
COMMENT 'Lista de variáveis utilizadas no cálculo em formato JSON (chave: valor)';

-- 7. Adicionar coluna booleana para indicar se é divergência
ALTER TABLE tb_conciliacao_folhaponto_divergencia 
ADD COLUMN eh_divergencia TINYINT(1) NOT NULL DEFAULT 1 
COMMENT 'Indica se este item representa uma divergência (1) ou não (0)';


-- 10. Atualizar registros existentes para definir eh_divergencia baseado no status
-- Por padrão, todos os registros existentes são divergências
UPDATE tb_conciliacao_folhaponto_divergencia 
SET eh_divergencia = 1, status = 'DIVERGENTE' 
WHERE eh_divergencia IS NULL OR status IS NULL;

-- 11. Adicionar índices para melhorar performance
CREATE INDEX idx_tb_conciliacao_folhaponto_divergencia_status 
ON tb_conciliacao_folhaponto_divergencia(status);

CREATE INDEX idx_tb_conciliacao_folhaponto_divergencia_eh_divergencia 
ON tb_conciliacao_folhaponto_divergencia(eh_divergencia);


