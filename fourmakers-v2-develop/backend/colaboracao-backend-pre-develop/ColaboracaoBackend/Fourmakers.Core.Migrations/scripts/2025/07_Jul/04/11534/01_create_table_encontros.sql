
create  table  tb_encontros_tipo_interacao (
    id INT NOT NULL AUTO_INCREMENT,
    descricao VARCHAR(255) NOT NULL,
    PRIMARY KEY (id)
);

insert into tb_encontros_tipo_interacao (id, descricao) values
(1,'Reuniao'),
(2,'call'),
(3,'whatsApp'),
(4,'email'),
(5,'presencial');

CREATE  TABLE tb_encontros (
    id INT NOT NULL AUTO_INCREMENT,
    tb_colaborador_codigo_interno_colaborador VARCHAR(36) DEFAULT NULL,
    tb_cliente_org_codigo_cliente VARCHAR(36) DEFAULT NULL,
    tb_encontros_tipo_interacao_id INT NOT NULL,
    data_requisicao TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
    titulo_interacao VARCHAR(255) NOT NULL,
    descricao_interacao TEXT NULL,
     resumo_interacao TEXT NULL,
    PRIMARY KEY (id),
    CONSTRAINT `fk_tb_encontros_colaborador`
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador)
        REFERENCES tb_colaborador(codigo_interno_colaborador)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT `fk_tb_encontros_cliente_org`
        FOREIGN KEY (tb_cliente_org_codigo_cliente)
        REFERENCES tb_cliente_org(codigo_cliente)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT `fk_tb_encontros_tipo_interacao`
        FOREIGN KEY (tb_encontros_tipo_interacao_id)
        REFERENCES tb_encontros_tipo_interacao(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

create  table tb_agenda_encontros (
    id INT NOT NULL AUTO_INCREMENT,
    tb_colaborador_codigo_interno_colaborador VARCHAR(36) DEFAULT NULL,
    tb_encontros_id INT NOT NULL,
    data_criacao TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    data_agendada TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
    data_inicio TIMESTAMP NULL DEFAULT NULL,
    data_fim TIMESTAMP NULL DEFAULT NULL,
    status ENUM('Pendente', 'Confirmado', 'Cancelado') NOT NULL DEFAULT 'Pendente',
    localizacao VARCHAR(255) NULL,
    link_reuniao VARCHAR(255) NULL,
    quantidade_participantes INT NOT NULL DEFAULT 0,
    titulo VARCHAR(255) NOT NULL,
    descricao TEXT NULL,
    PRIMARY KEY (id),
    CONSTRAINT `fk_tb_agenda_encontros_colaborador`
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador)
        REFERENCES tb_colaborador(codigo_interno_colaborador)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT `fk_tb_agenda_encontros_encontros`
        FOREIGN KEY (tb_encontros_id)
        REFERENCES tb_encontros(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);


create table tb_encontros_archives (
    id INT NOT NULL AUTO_INCREMENT,
    tb_encontros_id INT NOT NULL,
    data_archived TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
    link_audio_interacao VARCHAR(255) NULL,
    link_imagem_interacao VARCHAR(255) NULL,
    PRIMARY KEY (id),
    CONSTRAINT `fk_tb_encontros_archives_encontros`
        FOREIGN KEY (tb_encontros_id)
        REFERENCES tb_encontros(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

create  table  tb_encontros_participantes (
    id INT NOT NULL AUTO_INCREMENT,
    tb_encontros_id INT NOT NULL,
    tb_gestor_externo_cod_gestor_externo VARCHAR(36) DEFAULT NULL,
    tb_colaborador_codigo_interno_colaborador VARCHAR(36) DEFAULT NULL,
    confirmado BOOLEAN NOT NULL DEFAULT FALSE,
    data_confirmacao TIMESTAMP NULL DEFAULT NULL,
    interessado BOOLEAN NOT NULL DEFAULT FALSE,
    data_interesse TIMESTAMP NULL DEFAULT NULL,
    PRIMARY KEY (id),
    CONSTRAINT `fk_tb_encontros_participantes_encontros`
        FOREIGN KEY (tb_encontros_id)
        REFERENCES tb_encontros(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT `fk_tb_encontros_participantes_gestor_externo`
        FOREIGN KEY (tb_gestor_externo_cod_gestor_externo)
        REFERENCES tb_gestor_externo(cod_gestor_externo)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT `fk_tb_encontros_participantes_colaborador`
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador)
        REFERENCES tb_colaborador(codigo_interno_colaborador)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
