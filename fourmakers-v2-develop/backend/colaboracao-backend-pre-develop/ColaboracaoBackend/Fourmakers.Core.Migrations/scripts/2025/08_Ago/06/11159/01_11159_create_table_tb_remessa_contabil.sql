CREATE TABLE tb_remessa_contabil (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tb_vigencia_id CHAR(36) NOT NULL,
    cnpj VARCHAR(20) NOT NULL,
    tb_org_id INT NOT NULL,
    data_geracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ids_registros JSON NOT NULL COMMENT 'JSON com IDs dos registros: {"folhaponto":[],"reembolso":[],"rubrica":[]}',
    codigo_interno_colaborador_criacao VARCHAR(36) NOT NULL,

    FOREIGN KEY (tb_vigencia_id) REFERENCES tb_vigencia(id),
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    FOREIGN KEY (codigo_interno_colaborador_criacao) REFERENCES tb_colaborador(codigo_interno_colaborador),

    INDEX idx_vigencia_cnpj_org (tb_vigencia_id, cnpj, tb_org_id)
);
