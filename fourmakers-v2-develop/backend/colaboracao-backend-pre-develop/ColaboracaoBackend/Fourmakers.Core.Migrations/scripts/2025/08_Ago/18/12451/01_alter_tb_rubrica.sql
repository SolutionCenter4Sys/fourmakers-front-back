ALTER TABLE tb_rubrica ADD COLUMN refletir_contabil tinyint(1) default 1;

ALTER TABLE tb_rubrica 
MODIFY COLUMN `calculo_tipo` enum('Valor','Porcentagem','Hora') NOT NULL DEFAULT 'Valor';