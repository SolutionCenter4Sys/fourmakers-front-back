-- Cria a tabela tb_disponibilidade_entrevista
CREATE TABLE tb_disponibilidade_entrevista (
    id VARCHAR(36) PRIMARY KEY,
    descricao VARCHAR(100) NOT NULL
);

-- Insere os valores padrão
INSERT INTO tb_disponibilidade_entrevista (id, descricao) VALUES 
(UUID(), 'Manhã'),
(UUID(), 'Tarde'),
(UUID(), 'Noite');

-- Adiciona o campo tb_disponibilidade_entrevista_id na tabela tb_candidato_vaga
ALTER TABLE tb_candidato_vaga 
ADD COLUMN tb_disponibilidade_entrevista_id VARCHAR(36) NULL,
ADD CONSTRAINT fk_candidato_vaga_disponibilidade_entrevista 
FOREIGN KEY (tb_disponibilidade_entrevista_id) REFERENCES tb_disponibilidade_entrevista(id); 