CREATE TABLE tb_parametro_reembolso (
    id INT AUTO_INCREMENT PRIMARY KEY,
    limite_envio INT NOT NULL,
    dia_pagamento INT NOT NULL,
    validade_comprovante_dias INT NOT NULL,
    limite_envio_alternativo INT DEFAULT NULL,
    dia_pagamento_alternativo INT DEFAULT NULL,
    tb_org_id INT NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    colaborador_alteracao VARCHAR(36),
    
    FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    FOREIGN KEY (colaborador_alteracao) REFERENCES tb_colaborador(codigo_interno_colaborador)
);