-- Passo 1: Remover a chave UNIQUE atual
ALTER TABLE `tb_cliente_org`
DROP INDEX `UQ_nome_cliente_tb_org_id`;

-- Passo 2: Adicionar uma nova chave UNIQUE composta considerando o campo ativo
ALTER TABLE `tb_cliente_org`
ADD UNIQUE KEY `UQ_nome_cliente_ativo` (`nome_cliente`, `tb_org_id`, `ativo`);
