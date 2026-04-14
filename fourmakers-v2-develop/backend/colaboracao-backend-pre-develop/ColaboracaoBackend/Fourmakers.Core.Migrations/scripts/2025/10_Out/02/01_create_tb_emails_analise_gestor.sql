CREATE TABLE tb_emails_analise_gestor (
    tb_vaga_id VARCHAR(36) NOT NULL,
    email VARCHAR(255) NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ativo TINYINT NOT NULL DEFAULT 1,
    FOREIGN KEY (tb_vaga_id) REFERENCES tb_vaga(id) ON DELETE CASCADE
);