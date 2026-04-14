CREATE INDEX idx_orcamento_posicao_lookup
ON tb_organograma_posicao_orcamento
(tb_organograma_posicao_id, data_criacao DESC, id DESC);