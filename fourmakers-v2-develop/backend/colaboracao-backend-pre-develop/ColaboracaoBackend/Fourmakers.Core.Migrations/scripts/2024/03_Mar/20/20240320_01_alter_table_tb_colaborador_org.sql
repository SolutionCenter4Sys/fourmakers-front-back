ALTER TABLE tb_colaborador_org ADD CONSTRAINT uc_tb_org_id_tb_colaborador_cpf_ativo UNIQUE (tb_org_id, tb_colaborador_cpf, ativo);

UPDATE tb_colaborador_org 
SET cod_colaborador_externo = tb_colaborador_cpf 
WHERE tb_org_id = 1

--rollback
-- UPDATE tb_colaborador_org 
-- SET cod_colaborador_externo = ''
-- WHERE tb_org_id = 1

ALTER TABLE tb_colaborador_org ADD CONSTRAINT uc_tb_org_id_cod_colaborador_externo_ativo UNIQUE (tb_org_id, cod_colaborador_externo, ativo);