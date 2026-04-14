CREATE TABLE tb_funcionalidade_rota (
    id char(36) NOT NULL PRIMARY KEY,
    rota varchar(255) NOT NULL,
    tb_funcionalidade_sistema_id INT NOT NULL,
    tb_org_id INT NOT NULL,
    FOREIGN KEY (tb_funcionalidade_sistema_id) REFERENCES tb_funcionalidade_sistema(id),
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    UNIQUE (tb_funcionalidade_sistema_id, tb_org_id)
);
