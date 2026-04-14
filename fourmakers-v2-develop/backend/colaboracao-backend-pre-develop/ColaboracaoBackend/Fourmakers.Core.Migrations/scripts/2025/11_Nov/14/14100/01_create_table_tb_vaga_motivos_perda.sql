CREATE TABLE tb_vaga_motivos_perda (
    id CHAR(36) NOT NULL,                          -- GUID (UUID em formato texto)
    descricao VARCHAR(150) NOT NULL,               -- Descrição do motivo
    explicacao TEXT,                                -- Explicação detalhada do motivo
    ordem INT NOT NULL,                            -- Ordem de exibição
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,  -- Data de criação
    data_alteracao TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, -- Última alteração
    PRIMARY KEY (id)
);

