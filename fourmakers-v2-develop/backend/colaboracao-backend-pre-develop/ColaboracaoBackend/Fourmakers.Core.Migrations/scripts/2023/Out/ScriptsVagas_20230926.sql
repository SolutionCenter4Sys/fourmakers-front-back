BEGIN; 

ALTER TABLE `tb_vaga_favorito` 
DROP FOREIGN KEY `fk_tb_vaga_favorito_tb_vagas_srs1`;
TRUNCATE TABLE `tb_skill_vaga`;
TRUNCATE TABLE `tb_vagas_srs`;
ALTER TABLE `tb_vaga_favorito` 
ADD INDEX `fk_tb_vaga_favorito_tb_usuario1_idx` (`tb_usuario_id` ASC) VISIBLE,
DROP INDEX `fk_tb_vaga_favorito_tb_usuario1_idx`,
DROP INDEX `fk_tb_vaga_favorito_tb_vagas_srs1_idx`;


ALTER TABLE tb_vagas_srs
DROP COLUMN id;

ALTER TABLE tb_vagas_srs
ADD PRIMARY KEY (id_vaga);

ALTER TABLE `tb_skill_vaga` 
ADD CONSTRAINT `fk_tb_vaga_favorito_tb_vagas_srs1`
  FOREIGN KEY (`tb_vagas_srs_id`)
  REFERENCES `tb_vagas_srs` (`id_vaga`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `tb_skill_vaga` 
ADD CONSTRAINT `fk_tb_skill_vaga_tb_vagas_srs1`
  FOREIGN KEY (`tb_vagas_srs_id`)
  REFERENCES `tb_vagas_srs` (`id_vaga`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE tb_vagas_srs
ADD loc_trabalho VARCHAR(20);

COMMIT;