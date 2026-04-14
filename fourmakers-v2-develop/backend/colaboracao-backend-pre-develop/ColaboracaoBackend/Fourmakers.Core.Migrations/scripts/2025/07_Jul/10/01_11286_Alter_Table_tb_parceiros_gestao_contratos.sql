ALTER TABLE tb_parceiros 
		MODIFY COLUMN nome_parceiro VARCHAR(150) NULL,
        MODIFY COLUMN tipo_parceria VARCHAR(150) NULL;


ALTER TABLE tb_parceiros_gestao_contratos 
		MODIFY COLUMN unidade VARCHAR(50) NULL,
  		MODIFY COLUMN inicio_contrato TIMESTAMP NULL DEFAULT NULL;
