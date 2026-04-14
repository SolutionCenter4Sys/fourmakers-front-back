CREATE TABLE tb_comentario_vaga (
    tb_vaga_id CHAR(36) NOT NULL,
    tb_comentario_id CHAR(36) NOT NULL,
    PRIMARY KEY (tb_vaga_id, tb_comentario_id),
    CONSTRAINT fk_comentario_vaga_vaga
        FOREIGN KEY (tb_vaga_id)
        REFERENCES tb_vaga(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT fk_comentario_vaga_comentario
        FOREIGN KEY (tb_comentario_id)
        REFERENCES tb_comentario(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
