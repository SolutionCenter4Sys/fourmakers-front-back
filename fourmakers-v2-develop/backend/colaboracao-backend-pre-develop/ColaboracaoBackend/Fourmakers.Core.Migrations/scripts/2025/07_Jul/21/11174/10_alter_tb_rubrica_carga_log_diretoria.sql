-- Alterar tb_rubrica_carga_log para usar cod_diretoria ao invés de unidade
ALTER TABLE tb_rubrica_carga_log 
CHANGE COLUMN unidade cod_diretoria VARCHAR(50) NOT NULL COMMENT 'Código da diretoria';