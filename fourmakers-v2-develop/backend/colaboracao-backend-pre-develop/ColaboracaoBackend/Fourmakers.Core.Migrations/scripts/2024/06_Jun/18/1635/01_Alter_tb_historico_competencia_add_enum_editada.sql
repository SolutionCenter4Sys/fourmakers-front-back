ALTER TABLE `tb_historico_competencia` 
CHANGE COLUMN `situacao` `situacao` ENUM('Unificada', 'Aprovada', 'Reprovada', 'Adicionada', 'Editada') NOT NULL ;
