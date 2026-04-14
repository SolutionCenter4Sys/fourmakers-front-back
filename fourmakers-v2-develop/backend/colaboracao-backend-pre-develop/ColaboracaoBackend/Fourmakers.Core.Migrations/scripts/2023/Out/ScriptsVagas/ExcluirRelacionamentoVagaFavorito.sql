ALTER TABLE `tb_vaga_favorito` 
DROP FOREIGN KEY `fk_tb_vaga_favorito_tb_vagas_srs1`;
ALTER TABLE `tb_vaga_favorito` 
ADD INDEX `fk_tb_vaga_favorito_tb_usuario1_idx` (`tb_usuario_id` ASC) VISIBLE,
DROP INDEX `fk_tb_vaga_favorito_tb_usuario1_idx` ,
DROP INDEX `fk_tb_vaga_favorito_tb_vagas_srs1_idx` ;



