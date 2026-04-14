ALTER TABLE tb_gestor_externo_perfil
DROP FOREIGN KEY tb_gestor_externo_perfil_ibfk_4;

ALTER TABLE tb_modelo_trabalho
CHANGE COLUMN id id CHAR(36) NOT NULL;

ALTER TABLE tb_gestor_externo_perfil
CHANGE COLUMN tb_modelo_trabalho_id tb_modelo_trabalho_id CHAR(36) NULL;

ALTER TABLE tb_gestor_externo_perfil
ADD CONSTRAINT tb_gestor_externo_perfil_ibfk_4 
FOREIGN KEY (tb_modelo_trabalho_id) REFERENCES tb_modelo_trabalho(id);