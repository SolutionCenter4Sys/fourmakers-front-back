CREATE TABLE tb_classificacao_perfil (
    id CHAR(36) NOT NULL PRIMARY KEY,
    tb_gestor_externo_perfil_id CHAR(36) NOT NULL,
    categoria VARCHAR(255) NOT NULL,
    score DOUBLE NOT NULL DEFAULT 0,
    
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_classificacao_perfil_gestor_externo_perfil
        FOREIGN KEY (tb_gestor_externo_perfil_id)
            REFERENCES tb_gestor_externo_perfil (id)
            ON DELETE CASCADE
            ON UPDATE CASCADE,
            
    UNIQUE KEY uk_classificacao_perfil_gestor (tb_gestor_externo_perfil_id)
);

