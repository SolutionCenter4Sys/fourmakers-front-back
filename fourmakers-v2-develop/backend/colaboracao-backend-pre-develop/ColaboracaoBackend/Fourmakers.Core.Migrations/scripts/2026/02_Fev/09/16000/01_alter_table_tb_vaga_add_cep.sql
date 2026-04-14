-- Query para adicionar a coluna CEP na tabela tb_vaga
-- Data: 2026-02-09
-- Descrição: Adiciona campo CEP para armazenar o CEP da vaga de recrutamento

ALTER TABLE `tb_vaga` 
ADD COLUMN `cep` VARCHAR(20) NULL DEFAULT NULL AFTER `cidade`;

