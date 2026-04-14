CREATE TABLE tb_experiencia_projeto (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nome_projeto VARCHAR(255) NOT NULL,
    data_inicio DATE,
    data_fim DATE,
    experiencia_id BIGINT not null,
    FOREIGN KEY (experiencia_id) REFERENCES tb_experiencia(id)
);