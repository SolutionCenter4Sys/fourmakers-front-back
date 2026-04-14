-- ------------------------------------------------------------------------------
-- PDI: Corrige constraint única em pdi_skill (Evolução 16053)
-- A regra de negócio é: o mesmo codigo_skill não pode repetir DENTRO do mesmo PDI.
-- A constraint uk_pdi_skill_codigo (codigo_skill) impedia o mesmo código em PDIs diferentes.
-- Deve ser única por (pdi_id, codigo_skill).
-- ------------------------------------------------------------------------------

-- Remove a única global em codigo_skill
ALTER TABLE pdi_skill DROP INDEX uk_pdi_skill_codigo;

-- Adiciona única por (pdi_id, codigo_skill): mesma skill não pode repetir no mesmo PDI
ALTER TABLE pdi_skill ADD UNIQUE KEY uk_pdi_skill_pdi_codigo (pdi_id, codigo_skill);
