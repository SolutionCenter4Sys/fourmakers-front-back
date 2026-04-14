-- Remover foreign key e índice antigos
ALTER TABLE `tb_organograma_posicao_alocacao`
	DROP FOREIGN KEY `fk_aloc_colab`,
	DROP INDEX `idx_aloc_colab`;

-- Remover coluna codigo_interno_colaborador
ALTER TABLE `tb_organograma_posicao_alocacao`
	DROP COLUMN `codigo_interno_colaborador`;

-- Adicionar nova coluna tb_perfil_corporativo_alocacao_id
ALTER TABLE `tb_organograma_posicao_alocacao`
	ADD COLUMN `tb_perfil_corporativo_alocacao_id` varchar(36) NOT NULL AFTER `tb_organograma_posicao_id`;