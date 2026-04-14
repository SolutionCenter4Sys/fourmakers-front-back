ALTER TABLE tb_gestor_externo_perfil
DROP FOREIGN KEY tb_gestor_externo_perfil_ibfk_3;

ALTER TABLE tb_gestor_externo_area_atuacao
DROP FOREIGN KEY tb_gestor_externo_area_atuacao_ibfk_3;

ALTER TABLE tb_permanencia
CHANGE COLUMN id id CHAR(36) NOT NULL;

ALTER TABLE tb_gestor_externo_area_atuacao
CHANGE COLUMN tb_permanencia_id tb_permanencia_id CHAR(36);

ALTER TABLE tb_gestor_externo_perfil
CHANGE COLUMN tb_permanencia_id tb_permanencia_id CHAR(36);

ALTER TABLE tb_gestor_externo_perfil
ADD CONSTRAINT tb_gestor_externo_perfil_ibfk_3 
FOREIGN KEY (tb_permanencia_id) REFERENCES tb_permanencia(id);

ALTER TABLE tb_gestor_externo_area_atuacao
ADD CONSTRAINT tb_gestor_externo_area_atuacao_ibfk_3 
FOREIGN KEY (tb_permanencia_id) REFERENCES tb_permanencia(id);


