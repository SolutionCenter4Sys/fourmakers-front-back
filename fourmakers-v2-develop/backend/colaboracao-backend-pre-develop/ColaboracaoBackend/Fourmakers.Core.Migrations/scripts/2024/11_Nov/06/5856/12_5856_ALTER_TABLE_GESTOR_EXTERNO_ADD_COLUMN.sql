ALTER TABLE `tb_gestor_externo`
ADD COLUMN `codigo_cliente` VARCHAR(45) DEFAULT NULL;

ALTER TABLE `tb_gestor_externo`
ADD CONSTRAINT `fk_gestor_externo_codigo_cliente`
FOREIGN KEY (`codigo_cliente`) 
REFERENCES `tb_cliente_org` (`codigo_cliente`)
ON DELETE SET NULL
ON UPDATE CASCADE;