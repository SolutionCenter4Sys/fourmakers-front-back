-- =============================================================================
-- tb_parametrizacao_simulador — parâmetros do simulador por organização (1 linha / org)
-- Dependências: tb_org
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_parametrizacao_simulador (
  tb_org_id INT NOT NULL COMMENT 'Organização (tb_org)',
  porcentagem_minima_piso DECIMAL(10,4) NULL,
  porcentagem_excedente_custo DECIMAL(10,4) NULL,
  porcentagem_margem_custo DECIMAL(10,4) NULL,
  quantidade_maxima_calculos INT NULL,
  quantidade_horas_custo INT NULL,
  PRIMARY KEY (tb_org_id),
  CONSTRAINT fk_tb_parametrizacao_simulador_org
    FOREIGN KEY (tb_org_id) REFERENCES tb_org (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Parametrização do simulador por organização';
