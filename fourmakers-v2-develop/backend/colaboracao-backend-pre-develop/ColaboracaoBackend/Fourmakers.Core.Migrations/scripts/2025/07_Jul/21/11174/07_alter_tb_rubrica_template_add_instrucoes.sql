-- Adicionar coluna instrucoes_adicionais na tabela tb_rubrica_template
ALTER TABLE tb_rubrica_template 
ADD COLUMN instrucoes_adicionais TEXT NULL 
COMMENT 'Instruções específicas para este template sobre como extrair dados (ex: qual coluna usar para valores)';

-- Exemplo de atualização para o template ID 1
UPDATE tb_rubrica_template 
SET instrucoes_adicionais = 'Atenção: o valor monetário está na coluna "Vlr. à Rec.". Ignore outras colunas numéricas.'
WHERE id = 1;