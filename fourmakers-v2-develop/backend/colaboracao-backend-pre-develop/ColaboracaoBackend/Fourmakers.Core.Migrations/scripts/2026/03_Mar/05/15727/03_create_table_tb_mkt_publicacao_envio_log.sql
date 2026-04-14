-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Log de envio (notificacao e email) para publicacoes oficiais
-- Data: 2026-03-16
-- Pasta: scripts/2026/03_Mar/05/15727
-- Registra para cada publicacao oficial a quem foi enviada notificacao in-app e/ou email.
-- tipo: 'notificacao' = envio in-app; 'email' = registro de envio de email.
-- ------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS tb_mkt_publicacao_envio_log (
    id CHAR(36) NOT NULL,
    tb_mkt_publicacao_id CHAR(36) NOT NULL COMMENT 'Publicacao oficial que gerou o envio',
    tipo ENUM('notificacao', 'email') NOT NULL COMMENT 'notificacao = in-app; email = envio de email',
    codigo_interno_colaborador VARCHAR(36) NOT NULL COMMENT 'Colaborador que recebeu',
    tb_org_id INT NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    KEY ix_tb_mkt_publicacao_envio_log_publicacao (tb_mkt_publicacao_id),
    KEY ix_tb_mkt_publicacao_envio_log_org (tb_org_id),
    CONSTRAINT fk_tb_mkt_publicacao_envio_log_publicacao FOREIGN KEY (tb_mkt_publicacao_id) REFERENCES tb_mkt_publicacao(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_tb_mkt_publicacao_envio_log_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Log de envio de notificacao e email por publicacao oficial';
