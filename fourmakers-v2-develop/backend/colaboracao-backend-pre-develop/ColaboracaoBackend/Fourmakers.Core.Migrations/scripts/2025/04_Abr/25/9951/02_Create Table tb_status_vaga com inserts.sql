CREATE TABLE tb_status_vaga (
    id varchar(36) PRIMARY KEY,
    descricao VARCHAR(100) NOT NULL
);

INSERT INTO tb_status_vaga (id, descricao) VALUES 
(uuid(), 'Pipeline'),
(uuid(), 'Em Refinamento'),
(uuid(), 'Entrevista Inicial'),
(uuid(), 'Aplicação de Testes'),
(uuid(), 'Entrevista Técnica'),
(uuid(), 'Entrevista com Cliente'),
(uuid(), 'Carta-Oferta'),
(uuid(), 'Contratação');