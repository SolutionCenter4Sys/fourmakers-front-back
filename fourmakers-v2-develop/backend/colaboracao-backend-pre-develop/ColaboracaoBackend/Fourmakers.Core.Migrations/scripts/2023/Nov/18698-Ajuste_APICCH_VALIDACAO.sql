

ALTER TABLE `tb_diretoria` 
ADD COLUMN `id_externo` VARCHAR(45) NULL AFTER `org_id`;

UPDATE `tb_diretoria` SET `id_externo` = '15' WHERE (`id` = '1');
UPDATE `tb_diretoria` SET `id_externo` = '18' WHERE (`id` = '2');
UPDATE `tb_diretoria` SET `id_externo` = '20' WHERE (`id` = '4');
UPDATE `tb_diretoria` SET `id_externo` = '17' WHERE (`id` = '5');


UPDATE `tb_diretoria` SET `id_externo` = '18' WHERE (`id` = '3');
UPDATE `tb_diretoria` SET `id_externo` = '21' WHERE (`id` = '6');

-- DELETE FROM `tb_diretoria` WHERE (`id` = '3');
