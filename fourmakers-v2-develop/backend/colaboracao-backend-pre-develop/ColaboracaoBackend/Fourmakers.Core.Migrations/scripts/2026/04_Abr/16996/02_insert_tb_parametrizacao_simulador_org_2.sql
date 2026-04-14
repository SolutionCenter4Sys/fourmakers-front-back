-- =============================================================================
-- Carga inicial: parametrização do simulador para tb_org_id = 2
-- Reexecução: atualiza os mesmos campos se a linha já existir (PK tb_org_id).
-- =============================================================================
INSERT INTO tb_parametrizacao_simulador (
  tb_org_id,
  porcentagem_minima_piso,
  porcentagem_excedente_custo,
  porcentagem_margem_custo,
  quantidade_maxima_calculos,
  quantidade_horas_custo
) VALUES (
  2,
  25.0000,
  10.0000,
  0.1000,
  1000,
  168
)
ON DUPLICATE KEY UPDATE
  porcentagem_minima_piso = VALUES(porcentagem_minima_piso),
  porcentagem_excedente_custo = VALUES(porcentagem_excedente_custo),
  porcentagem_margem_custo = VALUES(porcentagem_margem_custo),
  quantidade_maxima_calculos = VALUES(quantidade_maxima_calculos),
  quantidade_horas_custo = VALUES(quantidade_horas_custo);
