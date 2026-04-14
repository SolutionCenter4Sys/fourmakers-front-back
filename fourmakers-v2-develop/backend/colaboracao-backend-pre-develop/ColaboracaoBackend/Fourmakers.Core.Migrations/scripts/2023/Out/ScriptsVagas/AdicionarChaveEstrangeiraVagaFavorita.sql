
ALTER TABLE `tb_skill_vaga` 
ADD CONSTRAINT `fk_tb_vaga_favorito_tb_vagas_srs1`
  FOREIGN KEY (`tb_vagas_srs_id`)
  REFERENCES `tb_vagas_srs` (`id_vaga`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
