
ALTER TABLE tb_agenda_encontros
  ADD COLUMN tb_cliente_org_codigo_cliente VARCHAR(36) NULL
        AFTER tb_colaborador_codigo_interno_colaborador,
  ADD COLUMN tb_encontros_tipo_interacao_id INT NOT NULL
        AFTER tb_cliente_org_codigo_cliente;

ALTER TABLE tb_agenda_encontros
  ADD CONSTRAINT fk_agenda_cliente
    FOREIGN KEY (tb_cliente_org_codigo_cliente)
    REFERENCES tb_cliente_org(codigo_cliente)
    ON DELETE CASCADE
    ON UPDATE CASCADE;
    
DELETE e
FROM tb_agenda_encontros AS e
LEFT JOIN tb_encontros_tipo_interacao AS t
  ON e.tb_encontros_tipo_interacao_id = t.id
WHERE t.id IS NULL;

    
ALTER TABLE tb_agenda_encontros
  MODIFY COLUMN tb_encontros_tipo_interacao_id INT NOT NULL;

ALTER TABLE tb_agenda_encontros
  ADD CONSTRAINT fk_tb_agenda_tipo_interacao
    FOREIGN KEY (tb_encontros_tipo_interacao_id)
    REFERENCES tb_encontros_tipo_interacao(id)
    ON DELETE CASCADE
    ON UPDATE CASCADE;


UPDATE tb_agenda_encontros a
JOIN   tb_encontros        e ON e.tb_agenda_encontros_id = a.id
SET    a.tb_cliente_org_codigo_cliente  = e.tb_cliente_org_codigo_cliente,
       a.tb_encontros_tipo_interacao_id = e.tb_encontros_tipo_interacao_id,
       a.data_criacao                = e.data_requisicao
WHERE (   a.tb_cliente_org_codigo_cliente  IS NULL
      OR  a.tb_encontros_tipo_interacao_id IS NULL);


ALTER TABLE tb_encontros
  DROP FOREIGN KEY  fk_tb_encontros_cliente_org,
  DROP FOREIGN KEY  fk_tb_encontros_tipo_interacao,
  DROP COLUMN    tb_cliente_org_codigo_cliente,
  DROP COLUMN    tb_encontros_tipo_interacao_id;


ALTER TABLE tb_encontros_archives
  ADD COLUMN transcricao TEXT NULL
        AFTER link_imagem_interacao;


ALTER TABLE tb_encontros_participantes
  ADD COLUMN tb_agenda_encontros_id INT NULL AFTER id;

ALTER TABLE tb_encontros_participantes
  ADD CONSTRAINT  fk_participantes_agenda
        FOREIGN KEY (tb_agenda_encontros_id)
        REFERENCES tb_agenda_encontros(id)
        ON DELETE CASCADE ON UPDATE CASCADE;


UPDATE tb_encontros_participantes p
JOIN   tb_encontros             e ON e.id = p.tb_encontros_id
SET    p.tb_agenda_encontros_id = e.tb_agenda_encontros_id
WHERE  p.tb_agenda_encontros_id IS NULL;

ALTER TABLE tb_encontros_participantes
  DROP FOREIGN KEY  fk_tb_encontros_participantes_encontros,
  DROP COLUMN    tb_encontros_id;


CREATE INDEX  idx_particip_agenda
        ON tb_encontros_participantes(tb_agenda_encontros_id);


CREATE TABLE  tb_encontros_ai (
    id              INT NOT NULL AUTO_INCREMENT,
    tb_encontros_id INT NOT NULL,
    resumo          TEXT NOT NULL,
    data_gerada     TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY(id),
    CONSTRAINT fk_ai_encontro
        FOREIGN KEY (tb_encontros_id)
        REFERENCES tb_encontros(id)
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE  tb_encontros_ai_passos (
    id              INT NOT NULL AUTO_INCREMENT,
    tb_encontros_ai_id INT NOT NULL,
    texto           TEXT NOT NULL,
    tb_colaborador_codigo_interno_colaborador VARCHAR(36) NULL,
    data_limite     TIMESTAMP NULL,
    PRIMARY KEY(id),
    CONSTRAINT fk_passo_ai
        FOREIGN KEY (tb_encontros_ai_id)
        REFERENCES tb_encontros_ai(id)
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_passo_responsavel
        FOREIGN KEY (tb_colaborador_codigo_interno_colaborador)
        REFERENCES tb_colaborador(codigo_interno_colaborador)
        ON DELETE SET NULL ON UPDATE CASCADE
);