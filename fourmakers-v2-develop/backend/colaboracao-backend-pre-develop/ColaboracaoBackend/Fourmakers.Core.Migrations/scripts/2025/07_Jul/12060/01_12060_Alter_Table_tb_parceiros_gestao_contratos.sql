ALTER TABLE tb_parceiros_gestao_contratos 
		ADD COLUMN nr_pagina   int NULL,
        ADD COLUMN vr_contrato decimal(15,2),
        ADD COLUMN cd_contrato VARCHAR(40),
        ADD COLUMN cd_contrato_anterior VARCHAR(40),
        ADD COLUMN cd_cotacao_relacionada VARCHAR(40),
        ADD COLUMN cd_status VARCHAR(30),
        ADD COLUMN cd_necessidade_adicional VARCHAR(40),
        ADD COLUMN cd_plataforma_digital VARCHAR(40),
		ADD COLUMN ds_reajuste_anual TEXT,
		ADD COLUMN cd_renovado VARCHAR(05);
