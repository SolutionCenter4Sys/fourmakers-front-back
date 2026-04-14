-- Alterar tabela tb_template_descricao_vaga para suportar texto_introducao e texto_finalizacao
-- Remover coluna descricao e adicionar novas colunas

ALTER TABLE tb_template_descricao_vaga
DROP COLUMN descricao,
ADD COLUMN texto_introducao LONGTEXT NOT NULL,
ADD COLUMN texto_finalizacao LONGTEXT NOT NULL;
