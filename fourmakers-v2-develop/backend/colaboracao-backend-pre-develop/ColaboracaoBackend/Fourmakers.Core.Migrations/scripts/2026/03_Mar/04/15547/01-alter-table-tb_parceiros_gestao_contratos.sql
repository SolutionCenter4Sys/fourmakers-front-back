ALTER TABLE tb_parceiros_gestao_contratos
		MODIFY COLUMN cd_contrato varchar(150) DEFAULT NULL;

ALTER TABLE tb_parceiros_gestao_contratos
		MODIFY COLUMN cd_contrato_anterior varchar(150) DEFAULT NULL;
