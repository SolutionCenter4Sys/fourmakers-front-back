ALTER TABLE `tb_atividade`
ADD CONSTRAINT `fk_tb_atividade_tb_org_id` FOREIGN KEY (`tb_org_id`) REFERENCES `tb_org` (`id`)
ON DELETE CASCADE
ON UPDATE CASCADE;