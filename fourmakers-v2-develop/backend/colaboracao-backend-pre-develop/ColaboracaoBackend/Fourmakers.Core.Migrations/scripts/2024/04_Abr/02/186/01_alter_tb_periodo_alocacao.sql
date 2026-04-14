ALTER TABLE `tb_periodo_alocacao` 
ADD COLUMN `observacao` TEXT NULL AFTER `data_alteracao`,
ADD COLUMN `oportunidade` TEXT NULL AFTER `observacao`,
ADD COLUMN `prioritario` TINYINT NULL DEFAULT '0' AFTER `oportunidade`,
ADD COLUMN `percentual` DOUBLE NULL AFTER `prioritario`;