ALTER TABLE tb_org 
ADD COLUMN prioridade INT NULL;

UPDATE tb_org
SET prioridade = 1
WHERE id = 2;

UPDATE tb_org
SET prioridade = 2
WHERE id = 4;

UPDATE tb_org
SET prioridade = 3
WHERE id = 6;

UPDATE tb_org
SET prioridade = 4
WHERE id = 7;

UPDATE tb_org
SET prioridade = 5
WHERE id = 5;

UPDATE tb_org
SET prioridade = 6
WHERE id = 1;