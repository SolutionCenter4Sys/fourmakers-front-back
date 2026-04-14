ALTER TABLE `tb_cliente_org`
ADD COLUMN `codigo_interno_colaborador_criacao` varchar(36) DEFAULT NULL,
ADD COLUMN `codigo_interno_colaborador_alteracao` varchar(36) DEFAULT NULL;

ALTER TABLE `tb_cliente_org`
ADD CONSTRAINT 
  FOREIGN KEY (`codigo_interno_colaborador_criacao`) 
  REFERENCES `tb_usuario` (`codigo_interno_colaborador`),
ADD CONSTRAINT 
  FOREIGN KEY (`codigo_interno_colaborador_alteracao`) 
  REFERENCES `tb_usuario` (`codigo_interno_colaborador`);

ALTER TABLE `tb_cliente_org`
ADD COLUMN `tipo_cadastro` VARCHAR(50) DEFAULT NULL 
AFTER `data_alteracao`;

UPDATE `tb_cliente_org` SET `tipo_cadastro` = 'CADASTRO_PROJETO'