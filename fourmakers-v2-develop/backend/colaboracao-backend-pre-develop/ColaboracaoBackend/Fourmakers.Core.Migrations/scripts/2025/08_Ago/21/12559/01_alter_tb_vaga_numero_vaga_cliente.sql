-- Adicionar campo numero_vaga_cliente na tabela tb_vaga
ALTER TABLE tb_vaga ADD COLUMN numero_vaga_cliente VARCHAR(255) NULL;

-- Adicionar campo tb_cliente_org_codigo_fourmakers na tabela tb_vaga
ALTER TABLE tb_vaga ADD COLUMN tb_cliente_org_codigo_fourmakers INT NULL;
