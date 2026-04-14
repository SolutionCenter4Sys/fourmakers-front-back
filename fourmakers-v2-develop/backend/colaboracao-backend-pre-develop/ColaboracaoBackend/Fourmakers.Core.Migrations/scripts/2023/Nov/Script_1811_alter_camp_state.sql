BEGIN;


ALTER TABLE tb_vagas_srs
RENAME COLUMN estado TO state;

COMMIT;