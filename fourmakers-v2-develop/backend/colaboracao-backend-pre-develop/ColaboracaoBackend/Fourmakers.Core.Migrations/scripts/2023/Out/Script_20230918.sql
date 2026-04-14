UPDATE `tb_colaborador` SET `genero` = '' WHERE (`genero` = 'Cisgênero');
UPDATE `tb_identidade_genero` SET `descricao` = 'Homem cisgênero' WHERE (`id` = '1');
INSERT INTO `tb_identidade_genero` (`descricao`, `ativo`) VALUES ('Mulher cisgênero', '1');

UPDATE `gcolb_prd`.`tb_colaborador` SET `genero` = '' WHERE (`genero` = 'Cisgênero');
UPDATE `gcolb_prd`.`tb_identidade_genero` SET `descricao` = 'Homem cisgênero' WHERE (`id` = '1');
INSERT INTO `gcolb_prd`.`tb_identidade_genero` (`descricao`, `ativo`) VALUES ('Mulher cisgênero', '1');


