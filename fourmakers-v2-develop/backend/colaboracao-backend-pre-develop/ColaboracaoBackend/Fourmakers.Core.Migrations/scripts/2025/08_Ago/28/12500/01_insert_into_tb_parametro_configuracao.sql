INSERT INTO
    `tb_parametro` (`id`, `nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`)
VALUES
    (uuid(), 'Horas excedentes apontamento Rotina', 'Esta parametrização fornece as horas que são excetendes para o disparo de emails a destinatarios', 'CONFIGURACAO_APONTAMENTO_EXCEDENTE_VALOR', 'TIMESHEET', '1', 'BACKEND');

INSERT INTO `tb_parametro_configuracao` (`id`, `tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(), '6', 'CONFIGURACAO_APONTAMENTO_EXCEDENTE_VALOR', '220', '3');

INSERT INTO
    `tb_parametro` (`id`, `nome_parametro`, `descricao_parametro`, `codigo_parametro`, `codigo_modulo_sistema`, `ativo`, `tipo_parametro`)
VALUES
    (uuid(), 'Emails destinatarios para disparo de apontamentos excedentes', 'Esta parametrização fornece os emails destinatarios do apontamento excedente', 'APONTAMENTO_EXCEDENTE_DESTINATARIOS', 'TIMESHEET', '1', 'BACKEND');

INSERT INTO `tb_parametro_configuracao` (`id`, `tb_org_id`, `codigo_parametro`, `valor_parametro`, `tb_parametro_nivel_id`)
VALUES (UUID(), '6', 'APONTAMENTO_EXCEDENTE_DESTINATARIOS', 'cramos@novacoop.com.br;mariana.ribeiro@atos.net;csilva@bwg.com.br;isabel.ramos@atos.net', '3');
