CREATE TABLE tb_notificacao (
    id char(36) NOT NULL PRIMARY KEY,
    tb_colaborador_cpf VARCHAR(11) NOT NULL,
    titulo VARCHAR(255) NOT NULL,
    mensagem TEXT NOT NULL,
    mensagem_html TEXT,
    lida BOOLEAN DEFAULT FALSE,
    data_envio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_leitura TIMESTAMP NULL,
    tb_funcionalidade_sistema_id INT DEFAULT NULL,
    tb_org_id INT NOT NULL,
    FOREIGN KEY (tb_funcionalidade_sistema_id) REFERENCES tb_funcionalidade_sistema(id),
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    FOREIGN KEY (tb_colaborador_cpf) REFERENCES tb_colaborador(cpf)
);