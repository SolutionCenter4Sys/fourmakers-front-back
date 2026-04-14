ALTER TABLE tb_rubrica_carga_log
MODIFY COLUMN tb_rubrica_id char(36) NOT NULL;

ALTER TABLE tb_rubrica_carga_log
ADD COLUMN tb_org_id INT NOT NULL,
ADD COLUMN codigo_interno_colaborador_criacao char(36) NOT NULL,
ADD COLUMN mes int NOT NULL,
ADD COLUMN ano int NOT NULL;