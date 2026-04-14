ALTER TABLE tb_status_apontamento
ADD exibir_status_gerente_projeto TINYINT(1);

UPDATE tb_status_apontamento SET exibir_status_gerente_projeto = 0;
UPDATE tb_status_apontamento SET exibir_status_gerente_projeto = 1 WHERE cod_status_apontamento IN (1,4,5,7);
