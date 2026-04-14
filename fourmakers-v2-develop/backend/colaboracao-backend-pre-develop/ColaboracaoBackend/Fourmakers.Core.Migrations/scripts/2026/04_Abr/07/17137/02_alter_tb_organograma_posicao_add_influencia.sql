-- =============================================================================
-- tb_organograma_posicao — influência obrigatória no mapa de relacionamento
-- Dependências: tb_mapa_relacionamento_influencia (script 01)
-- Linhas existentes sem valor recebem Operacional (id = 4) antes do NOT NULL.
-- =============================================================================
ALTER TABLE tb_organograma_posicao
  ADD COLUMN tb_mapa_relacionamento_influencia_id INT NULL
    COMMENT 'tb_mapa_relacionamento_influencia.id (obrigatório após carga)'
    AFTER profissional_externo;

UPDATE tb_organograma_posicao
SET tb_mapa_relacionamento_influencia_id = 4
WHERE tb_mapa_relacionamento_influencia_id IS NULL;

ALTER TABLE tb_organograma_posicao
  MODIFY COLUMN tb_mapa_relacionamento_influencia_id INT NOT NULL
    COMMENT 'tb_mapa_relacionamento_influencia.id',
  ADD KEY fk_tb_organograma_posicao_tb_mapa_rel_influencia (tb_mapa_relacionamento_influencia_id),
  ADD CONSTRAINT fk_tb_organograma_posicao_tb_mapa_rel_influencia
    FOREIGN KEY (tb_mapa_relacionamento_influencia_id)
    REFERENCES tb_mapa_relacionamento_influencia (id)
    ON DELETE RESTRICT
    ON UPDATE CASCADE;
