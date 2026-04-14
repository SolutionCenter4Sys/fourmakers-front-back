ALTER TABLE tb_vaga
ADD COLUMN tb_colaborador_codigo_interno_colaborador_responsavel VARCHAR(255);

ALTER TABLE tb_vaga
ADD CONSTRAINT fk_vaga_colaborador_responsavel
FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_responsavel)
REFERENCES tb_colaborador (codigo_interno_colaborador);

ALTER TABLE tb_vaga
ADD COLUMN maquina VARCHAR(255);