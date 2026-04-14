BEGIN;

ALTER TABLE tb_vagas_srs
ADD COLUMN textForLinkedin longtext;

COMMIT;