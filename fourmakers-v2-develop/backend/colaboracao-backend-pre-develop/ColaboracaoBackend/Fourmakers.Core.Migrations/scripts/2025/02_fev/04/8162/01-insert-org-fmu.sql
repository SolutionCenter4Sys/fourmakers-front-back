INSERT INTO tb_org (`id`, `descricao`, `subdominio`, `dominio_email`) VALUES ('7', 'FMU', 'fmu', 'fmu.br');

/************** Vai pra HML mas não vai pra PRD **************/

INSERT INTO `tb_colaborador` (`codigo_interno_colaborador`, `nome_completo`, `data_nascimento`, `rg`, `matricula`, `endereco_id`, `ativo`, `contato_principal`, `contato_outro`, `candidato`, `passaporte`, `colaborador_saude_id`, `estado_civil`, `genero`, `etnia`, `orientacao_sexual`, `escolaridade`, `refugiado`, `documento_colaborador`) 
VALUES (uuid(), 'Tiago Augusto Rocha Pessoal', '1989-05-12', '90558601', '', '256', '1', '15997548651', '1111648935', '0', '12345', '19', 'Casado (a)', 'Homem cisgênero', 'Parda', 'Heterossexual', 'Ensino médio técnico', '0', '374.694.938-67');
INSERT INTO `tb_usuario` (`codigo_interno_colaborador`, `email`, `password`, `primeiro_acesso_realizado`, `ativo`, `sistemico`)
VALUES ((select codigo_interno_colaborador from tb_colaborador where nome_completo = 'Tiago Augusto Rocha Pessoal'), 'tiagorochati@outlook.com', '664c7931189e3ca21115625178e26e62', '0', '1', '0');
INSERT INTO `tb_colaborador_org` (`tb_org_id`, `codigo_interno_colaborador`, `cod_diretoria`, `diretoria`, `departamento`, `cod_departamento`, `cod_colaborador_externo`, `data_admissao`, `ativo`)
VALUES ('7', (select codigo_interno_colaborador from tb_colaborador where nome_completo = 'Tiago Augusto Rocha Pessoal'), '1', 'Padrão', 'SEM DEPARTAMENTO INFORMADO', 'SEM DEPARTAMENTO INFORMADO', '00000000003', '2010-10-10 00:00:00', '1');

INSERT INTO `tb_colaborador` (`codigo_interno_colaborador`, `nome_completo`, `data_nascimento`, `rg`, `matricula`, `endereco_id`, `ativo`, `contato_principal`, `contato_outro`, `candidato`, `passaporte`, `colaborador_saude_id`, `estado_civil`, `genero`, `etnia`, `orientacao_sexual`, `escolaridade`, `refugiado`, `documento_colaborador`) 
VALUES (uuid(), 'Nayara Pessoal Resid Um', '1989-05-12', '90558601', '', '256', '1', '15997548651', '1111648935', '0', '12345', '19', 'Casado (a)', 'Homem cisgênero', 'Parda', 'Heterossexual', 'Ensino médio técnico', '0', '374.694.938-67');
INSERT INTO `tb_usuario` (`codigo_interno_colaborador`, `email`, `password`, `primeiro_acesso_realizado`, `ativo`, `sistemico`)
VALUES ((select codigo_interno_colaborador from tb_colaborador where nome_completo = 'Nayara Pessoal Resid Um'), 'lorenzoaigne@gmail.com', '664c7931189e3ca21115625178e26e62', '0', '1', '0');
INSERT INTO `tb_colaborador_org` (`tb_org_id`, `codigo_interno_colaborador`, `cod_diretoria`, `diretoria`, `departamento`, `cod_departamento`, `cod_colaborador_externo`, `data_admissao`, `ativo`)
VALUES ('7', (select codigo_interno_colaborador from tb_colaborador where nome_completo = 'Nayara Pessoal Resid Um'), '1', 'Padrão', 'SEM DEPARTAMENTO INFORMADO', 'SEM DEPARTAMENTO INFORMADO', '00000000002', '2010-10-10 00:00:00', '1');


INSERT INTO `tb_colaborador` (`codigo_interno_colaborador`, `nome_completo`, `data_nascimento`, `rg`, `matricula`, `endereco_id`, `ativo`, `contato_principal`, `contato_outro`, `candidato`, `passaporte`, `colaborador_saude_id`, `estado_civil`, `genero`, `etnia`, `orientacao_sexual`, `escolaridade`, `refugiado`, `documento_colaborador`) 
VALUES (uuid(), 'Nayara Pessoal Gest Dois', '1989-05-12', '90558601', '', '256', '1', '15997548651', '1111648935', '0', '12345', '19', 'Casado (a)', 'Homem cisgênero', 'Parda', 'Heterossexual', 'Ensino médio técnico', '0', '374.694.938-67');
INSERT INTO `tb_usuario` (`codigo_interno_colaborador`, `email`, `password`, `primeiro_acesso_realizado`, `ativo`, `sistemico`)
VALUES ((select codigo_interno_colaborador from tb_colaborador where nome_completo = 'Nayara Pessoal Gest Dois'), 'nayarargn@gmail.com', '664c7931189e3ca21115625178e26e62', '0', '1', '0');
INSERT INTO `tb_colaborador_org` (`tb_org_id`, `codigo_interno_colaborador`, `cod_diretoria`, `diretoria`, `departamento`, `cod_departamento`, `cod_colaborador_externo`, `data_admissao`, `ativo`)
VALUES ('7', (select codigo_interno_colaborador from tb_colaborador where nome_completo = 'Nayara Pessoal Gest Dois'), '1', 'Padrão', 'SEM DEPARTAMENTO INFORMADO', 'SEM DEPARTAMENTO INFORMADO', '00000000001', '2010-10-10 00:00:00', '1');

/************** Vai pra HML mas não vai pra PRD **************/