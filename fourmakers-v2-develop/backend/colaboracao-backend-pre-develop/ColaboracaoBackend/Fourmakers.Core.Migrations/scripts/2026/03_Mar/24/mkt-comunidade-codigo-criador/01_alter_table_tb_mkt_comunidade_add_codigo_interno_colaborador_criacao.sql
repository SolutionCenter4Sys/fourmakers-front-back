-- Persiste o criador da comunidade (regra: moderador não-criador não pode removê-lo da moderação).
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_comunidade
    ADD COLUMN codigo_interno_colaborador_criacao VARCHAR(36) NULL
    AFTER data_criacao;

UPDATE tb_mkt_comunidade c
INNER JOIN (
    SELECT tb_mkt_comunidade_id, codigo_interno_colaborador
    FROM (
        SELECT
            tb_mkt_comunidade_id,
            codigo_interno_colaborador,
            ROW_NUMBER() OVER (PARTITION BY tb_mkt_comunidade_id ORDER BY data_criacao ASC, id ASC) AS rn
        FROM tb_mkt_comunidade_moderador
    ) t
    WHERE rn = 1
) m ON m.tb_mkt_comunidade_id = c.id
SET c.codigo_interno_colaborador_criacao = m.codigo_interno_colaborador
WHERE c.codigo_interno_colaborador_criacao IS NULL;
