INSERT INTO tb_parametro_configuracao
(id, tb_org_id, codigo_interno_colaborador, tb_grupo_acesso_id, codigo_parametro, valor_parametro, tb_parametro_nivel_id, tb_usuario_id_criacao, tb_usuario_id_alteracao)
VALUES(UUID(), 9, NULL, NULL, 'TIMESHEET_APRESENTAR_ESPELHO_PONTO', 'true', 3, NULL, NULL);


INSERT INTO tb_parametro_configuracao
(id, tb_org_id, codigo_interno_colaborador, tb_grupo_acesso_id, codigo_parametro, valor_parametro, tb_parametro_nivel_id, tb_usuario_id_criacao, tb_usuario_id_alteracao)
VALUES(UUID(), 6, NULL, NULL, 'TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_APROVAR', 'true', 3, NULL, NULL);

INSERT INTO tb_parametro_configuracao
(id, tb_org_id, codigo_interno_colaborador, tb_grupo_acesso_id, codigo_parametro, valor_parametro, tb_parametro_nivel_id, tb_usuario_id_criacao, tb_usuario_id_alteracao)
VALUES
    (UUID(), 4, NULL, NULL, 'TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_REPROVAR', 'true', 3, NULL, NULL),
    (UUID(), 5, NULL, NULL, 'TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_REPROVAR', 'true', 3, NULL, NULL),
    (UUID(), 6, NULL, NULL, 'TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_REPROVAR', 'true', 3, NULL, NULL),
    (UUID(), 8, NULL, NULL, 'TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_REPROVAR', 'true', 3, NULL, NULL);