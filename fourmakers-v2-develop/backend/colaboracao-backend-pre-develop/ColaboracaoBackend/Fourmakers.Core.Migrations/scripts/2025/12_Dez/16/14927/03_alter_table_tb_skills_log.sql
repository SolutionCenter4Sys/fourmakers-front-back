ALTER TABLE tb_skills_log
ADD COLUMN tb_colaborador_codigo_interno_colaborador_logado CHAR(36),
ADD COLUMN log_automatico BOOLEAN NOT NULL;

ALTER TABLE tb_skills_log
ADD CONSTRAINT fk_tb_skills_log_colaborador_logado
FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_logado)
REFERENCES tb_colaborador (codigo_interno_colaborador);