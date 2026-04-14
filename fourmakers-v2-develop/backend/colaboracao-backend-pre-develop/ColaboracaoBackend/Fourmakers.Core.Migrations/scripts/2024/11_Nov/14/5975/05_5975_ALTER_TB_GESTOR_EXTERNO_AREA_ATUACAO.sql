ALTER TABLE tb_gestor_externo_area_atuacao
DROP FOREIGN KEY tb_gestor_externo_area_atuacao_ibfk_2;

ALTER TABLE tb_gestor_externo_area_atuacao
CHANGE COLUMN tb_area_atuacao_id tb_area_atuacao_id CHAR(36) NOT NULL;

ALTER TABLE tb_gestor_externo_area_atuacao
ADD CONSTRAINT tb_gestor_externo_area_atuacao_ibfk_2 
FOREIGN KEY (tb_area_atuacao_id) REFERENCES tb_area_atuacao(id);