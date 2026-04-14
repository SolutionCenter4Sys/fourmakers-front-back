-- =============================================================================
-- 16932 / 2026-03-26 - Fourmakers - Nomenclatura status candidatura (id 8)
--
-- Tabela: tb_candidato_status (PK composta: id + origem)
-- Alinha descricao com o enum StatusCandidaturaRecrutamento.CartaOfertaAceita (valor 8).
-- =============================================================================

UPDATE tb_candidato_status
SET descricao = 'Carta Oferta Aceita'
WHERE id = 8
  AND origem = 'Fourmakers';

-- Verificação (opcional):
-- SELECT id, descricao, origem FROM tb_candidato_status WHERE id = 8 AND origem = 'Fourmakers';
