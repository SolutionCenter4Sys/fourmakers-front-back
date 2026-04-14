CREATE TABLE tb_solicitacao_pagamento (
      id INT AUTO_INCREMENT PRIMARY KEY,

      tb_status_solicitacao_pagamento_id INT NOT NULL,
      codigo_interno_colaborador VARCHAR(36) NOT NULL,
      tb_org_id INT NOT NULL,
      tb_solicitacao_reembolso_id INT NOT NULL,

      origem VARCHAR(100) NOT NULL,
      valor DOUBLE(10,2) NOT NULL,

      data_solicitacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
      data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

-- Foreign Keys
      CONSTRAINT fk_status_pagamento FOREIGN KEY (tb_status_solicitacao_pagamento_id) REFERENCES tb_status_solicitacao_pagamento(id),
      CONSTRAINT fk_colaborador_solicitacao_pagamento FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador),
      CONSTRAINT fk_org_solicitacao_pagamento FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
      CONSTRAINT fk_reembolso_solicitacao_pagamento FOREIGN KEY (tb_solicitacao_reembolso_id) REFERENCES tb_solicitacao_reembolso(id)
);