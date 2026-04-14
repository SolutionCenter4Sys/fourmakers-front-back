-- =============================================================================
-- 16743 — Carga fixa (IDs estáveis para referência em API/código)
-- =============================================================================
INSERT INTO tb_nivel_vaga (id, descricao, codigo) VALUES
  ('a1000000-0000-4000-8000-000000000001', 'Júnior', 1),
  ('a1000000-0000-4000-8000-000000000002', 'Pleno', 2),
  ('a1000000-0000-4000-8000-000000000003', 'Sênior', 3),
  ('a1000000-0000-4000-8000-000000000004', 'Especialista', 4)
ON DUPLICATE KEY UPDATE descricao = VALUES(descricao);
