CREATE TABLE tb_cnab_retorno (
    id CHAR(36) NOT NULL,
    tb_cnab_remessa_id CHAR(36) NOT NULL,
    hash_remessa VARCHAR(64),
    nome_arquivo VARCHAR(255),
    data_processamento DATETIME,
    usuario_processamento VARCHAR(100),
    CONSTRAINT pk_tb_cnab_retorno
        PRIMARY KEY (id),
    CONSTRAINT fk_cnab_retorno_cnab_remessa
        FOREIGN KEY (tb_cnab_remessa_id)
        REFERENCES tb_cnab_remessa (id)
);

CREATE TABLE tb_cnab_retorno_item (
    id CHAR(36) NOT NULL,
    tb_cnab_retorno_id CHAR(36) NOT NULL,
    conteudo_linha VARCHAR(240),
    ordem INT,
    data_criacao DATETIME,
    CONSTRAINT pk_tb_cnab_retorno_item
        PRIMARY KEY (id),
    CONSTRAINT fk_cnab_retorno_item_cnab_retorno
        FOREIGN KEY (tb_cnab_retorno_id)
        REFERENCES tb_cnab_retorno (id)
);

-- ALTER TABLE tb_cnab_remessa
     -- ADD COLUMN valor_total DECIMAL(15, 2) NOT NULL DEFAULT 0.00 AFTER descricao;
	 
	 -- -- Adicionar campo competencia em tb_cnab_remessa para registrar a competência da remessa
-- ALTER TABLE tb_cnab_remessa
-- ADD COLUMN competencia VARCHAR(7) NULL AFTER codigo_diretoria;

-- -- Adicionar foreign key para tb_org
-- ALTER TABLE tb_cnab_remessa
-- ADD CONSTRAINT IF NOT EXISTS fk_tb_cnab_remessa_tb_org_id
    -- FOREIGN KEY (tb_org_id)
    -- REFERENCES tb_org (id);

	 
-- -- Adicionar campo tb_org_id se não existir
-- ALTER TABLE tb_cnab_remessa
-- ADD COLUMN IF NOT EXISTS tb_org_id INT NOT NULL AFTER tipo;
