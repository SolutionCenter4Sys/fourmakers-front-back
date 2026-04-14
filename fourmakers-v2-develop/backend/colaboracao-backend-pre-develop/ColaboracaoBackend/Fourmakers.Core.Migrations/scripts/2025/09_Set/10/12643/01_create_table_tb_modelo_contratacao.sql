CREATE TABLE tb_modelo_contratacao_org (
   id VARCHAR(36) NOT NULL,                               -- GUID
   codigo_modelo_contratacao VARCHAR(36) NOT NULL,        -- Código único do modelo
   descricao VARCHAR(100),                                -- Descrição (ex.: CLT, PJ, Estágio, etc.)
   tb_org_id INT NOT NULL,                                -- Referência à organização
   deve_criar_nf TINYINT(1) DEFAULT 0,
   PRIMARY KEY (id),
   UNIQUE KEY uq_modelo_contratacao (codigo_modelo_contratacao, tb_org_id), -- Evita duplicidade dentro da mesma org
   CONSTRAINT fk_modelo_contratacao_org_tb_org
       FOREIGN KEY (tb_org_id)
           REFERENCES tb_org (id)
);