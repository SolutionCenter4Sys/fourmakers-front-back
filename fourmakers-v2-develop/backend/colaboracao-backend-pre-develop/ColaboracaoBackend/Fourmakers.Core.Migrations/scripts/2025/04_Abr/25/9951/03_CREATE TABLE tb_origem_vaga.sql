CREATE TABLE tb_origem_vaga (
    id varchar(36) PRIMARY KEY,
    descricao VARCHAR(100) NOT NULL
);

INSERT INTO tb_origem_vaga (id, descricao) VALUES 
(uuid(), 'Criada Automaticamente'),
(uuid(), 'Fourmakers'),
(uuid(), 'Legado');