-- ------------------------------------------------------------------------------
-- PDI: Colunas de auditoria criacao/alteracao (Evolução 16127)
-- tb_pdi e tb_pdi_plano_acao passam a registrar quem criou e quem alterou.
-- ------------------------------------------------------------------------------

-- tb_pdi
ALTER TABLE tb_pdi
    ADD COLUMN codigo_interno_colaborador_criacao CHAR(36) NULL
        COMMENT 'Quem criou o PDI (tb_colaborador.codigo_interno_colaborador)',
    ADD COLUMN codigo_interno_colaborador_alteracao CHAR(36) NULL
        COMMENT 'Quem fez a última alteração';

ALTER TABLE tb_pdi
    ADD CONSTRAINT fk_tb_pdi_criacao_colaborador
        FOREIGN KEY (codigo_interno_colaborador_criacao)
        REFERENCES tb_colaborador (codigo_interno_colaborador),
    ADD CONSTRAINT fk_tb_pdi_alteracao_colaborador
        FOREIGN KEY (codigo_interno_colaborador_alteracao)
        REFERENCES tb_colaborador (codigo_interno_colaborador);

-- tb_pdi_plano_acao
ALTER TABLE tb_pdi_plano_acao
    ADD COLUMN codigo_interno_colaborador_criacao CHAR(36) NULL
        COMMENT 'Quem criou o plano de ação',
    ADD COLUMN codigo_interno_colaborador_alteracao CHAR(36) NULL
        COMMENT 'Quem concluiu ou alterou por último';

ALTER TABLE tb_pdi_plano_acao
    ADD CONSTRAINT fk_tb_pdi_plano_acao_criacao_colaborador
        FOREIGN KEY (codigo_interno_colaborador_criacao)
        REFERENCES tb_colaborador (codigo_interno_colaborador),
    ADD CONSTRAINT fk_tb_pdi_plano_acao_alteracao_colaborador
        FOREIGN KEY (codigo_interno_colaborador_alteracao)
        REFERENCES tb_colaborador (codigo_interno_colaborador);
