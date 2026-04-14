CREATE TABLE tb_classificacao_vaga (
    id CHAR(36) NOT NULL PRIMARY KEY,
    tb_vaga_id CHAR(36) NOT NULL,
    categoria VARCHAR(255) NOT NULL,
    score DOUBLE NOT NULL DEFAULT 0,
    
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_classificacao_vaga_vaga
        FOREIGN KEY (tb_vaga_id)
            REFERENCES tb_vaga (id)
            ON DELETE CASCADE
            ON UPDATE CASCADE,
            
    UNIQUE KEY uk_classificacao_vaga_vaga (tb_vaga_id)
);

