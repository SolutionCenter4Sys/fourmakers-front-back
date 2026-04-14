ALTER TABLE tb_questionario
ADD COLUMN mostrar_questionario tinyint(1) DEFAULT 1,
ADD COLUMN data_inicio TIMESTAMP NOT NULL,
ADD COLUMN data_fim TIMESTAMP NOT NULL;

UPDATE tb_questionario
SET
    data_inicio = CONCAT(CURDATE(), ' 00:00:00'),
    data_fim    = CONCAT(DATE_ADD(CURDATE(), INTERVAL 3 MONTH), ' 00:00:00')
WHERE codigo_questionario = '2025_01_COLETA_PERFIL_COLABORADOR'
  AND tb_org_id = 2;