ALTER TABLE tb_organograma_posicao
DROP CONSTRAINT fk_org_pos_perfil ;

ALTER TABLE tb_organograma_posicao
DROP COLUMN tb_gestor_externo_perfil_id ;

ALTER TABLE tb_organograma_posicao
ADD COLUMN tb_perfil_corporativo_id  char(36) CHARACTER SET utf8mb3  NULL ;

UPDATE tb_organograma_posicao
SET tb_perfil_corporativo_id = NULL
WHERE tb_perfil_corporativo_id NOT IN (
    SELECT id FROM tb_perfil_corporativo
);

ALTER TABLE tb_organograma_posicao
ADD CONSTRAINT fk_pfcorp_posicaoid FOREIGN KEY (tb_perfil_corporativo_id) REFERENCES tb_perfil_corporativo (id) ;
