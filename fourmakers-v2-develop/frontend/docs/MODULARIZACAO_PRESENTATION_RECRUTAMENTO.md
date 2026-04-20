# Modularização presentation — guia para outros times

**Público:** squads que vão modularizar **telas e hooks** da própria jornada, alinhado ao piloto Recrutamento.  
**Leia antes:** [`PROPOSTA_MODULARIZACAO_DI_ROTAS.md`](./PROPOSTA_MODULARIZACAO_DI_ROTAS.md) (DI, tokens, rotas).

---

## 1. O que o piloto Recrutamento já fez (presentation)

| Item | Onde | Status |
|------|------|--------|
| Páginas da jornada | `src/presentation/pages/recrutamento/` | ✅ Todas as telas de `/recrutamento/*` + `PublicVagaDetalhePage` |
| Barrel de import | `@presentation/pages/recrutamento` → `index.ts` | ✅ |
| Hooks da jornada | `src/presentation/hooks/recrutamento/` + barrel `@presentation/hooks/recrutamento` | ✅ Ver README na pasta |

**Exemplos de import:**

```ts
import { GestaoVagas, DashboardRecrutamento } from '@presentation/pages/recrutamento'
import { useGestaoVagas, formatDatePtBr } from '@presentation/hooks/recrutamento'
```

**Fora do módulo (decisão consciente):** `MatchTalentosPage` em `/prototipo/matchTalentos` permanece em `pages/` até a rota oficial migrar.

---

## 2. Checklist — replicar páginas em **sua** jornada

1. Criar pasta `src/presentation/pages/<sua-jornada>/` (ex.: `minha-jornada`, `folha`).
2. Mover **somente** páginas montadas pelas rotas da jornada (evitar mover telas compartilhadas).
3. Criar `index.ts` com **exports nomeados** (evita default export opaco no barrel):

   ```ts
   export { default as MinhaTelaPrincipal } from './MinhaTelaPrincipal'
   export { MinhaTelaDetalhe } from './MinhaTelaDetalhe' // se for named export
   ```

4. Atualizar o arquivo de rotas do módulo (`app/routes/modules/<jornada>/`) para importar do barrel.
5. **Hooks:** pasta `hooks/<jornada>/` + barrel; páginas importam `@presentation/hooks/<jornada>` (como Recrutamento).
6. Rodar `npm run build:hml` e smoke nas rotas afetadas (**http://localhost:8080** após `preview:8080`).
7. Documentar no README das pastas `pages/` e `hooks/` o prefixo de rota e os barrels.

**Não mover para a pasta da jornada:** componentes de DS, `common/`, telas usadas por **várias** jornadas.

---

## 3. Hooks por jornada (piloto Recrutamento concluído)

1. Criar `src/presentation/hooks/<sua-jornada>/` com os hooks **da jornada** (e subpastas de utils se fizer sentido, ex.: `gestaoVagas/`).
2. **`index.ts` (barrel)** reexportando hooks + tipos + helpers que as páginas e componentes da feature precisam importar num único lugar.
3. Atualizar imports em **páginas** do módulo; depois em **componentes** que só servem essa jornada (ex.: Recrutamento usa `gestao-vagas` → importam de `@presentation/hooks/recrutamento`).
4. Hooks **compartilhados** (`useViaCep`, `useGrupos`, etc.) permanecem em `presentation/hooks/` na raiz.

**Referência:** `src/presentation/hooks/recrutamento/README.md`.

---

## 4. Validação visual (build HML + preview)

Ambiente **homologation** (`.env.homologation`):

| Comando | Uso |
|---------|-----|
| `npm run build:hml` | `tsc -b` + `vite build --mode homologation` |
| `npm run preview:hml` | Build HML + sobe preview na porta **8080** |
| `npm run preview:8080` | Só preview (usa o `dist` já gerado) |

Abrir **http://localhost:8080/** e validar login, `/recrutamento`, redirects legados (`/candidaturas`, etc.) e `/public/vaga` se aplicável.

---

## 5. Mapa de arquivos — piloto Recrutamento (referência)

```
presentation/pages/recrutamento/
  index.ts
  GestaoVagas.tsx, GestaoVagasCandidatos.tsx, GestaoVagasRelatorios.tsx, GestaoVagasAdmissao.tsx
  DashboardRecrutamento.tsx, ParametrizacaoRecrutamento.tsx, CandidaturasFMU.tsx
  CriarPerfilAtuacao.tsx, HistoricoCandidatura.tsx, EditarVaga.tsx, VagasDetalhePage.tsx
  GerarMatchPromptPage.tsx, TemplateContratacaoCandidato.tsx + .constants.ts
  TalentosInscritos.tsx, MinhasImportacoes.tsx, PublicVagaDetalhePage.tsx

presentation/hooks/recrutamento/
  index.ts (barrel), README.md
  useGestaoVagas.ts, useGestaoVagasCandidatos.ts, useDashboardRecrutamento.ts
  usePublicVagaDetalhe.ts, useTemplateContratacaoCandidato.ts
  usePerfilAtuacao.ts, usePerfilAtuacaoPage.ts
  gestaoVagas/gestaoVagasUtils.ts, gestaoVagas/gestaoVagasStatusFlows.ts
```

Rotas: `RecrutamentoChildRoutes` + `path="/recrutamento"` + `<Outlet />` (React Router **7** — não usar `<Routes>` dentro do `element` da rota pai). Redirects em `AppRoutes`.

---

## 6. Conflitos de merge — onde cada time edita

| Jornada | Pastas típicas (não competir com outro módulo) |
|---------|------------------------------------------------|
| Recrutamento | `di/modules/recrutamento/registerRecrutamento*.ts`, `di/tokens/recrutamento/recrutamento.tokens.ts`, `routes/modules/recrutamento/`, `pages/recrutamento/`, **`hooks/recrutamento/`** |
| *Sua jornada* | `modules/sua-jornada/register{SuaJornada}*.ts`, `tokens/sua-jornada/suaJornada.tokens.ts`, `routes/modules/sua-jornada/`, `pages/sua-jornada/`, `hooks/sua-jornada/` |

O `container.ts` e `AppRoutes.tsx` ganham poucas linhas por módulo (`registerX`, bloco `<Route path="/jornada" element={<Outlet />}>` + child routes). Recrutamento também deixa redirects legados e `/public/vaga` em `AppRoutes` — é intencional (RR7).

---

## 7. React Router 7 — não repetir os erros do piloto

Antes de abrir PR de rotas, confira:

1. **Nada** de `<MinhaRotaWrapper />` como filho direto de `<Routes>` se o wrapper não for literalmente `<Route>` ou `<>` com `<Route>` dentro.
2. **Nada** de segundo `<Routes>` dentro do `element` de uma rota pai tipo `path="/jornada/*"` — use **`path="/jornada"` + `<Outlet />` + filhos `<Route>`** (exportar Fragment como no `RecrutamentoChildRoutes`).
3. Rotas públicas isoladas (`/public/...`) no mesmo `<Routes>` raiz: **`<Route path="..." element={<Página />} />`** explícito em `AppRoutes`, se não couber no Fragment aninhado.
4. Depois de alterar rotas: **`npm run build:hml`** e smoke em **8080**.

Detalhes e tabela: [**PROPOSTA §12**](./PROPOSTA_MODULARIZACAO_DI_ROTAS.md#12-react-router-7--erros-que-já-ocorreram-no-build--runtime).
