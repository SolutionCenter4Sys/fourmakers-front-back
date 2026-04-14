-- Adicionar campo tb_comentario_id na tabela tb_candidato_vaga_log
-- Relacionamento 1:1 entre log de mudança de status e comentário

ALTER TABLE tb_candidato_vaga_log 
ADD COLUMN tb_comentario_id VARCHAR(36) NULL,
ADD CONSTRAINT fk_tb_candidato_vaga_log_comentario 
    FOREIGN KEY (tb_comentario_id) REFERENCES tb_comentario(id);