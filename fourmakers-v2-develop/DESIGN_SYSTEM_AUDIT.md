# Auditoria de Design System e Qualidade de Código Front

Este documento é o **relatório de auditoria** de conformidade do frontend com o Design System, arquitetura e boas práticas do projeto. Deve ser executado sempre que uma **nova feature** for entregue (ou no projeto como um todo, quando não houver feature específica no prompt) para garantir qualidade, consistência e conformidade.

---

## Referência obrigatória: Design Toolkit

**Localização do Design Toolkit (referência para esta auditoria):**

| Recurso | Caminho | Uso |
|--------|---------|-----|
| **Design Toolkit** | `public/design-toolkit.md` | Guia completo: tokens, componentes, modais, acessibilidade, arquitetura, hooks, organização. **Consulte este arquivo para critérios de conformidade.** |
| **Arquitetura** | `ARCHITECTURE.md` (raiz) | Clean Architecture, fluxo Page → UseCase → Repository → API → httpClient. |
| **Guia de Componentes** | Rota `/documentacao` (app) | Exemplos visuais e uso de componentes do DS. |
| **Documentação técnica de features** | `docs/ORIENTACAO_DOCUMENTACAO_TECNICA_FEATURES.md` | Orientações para criar documentação técnica de novas features (modelo: `docs/FEEDBACK360_DOCUMENTACAO_TECNICA.md`). |

**Como usar esta auditoria:**
- Para **uma feature nova**: informe no prompt a rota ou pasta da feature (ex.: `/permissionamento`, `src/presentation/components/permissionamento`). A análise será focada nessa área, com menção a impacto no projeto quando relevante.
- Para **projeto como um todo**: não especifique feature; a varredura cobre `src/presentation` e componentes globais.
- Cada execução deve: (1) atualizar a **Timeline** com um resumo do escopo; (2) preencher **Auditoria e problemas**; (3) **Sugestões e plano de ação**; (4) **Relatórios de uso e componentes**; (5) **Resumo para negócio**; (6) adicionar uma linha no **Changelog** ao final.
- **Documentação técnica em `docs/`:** Se a feature auditada **não tiver** documentação técnica na pasta `docs/` (ex.: `[FEATURE]_DOCUMENTACAO_TECNICA.md`), ela deve ser **criada** quando forem realizadas as correções apontadas na auditoria, seguindo `docs/ORIENTACAO_DOCUMENTACAO_TECNICA_FEATURES.md` e o modelo `docs/FEEDBACK360_DOCUMENTACAO_TECNICA.md`. Se a feature **já tiver** documentação em `docs/`, as correções da auditoria devem ser **refletidas na documentação** quando necessário (mudança de contratos, endpoints ou regras de negócio).

---

## Timeline de auditorias

Resumo das últimas execuções (escopo e resultado principal).

| Data | Escopo | Resumo |
|------|--------|--------|
| Jan 2026 | Projeto (inicial) | Auditoria inicial; Input com estado de erro aplicado e documentado; relatório de uso e componentes não mapeados. |
| Fev 2026 | Feature **Permissionamento** (`/permissionamento`) | Loading corrigido (Loader2 → Spinner); cores hardcoded em status ativo corrigidas (verde → tokens `success`); modais e httpClient em conformidade. |
| Fev 2026 | **Permissionamento** — conclusão e doc | Correções pertinentes à branch/página aplicadas; auditoria atualizada; itens globais e de outras features mantidos para outros times. Feature em conformidade. |
| Fev 2026 | **Permissionamento** — conformidade arquitetural | Presentation não importa mais @data/api; criados GrupoAcessoRepository, FuncionalidadeSistemaRepository, PermissionamentoRepository e UseCases; componentes e useFeedback360 refatorados para usar UseCases via DI. |
| Fev 2026 | Feature **Gestão de Vagas / Candidatos** (`/gestaodevagas/candidatos`, `gestao-vagas`) | Auditoria pós-sync com develop. Cores em conformidade (tokens destructive/success/warning). Identificados: violação arquitetural (presentation → @data/api), loading com RefreshCw/Loader2 em vez de Spinner, modais com subtítulo em \<p\> em vez de DialogDescription. |
| Fev 2026 | Feature **Relatórios de Vagas** (`/gestaodevagas/relatorios`) | Página em conformidade: Use Cases via DI, Input do DS, Spinner no loading, tokens do DS; sem @data/api na presentation. |
| Fev 2026 | **Hotfix Sidebar Royal** (`hotfix/sidebar-royal`) | Label do menu lateral: para orgId 9 (Royal), exibir "Gestão Unidades" no lugar de "Gestão Clientes". Sidebar em conformidade: sem @data/api; orgId do auth; DS e acessibilidade preservados. |
| Fev 2026 | **Fix Jornada Recrutamento Vagas** (`fix/jornada-recrutamento-vagas`) | Ajustes de UX na jornada Gestão de Vagas: período 30d em relatórios; labels de data no kanban; SLA nos cards de vaga; botão voltar só em páginas internas; modal Meus Candidatos (header em 2 linhas); rota histórico em /gestaodevagas/historico-candidato; link Histórico em Talentos inscritos; Ratecard opcional; bordas de erro em campos obrigatórios ao salvar vaga. Conformidade: arquitetura, DS e front preservados. |

---

## 1. Auditoria e problemas levantados

### 1.1 O que está aplicado (projeto / feature)

- **Design System:** Componentes base do DS (Button, Card, Input, Select, Dialog, Badge, Table, etc.); uso de tokens (`border-borderSoft`, `bg-surfaceElevated`, `text-muted-foreground`); modais com `DialogTitle` e `DialogDescription`; radius (LG para inputs/cards, Pill para botões).
- **Input com estado de erro:** `@/components/ui/input` com `error?: boolean`, `aria-invalid`; páginas com validação devem passar `error` e exibir mensagem (ver seção *Input com estado de erro* mais abaixo).
- **Arquitetura:** APIs usando `httpClient` (não `fetch()` direto); UseCases/Repositories onde aplicável; páginas orquestrando UI. **Feature Permissionamento e hook useFeedback360:** conformidade rigorosa — presentation usa apenas UseCases (ListarGruposAcesso, Criar/EditarGrupoAcesso, ListarFuncionalidadesSistema, Atribuir/RemoverFuncionalidadeGrupo, Adicionar/RemoverUsuarioGrupo, ListarClientesGestaoAlocados, ObterUsuarioIdPorCpf; ListarColaboradoresOrg no Feedback360); repositórios e implementações em domain/data.

### 1.2 Input com estado de erro: como analisar e aplicar

- **Componente:** `Input` em `@/components/ui/input` — aceita `error?: boolean`; exibe borda destrutiva e `aria-invalid` quando `error={true}`.
- **Página/formulário:** Define quando o campo está em erro; mantém estado por campo; passa `error={...}` ao Input; exibe mensagem de erro; limpa erro quando validação passar.
- Qualquer formulário com validação que use o Input do DS deve seguir essa lógica (consistência e acessibilidade).

### 1.3 Problemas identificados (abertos)

**Nota:** Itens abaixo que **não** são da feature Permissionamento (`/permissionamento`) ou da branch atual permanecem na auditoria para revisão pelos respectivos times. Para a **feature Permissionamento (branch feat/permissionamento-v1)** todos os itens pertinentes foram corrigidos (loading com Spinner; cores de status com tokens success).

#### Cores hardcoded (Alta prioridade) — outros times

- **`src/presentation/components/common/StatusBadge.tsx`**  
  Usa `bg-blue-100`, `bg-gray-100`, `bg-green-100` etc. → trocar por tokens (`bg-info/10`, `bg-muted`, `bg-success/10` etc.).

- **`src/presentation/components/mapa-alocacao/VisaoGerencialTab.tsx`**  
  Cores amarelo/verde/vermelho e `bg-white` → usar tokens de status e `bg-surfaceElevated`.

- **`src/presentation/components/mapa-alocacao/AlocacoesTab.tsx`**  
  Múltiplas cores green/white hardcoded → tokens e `bg-surfaceElevated`.

- **`src/presentation/components/mapa-alocacao/ChatIan.tsx`**  
  `text-green-*`, `text-red-*` e modal sem `DialogDescription` → tokens e adicionar `DialogDescription`.

- **`src/presentation/components/profile/ExperienceSection.tsx`**  
  `bg-blue-*` → `bg-primary` / `bg-accent` e tokens.

- **`src/presentation/components/minha-jornada/SkillCard.tsx`**  
  `bg-gray-*`, `bg-white/50` → `bg-muted/20`, `bg-background/50`.

- **Feature Permissionamento (feat/permissionamento-v1):**  
  **`GruposTab.tsx`** e **`FuncionalidadesSistemaTab.tsx`**: ~~uso de `bg-green-100 text-green-800` em Badge/célula de status ativo~~ **Corrigido.** Substituído por `bg-success/10 text-success hover:bg-success/20`. Nenhum item em aberto para esta feature.

#### Violações de arquitetura (Alta prioridade) — outros times

- **`src/presentation/pages/RemessaCNAB.tsx`**  
  Uso de `fetch()` direto → usar `httpClient.get` (ou equivalente) conforme `ARCHITECTURE.md`.

#### Modais sem DialogDescription (Média prioridade) — outros times

- **`src/presentation/components/mapa-alocacao/ChatIan.tsx`**  
  Adicionar `DialogDescription` no modal.

#### Feature Gestão de Vagas / Candidatos (refactor/kanban-candidatos)

**O que está em conformidade:**

- **Cores:** Uso consistente de tokens do DS (`border-destructive`, `text-destructive`, `bg-success/10`, `text-success`, `bg-warning/10`, `text-warning`, `text-muted-foreground`) em `GestaoVagasCandidatos.tsx` e em `AlterarStatusCandidatoModal`, `GerarMatchPromptModal`, `InscreverTalentoModal`, etc. Nenhuma cor Tailwind hardcoded (ex.: `bg-green-500`).
- **Componentes:** Button, Card, Dialog, Input, Label, Select, Textarea, Badge, Tooltip, Table, Sheet, etc. da lib `@/components/ui`.
- **Formulários:** AlterarStatusCandidatoModal usa estado de erro em Select (borda destrutiva + mensagem) e Textarea com `aria-invalid`/mensagem de erro.
- **APIs:** Chamadas via `httpClient` (não há `fetch()` direto na feature); APIs em `@data/api` (CandidaturaApi, VagaApi, ColaboradoresApi, etc.) centralizam as requisições.

**Correções já aplicadas (Fev 2026):**

- **Spinner:** InscreverTalentoModal, MeusCandidatosModal, ModalDetalhesLote passaram a usar `Spinner` em vez de RefreshCw/Loader2.
- **DialogDescription:** AlterarStatusCandidatoModal (principal + modal sucesso), InscreverTalentoModal, GerarMatchPromptModal, MeusCandidatosModal, ModalDetalhesLote passaram a usar `DialogDescription` para texto descritivo.
- **Arquitetura (parcial):** Criados Use Cases e Repositories (Candidatura, Vaga, ColaboradorBancoDeTalentos, PerfilAtuacao); **AlterarStatusCandidatoModal**, **InscreverTalentoModal**, **GerarMatchPromptModal**, **MeusCandidatosModal**, **ModalDetalhesLote** passaram a usar apenas Use Cases (sem import de `@data/api`). Tipos de domínio em `@domain/entities/GestaoVagasCandidatos`.
- **Arquitetura — página e hook (Fev 2026):** **GestaoVagasCandidatos.tsx** e **useGestaoVagasCandidatos** passaram a usar apenas Use Cases (GetVagaDetalhes, ListarStatusCandidaturaRecrutamento, ObterTotaisInscritos, ListarCandidatosInscritos, ListarCandidatosAderentes, ListarUnidadesVaga, ListarTiposVaga, ListarTiposContratacaoVaga, CandidatarOutraPessoa, ListarOrigensColaborador) e tipos de `@domain`/`@shared`; normalizers em `@shared/utils/gestaoVagasNormalizers`. VagaRepository e ColaboradoresRepository estendidos; adapter `retornoMatchToAdherence` passou a importar `RetornoMatchRaw` de `@domain`.
- **Arquitetura — 8 modais (Fev 2026):** **AtribuirRecrutadorCandidatoModal**, **AtribuirRecrutadorModal**, **PerdaVagaModal**, **MovimentacaoVagaModal**, **InscricaoPreferenciasModal**, **InscreverCandidatoModal**, **ImportarTalentosModal**, **DuplicarVagaModal** passaram a usar apenas Use Cases e tipos `@domain`/`@shared`. VagaRepository estendido (adicionarRecrutadorVaga, listarMotivosPerdaVaga, gravarPerdaVaga, mudarStatusVaga, listarOpcoesContato, listarVagasRecrutamentoPorParentEmAndamento, inserirInformacoesComplementaresVagaRecrutamento). CurriculoColaboradorRepository e Use Cases (ImportarColaborador, ImportarColaboradorLinkedin, ImportarColaboradorLote, ImportarColaboradorLotePlanilha). PerfilAtuacaoRepository: GestorItem com email/codGestorExterno; ListarGestoresUseCase. **Feature Gestão de Vagas / Candidatos sem pontos críticos de arquitetura em aberto.**

**Problemas ainda em aberto para esta feature:** Nenhum (conformidade arquitetural concluída).

#### Fix Jornada Recrutamento Vagas (fix/jornada-recrutamento-vagas) — Fev 2026

**Escopo dos ajustes:**

- **Relatórios** (`/gestaodevagas/relatorios`): Período padrão 30 dias (Período de = D-30, Até = hoje).
- **Kanban de vagas** (`/gestaodevagas`): Labels dos filtros de data "Mostrando vagas criadas de" e "Até"; SLA/tempo decorrido nos cards (badge + tooltip, mesma experiência do kanban de candidatos); **sem** botão voltar na raiz.
- **Criar/editar perfil e vaga** (`/gestaodevagas/perfil`): Dias presenciais (híbrido) 1–4; skills novas com id 0 (Criar habilidade); Ratecard opcional ao salvar vaga; "Atualizar com Template" usando modelo de trabalho e dias do formulário; borda de erro em todos os campos obrigatórios ao falhar validação; botão voltar à esquerda do título.
- **Modal Meus Candidatos**: Título e texto auxiliar em duas linhas; `pr-8`/`pr-10` no header para não sobrepor o botão fechar.
- **Histórico de candidato**: Rota em `/gestaodevagas/historico-candidato/:codigoInternoColaborador`; redirect de `/recrutamento/historico-candidato/:id`; botão voltar na página; link "Histórico do candidato" em Talentos inscritos e em Meus Candidatos (modal) usando `codigoInternoColaborador`.

**Arquitetura:**

- Nenhuma nova importação de `@data/api` na camada de apresentação. Alterações restritas a rotas, labels, estado de UI, navegação e layout.
- Componente de redirect `RedirectHistoricoCandidatoToGestao` em `AppRoutes.tsx` usa `useParams` (adequado na camada de rotas). Páginas e modais apenas trocam URLs de navegação; Use Cases e Repositories inalterados.

**Design System:**

- Uso dos componentes existentes: Button, Badge, Tooltip, Input, Select, DialogHeader, PageHeader, PageBreadcrumb.
- Tokens: `border-destructive`, `focus-visible:ring-destructive`, `text-success`, `bg-success/10`, `border-success/50`, `pr-8`, `pr-10`, `text-muted-foreground`.
- Padrão do botão voltar: `variant="ghost"` `size="icon"`, ícone `ArrowLeft`, `aria-label="Voltar"`, alinhado à esquerda do título com `flex items-center gap-3`.
- Modal Meus Candidatos: header em `flex-col`, subtítulo com alinhamento (pl-[52px]); sem cores hardcoded.

**Frontend e acessibilidade:**

- Navegação: `navigate(-1)` no histórico; `navigate('/gestaodevagas')` onde aplicável; `encodeURIComponent` nos parâmetros de rota.
- SLA nos cards de vaga: mesma lógica e componentes do kanban de candidatos (`parseSlaToHours`, `formatSlaTooltip`, Badge com variante destrutiva quando > 2h).
- Breadcrumbs e `usePageTracking` atualizados para a nova rota de histórico.
- Sem itens em aberto para esta branch.

---

## 2. Sugestões de melhorias, varredura de TODOs e plano de ação

### 2.1 Varredura sugerida (comandos úteis)

```bash
# Cores hardcoded
grep -r "bg-\(white\|gray\|blue\|green\|red\|yellow\)-[0-9]" src/presentation

# fetch() direto
grep -r "fetch(" src/presentation

# Modais sem DialogDescription (checagem manual após grep)
grep -r "DialogTitle" src/presentation/components | grep -v "DialogDescription"
```

### 2.2 Plano de ação para conformidade

| Fase | Ação |
|------|------|
| **Fase 1 (Crítico)** | Corrigir StatusBadge; RemessaCNAB (httpClient); cores em VisaoGerencialTab e AlocacoesTab. ~~**Permissionamento**: verde em GruposTab/FuncionalidadesSistemaTab~~ (já corrigido). ~~**Gestão de Vagas / Candidatos:** UseCases/Repositories~~ — Conformidade concluída (página, hook e todos os 13 modais da feature sem import de `@data/api`). |
| **Fase 2 (DS)** | Corrigir cores em ChatIan, ExperienceSection, SkillCard; adicionar DialogDescription em ChatIan. ~~**Gestão de Vagas / Candidatos:** Spinner e DialogDescription~~ (já aplicado). |
| **Fase 3 (Validação)** | Nova varredura de cores, modais e uso de httpClient; re-executar esta auditoria após correções. |

### 2.3 TODOs recorrentes

- Garantir que **toda** nova feature use **Spinner** (`@/components/ui/spinner`) para loading, não ícones de font (ex.: Loader2 de system-icons) que possam exibir nome/código antes de carregar.
- Novos modais: sempre `DialogTitle` + `DialogDescription`.
- Novos formulários com Input do DS: usar prop `error` e mensagem de erro acessível.
- **Documentação técnica:** Feature nova auditada sem doc em `docs/` → criar `docs/[FEATURE]_DOCUMENTACAO_TECNICA.md` ao aplicar correções (ver `docs/ORIENTACAO_DOCUMENTACAO_TECNICA_FEATURES.md`). Feature já documentada → atualizar o doc se as correções da auditoria impactarem contratos, APIs ou regras de negócio.

---

## 3. Relatórios de uso, componentes não mapeados e melhorias em componentes globais

### 3.1 Consumo da lib global (`@/components/ui`) por página/feature

| Página / Feature | Componentes consumidos |
|-------------------|-------------------------|
| **Permissionamento** | Card, Button, Input, Label, Select, Dialog (Title, Description, Footer), Popover, Command, Table, Badge, AlertDialog, Tooltip, Spinner, Switch, Checkbox, ScrollArea, system-icons |
| **Simulator** | Button, Card, Dialog, Label, Textarea, Select, Badge, Switch, Tooltip, system-icons |
| **Gestão de Vagas / Relatórios** (`/gestaodevagas/relatorios`) | Button, Card, Input, Label, Collapsible, Spinner, PageBreadcrumb, PageHeader, system-icons |
| **RemessaCNAB** | Card, Button, Input, Label, Select, Popover, Command, Checkbox, Dialog, AlertDialog, Alert, system-icons |
| **GestaoVagas / GestaoVagasCandidatos** | Card, Button, Input, Checkbox, Popover, Select, Tooltip, Label, Switch, Badge, Dialog (Title, Description, Content, Header, Footer), Sheet, Table, Textarea, Slider, system-icons |
| **Gestão Desempenho / Parceria / Perfil Atuacao** | Dialog, Button, Input, Label, Textarea, Select, Badge, Popover, Command, ScrollArea, Separator, Alert, Tooltip, system-icons |
| **Timesheet / AprovarTimesheet** | Tabs, Button, Card, AlertDialog, Calendar, Popover, Label, RadioGroup, Badge, Skeleton, Checkbox, Textarea |
| **Minha Equipe / Mapa Relacionamento** | Card, Badge, Progress, Table, Button, Tooltip, Dialog, Select, Input, Switch, Avatar, AlertDialog, system-icons |

### 3.2 Componentes não mapeados (candidatos a globais ou melhorias)

| Componente | Localização | Uso atual | Sugestão |
|------------|-------------|-----------|----------|
| **PageHeader** | `presentation/components/common/PageHeader.tsx` | Múltiplas páginas | Manter em common; documentar em `/documentacao` como padrão de layout. |
| **PageBreadcrumb** | `presentation/components/common/PageBreadcrumb.tsx` | Idem | Idem. |
| **DataTable** | `presentation/components/common/DataTable.tsx` | Listagens em várias features | Manter common; considerar variante tokenizada ou spec no DocComponents. |
| **TablePagination** | `presentation/components/common/TablePagination.tsx` | Junto de DataTable | Documentar e alinhar com tokens. |
| **StatusBadge** | `presentation/components/common/StatusBadge.tsx` | Várias telas | **Prioridade:** corrigir cores hardcoded; depois considerar entrada na lib ou doc. |
| **StatCard** | `presentation/components/common/StatCard.tsx` | Dashboards, Reembolso, etc. | Candidato a componente de DS (card número + label + ícone). |
| **SearchCard** | `presentation/components/common/SearchCard.tsx` | Filtros em card | Padronizar com tokens; documentar. |
| **FullScreenLoader** | `presentation/components/common/FullScreenLoader.tsx` | Loading global | Candidato a Spinner/Overlay na lib com tokens. |

### 3.3 Reflexo na página `/documentacao`

- **Guia de Componentes:** inclui Input (com estado normal e erro), Button, Card, Tabs, Select, Dialog, etc.
- **Guia de Tokens:** cores, raios, espaçamentos, sombras.
- Recomendação: incluir exemplos de **Spinner** (loading) e padrão de **DialogTitle + DialogDescription** no guia.

---

## 4. Resumo para negócio (o que foi analisado, ações e sugestões)

- **O que é esta auditoria:** Verificação automática/assistida do código front para alinhamento ao Design System (componentes, cores, acessibilidade), à arquitetura (APIs, camadas) e às boas práticas (loading, modais, formulários).
- **O que foi analisado (ex.: Permissionamento):** Página e todos os componentes da feature (tabs, modais, listagens). Verificado: uso de componentes oficiais, tokens, loading, modais com título e descrição, uso de APIs via httpClient.
- **Ações já tomadas (Permissionamento):** Troca de ícone de loading (Loader2) por Spinner; substituição de cores verdes em status ativo por tokens (bg-success/10 text-success); sincronização da branch com develop. **Para esta branch/página não há itens em aberto.**
- **Itens mantidos na auditoria (outros times):** StatusBadge, RemessaCNAB, VisaoGerencialTab, AlocacoesTab, ChatIan, ExperienceSection, SkillCard, modais sem DialogDescription — revisão pelos respectivos times.
- **Sugestões:** Manter padrão de usar Spinner em novas features; demais itens do plano de ação seguem para os times responsáveis.
- **Feature Gestão de Vagas / Candidatos (Fev 2026):** Conformidade em cores (tokens) e componentes do DS. Aplicados: Spinner e DialogDescription nos modais; conformidade arquitetural concluída — página, hook e todos os modais da feature passaram a usar apenas Use Cases/Repositories (sem import de `@data/api`). Documentação técnica da feature pode ser criada em `docs/` (ver `docs/ORIENTACAO_DOCUMENTACAO_TECNICA_FEATURES.md`).

---

## 5. Changelog da auditoria

Cada execução desta auditoria deve adicionar **uma linha** com: utilizador (ou “Sistema”/“Auditoria automática”), data/hora, feature (ou “projeto”), branch, commit.

| Data/Hora | Usuário / Origem | Escopo | Branch | Commit | Observação |
|-----------|-------------------|--------|--------|--------|------------|
| Jan 2026 | — | Projeto (inicial) | — | — | Auditoria inicial; Input com erro; relatório de uso e componentes. |
| 2026-02-11 | Auditoria automática | Feature Permissionamento (`/permissionamento`) | feat/permissionamento-v1 | 27246c07 | Reestruturação do doc; referência design-toolkit; Timeline; auditoria Permissionamento: Spinner aplicado, cores green substituídas por tokens success. |
| 2026-02-11 | Auditoria automática | Feature Permissionamento (correções e atualização do doc) | feat/permissionamento-v1 | (atual) | Correções pertinentes à branch/página concluídas; auditoria atualizada com nota de escopo (itens globais/outras features mantidos para outros times). Permissionamento em conformidade. |
| 2026-02-12 | Auditoria automática | Conformidade arquitetural Permissionamento + useFeedback360 | feat/permissionamento | — | Repositórios e UseCases criados; componentes e hooks refatorados para não importar @data/api. Build HML e preview 8080 ok. |
| 2026-02-14 | Auditoria automática | Feature Gestão de Vagas / Candidatos (`/gestaodevagas/candidatos`, `gestao-vagas`) | refactor/kanban-candidatos | (sync develop) | Sync com develop; auditoria de design e front. Conforme: tokens de cor, componentes DS, formulários com erro. Em aberto: arquitetura (presentation → @data/api), Spinner em loading, DialogDescription em modais. |
| 2026-02-15 | Conformidade arquitetural | Gestão de Vagas / Candidatos — Use Cases e Repositories | refactor/kanban-candidatos | — | Domain: entidades GestaoVagasCandidatos, ColaboradorBancoDeTalentosRepository, PerfilAtuacaoRepository; CandidaturaRepository e VagaRepository estendidos. Data: ColaboradorBancoDeTalentosRepositoryImpl, PerfilAtuacaoRepositoryImpl; CandidaturaRepositoryImpl e VagaRepositoryImpl estendidos. Use Cases criados e registrados no container. AlterarStatusCandidatoModal, InscreverTalentoModal, GerarMatchPromptModal, MeusCandidatosModal, ModalDetalhesLote passaram a usar apenas Use Cases (sem @data/api). Página e demais modais ainda em aberto. Build HML ok. |
| 2026-02-17 | Conformidade arquitetural | Gestão de Vagas / Candidatos — Página e hook sem @data/api | refactor/kanban-candidatos | — | VagaRepository estendido (listarStatusCandidaturaRecrutamento, obterTotaisInscritos, listarUnidades, listarTiposVaga, listarTiposContratacao, candidatarOutraPessoa). ColaboradoresRepository: listarOrigensColaborador. Use Cases: ListarStatusCandidaturaRecrutamento, ObterTotaisInscritos, ListarCandidatosInscritos, ListarCandidatosAderentes, ListarUnidadesVaga, ListarTiposVaga, ListarTiposContratacaoVaga, CandidatarOutraPessoa, ListarOrigensColaborador. Normalizers em shared/utils/gestaoVagasNormalizers. GestaoVagasCandidatos.tsx e useGestaoVagasCandidatos passaram a usar apenas Use Cases e tipos @domain/@shared. RetornoMatchRaw em domain; adapter retornoMatchToAdherence atualizado. Build HML ok. 8 modais ainda em aberto. |
| 2026-02-17 | Conformidade arquitetural | Gestão de Vagas / Candidatos — 8 modais sem @data/api | refactor/kanban-candidatos | — | VagaRepository: adicionarRecrutadorVaga, listarMotivosPerdaVaga, gravarPerdaVaga, mudarStatusVaga, listarOpcoesContato, listarVagasRecrutamentoPorParentEmAndamento, inserirInformacoesComplementaresVagaRecrutamento. CurriculoColaboradorRepository + Impl + Use Cases (ImportarColaborador, ImportarColaboradorLinkedin, ImportarColaboradorLote, ImportarColaboradorLotePlanilha). PerfilAtuacaoRepository: GestorItem com email/codGestorExterno; ListarGestoresUseCase. Domain: MotivoPerdaVagaItem, GravarPerdaVagaPayload, MudarStatusVagaPayload, InserirInformacoesComplementaresPayload, OpcaoContatoItem, VagaFilhaItem; CurriculoColaborador (PdfProcessadoItem, etc.). AtribuirRecrutadorCandidatoModal, AtribuirRecrutadorModal, PerdaVagaModal, MovimentacaoVagaModal, InscricaoPreferenciasModal, InscreverCandidatoModal, ImportarTalentosModal, DuplicarVagaModal refatorados para Use Cases. Branch sem pontos críticos de arquitetura. Build HML ok. |
| 2026-02-19 | Auditoria automática | Feature Relatórios de Vagas (`/gestaodevagas/relatorios`) | develop | — | Página em conformidade: Use Cases via DI; Input do DS; Spinner no loading; tokens do DS; sem @data/api na presentation. Ajustes: Input para datas, Spinner no botão ao gerar, remoção de texto duplicado. |
| 2026-02-24 | Auditoria automática | Hotfix Sidebar Royal (label "Gestão Unidades" para org 9) | hotfix/sidebar-royal | 5d6be844 | Sidebar: menuLabelPorOrg por orgId; orgId do auth; arquitetura e DS conformes. Doc: CODE_REVIEW_HOTFIX_SIDEBAR_ROYAL.md. |
| 2026-02-26 | Auditoria automática | Fix Simulador — botão Detalhes (habilitar após Gerar Cálculo) | fix/simulador-detalhes | — | useSimulator: lastValidCalculationContextRef, getCalculationContext, calculationSuccessVersion; botão Detalhes disabled conforme contexto; toast sucesso; DS e arquitetura conformes. Doc: CODE_REVIEW_FIX_SIMULADOR_DETALHES.md. |
| 2026-03-11 | Auditoria automática | Feature Nova experiência candidatura vaga pública (`PublicVagaDetalhePage`, step 2/3) | feat/nova-experiencia-candidatura | (correções) | Conformidade: hook sem @data/api (apenas Use Cases); APIs com httpClient; Spinner em loadings; Dialog com DialogTitle e DialogDescription; bloco de erro PDF com tokens destructive. Correções aplicadas: callout de competências (bg-amber-* → bg-warning/10 border-warning/30 text-warning); ícone sucesso e-mail (text-green-500 → text-success). |

---

*Referência principal para critérios: **`public/design-toolkit.md`**. Última atualização do documento: Fev 2026.*
