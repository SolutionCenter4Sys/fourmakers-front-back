CREATE TABLE tb_reembolso_saldo_colaborador (
    id INT AUTO_INCREMENT PRIMARY KEY,
    valor DECIMAL(10,2) NOT NULL DEFAULT 0,
    codigo_interno_colaborador VARCHAR(100) NOT NULL,
    tb_org_id INT NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_alteracao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    UNIQUE KEY uq_colaborador_org (codigo_interno_colaborador, tb_org_id)
);