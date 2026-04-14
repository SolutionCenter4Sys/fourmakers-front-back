ALTER TABLE tb_colaborador_pagamento_cnab
ADD COLUMN descricao_erro VARCHAR(255) DEFAULT NULL AFTER codigo_ocorrencia;