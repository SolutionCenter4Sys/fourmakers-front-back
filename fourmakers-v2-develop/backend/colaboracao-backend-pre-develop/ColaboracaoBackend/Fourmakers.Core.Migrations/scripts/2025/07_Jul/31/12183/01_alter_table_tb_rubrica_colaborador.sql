ALTER TABLE tb_rubrica_colaborador
ADD COLUMN mes_inicial INT NOT NULL,
ADD COLUMN ano_inicial INT NOT NULL,
ADD COLUMN mes_final INT DEFAULT NULL,
ADD COLUMN ano_final INT DEFAULT NULL;

ALTER TABLE tb_rubrica_colaborador
DROP FOREIGN KEY tb_rubrica_colaborador_ibfk_2,
DROP FOREIGN KEY tb_rubrica_colaborador_ibfk_3;

ALTER TABLE tb_rubrica_colaborador
DROP COLUMN tb_vigencia_inicial_id,
DROP COLUMN tb_vigencia_final_id;