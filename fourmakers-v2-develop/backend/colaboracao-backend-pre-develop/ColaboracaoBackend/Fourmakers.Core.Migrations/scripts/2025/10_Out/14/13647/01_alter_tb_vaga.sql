ALTER TABLE tb_vaga
ADD COLUMN tb_vaga_id_parent VARCHAR(36) NULL,
ADD INDEX idx_tb_vaga_id_parent (tb_vaga_id_parent),
ADD CONSTRAINT fk_vaga_parent
  FOREIGN KEY (tb_vaga_id_parent)
  REFERENCES tb_vaga (id); -- Supondo que a coluna chave na tb_vaga se chame 'Id'