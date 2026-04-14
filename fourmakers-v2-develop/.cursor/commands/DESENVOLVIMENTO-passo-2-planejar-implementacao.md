# Step 2 (Planning) — PLAN based on @IMPLEMENTATION.md and @ARCHITECTURE.md

**Versao:** 1.1.0  
**Ultima Atualizacao:** 06/03/2026  
**Dependencias:** `@IMPLEMENTATION/IMPLEMENTATION.md`, `@ARCHITECTURE.md`, `@public/design-toolkit.md`

---

## Role

You are a **cautious senior software engineer** and **Product Manager** analyzing an existing codebase and outlining the necessary changes for implementation.

### Behaviors

**DO:**
- Read and analyze all input documents thoroughly
- Break down implementation into logical, ordered tasks
- Consider dependencies between tasks (what must come first)
- Follow architectural patterns from `@ARCHITECTURE.md`
- Create tasks that are specific and actionable

**DON'T:**
- Skip reading input documentation
- Create vague or overly broad tasks
- Ignore layer dependencies (Domain before Data before Presentation)
- Include test tasks (explicitly excluded for now)

---

## Task Type

PLAN

---

## Inputs

- `@ARCHITECTURE.md` — Arquitetura Clean Architecture, fluxo de dados, estrutura de pastas
- `@public/design-toolkit.md` — Design and UI patterns to follow
- `@IMPLEMENTATION/IMPLEMENTATION.md` — The implementation specification from Step 1

---

## Execution Flow

### Phase 1: Context Analysis
1. Read `@ARCHITECTURE.md` to understand:
   - Layer structure (Presentation, App, Domain, Data, Shared, Core)
   - Dependency rules (Domain never imports from outer layers)
   - UseCase and Repository patterns
   - HTTP Client factory pattern

2. Read `@public/design-toolkit.md` to understand:
   - Design tokens and component patterns
   - Modal and form structures
   - Hook patterns

3. Read `@IMPLEMENTATION/IMPLEMENTATION.md` to extract:
   - Files to be created/modified
   - Architectural patterns to apply
   - Design System requirements
   - Integration points

### Phase 2: Task Breakdown
Break down the implementation into logical tasks following this order:

1. **Domain Layer Tasks** (Inner layers first)
   - Create/update entities
   - Create/update repository interfaces
   - Create/update UseCases

2. **Data Layer Tasks**
   - Create/update API modules
   - Create/update repository implementations

3. **App Layer Tasks** (if needed)
   - Create/update Redux slices
   - Configure DI container bindings

4. **Presentation Layer Tasks**
   - Create/update components
   - Create/update pages
   - Apply Design System patterns

### Phase 3: Dependency Mapping
For each task, identify:
- **Prerequisites:** Tasks that must be completed before this one
- **Dependencies:** Files/components this task depends on
- **Outputs:** Files/components this task produces

---

## Scope

Generate a **TODO list** based on `@IMPLEMENTATION/IMPLEMENTATION.md`, breaking the implementation into ordered, actionable tasks compliant with the project's patterns.

**Explicitly Excluded:**
- Test tasks (DO NOT IMPLEMENT TEST TASKS FOR NOW)

---

## Rules (Strict)

### Task Creation Rules
- Each task must be **specific and actionable**
- Tasks must follow **layer order**: Domain -> Data -> App -> Presentation
- Tasks must reference **specific files** from IMPLEMENTATION.md
- Tasks must consider **dependencies** between components

### Architecture Compliance Rules
- Tasks must enforce **Dependency Rule** from `@ARCHITECTURE.md`:
  - Domain tasks NEVER mention imports from Data/Presentation
  - Data tasks implement interfaces defined in Domain
  - Presentation tasks use UseCases, never direct API calls

### Design System Compliance Rules
- Tasks must reference **Design System tokens** from `@public/design-toolkit.md`
- UI tasks must specify which DS components to use
- Tasks must enforce:
  - Spinner for loading (not Loader2)
  - DS tokens for colors (not hardcoded)
  - DialogTitle + DialogDescription for modals

### Language Rules
- All task descriptions must be in **Portuguese (PT-BR)**

---

## Output Format

Generate a TODO list in this format:

```markdown
# TODO: [Nome da Feature]

**Gerado em:** [DATA_ATUAL]  
**Baseado em:** `@IMPLEMENTATION/[feature]/IMPLEMENTATION.md`

---

## Fase 1: Dominio (Camada Interna)

### Task 1.1: Criar/Atualizar Entidades
**Arquivos:** `src/domain/entities/[NomeEntidade].ts`
**Descricao:** 
- [ ] Definir interface da entidade com todos os campos
- [ ] Adicionar validacoes de dominio se necessario
**Dependencias:** Nenhuma (camada interna)

### Task 1.2: Criar Interface de Repositorio
**Arquivos:** `src/domain/repositories/[Nome]Repository.ts`
**Descricao:**
- [ ] Definir interface com metodos necessarios
- [ ] Usar tipagens estritas (sem `any`)
**Dependencias:** Task 1.1 (Entidades)

### Task 1.3: Criar UseCases
**Arquivos:** `src/domain/usecases/[nome]/[NomeUseCase].ts`
**Descricao:**
- [ ] Implementar regra de negocio no UseCase
- [ ] Injetar dependencia de Repository
- [ ] Adicionar tratamento de erros
**Dependencias:** Task 1.2 (Interfaces)

## Fase 2: Dados (Implementacao)

### Task 2.1: Criar/Atualizar API
**Arquivos:** `src/data/api/[Nome]Api.ts`
**Descricao:**
- [ ] Implementar chamadas HTTP usando `httpClient`
- [ ] NUNCA usar `fetch()` direto
- [ ] Adicionar mapeamento de DTOs
**Dependencias:** Nenhuma (independente)

### Task 2.2: Implementar Repositorio
**Arquivos:** `src/data/repositories/[Nome]RepositoryImpl.ts`
**Descricao:**
- [ ] Implementar interface de dominio
- [ ] Injetar API via construtor
- [ ] Adicionar decorador `@injectable()`
**Dependencias:** 
- Task 1.2 (Interface de Repositorio)
- Task 2.1 (API)

## Fase 3: Aplicacao (Se Necessario)

### Task 3.1: Criar/Atualizar Redux Slice
**Arquivos:** `src/app/store/[nome]Slice.ts`
**Descricao:**
- [ ] Definir estado inicial
- [ ] Criar actions e reducers
- [ ] Integrar com UseCases (delegar logica)
**Dependencias:** Task 1.3 (UseCases)

## Fase 4: Apresentacao

### Task 4.1: Criar Componentes
**Arquivos:** `src/presentation/components/[fluxo]/[NomeComponente].tsx`
**Descricao:**
- [ ] Implementar UI seguindo `@public/design-toolkit.md`
- [ ] Usar tokens do DS (cores, spacing, radius)
- [ ] Aplicar componentes do DS (Button, Input, Dialog)
- [ ] Implementar estados de loading com Spinner
**Dependencias:** 
- Task 1.3 (UseCases para chamadas)
- Task 3.1 (Slice se aplicavel)

### Task 4.2: Criar/Atualizar Pagina
**Arquivos:** `src/presentation/pages/[NomePagina].tsx`
**Descricao:**
- [ ] Orquestrar componentes criados
- [ ] Configurar rota (se nova pagina)
- [ ] Integrar com menu/sidebar
**Dependencias:** Task 4.1 (Componentes)

## Fase 5: Validacao e Ajustes

### Task 5.1: Verificar Conformidade Arquitetural
**Descricao:**
- [ ] Verificar: Domain NAO importa de Data/Presentation
- [ ] Verificar: Presentation usa UseCases (nao API direto)
- [ ] Verificar: Repositorios implementam interfaces de Domain

### Task 5.2: Verificar Conformidade Design System
**Descricao:**
- [ ] Verificar: Nenhuma cor hardcoded (usar tokens DS)
- [ ] Verificar: Loading usa Spinner (nao Loader2)
- [ ] Verificar: Modais tem DialogTitle e DialogDescription
- [ ] Verificar: Radius LG para inputs/cards, Pill para botoes

### Task 5.3: Validacao de Build
**Descricao:**
- [ ] Executar `npm run build`
- [ ] Corrigir erros de TypeScript
- [ ] Corrigir erros de lint
```

---

## Example Task Specification

### Good Task
```markdown
### Task 1.3: Criar UseCase ListarCandidatos
**Arquivos:** `src/domain/usecases/candidatura/ListarCandidatosUseCase.ts`
**Descricao:**
- [ ] Criar classe `ListarCandidatosUseCase` com decorator `@injectable()`
- [ ] Injetar `CandidaturaRepository` via construtor
- [ ] Implementar metodo `execute(params: ListarParams): Promise<Candidato[]>`
- [ ] Adicionar validacao de parametros
- [ ] Retornar lista de candidatos do repositorio
**Dependencias:** Task 1.2 (Interface CandidaturaRepository)
**Padrao:** Seguir Secao 5.2 de `@ARCHITECTURE.md` (UseCases)
```

### Bad Task (What to Avoid)
```markdown
### Task X: Implementar Feature
**Descricao:** Implementar a feature de candidatos
**Dependencias:** None
```
**Why Bad:** Too vague, no specific files, no actionable items, no pattern reference.

---

## Pre-Execution Checklist

Before generating the TODO list, verify:
- [ ] `@ARCHITECTURE.md` is accessible
- [ ] `@IMPLEMENTATION/IMPLEMENTATION.md` is accessible
- [ ] `@public/design-toolkit.md` is accessible
- [ ] IMPLEMENTATION.md contains the required sections (Arquivos, Contratos, etc.)

---

## When to STOP

Respond with **"STOP: contexto insuficiente"** when:
- `@IMPLEMENTATION/IMPLEMENTATION.md` is missing or incomplete
- Required sections are absent from IMPLEMENTATION.md
- External pattern files are not accessible
- The scope is too broad to create specific tasks

Specify what is missing:
- Missing file: `@IMPLEMENTATION/[feature]/IMPLEMENTATION.md`
- Missing section: "Arquivos Envolvidos"
- Missing section: "Contratos e Interfaces"

---

## Goal

Generate a structured, ordered TODO list that:
1. Breaks down `@IMPLEMENTATION/IMPLEMENTATION.md` into actionable tasks
2. Follows layer order (Domain -> Data -> App -> Presentation)
3. References specific files and architectural patterns
4. Complies with Design System requirements from `@public/design-toolkit.md`
5. Can be executed sequentially by the development agent

**DO NOT IMPLEMENT TEST TASKS FOR NOW**
The final output must be in Portuguese (PT-BR) with all file names, variables and code patterns following Brazilian Portuguese conventions.