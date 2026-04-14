ALTER TABLE tb_beneficios_template_pdf
ADD COLUMN custo_hora DECIMAL(10,2) NULL;

ALTER TABLE tb_acessos_usuario_template_pdf
ADD COLUMN maquina VARCHAR(100) NULL;

ALTER TABLE tb_acessos_usuario_grupos_template_pdf
ADD COLUMN maquina VARCHAR(100) NULL;