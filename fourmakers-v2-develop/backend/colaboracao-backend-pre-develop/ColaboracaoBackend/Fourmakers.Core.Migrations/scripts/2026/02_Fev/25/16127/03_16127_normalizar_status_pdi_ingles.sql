-- ------------------------------------------------------------------------------
-- PDI: Normaliza status para inglês (Evolução 16127)
-- Garante um único idioma (inglês) na coluna status da tb_pdi.
-- Valores: NOT_STARTED, IN_ANALYSIS, IN_PROGRESS, COMPLETED, CANCELLED
-- ------------------------------------------------------------------------------

UPDATE tb_pdi SET status = 'IN_ANALYSIS' WHERE status IN ('em_analise', 'Em análise', 'EM_ANALISE');
UPDATE tb_pdi SET status = 'IN_PROGRESS' WHERE status IN ('em_andamento', 'Em andamento', 'EM_ANDAMENTO');
UPDATE tb_pdi SET status = 'COMPLETED' WHERE status IN ('finalizado', 'Finalizado', 'FINALIZADO');
UPDATE tb_pdi SET status = 'CANCELLED' WHERE status IN ('cancelado', 'Cancelado', 'CANCELADO');
UPDATE tb_pdi SET status = 'NOT_STARTED' WHERE status IN ('não iniciado', 'nao iniciado', 'Nao iniciado');
