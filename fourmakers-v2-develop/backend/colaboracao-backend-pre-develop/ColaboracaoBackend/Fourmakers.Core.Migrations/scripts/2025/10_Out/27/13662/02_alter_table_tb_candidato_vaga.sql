ALTER TABLE tb_candidato_vaga
ADD COLUMN tb_motivo_reprovacao_id CHAR(36) NULL,
ADD CONSTRAINT fk_candidato_vaga_motivo_reprovacao
    FOREIGN KEY (tb_motivo_reprovacao_id)
    REFERENCES tb_motivo_reprovacao(id)
    ON UPDATE CASCADE
    ON DELETE SET NULL;