INSERT INTO `tb_funcionalidade_sistema` (`id`, `descricao`, `ativo`) VALUES ('29', 'CADASTRO_GESTAO_ALOCADOS', '1');

-- somente prd (pois já foi em hml)
INSERT INTO `gcolb_prd`.`tb_grupo_acesso` (`descricao`, `ativo`, `tb_org_id`) VALUES ('GESTÃO DE ALOCADOS', '1', '2');

INSERT INTO `gcolb_prd`.`tb_grupo_acesso_funcionalidade_sistema` (`tb_grupo_acesso_id`, `tb_funcionalidade_sistema_id`, `ativo`)
select id,29,1 from gcolb_prd.tb_grupo_acesso where descricao = 'GESTÃO DE ALOCADOS';
