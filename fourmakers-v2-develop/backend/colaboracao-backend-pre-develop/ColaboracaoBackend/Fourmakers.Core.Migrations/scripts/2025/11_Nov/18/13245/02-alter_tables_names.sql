ALTER TABLE tb_agendas_comerciais
RENAME COLUMN tb_encontros_tipo_interacao_id to tb_tipo_agenda_id;

ALTER TABLE tb_interacoes
RENAME COLUMN tb_agenda_encontros_id to tb_agendas_comerciais_id;

ALTER TABLE tb_interacao_ai
RENAME COLUMN tb_encontros_id to tb_interacoes_id;

ALTER TABLE tb_interacao_acoes
RENAME COLUMN tb_encontros_ai_id to tb_interacao_ai_id;

ALTER TABLE tb_agenda_participantes
RENAME COLUMN tb_agenda_encontros_id to tb_agendas_comerciais_id;

ALTER TABLE tb_agenda_solicitacoes_participantes
RENAME COLUMN tb_agenda_encontros_id to tb_agendas_comerciais_id;

ALTER TABLE tb_interacoes_archives
RENAME COLUMN tb_encontros_id to tb_interacoes_id;

ALTER TABLE tb_participantes_externo
RENAME COLUMN tb_agenda_encontros_id to tb_agendas_comerciais_id;

