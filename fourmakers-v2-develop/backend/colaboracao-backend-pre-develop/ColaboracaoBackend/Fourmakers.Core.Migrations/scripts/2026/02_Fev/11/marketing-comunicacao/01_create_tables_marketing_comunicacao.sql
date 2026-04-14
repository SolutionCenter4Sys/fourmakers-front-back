-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao
-- Data: 2026-02-11
-- Pasta: scripts/2026/02_Fev/11/marketing-comunicacao
-- Observacao: tb_org_id foi definido como INT para compatibilidade com o schema atual.
-- ------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS tb_mkt_grupo (
    id CHAR(36) NOT NULL,
    nome VARCHAR(150) NOT NULL,
    descricao VARCHAR(500) NULL,
	tb_org_id INT NOT NULL,
    PRIMARY KEY (id),
	CONSTRAINT fk_tb_mkt_grupo_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_grupo_usuario (
    id CHAR(36) NOT NULL,
    tb_mkt_grupo_id CHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_grupo_usuario_grupo_colaborador_org (tb_mkt_grupo_id, codigo_interno_colaborador),
    KEY ix_tb_mkt_grupo_usuario_colaborador (codigo_interno_colaborador),
    CONSTRAINT fk_tb_mkt_grupo_usuario_grupo FOREIGN KEY (tb_mkt_grupo_id) REFERENCES tb_mkt_grupo(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_label (
    id CHAR(36) NOT NULL,
    nome VARCHAR(150) NOT NULL,
    tb_org_id INT NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_label_nome_org (nome, tb_org_id),
    KEY ix_tb_mkt_label_org (tb_org_id),
    CONSTRAINT fk_tb_mkt_label_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_publicacao (
    id CHAR(36) NOT NULL,
    tipo ENUM('informativo','documento') NOT NULL,
    publicacao_status ENUM('rascunho','agendada','ativa','expirada','arquivada','excluida') NOT NULL DEFAULT 'rascunho',
    aprovacao_status ENUM('nao_requer','pendente','aprovado','rejeitado') NOT NULL DEFAULT 'nao_requer',
    titulo VARCHAR(255) NOT NULL,
    conteudo TEXT NULL,
    tb_mkt_label_id CHAR(36) NULL,
    requer_confirmacao_leitura TINYINT(1) NOT NULL DEFAULT 0,
    permite_comentarios TINYINT(1) NOT NULL DEFAULT 0,
    permite_curtidas TINYINT(1) NOT NULL DEFAULT 0,
    publicacao_fixada_pelo_responsavel TINYINT(1) NOT NULL DEFAULT 0,
    data_agendamento_publicacao DATETIME NULL,
    data_publicacao DATETIME NULL,
    data_validade DATETIME NULL,
    codigo_interno_colaborador_aprovador VARCHAR(36) NULL,
    data_criacao_status_aprovacao DATETIME NULL,
    motivo_rejeicao VARCHAR(500) NULL,
    tb_org_id INT NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    codigo_interno_colaborador_criacao VARCHAR(36) NOT NULL,
    codigo_interno_colaborador_alteracao VARCHAR(36) NOT NULL,
    PRIMARY KEY (id),
    KEY ix_tb_mkt_publicacao_org (tb_org_id),
    KEY ix_tb_mkt_publicacao_tipo (tipo),
    KEY ix_tb_mkt_publicacao_status (publicacao_status),
    KEY ix_tb_mkt_publicacao_aprovacao_status (aprovacao_status),
    KEY ix_tb_mkt_publicacao_data_validade (data_validade),
    KEY ix_tb_mkt_publicacao_data_publicacao (data_publicacao),
    KEY ix_tb_mkt_publicacao_fixada (publicacao_fixada_pelo_responsavel),
    KEY ix_tb_mkt_publicacao_label_id (tb_mkt_label_id),
    CONSTRAINT fk_tb_mkt_publicacao_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    CONSTRAINT fk_tb_mkt_publicacao_label FOREIGN KEY (tb_mkt_label_id) REFERENCES tb_mkt_label(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_publicacao_label (
    id CHAR(36) NOT NULL,
    tb_mkt_label_id CHAR(36) NOT NULL,
    tb_mkt_publicacao_id CHAR(36) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_publicacao_label (tb_mkt_label_id, tb_mkt_publicacao_id),
    KEY ix_tb_mkt_publicacao_label_publicacao (tb_mkt_publicacao_id),
    CONSTRAINT fk_tb_mkt_publicacao_label_label FOREIGN KEY (tb_mkt_label_id) REFERENCES tb_mkt_label(id),
    CONSTRAINT fk_tb_mkt_publicacao_label_publicacao FOREIGN KEY (tb_mkt_publicacao_id) REFERENCES tb_mkt_publicacao(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_publicaco_grupo (
    id CHAR(36) NOT NULL,
    tb_mkt_publicacao_id CHAR(36) NOT NULL,
    tb_mkt_grupo_id CHAR(36) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_publicaco_grupo (tb_mkt_publicacao_id, tb_mkt_grupo_id),
    KEY ix_tb_mkt_publicaco_grupo_grupo (tb_mkt_grupo_id),
    CONSTRAINT fk_tb_mkt_publicaco_grupo_publicacao FOREIGN KEY (tb_mkt_publicacao_id) REFERENCES tb_mkt_publicacao(id),
    CONSTRAINT fk_tb_mkt_publicaco_grupo_grupo FOREIGN KEY (tb_mkt_grupo_id) REFERENCES tb_mkt_grupo(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_publicacao_anexo (
    id CHAR(36) NOT NULL,
    tb_mkt_publicacao_id CHAR(36) NOT NULL,
    nome_arquivo VARCHAR(255) NOT NULL,
    url_arquivo VARCHAR(500) NOT NULL,
    tamanho_bytes BIGINT NULL,
    tipo ENUM('imagem','video','documento') NOT NULL,
    PRIMARY KEY (id),
    KEY ix_tb_mkt_publicacao_anexo_publicacao (tb_mkt_publicacao_id),
    CONSTRAINT fk_tb_mkt_publicacao_anexo_publicacao FOREIGN KEY (tb_mkt_publicacao_id) REFERENCES tb_mkt_publicacao(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_publicacao_colaborador_interacao (
    id CHAR(36) NOT NULL,
    tb_mkt_publicacao_id CHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    data_primeira_entrega DATETIME NULL,
    visualizado TINYINT(1) NULL,
    data_visualizado DATETIME NULL,
    confirmou_leitura TINYINT(1) NULL,
    data_confirmou_leitura DATETIME NULL,
    curtida_emoji VARCHAR(50) NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_public_colab_inter_public_colab (tb_mkt_publicacao_id, codigo_interno_colaborador),
    KEY ix_tb_mkt_publicacao_colaborador_interacao_colaborador (codigo_interno_colaborador),
    CONSTRAINT fk_tb_mkt_publicacao_colaborador_interacao_publicacao FOREIGN KEY (tb_mkt_publicacao_id) REFERENCES tb_mkt_publicacao(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_publicacao_comentario (
    id CHAR(36) NOT NULL,
    tb_mkt_publicacao_id CHAR(36) NOT NULL,
    conteudo TEXT NOT NULL,
    comentario_pai_id CHAR(36) NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    KEY ix_tb_mkt_publicacao_comentario_publicacao (tb_mkt_publicacao_id),
    KEY ix_tb_mkt_publicacao_comentario_pai (comentario_pai_id),
    KEY ix_tb_mkt_publicacao_comentario_colaborador (codigo_interno_colaborador),
    CONSTRAINT fk_tb_mkt_publicacao_comentario_publicacao FOREIGN KEY (tb_mkt_publicacao_id) REFERENCES tb_mkt_publicacao(id),
    CONSTRAINT fk_tb_mkt_publicacao_comentario_pai FOREIGN KEY (comentario_pai_id) REFERENCES tb_mkt_publicacao_comentario(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_publicacao_comentario_interacao (
    id CHAR(36) NOT NULL,
    tb_mkt_publicacao_comentario_id CHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    curtida_emoji VARCHAR(50) NULL,
    data_interacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_publicacao_comentario_interacao_comentario_colaborador (tb_mkt_publicacao_comentario_id, codigo_interno_colaborador),
    KEY ix_tb_mkt_publicacao_comentario_interacao_colaborador (codigo_interno_colaborador),
    CONSTRAINT fk_tb_mkt_publicacao_comentario_interacao_comentario FOREIGN KEY (tb_mkt_publicacao_comentario_id) REFERENCES tb_mkt_publicacao_comentario(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_comunidade (
    id CHAR(36) NOT NULL,
    nome VARCHAR(150) NOT NULL,
    descricao VARCHAR(500) NULL,
    capa_url VARCHAR(1000) NULL,
    tipo ENUM('publica','privada') NOT NULL,
    permite_postagem_membro TINYINT(1) NOT NULL DEFAULT 0,
    permite_sair TINYINT(1) NOT NULL DEFAULT 1,
    publicacao_configuracao_politica ENUM('forcar-configuracao-comunidade','sugerir','desativado') NOT NULL DEFAULT 'sugerir',
    publicacao_permite_comentario TINYINT(1) NOT NULL DEFAULT 1,
    publicacao_permite_like_habilitado TINYINT(1) NOT NULL DEFAULT 1,
    tb_org_id INT NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    KEY ix_tb_mkt_comunidade_org (tb_org_id),
    KEY ix_tb_mkt_comunidade_tipo (tipo),
    CONSTRAINT fk_tb_mkt_comunidade_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_comunidade_publicacao (
    tb_mkt_comunidade_id CHAR(36) NOT NULL,
    tb_mkt_publicacao_id CHAR(36) NOT NULL,
    PRIMARY KEY (tb_mkt_comunidade_id, tb_mkt_publicacao_id),
    KEY ix_tb_mkt_comunidade_publicacao_publicacao (tb_mkt_publicacao_id),
    CONSTRAINT fk_tb_mkt_comunidade_publicacao_comunidade FOREIGN KEY (tb_mkt_comunidade_id) REFERENCES tb_mkt_comunidade(id),
    CONSTRAINT fk_tb_mkt_comunidade_publicacao_publicacao FOREIGN KEY (tb_mkt_publicacao_id) REFERENCES tb_mkt_publicacao(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_comunidade_usuario_participando (
    id CHAR(36) NOT NULL,
    tb_mkt_comunidade_id CHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_comunidade_usuario (tb_mkt_comunidade_id, codigo_interno_colaborador),
    KEY ix_tb_mkt_comunidade_usuario_colaborador (codigo_interno_colaborador),
    CONSTRAINT fk_tb_mkt_comunidade_usuario_comunidade FOREIGN KEY (tb_mkt_comunidade_id) REFERENCES tb_mkt_comunidade(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_comunidade_grupo (
    id CHAR(36) NOT NULL,
    tb_mkt_comunidade_id CHAR(36) NOT NULL,
    tb_mkt_grupo_id CHAR(36) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_comunidade_grupo (tb_mkt_comunidade_id, tb_mkt_grupo_id),
    KEY ix_tb_mkt_comunidade_grupo_grupo (tb_mkt_grupo_id),
    CONSTRAINT fk_tb_mkt_comunidade_grupo_comunidade FOREIGN KEY (tb_mkt_comunidade_id) REFERENCES tb_mkt_comunidade(id),
    CONSTRAINT fk_tb_mkt_comunidade_grupo_grupo FOREIGN KEY (tb_mkt_grupo_id) REFERENCES tb_mkt_grupo(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_comunidade_moderador (
    id CHAR(36) NOT NULL,
    tb_mkt_comunidade_id CHAR(36) NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_tb_mkt_comunidade_moderador (tb_mkt_comunidade_id, codigo_interno_colaborador),
    CONSTRAINT fk_tb_mkt_comunidade_moderador_comunidade FOREIGN KEY (tb_mkt_comunidade_id) REFERENCES tb_mkt_comunidade(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tb_mkt_colaborador_configuracao (
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,
    notifica_web TINYINT(1) NOT NULL DEFAULT 1,
    notifica_teams TINYINT(1) NOT NULL DEFAULT 0,
    notifica_email TINYINT(1) NOT NULL DEFAULT 1,
    notifica_plataforma TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (codigo_interno_colaborador, tb_org_id),
    KEY ix_tb_mkt_colaborador_configuracao_org (tb_org_id),
    CONSTRAINT fk_tb_mkt_colaborador_configuracao_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
) ENGINE=InnoDB;
