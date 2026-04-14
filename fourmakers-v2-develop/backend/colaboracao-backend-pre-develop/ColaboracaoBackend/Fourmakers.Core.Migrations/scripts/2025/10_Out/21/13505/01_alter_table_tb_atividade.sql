ALTER TABLE tb_atividade
ADD COLUMN ativo tinyint(1) DEFAULT 1;

UPDATE tb_atividade
SET ativo = 0
WHERE descricao = "PRE VENDAS (AMS)"
AND tb_org_id = 4;