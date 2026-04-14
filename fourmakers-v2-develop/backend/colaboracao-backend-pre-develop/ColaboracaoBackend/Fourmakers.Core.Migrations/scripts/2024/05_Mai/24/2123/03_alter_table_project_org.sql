ALTER TABLE `tb_projeto_org` 
CHANGE COLUMN `permite_apont_sem_alocacao` `permite_apont_sem_alocacao` TINYINT(1) NOT NULL DEFAULT '0' ,
CHANGE COLUMN `permite_apont_sem_alocacao_outro_colab` `permite_apont_sem_alocacao_outro_colab` TINYINT(1) NOT NULL DEFAULT '0' ;
