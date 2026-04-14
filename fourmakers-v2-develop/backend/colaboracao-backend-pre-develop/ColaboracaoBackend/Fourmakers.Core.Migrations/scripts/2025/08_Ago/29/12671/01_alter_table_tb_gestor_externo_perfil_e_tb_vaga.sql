ALTER TABLE tb_gestor_externo_perfil
ADD COLUMN tipo_emprego_linkedin CHAR(36) NULL,
ADD COLUMN nivel_experiencia_linkedin CHAR(36) NULL,
ADD CONSTRAINT fk_gestor_tipo_emprego
    FOREIGN KEY (tipo_emprego_linkedin) REFERENCES tb_tipos_emprego_linkedin(id),
ADD CONSTRAINT fk_gestor_nivel_experiencia
    FOREIGN KEY (nivel_experiencia_linkedin) REFERENCES tb_niveis_experiencia_linkedin(id);

    ALTER TABLE tb_vaga
ADD COLUMN tipo_emprego_linkedin CHAR(36) NULL,
ADD COLUMN nivel_experiencia_linkedin CHAR(36) NULL,
ADD CONSTRAINT fk_vaga_tipo_emprego
    FOREIGN KEY (tipo_emprego_linkedin) REFERENCES tb_tipos_emprego_linkedin(id),
ADD CONSTRAINT fk_vaga_nivel_experiencia
    FOREIGN KEY (nivel_experiencia_linkedin) REFERENCES tb_niveis_experiencia_linkedin(id);