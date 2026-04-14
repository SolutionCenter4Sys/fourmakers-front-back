CREATE TABLE IF NOT EXISTS tb_agenda_convidados (
    id INT NOT NULL AUTO_INCREMENT,
    tb_agendas_comerciais_id INT NOT NULL,
    codigo_colaborador_interno_externo VARCHAR(36) NOT NULL COMMENT 'Codigo Interno Colaborador ou Codigo Externo',
    tipo_codigo INT COMMENT '1-CodigoColaborador, 2-Codigo Externo',
    tb_status_app_id INT DEFAULT 0 COMMENT '0-Pendente, 1-Aceito, 2-Recusado)',
    data_convite  DATETIME,
    data_resposta DATETIME,
    PRIMARY KEY (id),
    UNIQUE  KEY idx_tb_agenda_convidados_agenda_codigo (tb_agendas_comerciais_id, codigo_colaborador_interno_externo),
    CONSTRAINT fk_convidado_status FOREIGN KEY (tb_status_app_id)  REFERENCES   tb_status_app(id),
    CONSTRAINT fk_convidado_agenda FOREIGN KEY (tb_agendas_comerciais_id) REFERENCES   tb_agendas_comerciais(id) 
) 
ENGINE=InnoDB 
DEFAULT CHARSET=utf8mb4 
COLLATE=utf8mb4_unicode_ci;
