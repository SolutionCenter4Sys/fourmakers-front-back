-- ------------------------------------------------------------------------------
-- PDI: Ajustes de nomenclatura – prefixo tb_ e coluna codigo_interno_colaborador (Evolução 16127)
-- Tabelas passam a ter prefixo tb_; em tb_pdi a coluna colaborador_id vira codigo_interno_colaborador.
-- ------------------------------------------------------------------------------

-- 1. Renomear tabela principal e alterar coluna colaborador_id
RENAME TABLE pdi TO tb_pdi;

ALTER TABLE tb_pdi
    DROP FOREIGN KEY fk_pdi_colaborador;

ALTER TABLE tb_pdi
    CHANGE COLUMN colaborador_id codigo_interno_colaborador CHAR(36) NOT NULL;

ALTER TABLE tb_pdi
    ADD CONSTRAINT fk_tb_pdi_colaborador
    FOREIGN KEY (codigo_interno_colaborador)
    REFERENCES tb_colaborador (codigo_interno_colaborador);

-- 2. Renomear tabelas filhas (FKs para pdi passam a referenciar tb_pdi automaticamente)
RENAME TABLE action_plan TO tb_pdi_plano_acao;
RENAME TABLE pdi_asset TO tb_pdi_ativos;
RENAME TABLE pdi_skill TO tb_pdi_skill;
