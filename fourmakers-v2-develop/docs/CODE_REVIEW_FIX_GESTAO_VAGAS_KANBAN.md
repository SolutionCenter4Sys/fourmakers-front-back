# Auditoria — fix/gestao-vagas-kanban

**Branch:** `fix/gestao-vagas-kanban`  
**Escopo:** Ajustes na jornada /gestaodevagas (Kanban e filtros)  
**Referências:** ARCHITECTURE.md, DESIGN_SYSTEM_AUDIT.md

---

## 1. Resumo dos ajustes

| Ajuste | Arquivo | Descrição |
|--------|---------|-----------|
| Busca por código da vaga | `useGestaoVagas.ts` | Inclusão de `vaga.codigo` no filtro client-side (`vagasFiltradas`). Ao buscar pelo código (ex.: "508"), a vaga retornada pela API passa a aparecer no Kanban. |
| Labels do filtro de datas | `GestaoVagas.tsx` | "Data Início" → "Vagas criadas de"; "Data Fim" → "Até". Deixa explícito que o período refere-se à data de criação das vagas. |
| Ordem das colunas de status | `useGestaoVagas.ts` | Removido `sort` por código; a ordem das colunas passa a seguir a ordem retornada pela API `ListarStatusVagaRecrutamento`. |

---

## 2. Conformidade

- **Arquitetura:** Sem alteração de camadas; uso de Use Cases e estado local mantidos.
- **Design System:** Apenas alteração de labels (texto); componentes e tokens inalterados.
- **Regressões:** Nenhuma. Busca vazia continua retornando todas as vagas; filtro por `ativo` mantido na lista de status.

---

## 3. Changelog

| Data | Branch | Observação |
|------|--------|------------|
| Fev 2026 | fix/gestao-vagas-kanban | Busca por código; labels "Vagas criadas de" / "Até"; ordem das colunas conforme API ListarStatusVagaRecrutamento. |
