CREATE TABLE tb_notificacao_contratos_vencidos_destinatarios
(
    tb_parceiros_gestao_contratos_id VARCHAR(36) NOT NULL,
    email VARCHAR(255) NOT NULL,
    PRIMARY KEY (email, tb_parceiros_gestao_contratos_id),
    CONSTRAINT fk_tncvd_contrato
        FOREIGN KEY (tb_parceiros_gestao_contratos_id)
        REFERENCES tb_parceiros_gestao_contratos(id)
        ON DELETE CASCADE
);

CREATE TABLE tb_notificacao_contratos_vencidos_destinatarios_padrao
(
	email VARCHAR(100) NOT NULL PRIMARY KEY
);

INSERT INTO tb_notificacao_contratos_vencidos_destinatarios_padrao (email)
VALUES
('luana.correia@foursys.com.br'),
('renata.ono@foursys.com.br'),
('rafael@foursys.com.br');