ALTER TABLE `tb_gestor_externo_area_atuacao` 
DROP FOREIGN KEY `tb_gestor_externo_area_atuacao_ibfk_1`;
ALTER TABLE `tb_gestor_externo_area_atuacao` 
ADD CONSTRAINT `tb_gestor_externo_area_atuacao_ibfk_1`
  FOREIGN KEY (`tb_org_id` , `cod_gestor_externo`)
  REFERENCES `tb_gestor_externo` (`tb_org_id` , `cod_gestor_externo`)
  ON UPDATE CASCADE;
