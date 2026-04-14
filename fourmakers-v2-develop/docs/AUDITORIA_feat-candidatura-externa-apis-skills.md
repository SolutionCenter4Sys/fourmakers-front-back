# Auditoria — feat/candidatura-externa (APIs por categoria no passo 2)

**Branch:** feat/candidatura-externa  
**Escopo:** Ajuste dos payloads das APIs de adicionar skill ao perfil 360 (passo 2 – “Sim, já possuo…”).  
**Data:** 2026-03-09

---

## 1. Resumo

| Critério        | Status | Resumo |
|-----------------|--------|--------|
| **Endpoints**   | ✅ OK  | HardSkill, SoftSkill, Idioma, Metodologia e Dominio usam os endpoints corretos em CompetenciasApi. |
| **Payloads**     | ✅ OK  | HardSkill e Metodologia passam a enviar body sem `cpf`; SoftSkill, Dominio e Idioma com `cpf`; Idioma com `idiomaId`. |
| **Build**       | ✅ OK  | `npm run build:homolog` concluído com sucesso. |
| **Lint (alterados)** | ✅ OK  | Ajuste no case `idioma` (bloco com chaves) para satisfazer `no-case-declarations`. |

**Veredicto:** Conforme. Payloads alinhados aos exemplos de API (curl) por categoria.

---

## 2. Alterações

| Arquivo | Alteração |
|---------|-----------|
| `src/data/api/CompetenciasApi.ts` | Tipos de `adicionarHardSkillColaborador` e `adicionarMetodologiaColaborador`: removido `cpf` do body (API não espera). |
| `src/data/repositories/MinhaJornadaRepositoryImpl.ts` | Em `adicionarSkillPerfil360`: HardSkill e Metodologia enviam items sem `cpf`; case `idioma` envolvido em `{}` para declaração lexical. |

---

## 3. Payload por categoria (referência)

- **HardSkill:** `id`, `descricao`, `nivelId`, `gestorExternoPerfil`
- **SoftSkill:** `id`, `descricao`, `nivelId`, `cpf`, `gestorExternoPerfil`
- **Idioma:** `idiomaId`, `descricao`, `nivelId`, `cpf`, `gestorExternoPerfil`
- **Metodologia:** `id`, `descricao`, `nivelId`, `gestorExternoPerfil`
- **Dominio:** `id`, `descricao`, `nivelId`, `cpf`, `gestorExternoPerfil`

Todos com `?minhaJornada=false`.
