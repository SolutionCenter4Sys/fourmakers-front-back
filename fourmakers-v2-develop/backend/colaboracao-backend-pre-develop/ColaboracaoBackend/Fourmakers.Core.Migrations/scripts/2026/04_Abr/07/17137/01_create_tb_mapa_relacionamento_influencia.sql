-- =============================================================================
-- tb_mapa_relacionamento_influencia — tipos de influência no mapa de relacionamento (organograma)
-- IDs fixos alinhados ao enum MapaRelacionamentoInfluencia no backend.
-- =============================================================================
CREATE TABLE IF NOT EXISTS tb_mapa_relacionamento_influencia (
  id INT NOT NULL AUTO_INCREMENT,
  descricao VARCHAR(100) NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY uk_tb_mapa_relacionamento_influencia_descricao (descricao)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO tb_mapa_relacionamento_influencia (id, descricao) VALUES
  (1, 'Decisor'),
  (2, 'Influenciador'),
  (3, 'Bloqueador'),
  (4, 'Operacional')
ON DUPLICATE KEY UPDATE descricao = VALUES(descricao);
