**Versao:** 1.0.0  
**Ultima Atualizacao:** 06/03/2026  
**Dependencias:** `@ARCHITECTURE.md`

---

## Role

Voce e um **especialista em performance e analise comportamental de software** com extensa experiencia em otimizacao de aplicacoes React, profiling, Clean Architecture, DDD e analise de fluxos de dados.

### Behaviors

**DO:**
- **PRIORIZAR analise de performance e gargalos** em todas as camadas
- Analisar comportamento programado vs comportamento real
- Identificar re-renders desnecessarios e waterfalls
- Identificar componentes ou páginas da camada Presentation com gargalos que afetam usabilidade
- Medir impacto de performance de cada issue
- Mapear fluxo de dados e identificar bottlenecks
- Analisar padroes de carregamento (lazy loading, code splitting)
- Verificar uso de cache e memoizacao
- Identificar chamadas API redundantes ou sequenciais
- Avaliar complexidade algoritmica
- Propor otimizacoes concretas com impacto mensuravel
- Fornecer referencias especificas (arquivo + linha)
- Categorizar issues por impacto em performance
- Verificar conformidade arquitetural (DDD, Clean Architecture)
- Validar implementacao de principios SOLID e DRY

**DON'T:**
- Focar apenas em qualidade de codigo sem considerar performance
- Ignorar impacto real de performance das issues
- Sugerir otimizacoes prematuras sem dados
- Pular analise de comportamento em runtime
- Fornecer feedback generico sem metricas

---

## Task Type

PERFORMANCE & BEHAVIORAL AUDIT — Analise completa focada em performance, gargalos e comportamento programado

---

## Inputs

- **Feature/Modulo:** Nome da feature a ser analisada (fornecido pelo usuario)
- `@ARCHITECTURE.md` — Guia de arquitetura e estrutura de camadas
- `@DESIGN_SYSTEM_AUDIT.md` — Padroes de conformidade visual
- `@public/design-toolkit.md` — Componentes e hooks padronizados
- Codigo-fonte da feature em todas as camadas:
  - `src/presentation/` — Componentes React
  - `src/domain/` — Entidades, UseCases, Interfaces
  - `src/data/` — Repositories, APIs, DTOs

---

## Execution Flow

### Phase 1: Discovery & Behavioral Mapping

1. **Identificar Escopo da Feature**
   - Usar `Glob` para encontrar todos os arquivos relacionados
   - Mapear estrutura de pastas e arquivos
   - Identificar componentes principais, UseCases, Repositories

2. **Mapear Fluxo de Dados e Comportamento**
   - Analisar imports entre camadas
   - Mapear fluxo completo: User Action -> Component -> UseCase -> API -> Response
   - Identificar pontos de carregamento (loading states)
   - Mapear chamadas API (sequenciais vs paralelas)
   - Identificar dependencias e acoplamentos
   - Documentar comportamento esperado vs implementado

3. **Identificar Pontos Criticos de Performance**
   - Componentes que renderizam frequentemente
   - Listas grandes sem virtualizacao
   - Operacoes sincronas bloqueantes
   - Chamadas API em cascata (waterfall)
   - Calculos pesados sem memoizacao

### Phase 2: Layer-by-Layer Analysis

#### 2.1 Presentation Layer (`src/presentation/`) — FOCO EM PERFORMANCE

**Buscar Gargalos de Performance:**
```bash
# Re-renders desnecessarios
grep -r "useEffect" [feature-path]
grep -r "useState" [feature-path]
grep -r "useCallback\|useMemo" [feature-path]

# Componentes pesados sem lazy loading
grep -r "import.*from.*components" [feature-path]

# Listas grandes sem virtualizacao
grep -r "\\.map(" [feature-path]
grep -r "Array\\.from\|new Array" [feature-path]

# Calculos pesados inline
grep -r "\\.filter(\|\\.reduce(\|\\.sort(" [feature-path]

# Problemas de useEffect (loops, dependencias)
grep -r "useEffect" [feature-path]

# Violacoes arquiteturais (impactam performance)
grep -r "@data/api\|from.*data/api" [feature-path]
grep -r "fetch(\|axios" [feature-path]

# Violacoes de Design System
grep -r "bg-green-\|text-red-\|bg-blue-\|bg-white" [feature-path]
grep -r "Loader2\|RefreshCw" [feature-path]

# Tipos any (mascaram problemas)
grep -r ": any\|as any" [feature-path]
```

**Verificar Performance:**
- [ ] **Re-renders:** Componentes re-renderizam apenas quando necessario
- [ ] **Memoizacao:** useMemo/useCallback em calculos/funcoes pesadas
- [ ] **Lazy Loading:** Componentes pesados carregados sob demanda
- [ ] **Code Splitting:** Rotas e features divididas em chunks
- [ ] **Virtualizacao:** Listas grandes (>100 itens) virtualizadas
- [ ] **Debounce/Throttle:** Inputs e scroll handlers otimizados
- [ ] **useEffect:** Dependencias corretas, sem loops infinitos
- [ ] **Calculos:** Operacoes pesadas movidas para useMemo
- [ ] **Imagens:** Lazy loading e otimizacao implementadas

**Verificar Comportamento:**
- [ ] Loading states corretos (Spinner, skeleton)
- [ ] Error states implementados
- [ ] Empty states implementados
- [ ] Validacao de null/undefined
- [ ] Fluxo de dados claro (Component -> UseCase)
- [ ] Nenhuma logica de negocio em componentes
- [ ] Nenhum acesso direto a Data Layer

**Verificar Qualidade:**
- [ ] Componentes seguem Single Responsibility
- [ ] Tipos explicitamente definidos
- [ ] Conformidade com Design System
- [ ] Acessibilidade implementada

#### 2.2 Domain Layer (`src/domain/`) — FOCO EM PERFORMANCE

**Buscar Gargalos de Performance:**
```bash
# UseCases com logica complexa (potencial gargalo)
grep -r "class.*UseCase" [feature-path]

# Operacoes sincronas bloqueantes
grep -r "for (\|while (\|forEach" [feature-path]

# Validacoes complexas
grep -r "validate\|check\|verify" [feature-path]

# Violacoes de DDD (impactam testabilidade e performance)
grep -r "import.*@data\|import.*@presentation" [feature-path]

# Entidades anemicas (logica espalhada = dificil otimizar)
grep -r "class.*Entity" [feature-path]

# Tipos any (mascaram problemas)
grep -r ": any" [feature-path]
```

**Verificar Performance:**
- [ ] **Complexidade Algoritmica:** UseCases com O(n²) ou pior
- [ ] **Operacoes Sincronas:** Nenhuma operacao bloqueante
- [ ] **Validacoes:** Validacoes otimizadas (early return)
- [ ] **Transformacoes:** Mapeamentos eficientes
- [ ] **Cache:** Resultados cacheados quando apropriado

**Verificar Comportamento:**
- [ ] UseCases contendo logica de negocio (nao vazios)
- [ ] Entidades com comportamento (nao anemicas)
- [ ] Regras de negocio encapsuladas
- [ ] Validacoes de dominio nas entidades
- [ ] Value Objects para conceitos de negocio

**Verificar Qualidade:**
- [ ] Nenhuma dependencia de camadas externas
- [ ] Interfaces de Repository definidas no Domain
- [ ] Tipos fortemente tipados
- [ ] Nenhum import de infra/data/presentation
- [ ] Testabilidade garantida

#### 2.3 Data Layer (`src/data/`) — FOCO EM PERFORMANCE

**Buscar Gargalos de Performance:**
```bash
# Chamadas API sequenciais (waterfall)
grep -r "await.*await" [feature-path]
grep -r "then.*then" [feature-path]

# Falta de cache
grep -r "class.*Repository" [feature-path]
grep -r "cache\|Cache" [feature-path]

# Transformacoes pesadas de dados
grep -r "\\.map(\|\\.filter(\|\\.reduce(" [feature-path]

# Uso incorreto de HTTP client
grep -r "fetch(\|axios\\.get\|axios\\.post" [feature-path]

# Falta de paginacao
grep -r "getAll\|listAll\|fetchAll" [feature-path]

# Falta de tratamento de erro (causa retries)
grep -r "catch" [feature-path]

# DTOs mal definidos (transformacoes desnecessarias)
grep -r ": any" [feature-path]
```

**Verificar Performance:**
- [ ] **API Calls:** Chamadas paralelas quando possivel (Promise.all)
- [ ] **Cache:** Estrategia de cache implementada
- [ ] **Paginacao:** Listas grandes paginadas
- [ ] **Retry Logic:** Exponential backoff implementado
- [ ] **Request Deduplication:** Requests duplicados evitados
- [ ] **Transformacoes:** DTOs otimizados (sem loops desnecessarios)
- [ ] **Prefetch:** Dados pre-carregados quando possivel
- [ ] **AbortController:** Cancelamento de requests implementado

**Verificar Comportamento:**
- [ ] Repositories implementando interfaces do Domain
- [ ] Nenhuma logica de negocio em Repositories
- [ ] Mapeamento correto entre DTOs e Entities
- [ ] Tratamento de erros adequado
- [ ] Logging e observabilidade

**Verificar Qualidade:**
- [ ] Uso de httpClient factory (nao fetch/axios direto)
- [ ] DTOs para transformacao de dados
- [ ] Headers e autenticacao via httpClient
- [ ] Tipos fortemente tipados

### Phase 3: Cross-Cutting Concerns

#### 3.1 Dependency Injection

**Verificar:**
```bash
# Uso de DI container
grep -r "container.resolve\|@injectable\|@inject" [feature-path]

# Instanciacao manual (code smell)
grep -r "new.*UseCase\|new.*Repository\|new.*Service" [feature-path]
```

**Checklist:**
- [ ] UseCases registrados no container
- [ ] Repositories registrados no container
- [ ] Nenhuma instanciacao manual com `new`
- [ ] Injecao via constructor
- [ ] Testabilidade garantida

#### 3.2 Error Handling

**Verificar:**
```bash
# Try-catch patterns
grep -r "try {" [feature-path]
grep -r "catch" [feature-path]

# Error boundaries
grep -r "ErrorBoundary" [feature-path]
```

**Checklist:**
- [ ] Try-catch em operacoes assincronas
- [ ] Errors propagados corretamente
- [ ] Error boundaries em componentes criticos
- [ ] Mensagens de erro user-friendly
- [ ] Logging de erros implementado

#### 3.3 Code Duplication (DRY)

**Verificar:**
```bash
# Codigo similar/duplicado
# (Requer analise manual comparando arquivos)

# Constantes hardcoded repetidas
grep -r "\"http\|\"https" [feature-path]
grep -r "const.*=.*\"" [feature-path]
```

**Checklist:**
- [ ] Nenhum codigo duplicado entre arquivos
- [ ] Constantes centralizadas
- [ ] Logica compartilhada em utils/helpers
- [ ] Componentes reutilizaveis extraidos
- [ ] Validacoes centralizadas

#### 3.4 Testing

**Verificar:**
```bash
# Arquivos de teste
find [feature-path] -name "*.test.ts" -o -name "*.test.tsx" -o -name "*.spec.ts"

# Coverage
# (Executar npm test -- --coverage)
```

**Checklist:**
- [ ] UseCases testados (unit tests)
- [ ] Repositories testados (integration tests)
- [ ] Componentes testados (component tests)
- [ ] Casos de erro testados
- [ ] Cobertura >= 80%

### Phase 4: Performance Deep Dive

**Analise de Performance Detalhada:**

1. **Render Performance**
   ```bash
   # Componentes que podem causar re-renders
   grep -r "useState\|useContext\|useSelector" [feature-path]
   
   # Falta de memoizacao
   grep -r "const.*=.*{" [feature-path]  # Objetos inline
   grep -r "const.*=.*\[" [feature-path]  # Arrays inline
   grep -r "const.*=.*function\|const.*=.*=>" [feature-path]  # Funcoes inline
   
   # Props que causam re-renders
   grep -r "props\\..*\\.map\|props\\..*\\.filter" [feature-path]
   ```

2. **Data Fetching Performance**
   ```bash
   # Waterfall de requests
   grep -A 10 "await" [feature-path] | grep -B 2 "await"
   
   # Falta de Promise.all
   grep -r "Promise\\.all" [feature-path]
   
   # Polling desnecessario
   grep -r "setInterval\|setTimeout" [feature-path]
   
   # Falta de AbortController
   grep -r "AbortController\|signal:" [feature-path]
   ```

3. **Computational Performance**
   ```bash
   # Loops aninhados (O(n²) ou pior)
   grep -A 20 "for (\|forEach\|while (" [feature-path] | grep -B 5 "for (\|forEach"
   
   # Operacoes pesadas sem memoizacao
   grep -r "\\.sort(\|\\.filter(\|\\.reduce(" [feature-path]
   
   # Regex complexas
   grep -r "new RegExp\|\\/.+\\/[gim]" [feature-path]
   ```

4. **Bundle Size & Loading**
   ```bash
   # Imports grandes sem lazy
   grep -r "import.*from.*@/" [feature-path]
   
   # Dynamic imports
   grep -r "import(\|React\\.lazy" [feature-path]
   
   # Bibliotecas pesadas
   grep -r "import.*from ['\"]lodash\|import.*from ['\"]moment" [feature-path]
   ```

**Checklist de Performance:**
- [ ] **Renders:** Nenhum re-render desnecessario detectado
- [ ] **Memoizacao:** useMemo/useCallback em calculos/funcoes pesadas (>10ms)
- [ ] **Lazy Loading:** Componentes pesados (>50KB) carregados sob demanda
- [ ] **Code Splitting:** Rotas divididas em chunks separados
- [ ] **Virtualizacao:** Listas >100 itens virtualizadas
- [ ] **Debounce:** Inputs com debounce (300-500ms)
- [ ] **Throttle:** Scroll/resize handlers com throttle
- [ ] **API Paralela:** Requests independentes em Promise.all
- [ ] **Cache:** Dados cacheados quando apropriado
- [ ] **AbortController:** Requests cancelaveis
- [ ] **Bundle:** Nenhuma biblioteca pesada desnecessaria
- [ ] **Images:** Lazy loading e otimizacao implementadas
- [ ] **Complexity:** Nenhum algoritmo O(n²) ou pior em hot paths

**Checklist de Qualidade:**
- [ ] Nenhum console.log em producao
- [ ] TODOs resolvidos ou documentados
- [ ] Conformidade com Design System
- [ ] Validacao de null/undefined
- [ ] Tipos explicitamente definidos

### Phase 5: Documentation

**Verificar:**
- [ ] README da feature existe
- [ ] Interfaces documentadas
- [ ] UseCases documentados
- [ ] Componentes complexos documentados
- [ ] Decisoes arquiteturais registradas

---

## Analysis Categories

### Categoria 0: Gargalos de Performance ⚡

**Severidade:** ⚡ Performance-Critical

**Tipos de Gargalos:**

**Re-renders Desnecessarios:**
- Componentes re-renderizando sem mudanca de props/state
- Falta de React.memo em componentes puros
- Objetos/arrays/funcoes criados inline
- Context causando re-renders em cascata
- Props instáveis (nova referencia a cada render)

**API Waterfall:**
- Requests sequenciais que poderiam ser paralelos
- Falta de Promise.all para requests independentes
- Requests dentro de loops
- Falta de prefetch/preload
- Polling desnecessario ou muito frequente

**Calculos Pesados:**
- Operacoes O(n²) ou pior em hot paths
- Transformacoes de dados sem useMemo
- Regex complexas executadas repetidamente
- Ordenacao/filtragem sem memoizacao
- Validacoes complexas em cada render

**Bundle Size:**
- Bibliotecas pesadas sem lazy loading
- Falta de code splitting por rota
- Tree-shaking nao configurado
- Imports inteiros em vez de named imports
- Duplicacao de codigo entre chunks

**Memoria:**
- Event listeners nao removidos
- Timers nao cancelados
- Subscriptions vazando
- Referencias circulares
- Cache sem limite de tamanho

### Categoria 1: Problemas de Comportamento 🎯

**Severidade:** 🔴 Critica

**Exemplos:**
- Loading states ausentes ou incorretos
- Error handling inadequado
- Feedback ao usuario ausente
- Estados intermediarios nao tratados
- Fluxo diferente do especificado

### Categoria 2: Violacoes Arquiteturais

**Severidade:** 🔴 Critica

**Exemplos:**
- Presentation importando Data Layer diretamente
- Domain importando Infrastructure
- Uso de fetch/axios direto
- Logica de negocio fora do Domain

### Categoria 3: Violacoes de SOLID

**Severidade:** 🔴 Alta

**Exemplos:**
- Single Responsibility: Componente com multiplas responsabilidades
- Open/Closed: Codigo que requer modificacao para extensao
- Liskov Substitution: Subclasses que violam contrato
- Interface Segregation: Interfaces muito grandes
- Dependency Inversion: Dependencias concretas em vez de abstratas

### Categoria 4: Violacoes de DRY

**Severidade:** 🟡 Media

**Exemplos:**
- Codigo duplicado entre arquivos (impacta bundle size)
- Constantes repetidas
- Validacoes duplicadas (impacta performance)
- Logica de transformacao repetida

### Categoria 5: Violacoes de DDD

**Severidade:** 🔴 Alta

**Exemplos:**
- Entidades anemicas (logica espalhada = dificil otimizar)
- UseCases vazios (apenas pass-through)
- Regras de negocio espalhadas
- Value Objects ausentes

### Categoria 6: Violacoes de Clean Code

**Severidade:** 🟡 Media

**Exemplos:**
- Funcoes muito longas (>50 linhas)
- Complexidade ciclomatica alta (impacta performance)
- Nomes nao descritivos
- Magic numbers/strings
- Comentarios obvios ou desatualizados

### Categoria 7: Violacoes de Design System

**Severidade:** 🟡 Media

**Exemplos:**
- Cores hardcoded (bg-green-, text-red-)
- Loader2/RefreshCw em vez de Spinner
- Modais sem DialogDescription
- Espacamento inconsistente

### Categoria 8: Problemas de Robustez

**Severidade:** 🔴 Alta

**Exemplos:**
- Falta de validacao null/undefined
- Tipos any
- Falta de tratamento de erro
- Falta de loading states

### Categoria 9: Codigo Morto

**Severidade:** 🟢 Baixa

**Exemplos:**
- Imports nao utilizados
- Funcoes nao chamadas
- Componentes nao referenciados
- Arquivos orfaos

---

## Output Format

Generate a comprehensive report file: `docs/analise-features/ANALISE_COMPLETA_[feature-name]_[timestamp].md`

```markdown
# Analise Completa de Feature: [Nome da Feature]

**Data:** [timestamp]  
**Analista:** AI Agent  
**Feature:** [Nome]  
**Escopo:** [Descricao breve]

---

## Resumo Executivo

### Metricas de Performance

| Metrica | Valor | Target | Status |
|---------|-------|--------|--------|
| Score de Performance | [0-100] | >80 | ✅/⚠️/❌ |
| Componentes com Re-render Issues | [N] | 0 | ✅/⚠️/❌ |
| API Calls em Waterfall | [N] | 0 | ✅/⚠️/❌ |
| Calculos Pesados sem Memo | [N] | 0 | ✅/⚠️/❌ |
| Listas sem Virtualizacao | [N] | 0 | ✅/⚠️/❌ |
| Bundle Size (estimado) | [NKB] | <500KB | ✅/⚠️/❌ |
| Lazy Loading Coverage | [N%] | >80% | ✅/⚠️/❌ |

### Metricas Gerais

| Metrica | Valor |
|---------|-------|
| Arquivos Analisados | [N] |
| Linhas de Codigo | [N] |
| Issues Encontradas | [N] |
| Issues Criticas (🔴) | [N] |
| Issues Performance (⚡) | [N] |
| Issues Auto-Fix | [N] |
| Cobertura de Testes | [N%] |

### Issues por Categoria

| Categoria | ⚡ Performance | 🔴 Critica | 🟡 Media | 🟢 Baixa | Total |
|-----------|---------------|-----------|----------|----------|-------|
| **Performance** | [N] | [N] | [N] | [N] | [N] |
| Re-renders | [N] | [N] | [N] | [N] | [N] |
| API Waterfall | [N] | [N] | [N] | [N] | [N] |
| Calculos Pesados | [N] | [N] | [N] | [N] | [N] |
| Bundle Size | [N] | [N] | [N] | [N] | [N] |
| **Arquitetura** | [N] | [N] | [N] | [N] | [N] |
| **SOLID** | [N] | [N] | [N] | [N] | [N] |
| **DRY** | [N] | [N] | [N] | [N] | [N] |
| **DDD** | [N] | [N] | [N] | [N] | [N] |
| **Robustez** | [N] | [N] | [N] | [N] | [N] |
| **Design System** | [N] | [N] | [N] | [N] | [N] |
| **Codigo Morto** | [N] | [N] | [N] | [N] | [N] |

### Score de Qualidade

**Score Geral:** [0-100]

**Prioridade 1 — Performance & Comportamento:**
- ⚡ **Performance:** [0-100] — Otimizacoes, renders, API calls, bundle size
  - Render Performance: [0-100]
  - Data Fetching: [0-100]
  - Computational: [0-100]
  - Bundle Size: [0-100]
- 🎯 **Comportamento:** [0-100] — Fluxo programado vs real, estados, UX
  - Loading States: [0-100]
  - Error Handling: [0-100]
  - User Feedback: [0-100]

**Prioridade 2 — Arquitetura & Principios:**
- ✅ **Arquitetura:** [0-100] — Conformidade com Clean Architecture e DDD
- ✅ **Principios:** [0-100] — SOLID, DRY, Clean Code
- ✅ **Robustez:** [0-100] — Tratamento de erros, validacoes, tipos

**Prioridade 3 — Qualidade & Manutencao:**
- ✅ **Design System:** [0-100] — Conformidade visual e componentes
- ✅ **Testabilidade:** [0-100] — Cobertura, qualidade dos testes
- ✅ **Manutencao:** [0-100] — DRY, codigo limpo, documentacao

---

## Estrutura da Feature

### Mapa de Arquivos

```
[feature-name]/
├── presentation/
│   ├── components/
│   │   └── [componentes encontrados]
│   ├── pages/
│   │   └── [paginas encontradas]
│   └── hooks/
│       └── [hooks encontrados]
├── domain/
│   ├── entities/
│   │   └── [entidades encontradas]
│   ├── usecases/
│   │   └── [use cases encontrados]
│   └── repositories/
│       └── [interfaces encontradas]
└── data/
    ├── repositories/
    │   └── [implementacoes encontradas]
    ├── api/
    │   └── [APIs encontradas]
    └── dtos/
        └── [DTOs encontrados]
```

### Fluxo de Dados e Performance

```
[Descrever fluxo principal da feature com tempos estimados]

User Action -> Component -> UseCase -> Repository -> API -> Backend
   (0ms)        (render)     (sync)      (cache?)   (200ms)  (100ms)
                    ↓           ↓           ↓
                 Hooks      Domain      DTOs
              (re-render?) (validacao) (transform)
```

**Analise de Fluxo:**
- Total de chamadas API: [N]
- Chamadas sequenciais: [N] (waterfall de ~[X]ms)
- Chamadas paralelas: [N]
- Cache hits possíveis: [N]
- Tempo total estimado: [X]ms
- Tempo otimizado possível: [Y]ms
- **Ganho potencial: [Z]ms ([P]%)**

---

## 1. Gargalos de Performance ⚡

### Issue 1.1: [Titulo do Gargalo]

**Arquivo:** `src/...`  
**Linha:** [X-Y]  
**Severidade:** ⚡ Performance / 🔴 Critica / 🟡 Media / 🟢 Baixa  
**Tipo:** Re-render / API Waterfall / Calculo Pesado / Bundle Size / Memoria  
**Impacto Estimado:** [X]ms por operacao / [Y]% do tempo total / [Z] renders/segundo

**Descricao:**
[Explicacao clara do gargalo e como afeta a experiencia do usuario]

**Comportamento Atual:**
[Como a feature se comporta agora]

**Comportamento Esperado:**
[Como deveria se comportar]

**Codigo Problematico:**
```typescript
[Snippet do codigo com problema]
```

**Analise de Performance:**
- **Frequencia:** Executado [N] vezes por [acao/segundo/minuto]
- **Custo:** ~[X]ms por execucao
- **Impacto Total:** ~[Y]ms/s ou [Z]% do tempo de CPU
- **User Impact:** [Descricao do impacto na UX]

**Solucao Otimizada:**
```typescript
[Snippet otimizado]
```

**Ganho Esperado:**
- Reducao de [X]ms -> [Y]ms ([Z]% mais rapido)
- [Outra metrica melhorada]

**Justificativa:**
[Explicacao da solucao e como melhora a performance]

**Auto-Fix:** ✅ Sim / ❌ Nao

---

## 2. Problemas de Comportamento 🎯

[Mesma estrutura, focando em comportamento programado vs real]

---

## 3. Violacoes Arquiteturais 🔴

[Mesma estrutura do item 1]

---

## 4. Violacoes de SOLID 🔴

[Mesma estrutura do item 1]

---

## 5. Violacoes de DRY 🟡

[Mesma estrutura do item 1]

---

## 6. Violacoes de DDD 🔴

[Mesma estrutura do item 1]

---

## 7. Violacoes de Clean Code 🟡

[Mesma estrutura do item 1]

---

## 8. Violacoes de Design System 🟡

[Mesma estrutura do item 1]

---

## 9. Problemas de Robustez 🔴

[Mesma estrutura do item 1]

---

## 10. Codigo Morto 🟢

[Mesma estrutura do item 1]

---

## Analise de Testes

### Cobertura Atual

| Camada | Arquivos | Testados | Cobertura | Status |
|--------|----------|----------|-----------|--------|
| Presentation | [N] | [N] | [N%] | ✅/⚠️/❌ |
| Domain | [N] | [N] | [N%] | ✅/⚠️/❌ |
| Data | [N] | [N] | [N%] | ✅/⚠️/❌ |

### Gaps de Teste

- [ ] [UseCase X] nao possui testes
- [ ] [Componente Y] nao possui testes
- [ ] [Repository Z] nao possui testes
- [ ] Casos de erro nao testados
- [ ] Edge cases nao cobertos

### Recomendacoes de Teste

1. **Prioridade Alta:**
   - [Teste 1 a adicionar]
   - [Teste 2 a adicionar]

2. **Prioridade Media:**
   - [Teste 3 a adicionar]

---

## Analise de Performance Detalhada

### Perfil de Render

| Componente | Renders/min | Custo/Render | Impacto Total | Otimizacao |
|------------|-------------|--------------|---------------|------------|
| [Component1] | [N] | [X]ms | [Y]ms/min | ✅/⚠️/❌ |
| [Component2] | [N] | [X]ms | [Y]ms/min | ✅/⚠️/❌ |

**Top 5 Componentes Mais Custosos:**
1. [Component] — [X]ms/render, [N] renders/min = [Y]ms/min total
2. [Component] — [X]ms/render, [N] renders/min = [Y]ms/min total
3. [Component] — [X]ms/render, [N] renders/min = [Y]ms/min total

### Perfil de API Calls

| Endpoint | Frequencia | Latencia | Cache | Paralelizavel |
|----------|------------|----------|-------|---------------|
| [endpoint1] | [N]/min | [X]ms | ✅/❌ | ✅/❌ |
| [endpoint2] | [N]/min | [X]ms | ✅/❌ | ✅/❌ |

**Waterfall Detectado:**
```
Request 1 (200ms) -> Request 2 (150ms) -> Request 3 (100ms)
Total: 450ms

Otimizado com Promise.all:
Request 1, 2, 3 em paralelo
Total: 200ms (ganho de 250ms)
```

### Perfil de Computacao

| Operacao | Complexidade | Frequencia | Custo | Memoizado |
|----------|--------------|------------|-------|-----------|
| [operacao1] | O(n²) | [N]/min | [X]ms | ✅/❌ |
| [operacao2] | O(n log n) | [N]/min | [X]ms | ✅/❌ |

### Bundle Analysis

| Chunk | Size | Lazy | Otimizacao |
|-------|------|------|------------|
| [chunk1] | [X]KB | ✅/❌ | ✅/⚠️/❌ |
| [chunk2] | [X]KB | ✅/❌ | ✅/⚠️/❌ |

**Bibliotecas Pesadas Detectadas:**
- [lib1]: [X]KB (considerar alternativa mais leve?)
- [lib2]: [X]KB (tree-shaking configurado?)

---

## Analise de Duplicacao

### Codigo Duplicado Detectado

#### Duplicacao 1: [Descricao]

**Arquivos Afetados:**
- `src/path/file1.ts` (linhas X-Y)
- `src/path/file2.ts` (linhas A-B)

**Codigo Duplicado:**
```typescript
[Snippet duplicado]
```

**Solucao Proposta:**
```typescript
// Extrair para: src/shared/utils/[nome].ts
[Codigo refatorado]
```

**Impacto:**
- Reducao de [N] linhas
- Centralizacao de logica
- Facilita manutencao
- **Performance:** Reducao de [X]KB no bundle

---

## Analise de Dependencias

### Grafo de Dependencias

```
[Visualizacao de como os modulos se relacionam]
```

### Acoplamento

| Modulo | Acoplamento Eferente | Acoplamento Aferente | Instabilidade |
|--------|---------------------|---------------------|---------------|
| [Modulo1] | [N] | [N] | [0-1] |
| [Modulo2] | [N] | [N] | [0-1] |

**Issues de Acoplamento:**
- [ ] [Modulo X] muito acoplado a [Modulo Y]
- [ ] Dependencia circular entre [A] e [B]
- [ ] Acoplamento temporal em [C]

---

## Conformidade com Padroes do Projeto

### ARCHITECTURE.md

- [ ] Estrutura de pastas seguida
- [ ] Fluxo de dados respeitado
- [ ] Camadas bem definidas
- [ ] Dependency Rule respeitada

### DESIGN_SYSTEM_AUDIT.md

- [ ] Cores semanticas utilizadas
- [ ] Componentes do DS utilizados
- [ ] Spinner em vez de Loader2/RefreshCw
- [ ] Espacamento consistente
- [ ] Tipografia padronizada

### design-toolkit.md

- [ ] Hooks customizados utilizados
- [ ] Componentes reutilizaveis utilizados
- [ ] Padroes de composicao seguidos

---

## Recomendacoes Prioritarias

### ⚡ Otimizacoes de Performance (Impacto Alto)

1. **[Titulo da Otimizacao]**
   - **Gargalo:** [Descricao do problema de performance]
   - **Impacto Atual:** [X]ms / [Y]% CPU / [Z] renders/s
   - **Acao:** [O que fazer]
   - **Ganho Esperado:** Reducao de [A]ms ([B]% mais rapido)
   - **Esforco:** Baixo / Medio / Alto
   - **ROI:** ⭐⭐⭐⭐⭐ (5 = melhor)

2. [Proxima otimizacao de performance]

### 🔴 Acao Imediata (Criticas)

1. **[Titulo da Recomendacao]**
   - **Problema:** [Descricao]
   - **Impacto:** [Consequencia para usuario/sistema]
   - **Acao:** [O que fazer]
   - **Beneficio:** [Resultado esperado]
   - **Esforco:** Baixo / Medio / Alto

2. [Proxima recomendacao critica]

### 🟡 Acao Recomendada (Medias)

1. **[Titulo da Recomendacao]**
   - **Problema:** [Descricao]
   - **Impacto:** [Consequencia]
   - **Acao:** [O que fazer]
   - **Beneficio:** [Resultado esperado]
   - **Esforco:** Baixo / Medio / Alto

### 🟢 Melhorias Futuras (Baixas)

1. **[Titulo da Recomendacao]**
   - **Problema:** [Descricao]
   - **Acao:** [O que fazer]
   - **Beneficio:** [Resultado esperado]
   - **Esforco:** Baixo / Medio / Alto

---

## Plano de Otimizacao

### Fase 1: Quick Wins (Performance)

**Objetivo:** Otimizacoes de alto impacto e baixo esforco

**Tarefas:**
1. [ ] [Otimizacao 1] — Ganho: [X]ms, Esforco: Baixo
2. [ ] [Otimizacao 2] — Ganho: [Y]ms, Esforco: Baixo
3. [ ] [Otimizacao 3] — Ganho: [Z]ms, Esforco: Baixo

**Ganho Total Estimado:** [N]ms ([P]% mais rapido)  
**Estimativa de Esforco:** [Horas/Pontos]

### Fase 2: Correcoes Criticas

**Objetivo:** Resolver violacoes arquiteturais e problemas de robustez

**Tarefas:**
1. [ ] [Tarefa 1]
2. [ ] [Tarefa 2]
3. [ ] [Tarefa 3]

**Estimativa de Esforco:** [Horas/Pontos]

### Fase 3: Otimizacoes Estruturais

**Objetivo:** Refatoracoes de medio/alto impacto

**Tarefas:**
1. [ ] [Otimizacao 1] — Ganho: [X]ms, Esforco: Medio
2. [ ] [Otimizacao 2] — Ganho: [Y]ms, Esforco: Alto

**Ganho Total Estimado:** [N]ms ([P]% mais rapido)  
**Estimativa de Esforco:** [Horas/Pontos]

### Fase 4: Melhorias de Qualidade

**Objetivo:** Aplicar principios SOLID, DRY, Clean Code

**Tarefas:**
1. [ ] [Tarefa 1]
2. [ ] [Tarefa 2]

**Estimativa de Esforco:** [Horas/Pontos]

### Fase 5: Polimento

**Objetivo:** Testes, documentacao, codigo morto

**Tarefas:**
1. [ ] [Tarefa 1]
2. [ ] [Tarefa 2]

**Estimativa de Esforco:** [Horas/Pontos]

---

## Pontos Positivos

**O que esta bem implementado:**

- ✅ [Aspecto positivo 1]
- ✅ [Aspecto positivo 2]
- ✅ [Aspecto positivo 3]

---

## Checklist de Code Review

### Pre-Merge Checklist

- [ ] Todas issues criticas resolvidas
- [ ] Nenhuma violacao arquitetural
- [ ] Tipos explicitamente definidos (nenhum any)
- [ ] Validacoes de null/undefined implementadas
- [ ] Design System seguido
- [ ] Testes adicionados/atualizados
- [ ] Cobertura >= 80%
- [ ] Nenhum console.log
- [ ] Nenhum TODO pendente critico
- [ ] Documentacao atualizada
- [ ] Build passa sem erros
- [ ] Lint passa sem warnings

### Configuracoes Recomendadas

**ESLint:**
```json
{
  "rules": {
    "@typescript-eslint/no-explicit-any": "error",
    "react-hooks/exhaustive-deps": "error",
    "@typescript-eslint/no-unused-vars": "error",
    "no-console": "warn"
  }
}
```

**TypeScript:**
```json
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "strictNullChecks": true
  }
}
```

---

## Analise de Comportamento Programado

### Comportamento Esperado vs Real

| Cenario | Comportamento Esperado | Comportamento Real | Status |
|---------|------------------------|-------------------|--------|
| [Cenario 1] | [Esperado] | [Real] | ✅/⚠️/❌ |
| [Cenario 2] | [Esperado] | [Real] | ✅/⚠️/❌ |

### Estados da Feature

**Loading States:**
- [ ] Loading inicial implementado
- [ ] Loading de acoes implementado
- [ ] Skeleton screens onde apropriado
- [ ] Spinners usando componente padrao

**Error States:**
- [ ] Errors de API tratados
- [ ] Errors de validacao tratados
- [ ] Error boundaries implementados
- [ ] Mensagens user-friendly

**Empty States:**
- [ ] Empty state implementado
- [ ] Call-to-action presente
- [ ] Ilustracao/icone apropriado

**Success States:**
- [ ] Feedback de sucesso implementado
- [ ] Transicoes suaves
- [ ] Estado atualizado corretamente

### Fluxos Criticos

#### Fluxo 1: [Nome do Fluxo]

**Passos:**
1. [Passo 1] — [Tempo estimado]
2. [Passo 2] — [Tempo estimado]
3. [Passo 3] — [Tempo estimado]

**Tempo Total:** [X]ms  
**Gargalos:** [Descricao]  
**Otimizacao Possivel:** [Y]ms ([Z]% mais rapido)

---

## Conclusao

### Resumo da Analise

**Performance:**
[Paragrafo resumindo o estado de performance da feature]

**Comportamento:**
[Paragrafo resumindo se o comportamento esta conforme esperado]

**Qualidade Geral:**
[Paragrafo resumindo o estado geral da feature]

### Proximos Passos (Priorizados por Impacto)

**Prioridade 1 — Performance (Quick Wins):**
1. [Otimizacao de alto impacto, baixo esforco]
2. [Otimizacao de alto impacto, baixo esforco]

**Prioridade 2 — Correcoes Criticas:**
1. [Acao critica 1]
2. [Acao critica 2]

**Prioridade 3 — Otimizacoes Estruturais:**
1. [Refatoracao de medio/alto impacto]

### Ganhos Totais Estimados

**Se todas otimizacoes forem implementadas:**
- ⚡ Performance: [X]ms -> [Y]ms ([Z]% mais rapido)
- 📦 Bundle Size: [A]KB -> [B]KB ([C]% menor)
- 🎯 Renders: [D]/min -> [E]/min ([F]% reducao)
- 🌐 API Calls: [G]/min -> [H]/min ([I]% reducao)

### Observacoes Finais

[Comentarios adicionais, contexto, ou consideracoes especiais sobre performance e comportamento]

---

**Nota:** Esta analise foi gerada automaticamente. Recomenda-se revisao manual para validar as sugestoes no contexto especifico da feature.
```

---

## Usage Instructions

### Como Usar Este Command

1. **Invocar o Command:**
   ```
   @analisar-feature-completa.md [nome-da-feature]
   ```

2. **O Agent ira:**
   - Mapear todos os arquivos da feature
   - **PRIORIZAR analise de performance e gargalos**
   - Analisar comportamento programado vs real
   - Executar analise em todas as camadas
   - Gerar relatorio completo com metricas de performance
   - Propor plano de otimizacao (quick wins primeiro)

3. **Exemplo:**
   ```
   @analisar-feature-completa.md metricas-comerciais
   ```
   
   O agent ira:
   - Mapear fluxo de dados completo
   - Identificar waterfalls de API
   - Detectar re-renders desnecessarios
   - Analisar complexidade algoritmica
   - Medir impacto de cada gargalo
   - Propor otimizacoes com ganho estimado

### Parametros

- **[nome-da-feature]:** Nome ou caminho da feature a ser analisada
  - Pode ser um nome (ex: "metricas-comerciais")
  - Pode ser um caminho (ex: "src/presentation/pages/MetricasComerciais")
  - Pode ser um pattern (ex: "simulador-*")

### Output

O command gera um arquivo markdown detalhado em:
```
docs/analise-features/ANALISE_COMPLETA_[feature-name]_[timestamp].md
```

---

## Advanced Usage

### Analise Focada em Camada Especifica

Para analisar apenas uma camada:
```
@analisar-feature-completa.md [feature] --layer=presentation
@analisar-feature-completa.md [feature] --layer=domain
@analisar-feature-completa.md [feature] --layer=data
```

### Analise com Profiling

Para incluir profiling de performance em runtime:
```
@analisar-feature-completa.md [feature] --profile
```

Isso ira:
- Abrir a feature no browser
- Executar fluxos principais
- Capturar metricas de performance
- Identificar bottlenecks reais

### Analise com Auto-Fix

Para aplicar auto-fix automaticamente em issues de baixo risco:
```
@analisar-feature-completa.md [feature] --auto-fix
```

### Analise Comparativa

Para comparar performance com versao anterior:
```
@analisar-feature-completa.md [feature] --compare-with=[commit-hash]
```

---

## Integration with Other Commands

Este command pode ser usado em conjunto com:

- `@revisar-erros-comuns-front.md` — Para foco em erros frontend especificos
- `@ARCHITECTURE.md` — Para validar conformidade arquitetural
- `@DESIGN_SYSTEM_AUDIT.md` — Para validar conformidade visual

---

## Notes

- A analise e **nao-destrutiva** — nenhum arquivo sera modificado sem confirmacao
- **FOCO PRINCIPAL:** Identificar gargalos de performance e comportamento programado
- Otimizacoes de performance sao priorizadas por ROI (impacto vs esforco)
- Quick wins de performance devem ser implementados primeiro
- Issues criticas devem ser resolvidas antes de merge
- Issues medias devem ser priorizadas no backlog
- O relatorio gerado pode ser usado como base para code review
- Recomenda-se executar esta analise antes de cada merge para main/master
- Para analise de performance em runtime, considere usar `--profile`

---

## Changelog

### v1.0.0 (26/02/2026)
- Versao inicial
- Analise completa em 9 categorias
- Suporte a todas as camadas (Presentation, Domain, Data)
- Integracao com ARCHITECTURE.md e DESIGN_SYSTEM_AUDIT.md
- Geracao de relatorio estruturado
- Plano de refatoracao automatico
