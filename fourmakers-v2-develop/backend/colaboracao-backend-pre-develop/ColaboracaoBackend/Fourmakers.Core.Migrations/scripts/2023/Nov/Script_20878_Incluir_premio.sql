begin;

ALTER TABLE tb_vagas_srs
ADD tipoVaga INT;

ALTER TABLE tb_vagas_srs
ADD confidential_job INT;

commit;