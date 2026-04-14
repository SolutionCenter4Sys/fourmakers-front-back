-- Especificação livre quando tb_feedback360_relacionamento_id = Outro (ex.: cliente, gestor de outra área).
ALTER TABLE tb_feedback360
    ADD COLUMN relacionamento_outro VARCHAR(60) NULL
        COMMENT 'Texto quando relacionamento é Outro (até 60 caracteres)'
        AFTER tb_feedback360_relacionamento_id;
