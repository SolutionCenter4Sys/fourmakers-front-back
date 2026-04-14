# Auditoria: fix/dashboard-metricas

**Branch:** `fix/dashboard-metricas`  
**Escopo:** Tela `/recrutamento/dashboard` — listagem de recrutadores (nova API RecrutadorListagem) e parâmetro CodigoRecrutador nas métricas.  
**Objetivo:** Verificar conformidade com **arquitetura**, **frontend** e **design system** do projeto.

Referências: `ARCHITECTURE.md`, `public/design-toolkit.md`, `DESIGN_SYSTEM_AUDIT.md`.

---

## 1. Resumo executivo

| Categoria        | Status | Observações |
|------------------|--------|-------------|
| **Arquitetura**   | ✅ Conforme | Presentation usa apenas Use Cases (ListarRecrutadoresGestaoAlocadosUseCase, ObterDashboardMetricasRecrutamentoUseCase, etc.); APIs em @data; parâmetro CodigoRecrutador nos filtros. |
| **Frontend**      | ✅ Conforme | Hook useDashboardRecrutamento orquestra estado e Use Cases; buildFiltrosParams usa codigoRecrutador; sem fetch() direto. |
| **Design System** | ✅ Conforme | Componentes DS (Button, Card, Input, Label, Badge, Command, Table); tokens (bg-destructive, bg-success, bg-warning, text-muted-foreground); system-icons. |

**Conclusão:** As alterações estão em conformidade. Nenhum item crítico em aberto.

---

## 2. Arquitetura

### 2.1 Camadas e dependências

- **Presentation:**
  - **DashboardRecrutamento.tsx** importa apenas `useDashboardRecrutamento` e componentes de apresentação/common; nenhum import de `@data/api`.
  - **useDashboardRecrutamento.ts** usa apenas Use Cases resolvidos via `container.resolve`: ListarRecrutadoresGestaoAlocadosUseCase, ListarClientesGestaoAlocadosUseCase, ObterDashboardMetricasRecrutamentoUseCase, ObterDashboardMetricasVagasEmFocoUseCase, ObterDashboardMetricasFunilDeVagasUseCase, ObterDashboardMetricasVagasPerdidasMotivoUseCase, ObterDashboardNovosCandidatosPorOrigemUseCase.

- **Data:**
  - **RecrutadoresGestaoAlocadosApi**: passa a chamar `GET /api/Candidatura/RecrutadorListagem?cursor=0&limite=100`; mapeia retorno `{ codigoRecrutador, nomeRecrutador }` para `RecrutadorGestaoAlocados`; filtro por busca no cliente.
  - **DashboardMetricasRecrutamentoApi**: usa `buildCandidaturaQueryParamsRecrutamento`, que agora envia **CodigoRecrutador** (não CodigoGestor) na query de `dashboardMetricasRecrutamento`.

- **Domain:**
  - **RecrutadorGestaoAlocados**: campo opcional `codigoRecrutador` adicionado; usado nos filtros.
  - **FiltrosCandidaturaParams**: já possuía `CodigoRecrutador`; alimentado a partir de `codigoRecrutador` (ou fallback codGestorExterno/codigoInternoColaborador) em `buildFiltrosParams`.

### 2.2 Verificação

| Critério | Atende? | Evidência |
|----------|---------|-----------|
| Presentation não importa @data/api | ✅ | DashboardRecrutamento e useDashboardRecrutamento usam apenas Use Cases e tipos @domain/@shared. |
| APIs em @data com httpClient | ✅ | RecrutadoresGestaoAlocadosApi e DashboardMetricasRecrutamentoApi usam httpClient.get. |
| Parâmetro CodigoRecrutador na métrica principal | ✅ | buildCandidaturaQueryParamsRecrutamento passou a usar CodigoRecrutador em candidaturaFiltrosUtils. |
| Filtros alimentados com codigoRecrutador | ✅ | buildFiltrosParams (dashboardRecrutamentoUtils) usa r.codigoRecrutador ?? r.codGestorExterno ?? r.codigoInternoColaborador. |

---

## 3. Frontend

### 3.1 Comportamento

- Listagem de recrutadores: disparada ao digitar (busca) via ListarRecrutadoresGestaoAlocadosUseCase; resposta da RecrutadorListagem mapeada e filtrada no cliente por termo quando informado.
- Filtros: clientes e recrutadores selecionados viram CodigoCliente e CodigoRecrutador em FiltrosCandidaturaParams; todas as chamadas de métricas (dashboard principal, vagas em foco, funil, vagas perdidas, novos candidatos por origem) recebem os mesmos filtros.
- Estado: recrutadoresDisponiveis, recrutadoresSelecionados, buildFiltrosParams; identificação por codigoInternoColaborador (igual a codigoRecrutador no mapeamento da nova API) mantém compatibilidade com a UI (badges, remoção).

### 3.2 Verificação

| Critério | Atende? | Evidência |
|----------|---------|-----------|
| Sem fetch() direto na feature | ✅ | Nenhum fetch em DashboardRecrutamento, useDashboardRecrutamento ou APIs alteradas (httpClient). |
| useCallback/useMemo no hook | ✅ | useDashboardRecrutamento usa useCallback e useMemo onde apropriado. |
| Tipos de domínio (@domain/entities) | ✅ | FiltrosCandidaturaParams, RecrutadorGestaoAlocados, DashboardMetricasRecrutamentoRetorno, etc. |

---

## 4. Design System

### 4.1 Componentes e tokens

- **DashboardRecrutamento.tsx:** Button, Card, CardContent, Input, Label, Badge, Command, CommandInput, CommandList, CommandGroup, CommandItem, Table (TableHeader, TableBody, TableRow, TableCell, TableHead), PageHeader, PageBreadcrumb, DashboardRecrutamentoModal.
- **Tokens:** Badge com `bg-destructive text-destructive-foreground`, `bg-success text-success-foreground`, `bg-warning text-warning-foreground`; `text-muted-foreground`, `border-borderSoft`, `bg-background`.
- **Ícones:** AlertTriangle, ArrowUpDown, Briefcase, Timer, TrendingDown, TrendingUp, Users, X, XCircle de `@/components/ui/system-icons`.

### 4.2 Verificação

| Critério | Atende? | Evidência |
|----------|---------|-----------|
| Componentes da lib @/components/ui | ✅ | Uso consistente dos componentes do DS. |
| Tokens de cor (sem hardcode) | ✅ | Cores de status e textos com tokens (destructive, success, warning, muted). |
| system-icons | ✅ | Ícones importados de system-icons. |

---

## 5. Checklist final

| Item | Status |
|------|--------|
| Listagem de recrutadores via api/Candidatura/RecrutadorListagem | ✅ |
| Filtros de métricas com CodigoRecrutador (dashboardMetricasRecrutamento) | ✅ |
| buildFiltrosParams usa codigoRecrutador (entidade) | ✅ |
| Presentation sem import de @data/api | ✅ |
| Use Cases via DI no hook | ✅ |
| httpClient nas APIs (sem fetch direto) | ✅ |
| Componentes e tokens do DS na página | ✅ |

---

*Auditoria gerada para a branch `fix/dashboard-metricas`. Data: Fev 2026.*
