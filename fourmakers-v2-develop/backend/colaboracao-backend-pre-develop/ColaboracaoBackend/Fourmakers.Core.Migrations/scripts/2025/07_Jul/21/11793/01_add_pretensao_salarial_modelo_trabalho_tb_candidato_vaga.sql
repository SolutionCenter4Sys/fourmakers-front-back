-- Adiciona os campos pretensao_salarial e tb_modelo_trabalho_id na tabela tb_candidato_vaga
ALTER TABLE tb_candidato_vaga 
ADD COLUMN pretensao_salarial DECIMAL(10,2) NULL,
ADD COLUMN tb_modelo_trabalho_id VARCHAR(36) NULL,
ADD CONSTRAINT fk_candidato_vaga_modelo_trabalho 
FOREIGN KEY (tb_modelo_trabalho_id) REFERENCES tb_modelo_trabalho(id); 