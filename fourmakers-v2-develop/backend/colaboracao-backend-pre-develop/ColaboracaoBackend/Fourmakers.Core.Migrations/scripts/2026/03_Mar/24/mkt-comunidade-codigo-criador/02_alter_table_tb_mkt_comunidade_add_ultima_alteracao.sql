-- Último colaborador que alterou a comunidade e data da alteração.
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_comunidade
    ADD COLUMN codigo_interno_colaborador_ultima_alteracao VARCHAR(36) NULL
        AFTER codigo_interno_colaborador_criacao,
    ADD COLUMN data_ultima_alteracao DATETIME NULL
        AFTER codigo_interno_colaborador_ultima_alteracao;

UPDATE tb_mkt_comunidade
SET codigo_interno_colaborador_ultima_alteracao = codigo_interno_colaborador_criacao,
    data_ultima_alteracao = data_criacao
WHERE codigo_interno_colaborador_ultima_alteracao IS NULL
  AND codigo_interno_colaborador_criacao IS NOT NULL;
