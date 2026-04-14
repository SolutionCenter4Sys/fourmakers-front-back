CREATE TABLE tb_agenda_solicitacoes_participantes (
    id CHAR(36) NOT NULL,
    tb_agenda_encontros_id INT NOT NULL,
    tb_colaborador_codigo_interno_colaborador_criador VARCHAR(36)  NULL,
    tb_colaborador_codigo_interno_colaborador_solicitante VARCHAR(36)  NULL,
    status TINYINT NOT NULL COMMENT '0 = Pendente, 1 = Aprovado, 2 = Recusado',
    data_solicitacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_decisao DATETIME NULL,

    PRIMARY KEY(id),

    CONSTRAINT fk_agenda_participacoes_agenda
        FOREIGN KEY (tb_agenda_encontros_id) 
        REFERENCES tb_agenda_encontros(id)
        ON DELETE CASCADE ON UPDATE CASCADE,

    CONSTRAINT fk_agenda_participacoes_criador
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_criador) 
        REFERENCES tb_colaborador(codigo_interno_colaborador)
        ON DELETE CASCADE ON UPDATE CASCADE,

    CONSTRAINT fk_agenda_participacoes_solicitante
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador_solicitante) 
        REFERENCES tb_colaborador(codigo_interno_colaborador)
        ON DELETE CASCADE ON UPDATE CASCADE
);