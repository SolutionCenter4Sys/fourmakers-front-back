CREATE TABLE tb_extrato_pagamento (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tipo_pagamento INT NOT NULL,
    tb_solicitacao_pagamento_id INT NOT NULL,
    tb_org_id INT NOT NULL,
    valor DOUBLE(10,2),
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    
    -- Foreign Keys
    CONSTRAINT fk_extrato_pagamento_solicitacao FOREIGN KEY (tb_solicitacao_pagamento_id) REFERENCES tb_solicitacao_pagamento(id),
    CONSTRAINT fk_extrato_pagamento_org FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
    CONSTRAINT fk_extrato_pagamento_colaborador FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador)
);