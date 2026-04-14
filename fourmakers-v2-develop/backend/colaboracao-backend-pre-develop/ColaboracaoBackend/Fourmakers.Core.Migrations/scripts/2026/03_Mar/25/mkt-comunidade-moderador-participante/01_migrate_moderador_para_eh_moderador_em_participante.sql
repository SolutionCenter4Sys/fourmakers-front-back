-- Moderador da comunidade passa a ser flag em tb_mkt_comunidade_usuario_participando (eh_moderador).
-- Remove tb_mkt_comunidade_moderador após copiar dados.
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_comunidade_usuario_participando
    ADD COLUMN eh_moderador TINYINT(1) NOT NULL DEFAULT 0 COMMENT '1 = moderador da comunidade'
    AFTER codigo_interno_colaborador;

UPDATE tb_mkt_comunidade_usuario_participando cup
INNER JOIN tb_mkt_comunidade_moderador m
    ON m.tb_mkt_comunidade_id = cup.tb_mkt_comunidade_id
    AND m.codigo_interno_colaborador = cup.codigo_interno_colaborador
SET cup.eh_moderador = 1;

INSERT INTO tb_mkt_comunidade_usuario_participando (id, tb_mkt_comunidade_id, codigo_interno_colaborador, eh_moderador)
SELECT m.id, m.tb_mkt_comunidade_id, m.codigo_interno_colaborador, 1
FROM tb_mkt_comunidade_moderador m
WHERE NOT EXISTS (
    SELECT 1 FROM tb_mkt_comunidade_usuario_participando cup
    WHERE cup.tb_mkt_comunidade_id = m.tb_mkt_comunidade_id
      AND cup.codigo_interno_colaborador = m.codigo_interno_colaborador
);

DROP TABLE IF EXISTS tb_mkt_comunidade_moderador;
