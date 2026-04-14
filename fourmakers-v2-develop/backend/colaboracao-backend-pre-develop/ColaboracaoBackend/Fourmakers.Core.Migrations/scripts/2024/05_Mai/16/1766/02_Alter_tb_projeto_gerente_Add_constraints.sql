ALTER TABLE tb_projeto_gerente
ADD CONSTRAINT fk_tb_proj_ger_tb_colab_org
FOREIGN KEY (cod_colaborador_gerente, tb_org_id)
REFERENCES tb_colaborador_org(cod_colaborador_externo, tb_org_id);



