-- MySQL Workbench Synchronization
-- Generated: 2023-11-27 16:10
-- Model: New Model
-- Version: 1.0
-- Project: Name of the project
-- Author: carlos.domingos

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

ALTER TABLE `tb_colaborador_org` 
CHANGE COLUMN `tb_org_id` `tb_org_id` INT(11) NOT NULL ;

ALTER TABLE `tb_colaborador_hierarquia` 
CHANGE COLUMN `tb_org_id` `tb_org_id` INT(11) NOT NULL ;

ALTER TABLE `tb_projeto_org` 
CHANGE COLUMN `qtd_horas_planejadas` `qtd_horas_planejadas` DECIMAL NOT NULL ,
CHANGE COLUMN `qtd_horas_executadas` `qtd_horas_executadas` DECIMAL NOT NULL ,
CHANGE COLUMN `tb_org_id` `tb_org_id` INT(11) NOT NULL ;

ALTER TABLE `tb_projeto_gerente` 
CHANGE COLUMN `tb_org_id` `tb_org_id` INT(11) NOT NULL ;

ALTER TABLE `tb_colaborador_alocado` 
ADD CONSTRAINT `fk_tb_colaborador_alocado_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `tb_empresa` 
ADD CONSTRAINT `fk_tb_empresa_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`);

ALTER TABLE `tb_grupo_acesso` 
ADD CONSTRAINT `fk_tb_grupo_acesso_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`);

ALTER TABLE `tb_colaborador_org` 
ADD CONSTRAINT `fk_tb_colaborador_org_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_tb_colaborador_org_tb_colaborador1`
  FOREIGN KEY (`tb_colaborador_cpf`)
  REFERENCES `tb_colaborador` (`cpf`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `tb_colaborador_hierarquia` 
ADD CONSTRAINT `fk_tb_colaborador_hierarquia_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `tb_projeto_org` 
ADD CONSTRAINT `fk_tb_projeto_org_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `tb_projeto_gerente` 
ADD CONSTRAINT `fk_tb_projeto_gerente_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
