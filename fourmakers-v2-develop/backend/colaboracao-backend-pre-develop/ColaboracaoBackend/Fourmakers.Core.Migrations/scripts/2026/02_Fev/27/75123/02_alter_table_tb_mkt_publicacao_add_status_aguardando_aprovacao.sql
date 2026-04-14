-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Adiciona status aguardando_aprovacao em publicacao
-- Data: 2026-02-27
-- Pasta: scripts/2026/02_Fev/27/75123
-- Inclui o valor 'aguardando_aprovacao' no ENUM publicacao_status da tb_mkt_publicacao.
-- Usado quando a publicacao (informativo/documento) requer aprovacao e ainda nao foi aprovada.
-- ------------------------------------------------------------------------------

ALTER TABLE tb_mkt_publicacao
    MODIFY COLUMN publicacao_status ENUM(
        'rascunho',
        'agendada',
        'aguardando_aprovacao',
        'ativa',
        'expirada',
        'arquivada',
        'excluida'
    ) NOT NULL DEFAULT 'rascunho';
