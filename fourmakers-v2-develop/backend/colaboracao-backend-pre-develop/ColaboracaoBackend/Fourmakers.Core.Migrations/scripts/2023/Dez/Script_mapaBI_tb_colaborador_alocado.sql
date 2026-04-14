ALTER TABLE `tb_colaborador_alocado` 
ADD COLUMN `codigo_colaborador` BIGINT NULL AFTER `id`,
ADD COLUMN `nome_colaborador` VARCHAR(120) NULL AFTER `codigo_colaborador`,
ADD COLUMN `email_gestor` VARCHAR(255) NULL AFTER `nome_gestor`;

-- Rollback
ALTER TABLE `tb_colaborador_alocado` 
DROP COLUMN `email_gestor`,
DROP COLUMN `nome_colaborador`,
DROP COLUMN `codigo_colaborador`;


