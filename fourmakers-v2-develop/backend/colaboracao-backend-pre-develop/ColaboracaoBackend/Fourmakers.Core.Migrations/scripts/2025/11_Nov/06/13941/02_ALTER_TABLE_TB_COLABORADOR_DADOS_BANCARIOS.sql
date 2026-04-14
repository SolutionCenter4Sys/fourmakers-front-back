ALTER TABLE tb_colaborador_dados_bancarios
ADD COLUMN forma_pagamento ENUM('PIX', 'TED') NOT NULL;