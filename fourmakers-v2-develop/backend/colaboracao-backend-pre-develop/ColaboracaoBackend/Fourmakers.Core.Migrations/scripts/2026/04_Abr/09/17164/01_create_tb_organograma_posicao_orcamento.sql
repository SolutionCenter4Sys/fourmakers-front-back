-- Histórico de orçamento por posição (somente INSERT; leitura do vigente = último por data_criacao)

CREATE TABLE tb_organograma_posicao_orcamento (
  id char(36) NOT NULL,
  tb_organograma_posicao_id varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  orcamento decimal(18,2) NOT NULL,
  data_inicio date DEFAULT NULL,
  data_fim date DEFAULT NULL,
  codigo_interno_colaborador_alterador varchar(36) NOT NULL,
  tb_org_id int NOT NULL,
  data_criacao timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY idx_pos_orc_pos_criacao (tb_organograma_posicao_id, data_criacao DESC, id DESC),
  KEY idx_pos_orc_org (tb_org_id),
  CONSTRAINT fk_pos_orc_pos FOREIGN KEY (tb_organograma_posicao_id) REFERENCES tb_organograma_posicao (id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_pos_orc_org FOREIGN KEY (tb_org_id) REFERENCES tb_org (id),
  CONSTRAINT fk_pos_orc_colab FOREIGN KEY (codigo_interno_colaborador_alterador) REFERENCES tb_colaborador (codigo_interno_colaborador) ON DELETE RESTRICT ON UPDATE CASCADE
);