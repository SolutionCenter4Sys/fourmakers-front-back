DROP TABLE tb_banco_talentos_processamento_lote;

CREATE TABLE tb_banco_talentos_processamento_lote (
  tb_banco_talentos_codigo_interno_colaborador varchar(255) DEFAULT NULL,
  tb_processamento_curriculo_lote_id varchar(36) NOT NULL,
  message_receipt_handle varchar(500) NOT NULL,
  mensagem_erro text DEFAULT NULL,
  stack_trace text DEFAULT NULL,
  data_ocorrencia_erro timestamp NULL DEFAULT NULL,
  body_mensagem longtext DEFAULT NULL,
  processado_com_erro boolean DEFAULT FALSE,
  PRIMARY KEY (tb_processamento_curriculo_lote_id, message_receipt_handle)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;