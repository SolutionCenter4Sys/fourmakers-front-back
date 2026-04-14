INSERT INTO tb_parametro
(id, nome_parametro, descricao_parametro, codigo_parametro, codigo_modulo_sistema, ativo, tipo_parametro, tb_usuario_id_criacao, tb_usuario_id_alteracao)
VALUES(UUID(), 'Apresentar espelho do ponto no timesheet', 'Permitir que um usuario possa vizualizar um botão para apresentar o espelho do ponto', 'TIMESHEET_APRESENTAR_ESPELHO_PONTO', 'TIMESHEET', 1, 'FRONTEND', NULL, NULL);

INSERT INTO tb_parametro
(id, nome_parametro, descricao_parametro, codigo_parametro, codigo_modulo_sistema, ativo, tipo_parametro, tb_usuario_id_criacao, tb_usuario_id_alteracao)
VALUES(UUID(), 'Timesheet obrigar justificativa ao aprovar', 'Obrigar colocar justificativa ao aprovar', 'TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_APROVAR', 'TIMESHEET', 1, 'FRONTEND', NULL, NULL);

INSERT INTO tb_parametro
(id, nome_parametro, descricao_parametro, codigo_parametro, codigo_modulo_sistema, ativo, tipo_parametro, tb_usuario_id_criacao, tb_usuario_id_alteracao)
VALUES(UUID(), 'Timesheet obrigar justificativa ao reprovar', 'Obrigar colocar justificativa ao reprovar', 'TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_REPROVAR', 'TIMESHEET', 1, 'FRONTEND', NULL, NULL);
