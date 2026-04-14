-- Adicionar coluna observacoes_internas na tabela tb_vaga
ALTER TABLE tb_vaga 
ADD COLUMN observacoes_internas TEXT NULL COMMENT 'Observações internas da vaga' 
AFTER maquina;

