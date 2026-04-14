CREATE TABLE tb_apontamento_periodo_fechado_log (
    id INT AUTO_INCREMENT NOT NULL,
    tb_apontamento_periodo_fechado_id INT,
    data_fim_anterior DATE,
    data_fim_nova DATE,
    tb_org_id INT,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    tb_colaborador_cpf_criacao VARCHAR(11),
    tb_colaborador_cpf_alteracao VARCHAR(11),
    PRIMARY KEY (id),
    FOREIGN KEY (tb_apontamento_periodo_fechado_id) REFERENCES tb_apontamento_periodo_fechado(id),
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
);
