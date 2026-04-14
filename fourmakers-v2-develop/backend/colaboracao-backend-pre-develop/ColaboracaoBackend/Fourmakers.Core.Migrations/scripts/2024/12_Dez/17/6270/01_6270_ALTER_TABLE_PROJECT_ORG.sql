DROP TABLE tb_colaborador_projeto;
DROP TABLE tb_projeto;

ALTER TABLE tb_projeto_org
	ADD `cod_cliente_registro_carga` varchar(255) NULL,
    ADD `nome_cliente_registro_carga` varchar(255) NULL,
	ADD `tipo_cadastro` varchar(50) DEFAULT NULL,
    ADD `data_criacao` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
    ADD `data_alteracao` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    ADD `codigo_interno_colaborador_criacao` varchar(36) DEFAULT NULL,
    ADD `codigo_interno_colaborador_alteracao` varchar(36) DEFAULT NULL;
   
update 
	tb_projeto_org
set 
	cod_cliente_registro_carga = cod_cliente,
    nome_cliente_registro_carga = cliente;
    
ALTER TABLE tb_projeto_org DROP COLUMN cliente;


