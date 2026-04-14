CREATE TABLE tb_motivo_reprovacao (
    id CHAR(36) NOT NULL,                          -- GUID (UUID em formato texto)
    descricao VARCHAR(150) NOT NULL,               -- Descrição do motivo
    ativo BOOLEAN NOT NULL DEFAULT TRUE,           -- Campo ativo (TRUE/FALSE ou 1/0)
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,  -- Data de criação
    data_alteracao TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, -- Última alteração
    PRIMARY KEY (id)
);

INSERT INTO tb_motivo_reprovacao (id, descricao, ativo)
VALUES
(UUID(), 'Pretensão acima', TRUE),
(UUID(), 'Não aceita híbrido', TRUE),
(UUID(), 'Não aceita presencial', TRUE),
(UUID(), 'Não aceita modelo remuneração', TRUE),
(UUID(), 'Reprovado por hard skill', TRUE),
(UUID(), 'Reprovado por soft skill', TRUE);