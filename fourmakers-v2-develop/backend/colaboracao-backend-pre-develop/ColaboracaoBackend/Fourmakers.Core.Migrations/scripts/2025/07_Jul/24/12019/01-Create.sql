CREATE TABLE tb_participantes_externo (
    id INT PRIMARY KEY AUTO_INCREMENT,
    tb_agenda_encontros_id INT NOT NULL,
    nome VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    confirmado TINYINT(1) DEFAULT 0,
    data_confirmacao TIMESTAMP NULL DEFAULT NULL,
    interessado TINYINT(1) DEFAULT 0,
    data_interesse TIMESTAMP NULL DEFAULT NULL,
    CONSTRAINT fk_agenda_encontros
        FOREIGN KEY (tb_agenda_encontros_id)
        REFERENCES tb_agenda_encontros(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);