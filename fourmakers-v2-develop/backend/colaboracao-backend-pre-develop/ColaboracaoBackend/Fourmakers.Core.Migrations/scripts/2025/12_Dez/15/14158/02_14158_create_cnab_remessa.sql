
create TABLE `tb_cnab_remessa` (
  `id` char(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `codigo_banco` char(3) DEFAULT NULL,
  `agencia` char(5) DEFAULT NULL,
  `agencia_dv` char(1) DEFAULT NULL,
  `conta` char(12) DEFAULT NULL,
  `conta_dv` char(1) DEFAULT NULL,
  `convenio` varchar(20) DEFAULT NULL,
  `descricao` varchar(100) DEFAULT NULL,
  `valor_total` decimal(15,2) NOT NULL DEFAULT '0.00',
  `hash_remessa` varchar(64) NOT NULL,
  `nome_arquivo` varchar(255) NOT NULL,
  `modelo_cnab` varchar(50) NOT NULL,
  `tipo` varchar(50) NOT NULL,
  `codigo_diretoria` varchar(255) NOT NULL,
  `status` enum('GERANDO ARQUIVO','ARQUIVO GERADO','FINALIZADO','CANCELADO') NOT NULL,
  `data_criacao` datetime DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `codigo_interno_colaborador_criacao` char(36) DEFAULT NULL,
  `codigo_interno_colaborador_alteracao` char(36) DEFAULT NULL,
  PRIMARY KEY (`id`)
);


create TABLE tb_cnab_remessa_item (
    id CHAR(36) NOT NULL,
    tb_cnab_remessa_id CHAR(36) NOT NULL,
    conteudo_linha VARCHAR(240) NOT NULL,
    ordem INT NOT NULL,
    data_criacao DATETIME NOT NULL,
    CONSTRAINT pk_tb_cnab_remessa_item PRIMARY KEY (id),
    CONSTRAINT fk_tb_cnab_remessa_item_tb_cnab_remessa_id
        FOREIGN KEY (tb_cnab_remessa_id)
        REFERENCES tb_cnab_remessa (id)
);


create TABLE tb_colaborador_pagamento_cnab (
    id CHAR(36) NOT NULL,
    tb_org_id INT NOT NULL,
    codigo_interno_colaborador VARCHAR(36) NOT NULL,
    tb_cnab_remessa_item_id CHAR(36) NOT NULL,
    valor_pagamento_total DECIMAL(15, 2) NOT NULL,
    forma_pagamento ENUM('PIX', 'TED') NOT NULL,
    codigo_banco CHAR(3) NOT NULL,
    agencia CHAR(5) NOT NULL,
    agencia_dv CHAR(1) NOT NULL,
    conta CHAR(12) NOT NULL,
    conta_dv CHAR(1) NOT NULL,
    chave_pix VARCHAR(100),
    tipo_chave_pix CHAR(1),
    status_cnab ENUM('AGUARDANDO', 'ENVIADO', 'REJEITADO', 'PAGO') NOT NULL,
    hash_pagamento VARCHAR(64) NOT NULL,
    data_criacao DATETIME NOT NULL,
    data_atualizacao DATETIME NOT NULL,
    CONSTRAINT pk_tb_colaborador_pagamento_cnab PRIMARY KEY (id),
    CONSTRAINT fk_tb_colaborador_pagamento_cnab_tb_org_id
        FOREIGN KEY (tb_org_id)
        REFERENCES tb_org (id),
    CONSTRAINT fk_tb_colaborador_pagamento_cnab_codigo_interno_colaborador
        FOREIGN KEY (codigo_interno_colaborador)
        REFERENCES tb_colaborador (codigo_interno_colaborador),
    CONSTRAINT fk_tb_colaborador_pagamento_cnab_tb_cnab_remessa_item_id
        FOREIGN KEY (tb_cnab_remessa_item_id)
        REFERENCES tb_cnab_remessa_item (id)
);


-- Feito separado pois colaborador pagamento pode ter outras "origens" no futuro
CREATE TABLE tb_colaborador_pagamento_cnab_solicitacao_pagamento (
    id CHAR(36) NOT NULL,
    tb_colaborador_pagamento_cnab_id CHAR(36) NOT NULL,
    tb_solicitacao_pagamento_id INT NOT NULL,
    valor_pagamento DECIMAL(15, 2) NOT NULL,
    CONSTRAINT pk_tb_colaborador_pagamento_cnab_solicitacao_pagamento
        PRIMARY KEY (id),
    CONSTRAINT fk_tb_colab_pag_cnab_sol_pag_cnab_id
        FOREIGN KEY (tb_colaborador_pagamento_cnab_id)
        REFERENCES tb_colaborador_pagamento_cnab (id),
    CONSTRAINT fk_tb_colab_pag_cnab_sol_pag_solicitacao_id
        FOREIGN KEY (tb_solicitacao_pagamento_id)
        REFERENCES tb_solicitacao_pagamento (id)
);


