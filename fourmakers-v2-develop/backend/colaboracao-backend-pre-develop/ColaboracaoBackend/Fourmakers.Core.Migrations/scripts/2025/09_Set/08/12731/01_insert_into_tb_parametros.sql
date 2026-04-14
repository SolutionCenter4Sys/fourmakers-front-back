INSERT INTO tb_parametro (id, nome_parametro, descricao_parametro, codigo_parametro, codigo_modulo_sistema, data_criacao, data_alteracao, ativo, tipo_parametro, tb_usuario_id_criacao, tb_usuario_id_alteracao)
VALUES(UUID(), 'Configuração para definir se mostra ou não email de aniversariantes', 'Permitir configurar ocultamento do email em aniversariantes', 'ANIVERSARIANTE_OCULTA_EMAIL', 'GESTAO', NULL, NULL, 1, 'BACKEND', NULL, NULL);

INSERT INTO tb_parametro (id, nome_parametro, descricao_parametro, codigo_parametro, codigo_modulo_sistema, data_criacao, data_alteracao, ativo, tipo_parametro, tb_usuario_id_criacao, tb_usuario_id_alteracao)
VALUES(UUID(), 'Configuração para definir a quantidade de dias a partir da hoje para listar colaboradores na tela de aniversariantes', 'Permitir configurar dias para aniversariantes', 'ANIVERSARIANTE_PROXIMOS_DIAS_QUANTIDADE', 'GESTAO', NULL, NULL, 1, 'BACKEND', NULL, NULL);
