-- Adiciona os campos tb_modelo_trabalho_id e quantidade_dias_presencial na tabela tb_template_contratacao
ALTER TABLE tb_template_contratacao 
ADD COLUMN tb_modelo_trabalho_id CHAR(36) NULL COMMENT 'ID do modelo de trabalho (FK de tb_modelo_trabalho)',
ADD COLUMN quantidade_dias_presencial INT NULL COMMENT 'Quantidade de dias presenciais',
ADD CONSTRAINT fk_template_contratacao_modelo_trabalho 
    FOREIGN KEY (tb_modelo_trabalho_id) REFERENCES tb_modelo_trabalho(id) 
    ON DELETE SET NULL ON UPDATE CASCADE;

