ALTER TABLE tb_vaga_competencia
  DROP FOREIGN KEY tb_vaga_competencia_ibfk_6;
  
ALTER TABLE tb_colaborador_idioma
  DROP FOREIGN KEY fk_tb_colaborador_idioma_tb_idioma1;

ALTER TABLE tb_vaga_competencia
  MODIFY COLUMN idioma_id BIGINT;

ALTER TABLE tb_colaborador_idioma
  MODIFY COLUMN idioma_id BIGINT;
  
ALTER TABLE tb_idioma
  DROP PRIMARY KEY,
  MODIFY COLUMN id BIGINT AUTO_INCREMENT,
  ADD PRIMARY KEY (id);

ALTER TABLE tb_vaga_competencia
  ADD FOREIGN KEY (idioma_id) REFERENCES tb_idioma(id);

ALTER TABLE tb_colaborador_idioma
  ADD FOREIGN KEY (idioma_id) REFERENCES tb_idioma(id);


CREATE TABLE tb_gestor_externo_perfil_skill (
     tb_gestor_externo_perfil_id VARCHAR(36) NOT NULL,
     tb_item_perfil_id BIGINT NOT NULL,
     skill_id BIGINT NULL,  -- Essa skill_id pode vir de qualquer uma das tabelas
     tb_nivel_id BIGINT NOT NULL,
     
     -- Chaves primárias e estrangeiras
     PRIMARY KEY (tb_gestor_externo_perfil_id),
     
     -- Chaves estrangeiras para as diferentes tabelas de skill
     FOREIGN KEY (tb_gestor_externo_perfil_id) 
         REFERENCES tb_gestor_externo_perfil(id),
     FOREIGN KEY (tb_item_perfil_id) 
         REFERENCES tb_item_perfil(id),
     FOREIGN KEY (tb_nivel_id) 
         REFERENCES tb_nivel(id)

);