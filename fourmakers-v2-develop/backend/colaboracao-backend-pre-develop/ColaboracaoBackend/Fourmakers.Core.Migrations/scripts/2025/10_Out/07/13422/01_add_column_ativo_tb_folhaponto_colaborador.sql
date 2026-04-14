ALTER TABLE tb_folhaponto_colaborador
ADD COLUMN ativo TINYINT NOT NULL DEFAULT 0;

-- Atualizar ativo = 1 apenas para o registro mais recente de cada competência e CNPJ onde aprovado = 1
UPDATE tb_conciliacao_folhaponto t1
INNER JOIN (
  SELECT competencia, cnpj, MAX(data_aprovacao) as max_data_aprovacao
  FROM tb_conciliacao_folhaponto
  WHERE aprovado = 1
  GROUP BY competencia, cnpj
) t2 ON t1.competencia = t2.competencia 
    AND t1.cnpj = t2.cnpj 
    AND t1.data_aprovacao = t2.max_data_aprovacao
SET t1.ativo = 1
WHERE t1.aprovado = 1;

-- Atualizar ativo = 1 para o registro mais recente (por data_criacao do lote) de cada competência e CNPJ onde aprovado = 0
UPDATE tb_conciliacao_folhaponto tcf
INNER JOIN tb_lote l ON tcf.tb_lote_id = l.id
INNER JOIN (
  SELECT tcf2.competencia, tcf2.cnpj, MAX(l2.data_criacao) as max_data_criacao
  FROM tb_conciliacao_folhaponto tcf2
  INNER JOIN tb_lote l2 ON tcf2.tb_lote_id = l2.id
  WHERE tcf2.aprovado = 0
  GROUP BY tcf2.competencia, tcf2.cnpj
) t2 ON tcf.competencia = t2.competencia 
    AND tcf.cnpj = t2.cnpj 
    AND l.data_criacao = t2.max_data_criacao
SET tcf.ativo = 1
WHERE tcf.aprovado = 0;

UPDATE tb_conciliacao_folhaponto t1
INNER JOIN (
  SELECT competencia, cnpj, MAX(data_aprovacao) as max_data_aprovacao
  FROM tb_conciliacao_folhaponto
  WHERE aprovado = 1
  GROUP BY competencia, cnpj
) t2 ON t1.competencia = t2.competencia 
    AND t1.cnpj = t2.cnpj 
set t1.ativo  = 0 
where t1.aprovado = 0;


