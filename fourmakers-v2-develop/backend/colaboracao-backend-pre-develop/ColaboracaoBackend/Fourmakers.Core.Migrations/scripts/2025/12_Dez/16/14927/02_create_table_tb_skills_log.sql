CREATE TABLE tb_skills_log(
id CHAR(36) PRIMARY KEY,
tb_colaborador_codigo_interno_colaborador VARCHAR(36) NOT NULL,
tb_gestor_externo_perfil_id CHAR(36) NOT NULL,
skill_id BIGINT NOT NULL,
tb_item_perfil_id BIGINT NOT NULL,
tb_nivel_id BIGINT NOT NULL,
tb_skills_movimentacao_id BIGINT NOT NULL,
data_criacao TIMESTAMP  DEFAULT CURRENT_TIMESTAMP,
data_alteracao TIMESTAMP  DEFAULT CURRENT_TIMESTAMP,
CONSTRAINT fk_colaborador_skills_log FOREIGN KEY (tb_colaborador_codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador),
CONSTRAINT fk_gestor_perfil_skills_log FOREIGN KEY (tb_gestor_externo_perfil_id) REFERENCES tb_gestor_externo_perfil(id),
CONSTRAINT fk_item_perfil_skills_log FOREIGN KEY (tb_item_perfil_id) REFERENCES tb_item_perfil(id),
CONSTRAINT fk_nivel_skills_log FOREIGN KEY (tb_nivel_id) REFERENCES tb_nivel(id),
CONSTRAINT fk_skills_movimentacoao_log FOREIGN KEY (tb_skills_movimentacao_id) REFERENCES tb_skills_movimentacao(id)
);