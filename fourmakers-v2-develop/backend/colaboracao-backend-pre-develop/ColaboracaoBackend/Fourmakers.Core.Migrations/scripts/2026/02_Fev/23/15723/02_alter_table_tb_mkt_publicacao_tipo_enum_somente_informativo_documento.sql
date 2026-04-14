-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Restringe tipo de publicacao a informativo e documento
-- Data: 2026-02-24
-- Pasta: scripts/2026/02_Fev/23/15723
-- Remove valor 'comunidade' do ENUM tipo em tb_mkt_publicacao.
-- Publicacoes de comunidade passam a ser identificadas por tb_mkt_comunidade_publicacao + tipo informativo/documento.
-- ------------------------------------------------------------------------------

-- Converte registros existentes com tipo 'comunidade' para 'informativo' antes de alterar o ENUM
UPDATE tb_mkt_publicacao
SET tipo = 'informativo'
WHERE tipo = 'comunidade';

-- Restringe o ENUM tipo a apenas 'informativo' e 'documento'
ALTER TABLE tb_mkt_publicacao
    MODIFY COLUMN tipo ENUM('informativo','documento') NOT NULL;
