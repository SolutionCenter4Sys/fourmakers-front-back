CREATE TABLE tb_vaga_fourmakers_log (
    id varchar(36) PRIMARY KEY,
    tb_vaga_id varchar(36) NOT NULL,
    tb_status_vaga_id VARCHAR(36) NOT NULL,
    tb_usuario_id BIGINT NOT NULL,
    data_alteracao DATETIME DEFAULT CURRENT_TIMESTAMP,
    objeto TEXT NULL,

    CONSTRAINT fk_status_vaga FOREIGN KEY (tb_vaga_id) REFERENCES tb_vaga(id),
    CONSTRAINT fk_id_vaga FOREIGN KEY (tb_status_vaga_id) REFERENCES tb_status_vaga(id),
    CONSTRAINT fk_id_usuario FOREIGN KEY (tb_usuario_id) REFERENCES tb_usuario(id)
);