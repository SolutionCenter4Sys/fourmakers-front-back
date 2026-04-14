CREATE TABLE tb_cidadania_status (
  id INT PRIMARY KEY AUTO_INCREMENT,
  descricao VARCHAR(255) NOT NULL
);

INSERT INTO tb_cidadania_status (descricao) VALUES
("Solicitada"),
("Concedida");