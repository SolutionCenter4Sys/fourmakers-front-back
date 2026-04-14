-- Adiciona o campo cargo_confianca na tabela tb_template_contratacao
ALTER TABLE tb_template_contratacao
ADD COLUMN cargo_confianca TINYINT NOT NULL DEFAULT 0 COMMENT 'Indica se o colaborador está em cargo de confiança';

