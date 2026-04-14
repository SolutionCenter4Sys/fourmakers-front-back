


ALTER TABLE tb_cliente_org
DROP INDEX UQ_nome_cliente,
ADD UNIQUE KEY UQ_nome_cliente_tb_org_id (nome_cliente, tb_org_id);