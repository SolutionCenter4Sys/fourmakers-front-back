INSERT INTO 
`tb_parametro` (`id`, `nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `data_criacao`, `data_alteracao`, `ativo`, `tipo_parametro`,`tb_usuario_id_criacao`, `tb_usuario_id_alteracao`)
VALUES 
(uuid(), 'Habilitar Relatórios Apontamento Simplificado', 'Esta parametrização altera a procedure de extração de relatório, em vez de utilizar a procedure SPR_RPT_RELATORIO_APONTAMENTO, vai utilizar a procedure SPR_RPT_RELATORIO_APONTAMENTO', 'HABILITAR_RELATORIO_APONTAMENTO_SIMPLIFICADO', 'MAPA_DE_ALOCACAO', '2024-08-08 19:29:00', '2024-08-08 19:29:00', '1', 'BACKEND','1187','1187');

INSERT INTO `tb_parametro_configuracao` (`id`, `tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`, `tb_usuario_id_criacao`, `tb_usuario_id_alteracao`) 
VALUES (UUID(), '6', 'HABILITAR_RELATORIO_APONTAMENTO_SIMPLIFICADO', 'true', '3', '1187', '1187');

