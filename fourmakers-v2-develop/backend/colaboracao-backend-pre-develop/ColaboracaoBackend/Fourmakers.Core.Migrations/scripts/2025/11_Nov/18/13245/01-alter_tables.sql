ALTER TABLE tb_agenda_encontros RENAME TO tb_agendas_comerciais;

ALTER TABLE tb_encontros RENAME TO tb_interacoes;

ALTER TABLE tb_encontros_participantes RENAME TO tb_agenda_participantes;

ALTER TABLE tb_encontros_archives RENAME TO tb_interacoes_archives;

ALTER TABLE tb_encontros_ai RENAME TO tb_interacao_ai;

ALTER TABLE tb_encontros_ai_passos RENAME TO tb_interacao_acoes;

ALTER TABLE tb_encontros_tipo_interacao RENAME TO tb_tipo_agenda;

