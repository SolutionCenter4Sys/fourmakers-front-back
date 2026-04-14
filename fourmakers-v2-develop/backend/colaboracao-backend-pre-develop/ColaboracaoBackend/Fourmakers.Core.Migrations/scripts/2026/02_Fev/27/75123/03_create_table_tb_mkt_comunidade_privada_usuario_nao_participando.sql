-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Comunidade privada com permite_sair (opt-out)
-- Data: 2026-02-27
-- Pasta: scripts/2026/02_Fev/27/75123
-- Tabela para registrar usuários que saíram de comunidade privada quando permite_sair = 1.
-- Participação em privada é derivada dos grupos (tb_mkt_comunidade_grupo + tb_mkt_grupo_usuario);
-- quem está aqui não está mais participando. Não espelha grupo em participando.
-- ------------------------------------------------------------------------------

CREATE TABLE tb_mkt_comunidade_privada_usuario_nao_participando (
    id CHAR(36) NOT NULL,
    tb_mkt_comunidade_id CHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    data_saida DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_comunidade_privada_nao_participando (tb_mkt_comunidade_id, codigo_interno_colaborador),
    KEY ix_tb_mkt_comunidade_privada_nao_participando_comunidade (tb_mkt_comunidade_id),
    KEY ix_tb_mkt_comunidade_privada_nao_participando_colaborador (codigo_interno_colaborador),
    CONSTRAINT fk_tb_mkt_comunidade_privada_nao_participando_comunidade
        FOREIGN KEY (tb_mkt_comunidade_id) REFERENCES tb_mkt_comunidade (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
