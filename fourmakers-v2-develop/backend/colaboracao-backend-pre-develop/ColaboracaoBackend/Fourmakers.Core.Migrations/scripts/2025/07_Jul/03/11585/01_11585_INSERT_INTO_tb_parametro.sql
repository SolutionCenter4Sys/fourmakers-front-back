INSERT INTO tb_parametro(id, codigo_modulo_sistema, codigo_parametro, descricao_parametro, nome_parametro, tipo_parametro)
VALUES
    (UUID(),'TIMESHEET','OCULTAR_MODULO_TIMESHEET_MODELO_CONTRATACAO', 'Ocultar timesheet por modelo de contratação','Ocultar modulo timesheet modelo de contratacao','FRONTEND'),
    (UUID(),'TIMESHEET','OCULTAR_CONSULTAS_TIMESHEET_MODELO_CONTRATACAO', 'Ocultar consultas em listagem, relatorios e demais sobre usuarios por modelo de contratação','Ocultar consultas timesheet modelo de contratação','BACKEND');
