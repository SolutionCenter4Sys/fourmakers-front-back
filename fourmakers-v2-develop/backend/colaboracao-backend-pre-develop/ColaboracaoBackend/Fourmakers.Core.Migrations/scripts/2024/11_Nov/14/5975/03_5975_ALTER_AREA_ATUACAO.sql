ALTER TABLE tb_area_atuacao
ADD COLUMN tb_org_id INT,
ADD CONSTRAINT fk_tb_area_atuacao_tb_org
FOREIGN KEY (tb_org_id) REFERENCES tb_org(id);

ALTER TABLE tb_gestor_externo_area_atuacao
DROP FOREIGN KEY tb_gestor_externo_area_atuacao_ibfk_2;

ALTER TABLE tb_area_atuacao
CHANGE COLUMN id id CHAR(36) NOT NULL;

ALTER TABLE tb_gestor_externo_area_atuacao
ADD CONSTRAINT tb_gestor_externo_area_atuacao_ibfk_2 
FOREIGN KEY (tb_area_atuacao_id) REFERENCES tb_area_atuacao(id);

ALTER TABLE tb_gestor_externo_perfil_skill
DROP FOREIGN KEY tb_gestor_externo_perfil_skill_ibfk_1 ;

ALTER TABLE tb_gestor_externo_perfil
CHANGE COLUMN id id CHAR(36) NOT NULL;

ALTER TABLE tb_gestor_externo_perfil_skill
ADD CONSTRAINT tb_gestor_externo_perfil_skill_ibfk_1 
FOREIGN KEY (tb_gestor_externo_perfil_id) REFERENCES tb_gestor_externo_perfil(id);



