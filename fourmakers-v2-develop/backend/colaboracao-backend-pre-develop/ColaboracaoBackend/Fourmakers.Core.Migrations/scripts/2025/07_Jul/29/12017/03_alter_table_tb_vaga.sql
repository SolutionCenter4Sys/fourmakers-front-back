ALTER TABLE tb_vaga 
ADD COLUMN tb_colaborador_codigo_interno_colaborador_gestor_org_logada VARCHAR(36) NULL,
ADD COLUMN proposta_crm VARCHAR(36) NULL,
ADD COLUMN tb_tipo_vaga_id VARCHAR(36) NULL,
ADD COLUMN tb_tipo_contratacao_id INT NULL,
ADD COLUMN tb_unidade_id VARCHAR(36) NULL;

 ALTER TABLE tb_vaga 
 ADD CONSTRAINT fk_tb_vaga_gestor_org_logada 
 FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_gestor_org_logada) 
 REFERENCES tb_colaborador(codigo_interno_colaborador);

 ALTER TABLE tb_vaga 
 ADD CONSTRAINT fk_tb_vaga_tipo_vaga 
 FOREIGN KEY (tb_tipo_vaga_id) 
 REFERENCES tb_tipo_vaga(id);

 ALTER TABLE tb_vaga 
 ADD CONSTRAINT fk_tb_vaga_tipo_contratacao 
 FOREIGN KEY (tb_tipo_contratacao_id) 
 REFERENCES tb_tipo_contratacao(id);

 ALTER TABLE tb_vaga 
 ADD CONSTRAINT fk_tb_vaga_unidade 
 FOREIGN KEY (tb_unidade_id) 
 REFERENCES tb_unidade(id); 