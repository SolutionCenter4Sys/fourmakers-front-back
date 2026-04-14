CREATE TABLE tb_historico_competencia (
    id CHAR(36) NOT NULL DEFAULT (UUID()),
    tipo_competencia_enum ENUM('HardSkill', 'SoftSkill', 'Metodologia', 'Dominio', 'Idioma') NOT NULL,
    descricao_competencia VARCHAR(255) NOT NULL,
    situacao ENUM('Unificada', 'Aprovada', 'Reprovada', 'Adicionada') NOT NULL,
    observacao VARCHAR(255) NOT NULL,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_alteracao DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
    tb_colaborador_cpf_criacao VARCHAR(11),
    tb_colaborador_cpf_alteracao VARCHAR(11),
    PRIMARY KEY (id)
);