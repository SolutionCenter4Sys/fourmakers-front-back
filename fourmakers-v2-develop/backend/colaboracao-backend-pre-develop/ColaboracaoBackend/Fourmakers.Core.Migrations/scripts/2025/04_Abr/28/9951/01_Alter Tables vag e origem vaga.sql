
ALTER TABLE tb_status_vaga
ADD COLUMN codigo INT;

ALTER TABLE tb_origem_vaga
ADD COLUMN codigo INT;

ALTER TABLE tb_vaga
RENAME COLUMN tb_status_vaga_id TO tb_status_vaga_cod,
RENAME COLUMN tb_origem_vaga_id TO tb_origem_vaga_cod;