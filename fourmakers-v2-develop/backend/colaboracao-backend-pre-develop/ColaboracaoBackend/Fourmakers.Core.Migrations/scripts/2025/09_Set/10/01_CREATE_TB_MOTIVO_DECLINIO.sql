
CREATE TABLE tb_motivo_declinio (
    id VARCHAR(36) PRIMARY KEY,
    descricao VARCHAR(255) NOT NULL,
    ativo TINYINT(1) DEFAULT 1,
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- INSERT INTO tb_motivo_declinio (id, descricao) VALUES
-- (uuid(), 'Proposta Salarial'),
-- (uuid(), 'Localização'),
-- (uuid(), 'Modelo de Trabalho'),
-- (uuid(), 'Cliente'),
-- (uuid(), 'Timing'),
-- (uuid(), 'Outra Oportunidade'),
-- (uuid(), 'Outros');

-- drop table tb_motivo_declinio;
-- ALTER TABLE tb_candidato_vaga DROP FOREIGN KEY fk_tb_candidato_vaga_tb_motivo_declinio;