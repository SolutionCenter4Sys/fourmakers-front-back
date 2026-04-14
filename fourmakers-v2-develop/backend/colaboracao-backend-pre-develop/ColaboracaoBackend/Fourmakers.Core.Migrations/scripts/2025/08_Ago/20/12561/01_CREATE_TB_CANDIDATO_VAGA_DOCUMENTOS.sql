CREATE TABLE tb_candidato_vaga_documentos (
    id VARCHAR(36) NOT NULL,
    tb_candidato_vaga_id VARCHAR(36) NOT NULL,
    tb_comentario_id VARCHAR(36) NOT NULL,
    tb_colaborador_codigo_interno_colaborador_criador VARCHAR(36) NULL,
    data_archived TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
    link_arquivo VARCHAR(255) NULL,
    PRIMARY KEY (id),
    
    CONSTRAINT fk_tb_candidato_vaga_documentos_candidato_vaga
        FOREIGN KEY (tb_candidato_vaga_id)
        REFERENCES tb_candidato_vaga(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
        
    CONSTRAINT fk_tb_candidato_vaga_documentos_tb_comentario
        FOREIGN KEY (tb_comentario_id)
        REFERENCES tb_comentario(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
        
    CONSTRAINT fk_tb_candidato_vaga_documentos_criador
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_criador)
        REFERENCES tb_colaborador(codigo_interno_colaborador)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);