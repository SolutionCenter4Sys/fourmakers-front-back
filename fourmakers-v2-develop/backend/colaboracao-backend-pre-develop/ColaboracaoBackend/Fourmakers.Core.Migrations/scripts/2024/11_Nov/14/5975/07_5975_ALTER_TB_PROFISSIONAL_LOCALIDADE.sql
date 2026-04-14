ALTER TABLE tb_gestor_externo_perfil
DROP FOREIGN KEY tb_gestor_externo_perfil_ibfk_5;

ALTER TABLE tb_profissional_localidade
CHANGE COLUMN id id CHAR(36) NOT NULL;

ALTER TABLE tb_gestor_externo_perfil
CHANGE COLUMN tb_profissional_localidade_id tb_profissional_localidade_id CHAR(36) NULL;

ALTER TABLE tb_gestor_externo_perfil
ADD CONSTRAINT tb_gestor_externo_perfil_ibfk_5 
FOREIGN KEY (tb_profissional_localidade_id) REFERENCES tb_profissional_localidade(id); 