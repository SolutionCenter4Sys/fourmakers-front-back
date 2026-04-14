CREATE TABLE tb_origem_colaborador (
    id varchar(36) PRIMARY KEY,
    descricao VARCHAR(100) NOT NULL,
    ativo TINYINT NOT NULL DEFAULT 1
);

INSERT INTO tb_origem_colaborador (id, descricao, ativo) VALUES 
(uuid(), 'Banco De Talentos', 1),
(uuid(), 'Colaborador', 1),
(uuid(), 'Linkedin', 1),
(uuid(), 'Inativo', 1);