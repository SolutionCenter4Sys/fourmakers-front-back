ALTER TABLE tb_colaborador_alocado
MODIFY COLUMN tb_colaborador_cpf varchar(11);

ALTER TABLE tb_colaborador_alocado
MODIFY COLUMN codigo_colaborador bigint DEFAULT NULL;