-- Adicionar campo tb_vaga_motivos_perda_id na tabela tb_vaga
ALTER TABLE tb_vaga 
ADD COLUMN tb_vaga_motivos_perda_id CHAR(36) NULL;

-- Adicionar foreign key para tb_vaga_motivos_perda
ALTER TABLE tb_vaga 
ADD CONSTRAINT fk_tb_vaga_tb_vaga_motivos_perda 
FOREIGN KEY (tb_vaga_motivos_perda_id) 
REFERENCES tb_vaga_motivos_perda(id);

