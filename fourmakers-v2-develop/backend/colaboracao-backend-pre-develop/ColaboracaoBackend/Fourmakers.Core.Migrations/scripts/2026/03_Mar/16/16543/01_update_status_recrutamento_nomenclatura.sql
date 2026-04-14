-- =============================================================================
-- 2026-03-16 - Fourmakers - Atualização de nomenclaturas
-- Status Candidatura e Status Vaga (Recrutamento)
--
-- Alterações:
-- 1. "Análise do gestor" / "Analise do Gestor" -> "Análise do CV pelo Gestor"
-- 2. "Entrevista com Cliente" -> "Entrevista com Gestor da Vaga"
-- =============================================================================

-- -----------------------------------------------------------------------------
-- tb_candidato_status (status da CANDIDATURA no recrutamento)
-- PK: (id, origem). Origem 'Fourmakers' é a utilizada no recrutamento.
-- -----------------------------------------------------------------------------

-- Status id 4: Análise do gestor -> Análise do CV pelo Gestor
UPDATE tb_candidato_status
SET descricao = 'Análise do CV pelo Gestor'
WHERE id = 4
  AND origem = 'Fourmakers';

-- Status id 7: Entrevista com Cliente -> Entrevista com Gestor da Vaga
UPDATE tb_candidato_status
SET descricao = 'Entrevista com Gestor da Vaga'
WHERE id = 7
  AND origem = 'Fourmakers';


-- -----------------------------------------------------------------------------
-- tb_status_vaga (status da VAGA no recrutamento)
-- Identificação pelo codigo (6 = Entrevista com Cliente).
-- -----------------------------------------------------------------------------

-- Código 6: Entrevista com Cliente -> Entrevista com Gestor da Vaga
UPDATE tb_status_vaga
SET descricao = 'Entrevista com Gestor da Vaga'
WHERE codigo = 6;


-- -----------------------------------------------------------------------------
-- Verificação (opcional - descomente para conferir após executar)
-- -----------------------------------------------------------------------------
-- SELECT id, descricao, origem FROM tb_candidato_status WHERE id IN (4, 7) AND origem = 'Fourmakers';
-- SELECT id, codigo, descricao, ordem FROM tb_status_vaga WHERE codigo = 6;
