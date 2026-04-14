ALTER TABLE tb_modelo_trabalho
ADD COLUMN codigo INT;

UPDATE tb_modelo_trabalho tmt SET codigo = 1 WHERE descricao = '100% Presencial';
UPDATE tb_modelo_trabalho tmt SET codigo = 3 WHERE descricao = '100% Remoto';
UPDATE tb_modelo_trabalho tmt SET codigo = 2 WHERE descricao = 'Hibrido';
