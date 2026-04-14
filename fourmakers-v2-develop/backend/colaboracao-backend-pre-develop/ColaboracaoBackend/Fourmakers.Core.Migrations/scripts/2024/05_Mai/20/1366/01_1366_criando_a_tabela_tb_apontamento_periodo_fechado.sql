CREATE TABLE tb_apontamento_periodo_fechado (
    id INT PRIMARY KEY,
    data_fim DATETIME,
    tb_org_id INT,
    data_criacao DATETIME,
    data_alteracao DATETIME,
    tb_colaborador_cpf_criacao VARCHAR(11),
    tb_colaborador_cpf_alteracao VARCHAR(11),
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
);
