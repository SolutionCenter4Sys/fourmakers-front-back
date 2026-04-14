CREATE TABLE tb_rubrica_tipo (
    id CHAR(36) NOT NULL PRIMARY KEY,
    descricao VARCHAR(100) NOT NULL,
	codigo_rubrica_tipo varchar(50) NOT NULL UNIQUE
);