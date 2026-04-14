ALTER TABLE tb_colaborador_apontamento
ADD COLUMN data_criacao timestamp DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN data_alteracao timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
ADD COLUMN tb_colaborador_cpf_criacao varchar(11) DEFAULT NULL,
ADD COLUMN tb_colaborador_cpf_alteracao varchar(11) DEFAULT NULL;