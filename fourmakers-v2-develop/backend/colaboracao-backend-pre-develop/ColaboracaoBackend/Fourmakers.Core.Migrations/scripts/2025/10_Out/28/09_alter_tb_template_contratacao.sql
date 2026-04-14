ALTER TABLE tb_template_contratacao 
ADD COLUMN tb_equipamento_padrao_cargo_funcao_id CHAR(36) NULL COMMENT 'ID do cargo da tabela tb_equipamento_padrao_cargo_funcao',
ADD CONSTRAINT fk_template_cargo_id 
    FOREIGN KEY (tb_equipamento_padrao_cargo_funcao_id) REFERENCES tb_equipamento_padrao_cargo_funcao(id) 
    ON DELETE SET NULL ON UPDATE CASCADE;