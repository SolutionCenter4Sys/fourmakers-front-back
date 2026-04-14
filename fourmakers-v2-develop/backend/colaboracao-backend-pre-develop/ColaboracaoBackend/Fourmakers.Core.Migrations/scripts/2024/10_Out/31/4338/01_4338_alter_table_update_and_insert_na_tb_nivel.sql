-- select * from tb_item_perfil where descricao in ('COMPETENCIA','METODOLOGIA','DOMINIONEGOCIO','SOFTSKILL','IDIOMA');
-- select * from tb_nivel where tb_item_perfil_id = 1 order by ordem_exibicao -- hardskill
-- select * from tb_nivel where tb_item_perfil_id = 3 order by ordem_exibicao -- metodologia
-- select * from tb_nivel where tb_item_perfil_id = 4 order by ordem_exibicao -- dominio negocio
-- select * from tb_nivel where tb_item_perfil_id = 8 order by ordem_exibicao -- softskill
-- select * from tb_nivel where tb_item_perfil_id = 9 order by ordem_exibicao -- idioma

ALTER TABLE tb_nivel ADD COLUMN ordem_exibicao int DEFAULT NULL;

-- hardskills

UPDATE `tb_nivel` SET `prioridade_unificacao` = 6, ordem_exibicao = 1 WHERE (`descricao` = 'A definir') AND tb_item_perfil_id = 1;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 5, ordem_exibicao = 2 WHERE (`descricao` = 'Trainee') AND tb_item_perfil_id = 1;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 4, ordem_exibicao = 3 WHERE (`descricao` = 'Júnior') AND tb_item_perfil_id = 1;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 3, ordem_exibicao = 4 WHERE (`descricao` = 'Pleno') AND tb_item_perfil_id = 1;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 2, ordem_exibicao = 5 WHERE (`descricao` = 'Sênior') AND tb_item_perfil_id = 1;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 1, ordem_exibicao = 6 WHERE (`descricao` = 'Especialista') AND tb_item_perfil_id = 1;
INSERT INTO 
	`tb_nivel` (`descricao`, `ativo`, `tb_item_perfil_id`, `prioridade_unificacao`, `ordem_exibicao`)
VALUES
	('Especialista', '1', '1', 1, 6);

-- softskills
UPDATE `tb_nivel` SET `descricao` = 'A definir', `prioridade_unificacao` = 5, ordem_exibicao = 1 WHERE (`descricao` = 'A definir') AND tb_item_perfil_id = 8;
UPDATE `tb_nivel` SET `descricao` = 'Iniciante', `prioridade_unificacao` = 4, ordem_exibicao = 2 WHERE (`descricao` = 'A desenvolver') AND tb_item_perfil_id = 8;
UPDATE `tb_nivel` SET `descricao` = 'Intermediário', `prioridade_unificacao` = 3, ordem_exibicao = 3 WHERE (`descricao` = 'Em desenvolvimento') AND tb_item_perfil_id = 8;
UPDATE `tb_nivel` SET `descricao` = 'Avançado ', `prioridade_unificacao` = 2, ordem_exibicao = 4 WHERE (`descricao` = 'Desenvolvido') AND tb_item_perfil_id = 8;
INSERT INTO 
	`tb_nivel` (`descricao`, `ativo`, `tb_item_perfil_id`, `prioridade_unificacao`, `ordem_exibicao`)
VALUES
	('Especialista', 1, 8, 1, 5);


-- metodologias
UPDATE `tb_nivel` SET `descricao` = 'A definir', `prioridade_unificacao` = 5, ordem_exibicao = 1 WHERE (`descricao` = 'A definir') AND tb_item_perfil_id = 3;
UPDATE `tb_nivel` SET `descricao` = 'Iniciante', `prioridade_unificacao` = 4, ordem_exibicao = 2 WHERE (`descricao` = 'Baixo') AND tb_item_perfil_id = 3;
UPDATE `tb_nivel` SET `descricao` = 'Intermediário', `prioridade_unificacao` = 3, ordem_exibicao = 3 WHERE (`descricao` = 'Médio') AND tb_item_perfil_id = 3;
UPDATE `tb_nivel` SET `descricao` = 'Avançado ', `prioridade_unificacao` = 2, ordem_exibicao = 4 WHERE (`descricao` = 'Alto') AND tb_item_perfil_id = 3;
UPDATE `tb_nivel` SET `descricao` = 'Especialista ', `prioridade_unificacao` = 1, ordem_exibicao = 5 WHERE (`descricao` = 'Certificado') AND tb_item_perfil_id = 3;

-- dominio de negócios
UPDATE `tb_nivel` SET `descricao` = 'A definir', `prioridade_unificacao` = 5, ordem_exibicao = 1 WHERE (`descricao` = 'A definir') AND tb_item_perfil_id = 4;
UPDATE `tb_nivel` SET `descricao` = 'Iniciante', `prioridade_unificacao` = 4, ordem_exibicao = 2 WHERE (`descricao` = 'Baixo') AND tb_item_perfil_id = 4;
UPDATE `tb_nivel` SET `descricao` = 'Intermediário', `prioridade_unificacao` = 3, ordem_exibicao = 3 WHERE (`descricao` = 'Médio') AND tb_item_perfil_id = 4;
UPDATE `tb_nivel` SET `descricao` = 'Avançado ', `prioridade_unificacao` = 2, ordem_exibicao = 4 WHERE (`descricao` = 'Alto') AND tb_item_perfil_id = 4;
INSERT INTO 
	`tb_nivel` (`descricao`, `ativo`, `tb_item_perfil_id`, `prioridade_unificacao`, `ordem_exibicao`)
VALUES
	('Especialista', 1, 4, 1, 5);
    
-- idioma
UPDATE `tb_nivel` SET `prioridade_unificacao` = 6, ordem_exibicao = 1 WHERE (`descricao` = 'A definir') AND tb_item_perfil_id = 9;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 5, ordem_exibicao = 2 WHERE (`descricao` = 'Básico') AND tb_item_perfil_id = 9;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 4, ordem_exibicao = 3 WHERE (`descricao` = 'Intermediário') AND tb_item_perfil_id = 9;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 3, ordem_exibicao = 4 WHERE (`descricao` = 'Avançado') AND tb_item_perfil_id = 9;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 2, ordem_exibicao = 5 WHERE (`descricao` = 'Fluente') AND tb_item_perfil_id = 9;
UPDATE `tb_nivel` SET `prioridade_unificacao` = 1, ordem_exibicao = 6 WHERE (`descricao` = 'Nativo') AND tb_item_perfil_id = 9;
