CREATE TABLE tb_agenda_versao (
    id INT NOT NULL AUTO_INCREMENT,
    descricao VARCHAR(1000) NOT NULL,
    versao    VARCHAR(50)  NOT NULL,
    data_atualizacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id)
) 
ENGINE=InnoDB 
DEFAULT CHARSET=utf8mb4 
COLLATE=utf8mb4_unicode_ci;
