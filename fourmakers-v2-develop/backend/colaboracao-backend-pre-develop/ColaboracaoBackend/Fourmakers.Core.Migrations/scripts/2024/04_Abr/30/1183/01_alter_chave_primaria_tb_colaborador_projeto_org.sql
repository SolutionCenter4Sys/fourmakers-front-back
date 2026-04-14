ALTER TABLE tb_colaborador_projeto_org
DROP CONSTRAINT `PRIMARY`;

ALTER TABLE tb_colaborador_projeto_org
ADD CONSTRAINT pk_colaborador_projeto_org PRIMARY KEY (cod_colaborador, cod_projeto, tb_org_id);