CREATE TABLE tb_verba_tipo (
       id INT AUTO_INCREMENT PRIMARY KEY,
       descricao varchar(100) NOT NULL,
       label varchar(100) DEFAULT NULL,
       operacao varchar(10) NOT NULL
);