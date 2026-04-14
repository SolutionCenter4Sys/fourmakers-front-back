ALTER TABLE tb_vaga_candidato
ADD COLUMN ativo TINYINT(1) NOT NULL DEFAULT 1;

ALTER TABLE tb_vaga_candidato
ADD COLUMN status_id INTEGER NOT NULL DEFAULT 1;

ALTER TABLE tb_vaga_candidato
ADD CONSTRAINT fk_status
FOREIGN KEY (status_id) REFERENCES tb_candidato_status(id)
ON DELETE RESTRICT;
