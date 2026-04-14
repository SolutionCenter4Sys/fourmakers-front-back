INSERT INTO
	tb_parametro (`id`,
				  `nome_parametro`,
                  `descricao_parametro`,
                  `codigo_parametro`,
                  `codigo_modulo_sistema`,
                  `ativo`, `tipo_parametro`,
                  `tb_usuario_id_criacao`)
VALUES 
	(UUID(),
    'Deve incluir inativos no Mapa de Alocação',
    'Este paramêtro indica que a Tela inicial do Mapa e Relatório de Extração do Mapa, devem apresentar também quem está com status inativado.',
    'DEVE_INCLUIR_INATIVOS_MAPA_ALOCACAO',
    'MAPA_DE_ALOCACAO',
    '1',
    'BACKEND',
    '1187');

INSERT INTO
	`tb_parametro_configuracao` (`id`,
								 `tb_org_id`,
                                 `codigo_parametro`,
                                 `valor_parametro`,
                                 `tb_parametro_nivel_id`,
                                 `tb_usuario_id_criacao`)
VALUES 
	(UUID(),
    '2',
    'DEVE_INCLUIR_INATIVOS_MAPA_ALOCACAO',
    'true',
    '3',
    '1187');