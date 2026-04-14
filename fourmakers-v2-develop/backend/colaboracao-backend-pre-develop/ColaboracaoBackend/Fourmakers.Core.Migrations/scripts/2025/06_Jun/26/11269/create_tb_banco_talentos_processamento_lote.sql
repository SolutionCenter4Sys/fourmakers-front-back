CREATE TABLE tb_banco_talentos_processamento_lote (
  `tb_banco_talentos_codigo_interno_colaborador` varchar(255) NOT NULL,
  `tb_processamento_curriculo_lote_id` varchar(36) NOT NULL,
  PRIMARY KEY (`tb_banco_talentos_codigo_interno_colaborador`, `tb_processamento_curriculo_lote_id`)
);
