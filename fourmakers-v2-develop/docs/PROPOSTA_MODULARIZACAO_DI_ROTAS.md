# Proposta: modularização para reduzir conflitos de merge (DI, rotas, tokens)

**Para outros times:** use este doc + [`MODULARIZACAO_PRESENTATION_RECRUTAMENTO.md`](./MODULARIZACAO_PRESENTATION_RECRUTAMENTO.md) como **playbook** para modularizar **a sua jornada** (Minha Jornada, Folha, Comercial, etc.). O piloto **Recrutamento** está implementado no código como referência.

**Contexto:** merges `main` ↔ `develop` geram muito retrabalho por conflitos em arquivos globais (`container.ts`, `tokens.ts`, `AppRoutes.tsx`).  
**Objetivo:** manter **Clean Architecture** (ARCHITECTURE.md), dar **autonomia por jornada/módulo** e **um ponto de composição mínimo** nos arquivos centrais.  
**Não objetivo:** abandonar camadas domain/data/presentation nem duplicar modais/páginas sem critério.

---

## 1. Diagnóstico

| Arquivo | Problema |
|---------|----------|
| `core/di/container.ts` | Milhares de linhas; qualquer feature nova altera o mesmo bloco → conflito com outras branches. |
| `core/di/tokens.ts` | Lista única de `Symbol.for(...)`; duas features novas = mesma região editada. |
| `app/routes/AppRoutes.tsx` | Todas as rotas no mesmo arquivo (ou poucos arquivos). |
| Agentes / devs | Refatorações automáticas tendem a reordenar imports/registros e piorar o diff. |

A regra de dependência **não exige** um único arquivo de registro; exige que **Presentation → Use Cases → Repositories (interface)** e que **Data implemente** as interfaces.

---

## 2. Princípios da solução (dentro do projeto)

1. **Um container TSyringe continua existindo** — composição no bootstrap, não N containers isolados (evita resolver duas vezes e quebra de singletons globais como `auth`).
2. **Registro por módulo de jornada** — cada squad/feature passa a **só commitar** arquivos sob um prefixo estável (ex.: `recrutamento`).
3. **Arquivo central mínimo** — `setupDependencyInjection` só **importa e chama** `registerX(container)` em ordem fixa (alfabética ou por camada: core → módulos).
4. **Tokens por módulo** — novos tokens em arquivo dedicado; **merge** tende a ser *add file* ou *append* em um objeto namespaced, não linha única no meio de 180 símbolos.
5. **Rotas por módulo** — `AppRoutes` monta **sub-árvores** (ex.: `/recrutamento` com filhos aninhados + `<Outlet />`) importando fragmentos de `<Route>` por pasta (ver §12 — React Router 7).
6. **UI comum** — modais e padrões continuam em `presentation/components` (DS); catálogo/documentação evita “dois modais iguais”.

---

## 3. Estrutura sugerida (fase 1 — só DI + rotas)

### 3.1 DI — pastas e responsabilidade

Cada **jornada** tem sua própria subpasta em `modules/` e em `tokens/`, mantendo os arquivos do módulo agrupados e reduzindo conflitos. Em **`registerRecrutamento.ts`** e **`registerRecrutamentoUseCases.ts`** (e equivalentes por jornada), manter **imports em ordem alfabética** (por grupo: types, depois APIs/impl) para facilitar revisão e evitar diffs desnecessários.

```
src/core/di/
  bootstrap.ts                 # export setupDependencyInjection — só orquestra
  tokens/
    index.ts                   # reexporta DiTokens unificado (compatível com código atual)
    core.tokens.ts             # auth, menu, profile, http-adjacent (poucas mudanças)
    recrutamento/
      recrutamento.tokens.ts   # vaga, candidatura, match, template…
    minhaJornada/
      minhaJornada.tokens.ts
    folhaPagamento/
      folhaPagamento.tokens.ts # ou financeiro
    …                          # uma subpasta por jornada estável
  modules/
    registerCore.ts            # APIs/repos/use cases “transversais”
    recrutamento/
      registerRecrutamento.ts        # APIs/repos recrutamento
      registerRecrutamentoUseCases.ts # use cases da jornada (fim do setup)
    minhaJornada/
      registerMinhaJornada.ts
    gestaoDesempenho/
      registerGestaoDesempenho.ts
    …
```

**`bootstrap.ts` (exemplo conceitual):**

```ts
export const setupDependencyInjection = () => {
  registerCore(container)
  registerRecrutamento(container)
  registerMinhaJornada(container)
  registerRecrutamentoUseCases(container) // no final: UCs que dependem de vários repos já registrados
  // ordem fixa; novos módulos = nova linha no FINAL (reduz conflito)
}
```

**`registerRecrutamento.ts`:** APIs e repositórios da jornada (inclui **GestorExternoPerfil**, usado no fluxo perfil atuação). **`registerRecrutamentoUseCases.ts`:** use cases da jornada — chamado **no final** de `setupDependencyInjection`, quando repositórios transversais já existem no container. Nesses **dois arquivos**, manter **imports em ordem alfabética** (types, depois APIs/impl) para facilitar code review.

**Tokens:**  
- Opção **A:** `DiTokens.recrutamento = { vagaApi: Symbol.for('...') }` — uso: `DiTokens.recrutamento.vagaApi`.  
- Opção **B:** manter flat `DiTokens.vagaApi` mas definido em `tokens/recrutamento/recrutamento.tokens.ts` e agregado em `tokens/index.ts`.  
Recomendação: **B** na migração (menos churn no código existente); **A** em greenfield.

### 3.2 Rotas

```
src/app/routes/
  AppRoutes.tsx
  modules/
    recrutamento/
      index.ts
      RecrutamentoRoutes.tsx              # Fragment de <Route> filhos (sem <Routes> aninhado — RR7)
      RecrutamentoLegacyPrivateRoutes.tsx # só RedirectGestaodevagas (React Router exige <Route> direto em AppRoutes)
    MinhaJornadaRoutes.tsx
```

`AppRoutes.tsx` fica com poucas linhas por módulo:

```tsx
<Route path="/recrutamento" element={<Outlet />}>
  {RecrutamentoChildRoutes /* Fragment com <Route> filhos */}
</Route>
```

**React Router 7:** evitar `<Routes>` dentro de `element={<AlgumComponente />}>` nesse padrão — pode gerar `[X] is not a <Route>`. Preferir **rotas aninhadas** com `<Outlet />` + filhos `<Route>`.

Conflitos: alterações de rotas de recrutamento concentram-se em `RecrutamentoRoutes.tsx` (filhos) e um bloco em `AppRoutes`.

---

## 4. Relação com domain / data / presentation (ARCHITECTURE.md)

**Não é obrigatório** na primeira fase mover `domain/usecases` para dentro de `modules/recrutamento/`.  
A modularização **imediata** é no **composição** (DI + rotas). Benefício alto, risco baixo.

**Fase 2 (opcional, vertical slice):**  
Para máxima autonomia, espelhar:

- `src/domain/recrutamento/` (interfaces + use cases da jornada)  
- `src/data/recrutamento/` (apis + repos impl)  

Isso exige política de imports e possivelmente path aliases (`@domain/recrutamento`). Só vale se o time assumir refator grande.

---

## 5. Widgets e componentes comuns (evitar duplicidade)

| Prática | Detalhe |
|---------|---------|
| **Catálogo** | Documentar em DESIGN_SYSTEM ou `docs/COMPONENTES_COMUNS.md`: “confirmação → `AlertDialog` padronizado X”, “form modal → padrão Y”. |
| **Composição** | Variantes via props (`variant`, `size`) em um único `ConfirmActionDialog`, não segundo arquivo copiado. |
| **Páginas semelhantes** | Extrair **layout + hooks** compartilhados; páginas finas só com wiring. |
| **Onde fica** | Continua em `presentation/components` agrupado por função (`common/`, `ui/` do shadcn), não por módulo de negócio — **exceto** quando o componente só serve uma jornada (aí pode ir em `pages/recrutamento/components`). |

Modularizar DI **não** incentiva dois modais iguais; o code review e o catálogo impedem.

---

## 6. Migração sugerida (incremental)

| Etapa | Ação | Risco |
|-------|------|-------|
| 1 | Criar `modules/recrutamento/registerRecrutamento.ts` e mover blocos de APIs/repos de recrutamento; chamar no início do bloco de APIs. | Baixo |
| 1b | Criar `registerRecrutamentoUseCases.ts` com os UCs da jornada; chamar **no final** de `setupDependencyInjection`. | Baixo |
| 2 | Extrair `tokens/recrutamento/recrutamento.tokens.ts` e compor `DiTokens` no `tokens/index.ts`. | Baixo |
| 3 | Repetir para próxima jornada mais conflituosa (ex.: gestão desempenho / PDI). | Baixo |
| 4 | Extrair rotas da jornada de `AppRoutes` → `routes/modules/<jornada>/` (nested + redirects legados + rotas públicas da jornada, se houver). | Baixo |
| 5 | **Presentation:** páginas da jornada em `pages/<jornada>/` + barrel `index.ts` (ver doc presentation). | Baixo |
| 6 | Regra de MR: **proibido** inflar `container.ts`/`AppRoutes` — novos registros só em `register{Modulo}*.ts` e novas rotas no módulo da jornada. | Processo |

---

## 7. Conflitos com agentes (IA)

- Instruir agentes: **nunca** reformatar arquivo inteiro de `container`/`tokens`; **sempre** editar apenas os arquivos em `src/core/di/modules/<jornada>/` do módulo da task. Manter **imports em ordem alfabética** em `register<Jornada>.ts` e `register<Jornada>UseCases.ts`.  
- Cursor rule sugerida: “Novos registros DI apenas em `src/core/di/modules/<jornada>/register{Feature}.ts` (ou `register{Feature}UseCases.ts`).”

---

## 8. Trade-offs

| Vantagem | Custo |
|----------|--------|
| Menos conflito entre squads | Mais arquivos; precisa descobrir “em qual register está X” (mitigação: README em `di/modules/`). |
| PRs menores e revisáveis | Migração inicial gera um PR grande **uma vez** por módulo extraído. |
| Mantém Clean Architecture | Não resolve sozinho domínio mal modelado — só isola mudanças mecânicas. |

---

## 9. Piloto implementado — módulo Recrutamento (referência no código)

| Artefato | Local |
|----------|--------|
| Tokens da jornada | `src/core/di/tokens/recrutamento/recrutamento.tokens.ts` (agregados em `tokens/index.ts`) |
| Registro DI (repos) | `src/core/di/modules/recrutamento/registerRecrutamento.ts` — APIs/repos + **GestorExternoPerfil**. Nesse arquivo: imports em ordem alfabética. |
| Registro DI (UCs) | `src/core/di/modules/recrutamento/registerRecrutamentoUseCases.ts` — chamado **no final** de `setupDependencyInjection`. Nesse arquivo: imports em ordem alfabética. |
| Rotas | `RecrutamentoChildRoutes` (Fragment de `<Route>`) sob `path="/recrutamento"` + `<Outlet />`; sem `<Routes>` aninhado. `/public/vaga` e redirects legados em `AppRoutes`. |
| Presentation — pages | `src/presentation/pages/recrutamento/` + barrel `@presentation/pages/recrutamento` |
| Hooks | `src/presentation/hooks/recrutamento/` + `@presentation/hooks/recrutamento` — ver `hooks/recrutamento/README.md` e doc presentation |

**Em `AppRoutes` (explícito):** bloco `<Route path="/recrutamento" element={<Outlet />}>` + `{RecrutamentoChildRoutes}`; rota `/public/vaga`; redirects legados (`/candidaturas`, etc.) como `<Route>` literais. `/prototipo/matchTalentos` na raiz até decisão de produto.

---

## 10. Checklist — replicar para **outra jornada** (outro time)

1. **Tokens:** criar subpasta `tokens/<jornada>/` com `<jornada>.tokens.ts` e compor em `tokens/index.ts` (padrão flat `DiTokens`, como Recrutamento).
2. **DI repos/APIs:** criar subpasta `di/modules/<jornada>/` com `register<Jornada>.ts` — registrar cedo no `setupDependencyInjection` (após padrão atual do `container.ts`). Nesse arquivo, **imports em ordem alfabética** (types, depois APIs/impl).
3. **DI use cases:** se a jornada tiver muitos UCs no `container.ts`, extrair `register<Jornada>UseCases.ts` na mesma pasta `di/modules/<jornada>/` e chamar **no final** do setup (após todos os repos necessários). Nesse arquivo também, imports em ordem alfabética.
4. **Uma linha** em `container.ts`: import de `./modules/<jornada>/register<Jornada>` e `./modules/<jornada>/register<Jornada>UseCases`; chamar `register<Jornada>Module(container)` e `register<Jornada>UseCases(container)` se existir.
5. **Rotas:** pasta `app/routes/modules/<jornada>/`; padrão **Outlet + Fragment de `<Route>`** (§12). Redirects no topo do mesmo `<Routes>` = `<Route>` explícitos em `AppRoutes`, não componente que só retorna `<Route>`.
6. **Páginas:** `presentation/pages/<jornada>/` + `index.ts`; rotas importam do barrel.
7. **Hooks:** `presentation/hooks/<jornada>/` + barrel `index.ts`; páginas e componentes da feature importam `@presentation/hooks/<jornada>`; hooks genéricos ficam na raiz de `hooks/`.
8. **Validação:** `npm run build:hml` + `npm run preview:8080` → http://localhost:8080/

**Scripts úteis (package.json):** `build:hml` (= homologation), `preview:hml` (build + preview 8080), `preview:8080` (só servir `dist`).

---

## 11. ARCHITECTURE.md

Subseção **“Módulos de DI e rotas”** + presentation em `ARCHITECTURE.md` referencia estes docs.

---

## 12. React Router 7 — erros que já ocorreram no build / runtime

O projeto usa **`react-router-dom` v7**. Dois padrões **quebram** a app (tela branca / ErrorBoundary / mensagem *"[X] is not a \<Route\> component"*):

| ❌ Evitar | Por quê | ✅ Fazer |
|-----------|---------|----------|
| Filho **direto** de `<Routes>` = componente customizado que retorna só `<Route>`(s) (ex.: `<MeuModuloPublicRoute />`) | O parser só aceita `<Route>` ou `<React.Fragment>` como filhos **imediatos** de `<Routes>`. | Declarar cada `<Route ... />` no JSX de `AppRoutes` **ou** usar `<Fragment>{...}</Fragment>` com `<Route>` dentro, **sem** wrapper function. |
| `<Route path="/foo/*" element={<ComponenteQueRenderizaRoutes />} />` com **`<Routes>` interno** | Em produção/minify o componente interno vira filho inválido da árvore de rotas. | **Rotas aninhadas:** `<Route path="/foo" element={<Outlet />}>` + filhos `<Route path="sub" ... />` exportados como **Fragment** (como `RecrutamentoChildRoutes`). |
| Esquecer `register<Jornada>UseCases` **no final** do DI | UCs podem depender de repos registrados depois do módulo. | Chamar `registerRecrutamentoUseCases(container)` (ou equivalente) **por último** em `setupDependencyInjection`. |

**Padrão de referência (Recrutamento):** `src/app/routes/modules/recrutamento/RecrutamentoRoutes.tsx` exporta `RecrutamentoChildRoutes` = `<Fragment><Route index .../><Route path="..." .../></Fragment>`; `AppRoutes` monta `<Route path="/recrutamento" element={<Outlet />}>{RecrutamentoChildRoutes}</Route>`.

**Validação após mudar rotas:** `npm run build:hml` + abrir **http://localhost:8080/** (login e uma rota do módulo).

---

**Resumo:** **`register{Jornada}`** + **`tokens.{jornada}`** + **`routes/modules/{jornada}/`** (Outlet + Fragment) + **`pages/{jornada}/`** reduzem merge. UI comum no DS; telas da jornada na pasta do módulo.
