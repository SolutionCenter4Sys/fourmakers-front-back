ALTER TABLE tb_acessos_pasta_rede_template_pdf_rel
    ADD COLUMN leitura TINYINT NOT NULL DEFAULT 0 AFTER tb_acessos_pasta_rede_template_pdf_id,
    ADD COLUMN escrita TINYINT NOT NULL DEFAULT 0 AFTER leitura;

CREATE UNIQUE INDEX ux_pasta_usr
ON tb_acessos_pasta_rede_template_pdf_rel
(tb_acessos_usuario_template_pdf_id, tb_acessos_pasta_rede_template_pdf_id);

ALTER TABLE tb_acessos_pasta_rede_template_pdf
    DROP COLUMN leitura,
    DROP COLUMN escrita;