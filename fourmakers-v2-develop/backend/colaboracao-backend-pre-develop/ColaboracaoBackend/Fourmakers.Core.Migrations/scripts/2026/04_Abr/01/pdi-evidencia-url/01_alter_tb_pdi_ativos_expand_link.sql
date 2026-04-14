-- PDI evidências: campo de API multipart/JSON <link> persiste na coluna link; amplia tamanho para URLs longas.
ALTER TABLE tb_pdi_ativos MODIFY COLUMN link VARCHAR(2048) NULL;
