ALTER TABLE tb_candidato_vaga 
ADD COLUMN tb_motivo_declinio_id VARCHAR(36) NULL,
ADD CONSTRAINT fk_tb_candidato_vaga_tb_motivo_declinio 
    FOREIGN KEY (tb_motivo_declinio_id) 
    REFERENCES tb_motivo_declinio(id);
