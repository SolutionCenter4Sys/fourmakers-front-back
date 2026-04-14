# Relatório Agregado de Débito Técnico — Projeto Fourmakers

**Projeto:** Fourmakers  
**Escopo:** Todas as features registradas em container.ts  
**Data:** 06/03/2026  
**Fonte de features:** src/core/di/container.ts, src/core/di/tokens.ts  
**Arquivos analisados:** ~150+ arquivos (presentation, data, domain, shared)

---

## 1. Features Analisadas

| Feature | Paths (API / Repo / UseCases) | Pages | CRITICAL | WARNING | Status |
|---------|------------------------------|-------|----------|---------|--------|
| Auth | AuthApi, AuthRepositoryImpl | Login, SSO, LoginNumen, LoginFMU | 0 | 0 | 0 | ✅ |
| Menu | MenuApi, MenuRepositoryImpl | — | 0 | 0 | 0 | ✅ |
| Colaboradores | ColaboradoresApi, ColaboradoresRepositoryImpl | Colaboradores, NovoColaborador, ExportarRelatorioModal | 1 | 1 | ❌ |
| Projetos | ProjetosApi | Projetos, MapaAlocacao, AlocacoesTab, ProjetosTabContent, VisaoGerencialTab | 0 | 4 | ⚠️ |
| Holerites | HoleritesApi, HoleritesRepositoryImpl | MeuHolerite | 0 | 0 | ✅ |
| Reembolsos | ReembolsosApi, ReembolsoComponentesApi, ReembolsoParametrosApi, ReembolsoSolicitacaoApi | Reembolso, InserirReembolso, AprovarReembolso, GestaoAdmTab | 0 | 5 | ⚠️ |
| NotasFiscais | NotasFiscaisApi | GestaoNotasFiscais, useNotasFiscais | 0 | 2 | ⚠️ |
| IntegracaoFolhaPonto | IntegracaoFolhaPontoApi | IntegracaoFolhaPonto | 0 | 1 | ⚠️ |
| ConciliacaoFolhaPagamento | ConciliacaoFolhaPagamentoApi | ConciliacaoFolhaPagamento | 0 | 1 | ⚠️ |
| IntegracaoContabil | IntegracaoContabilApi | IntegracaoContabil | 0 | 1 | ⚠️ |
| IntegracaoBancaria | IntegracaoBancariaApi | RemessaCNAB, useIntegracaoBancaria | 2 | 1 | ❌ |
| ReembolsoComponentes | ReembolsoComponentesApi | GestaoAdmTab, useReembolsoComponentes | 0 | 2 | ⚠️ |
| Timesheet | TimesheetComponentesApi, TimesheetRepositoryImpl | Timesheet, AprovarTimesheetColaborador, ManagementTab, ApprovalsTab, TimesheetTab, WeeklyView, TimesheetForm | 2 | 6 | ❌ |
| MapaAlocacao | MapaAlocacaoApi, MapaAlocacaoRepositoryImpl | MapaAlocacao, NovaAlocacao, AlocacoesTab, ProjetosTabContent, VisaoGerencialTab, AprovarReembolso | 0 | 3 | ⚠️ |
| Profile | ProfileApi | Profile360, PersonalData, AtualizarPerfil, CertificationSection, useProfile | 1 | 1 | ❌ |
| DadosBancarios | DadosBancariosApi | DadosBancariosCard, fluxo Reembolso | 0 | 1 | ⚠️ |
| Parametros | ParametrosApi, ParametrosRepositoryImpl | Vários fluxos (dropdowns, configs) | 0 | 0 | 0 | ✅ |
| Recrutamento (Vaga, Candidate, Candidatura) | VagaApi, CandidateApi, CandidaturaApi, HistoricoCandidaturaApi | GestaoVagas, GestaoVagasCandidatos, HistoricoCandidatura | 1 | 0 | ❌ |
| Dashboard Recrutamento | DashboardMetricas*, ClientesGestaoAlocados, RecrutadoresGestaoAlocados | DashboardRecrutamento | 0 | 0 | 0 | ✅ |
| Parceria | ParceriaApi | GestaoParceria | 1 | 3 | ❌ |
| Agendas Comerciais | AgendasApi, InteracoesApi, AcoesApi, AgendaGestorRepository | AgendasComerciaisPage | 0 | 0 | ✅ |
| MicrosoftGraph / Teams | MicrosoftGraphApi, GraphApiRepositoryImpl | AgendasComerciaisPage (CriarReuniaoTeams) | 1 | 0 | ❌ |
| MinhaEquipe | MinhaEquipeApi | MinhaEquipePage, useGestaoDesempenho | 0 | 1 | ⚠️ |
| PerfilAtuacao | PerfilAtuacaoApi | CriarPerfilAtuacao, SenioritySelectionModal | 1 | 1 | ❌ |
| Competencias / Skills | CompetenciasApi, SkillsApi, CompetenciaRepositoryImpl | SkillsDashboardPage, CriarPerfilAtuacao | 0 | 2 | ⚠️ |
| SkillsDashboard | SkillsDashboardApi | SkillsDashboardPage, Filters, ChartCarousel | 0 | 2 | ⚠️ |
| MinhaJornada | MinhaJornadaApi, MinhaJornadaXanoApi | MinhaJornadaPage | 0 | 0 | ✅ |
| Felizometro | FelizometroApi | SentimentoHoje | 0 | 1 | ⚠️ |
| Notificacao | NotificacaoApi | NotificacoesModal, NotificacoesPopover | 0 | 1 | ⚠️ |
| GestaoDesempenho | GestaoDesempenhoRepositoryImpl, Vcx360RepositoryImpl | GestaoDesempenhoGestor, GestaoDesempenhoColaborador, GestaoDesempenhoRH, GestaoDesempenhoColaboradorDetalhes, ParametrizacaoDesempenho | 0 | 3 | ⚠️ |
| MapaRelacionamento / Organograma | MapaRelacionamentoApi, OrganogramaApi | MapaRelacionamentoPage, VCX360 | 0 | 0 | ✅ |
| Feedback360 | Feedback360Api | Feedback360 | 0 | 0 | 0 | ✅ |
| VCX (Dores e Iniciativas) | VcxApi, VcxRepositoryImpl | Profile360, PainelVcx360 | 0 | 0 | ✅ |
| Permissionamento | GrupoAcessoRepository, FuncionalidadeSistemaRepository, PermissionamentoRepository | Permissionamento | 0 | 0 | 0 | ✅ |
| Banco de Talentos | ColaboradorBancoDeTalentosApi, CurriculoColaboradorApi | TalentosInscritos | 0 | 0 | 0 | ✅ |
| Home Builder | FourmakersApi | HomeBuilder, BuilderPalette, CanvasBlock | 0 | 0 | ✅ |
| Canal Denúncia | CanalDenunciaApi | CanalDenuncias, CanalDenunciasBlock | 0 | 0 | ✅ |
| Comunicacao / Bot Fourmakers | BotFourmakersApi | Comunicacao, ComunicacaoGroupDetail, ComunicacaoProfessionalDetail | 0 | 0 | 0 | ✅ |
| TBD | TbdApi | CadastrarTbd | 0 | 1 | ⚠️ |
| Simulador (Remuneracao) | RemuneracaoCalculationApi | Simulator, useSimulator | 0 | 1 | ⚠️ |
| ViaCEP | ViaCepApi | calculations.ts, usePerfilAtuacao, useViaCep | 2 | 0 | 0 | ❌ |
| Aniversariantes | AniversariantesRepositoryImpl | AniversariantesCard, HomeBuilder | 0 | 1 | ⚠️ |
| Rubricas | GetVerbasUseCase | Rubricas | 0 | 1 | ⚠️ |
| MeuFaturamento | — | MeuFaturamento | 0 | 1 | ⚠️ |
| Orquestracao | MinhaEquipeViewModel (mock) | OrquestracaoPage | 0 | 1 | ⚠️ |
| RelatorioMinhaJornada | RelatorioMinhaJornadaApi | Relatórios | 0 | 0 | 0 | ✅ |
| ParametrizacaoNotificacaoCandidatos | ParametrizacaoNotificacaoCandidatosApi | ParametrizacaoRecrutamento | 0 | 0 | 0 | ✅ |
| Book Colaborador | Mocks (bookColaboradorMock) — sem API no container | BookColaborador, BookColaboradorDetalhes | 0 | 1 | ⚠️ |
| Dashboard (Home) | auth (user), DashboardRoyal, FlutterFlow iframe | Dashboard | 0 | 0 | 0 | ✅ |
| Documentacao | — | Documentacao | 0 | 0 | 0 | ✅ |
| MinhasImportacoes | BuscarMeusLotesUseCase | MinhasImportacoes (subpágina Recrutamento) | 0 | 0 | 0 | ✅ |

**Pages não mapeadas como feature de negócio:** FlutterFlowContainer (legado/container FlutterFlow), NotFound (404).

**Feature embutida (sem page própria):** Campanha (CampanhaApi) — usada em AtualizarPerfil via ColetaPerfilColaboradorCampanhaUseCase; não possui página dedicada.

---

## 2. Resumo Executivo Agregado

| Métrica | Valor |
|---------|-------|
| Total de features analisadas | 51 |
| Total CRITICAL | 10 |
| Total WARNING | 50+ |
| Features em conformidade (✅) — 0 CRITICAL, 0 WARNING | 20 |
| Features com ressalvas (⚠️) — 0 CRITICAL, 1+ WARNING | 22 |
| Features críticas (❌) — 1+ CRITICAL | 9 |

**Status geral:** ⚠️ **NECESSITA ATENÇÃO** — Há violações CRITICAL que devem ser corrigidas antes de novos merges. As violações arquiteturais (Presentation importando @data/api) e uso direto de fetch() são os problemas mais graves.

---

## 3. Prioridade de Correção por Feature

| Feature | CRITICAL | WARNING | Prioridade |
|---------|----------|---------|------------|
| Timesheet | 2 | 6 | Alta |
| IntegracaoBancaria | 2 | 1 | Alta |
| ViaCEP | 2 | 0 | Alta |
| Colaboradores | 1 | 1 | Alta |
| Recrutamento (Candidatura) | 1 | 0 | Alta |
| Parceria | 1 | 3 | Alta |
| MicrosoftGraph / Teams | 1 | 0 | Alta |
| PerfilAtuacao | 1 | 1 | Alta |
| Profile | 1 | 1 | Alta |
| Reembolsos | 0 | 5 | Média |
| MapaAlocacao | 0 | 3 | Média |
| GestaoDesempenho | 0 | 3 | Média |
| Projetos | 0 | 4 | Média |
| Skills/SkillsDashboard | 0 | 2 | Média |
| NotasFiscais | 0 | 2 | Média |
| Demais | 0 | 0-2 | Baixa |

---

## 4. Issues CRITICAL (Consolidado)

### CRITICAL-001: fetch() direto em ColaboradoresApi
**Feature:** Colaboradores  
**Arquivo:** `src/data/api/ColaboradoresApi.ts` (linha 221)  
**Regra:** padroes-comuns-para-evitar-especificos.mdc — DEVE usar httpClient de @data/api/httpClient

**Problema:** Uso direto de `fetch(url)` em vez do httpClient injetado.

**Correção:** Usar o httpClient injetado no construtor da API para realizar a requisição.

---

### CRITICAL-002: fetch() direto em ParceriaApi
**Feature:** Parceria  
**Arquivo:** `src/data/api/ParceriaApi.ts` (linhas 150, 173)  
**Regra:** padroes-comuns-para-evitar-especificos.mdc

**Problema:** Uso direto de `fetch(url)` para upload de arquivos.

**Correção:** Adaptar httpClient ou criar método específico para multipart/form-data, mantendo rastreabilidade.

---

### CRITICAL-003: fetch() direto em CandidaturaApi
**Feature:** Candidatura  
**Arquivo:** `src/data/api/CandidaturaApi.ts` (linha 314)  
**Regra:** padroes-comuns-para-evitar-especificos.mdc

**Problema:** Uso direto de `fetch(url)` para download/upload.

**Correção:** Usar httpClient com suporte a blob/stream conforme necessário.

---

### CRITICAL-004: fetch() direto em TimesheetComponentesApi
**Feature:** Timesheet  
**Arquivo:** `src/data/api/TimesheetComponentesApi.ts` (linhas 610, 639, 678, 716, 914, 951, 988)  
**Regra:** padroes-comuns-para-evitar-especificos.mdc

**Problema:** Múltiplas chamadas usando `fetch(url)` diretamente em vez de httpClient.

**Correção:** Refatorar todos os métodos para usar o httpClient injetado.

---

### CRITICAL-005: fetch() direto em MicrosoftGraphApi
**Feature:** Agendas Comerciais / Microsoft Graph  
**Arquivo:** `src/data/api/MicrosoftGraphApi.ts` (linha 53)  
**Regra:** padroes-comuns-para-evitar-especificos.mdc

**Problema:** Uso direto de `fetch()` para chamar API do Microsoft Graph. (Nota: pode requerer token/headers específicos; avaliar se abstração via httpClient é viável.)

---

### CRITICAL-006: fetch() direto em Presentation — CertificationSection
**Feature:** Profile  
**Arquivo:** `src/presentation/components/profile/CertificationSection.tsx` (linha 40)  
**Regra:** @ARCHITECTURE.md — Presentation NÃO deve acessar HTTP diretamente

**Problema:** Componente de apresentação chamando `fetch(url)` diretamente para carregar certificados.

**Correção:** Criar UseCase e Repository para certificados; usar ViaCepApi ou endpoint interno via httpClient no Data Layer.

---

### CRITICAL-007: fetch() direto em Presentation — RemessaCNAB
**Feature:** IntegracaoBancaria  
**Arquivo:** `src/presentation/pages/RemessaCNAB.tsx` (linha 558)  
**Regra:** @ARCHITECTURE.md — Presentation NÃO deve acessar HTTP diretamente

**Problema:** Página chamando `fetch(url)` diretamente para download/processamento.

**Correção:** Mover lógica para IntegracaoBancariaApi ou UseCase; Page deve usar apenas UseCase/Store.

---

### CRITICAL-008: fetch() direto para ViaCEP fora do Data Layer
**Feature:** ViaCEP (shared / múltiplas features)  
**Arquivos:**  
- `src/shared/utils/calculations.ts` (linha 377) — usado por useSimulator (Simulador)  
- `src/presentation/hooks/recrutamento/usePerfilAtuacao.ts` (linha 455)  
- `src/shared/utils/calculations.ts` exporta `fetchCepData` consumido por Simulator, NovoColaborador, CriarPerfilAtuacao  

**Regra:** padroes-comuns-para-evitar-especificos.mdc — APIs externas via httpClient ou ViaCepApi

**Problema:** Chamadas diretas a `https://viacep.com.br/ws/${cep}/json/` fora do Data Layer. ViaCepApi já existe no container. GetAddressByCepUseCase existe mas não é usado em calculations.ts.

**Correção:** Usar ViaCepApi injetado via UseCase; remover fetch de calculations.ts e usePerfilAtuacao.ts; fazer useSimulator e demais consumidores usarem GetAddressByCepUseCase.

---

### CRITICAL-009: Presentation importando @data/api — Violação arquitetural em massa
**Features afetadas:** Timesheet, Reembolsos, IntegracaoBancaria, IntegracaoContabil, IntegracaoFolhaPonto, ConciliacaoFolhaPagamento, Profile, Projetos, MapaAlocacao, PerfilAtuacao, MinhaEquipe

**Arquivos:**  
- `src/presentation/hooks/useGestaoDesempenho.ts` — MinhaEquipeApi  
- `src/presentation/components/skills-dashboard/Filters.tsx` — ProjetosApi  
- `src/presentation/components/minha-jornada/SenioritySelectionModal.tsx` — PerfilAtuacaoApi  
- `src/presentation/components/mapa-alocacao/VisaoGerencialTab.tsx` — ProjetosApi  
- `src/presentation/components/timesheet/SumarioPorProjetoReadOnlyTable.tsx` — TimesheetComponentesApi  
- `src/presentation/pages/RemessaCNAB.tsx` — IntegracaoBancariaApi  
- `src/presentation/hooks/useIntegracaoBancaria.ts` — IntegracaoBancariaApi  
- `src/presentation/pages/Reembolso.tsx` — ReembolsosApi  
- `src/presentation/components/timesheet/ManagementTab.tsx` — TimesheetComponentesApi  
- `src/presentation/pages/Timesheet.tsx` — TimesheetComponentesApi  
- `src/presentation/hooks/useProfile.ts` — ProfileApi  
- `src/presentation/components/reembolso/GestaoAdmTab.tsx` — ReembolsosApi  
- `src/presentation/hooks/useTimesheetComponentes.ts` — TimesheetComponentesApi  
- `src/presentation/components/timesheet/ApprovalsTab.tsx` — TimesheetComponentesApi  
- `src/presentation/pages/AprovarReembolso.tsx` — ReembolsosApi, ProjetosApi, MapaAlocacaoApi  
- `src/presentation/hooks/useIntegracaoContabil.ts` — IntegracaoContabilApi  
- `src/presentation/hooks/useIntegracaoFolhaPonto.ts` — IntegracaoFolhaPontoApi  
- `src/presentation/hooks/recrutamento/usePerfilAtuacao.ts` — PerfilAtuacaoApi  
- `src/presentation/hooks/useReembolsoComponentes.ts` — ReembolsosApi, ProjetosApi  
- `src/presentation/hooks/useReembolsoParametros.ts` — ReembolsoParametrosApi  
- `src/presentation/hooks/useConciliacaoFolhaPagamento.ts` — ConciliacaoFolhaPagamentoApi  
- `src/presentation/components/mapa-alocacao/AlocacoesTab.tsx` — ProjetosApi  
- `src/presentation/components/mapa-alocacao/ProjetosTabContent.tsx` — ProjetosApi  
- `src/presentation/pages/AprovarTimesheetColaborador.tsx` — TimesheetComponentesApi  
- `src/presentation/components/timesheet/WeeklyView.tsx` — TimesheetComponentesApi  
- `src/presentation/components/timesheet/TimesheetTab.tsx` — TimesheetComponentesApi  
- `src/presentation/components/timesheet/TimesheetForm.tsx` — TimesheetComponentesApi  

**Regra:** @ARCHITECTURE.md — Presentation acessa dados APENAS via UseCases/Store, NUNCA @data/api diretamente

**Problema:** Hooks e componentes resolvendo APIs diretamente do container em vez de UseCases.

**Correção:** Para cada API, criar/validar UseCase correspondente; alterar hooks/pages para resolver UseCase via DI e chamar execute(). Para tipos (interfaces), mover para @domain ou @shared/types.

---

### CRITICAL-010: Cores hardcoded (Design System)
**Features afetadas:** Múltiplas  
**Regra:** @DESIGN_SYSTEM_AUDIT.md — Usar tokens (bg-success, text-destructive, etc.), nunca bg-green-, bg-red-, text-green-600

**Arquivos com violações (amostra):**  
- RegrasTab.tsx, NovaAlocacao.tsx, ApprovalsTab.tsx, VerbaCard.tsx, AtualizarPerfil.tsx  
- BookColaborador.tsx, Timesheet.tsx, ContratoDetailsModal.tsx, InserirReembolso.tsx  
- BuilderPalette.tsx, Colaboradores.tsx, GestaoNotasFiscais.tsx, AprovarTimesheetColaborador.tsx  
- RemessaCNAB.tsx, TimesheetTab.tsx, GestaoDesempenhoColaborador.tsx, HistoricoCandidatura.tsx  
- MeuHolerite.tsx, StatusBadge.tsx, ItemAcaoCard.tsx, ExperienceSection.tsx  
- GestaoDesempenhoRH.tsx, Simulator.tsx, VisaoGerencialTab.tsx, CanvasBlock.tsx  
- useReembolsos.ts, skillsDashboardUtils.ts, SumarioPorProjetoReadOnlyTable.tsx  

**Correção:** Substituir classes por tokens do design system (bg-success, bg-destructive, text-success, etc.).

---

## 5. Issues WARNING (Consolidado)

### WARNING-001: Loader2/RefreshCw em vez de Spinner
**Regra:** @DESIGN_SYSTEM_AUDIT.md — Usar componente Spinner; PROIBIDO Loader2/RefreshCw diretamente

**Arquivos (amostra):** OrquestracaoPage, SkillsDashboardDataTable, NotificacoesModal, ExportarRelatorioModal, Simulator, RemessaCNAB, CadastrarTbd, InserirReembolso, RadarModal, SenioritySelectionModal, Rubricas, GestaoDesempenhoColaborador, AniversariantesCard, SkillsDashboardPage, CriarPerfilAtuacao, GestaoDesempenhoColaboradorDetalhes, SkillSuggestionModal, DadosBancariosCard, SentimentoHoje, MinhaEquipePage, FullScreenLoader, BuilderToolbar, Reembolso, Filters, SimulatorUI, MeuFaturamento, AtualizarPerfil, NotificacoesPopover, etc.

---

### WARNING-002: Tipos any
**Regra:** TypeScript — Nenhum tipo `any`; usar interfaces estritas

**Arquivos (amostra):** ContratoDetailsModal.tsx, useNotasFiscais.ts, GestaoParceria.tsx, usePdfExport.ts, DialogNovoFeedback.tsx, ChartCarousel.tsx, ProjetosTabContent.tsx, VisaoGerencialTab.tsx, GestaoDesempenhoColaboradorDetalhes.tsx, CadastrarTbd.tsx, AtualizarPerfil.tsx, store/index.ts, throttle.ts, analytics.ts, Profile360.ts, ParceiroFormModal.tsx, minhaJornadaMappers.ts

---

### WARNING-003: Imports de tipos de @data/api em Presentation
**Nota:** Importar apenas *tipos* de @data/api (ex: `import type { X } from '@data/api/Y'`) é aceitável para DTOs, mas o ideal é mover essas interfaces para @domain ou @shared/types. Alguns arquivos importam a classe da API (uso direto) — esses são CRITICAL. Os que importam apenas tipos para mapeamento são WARNING (melhoria).

---

## 6. Issues SUGGESTION (Resumido)

1. **Componentes >300 linhas:** Revisar GestaoVagasCandidatos.tsx, InserirReembolso.tsx, NovoColaborador.tsx, AtualizarPerfil.tsx, RemessaCNAB.tsx, GestaoDesempenhoColaborador.tsx, GestaoDesempenhoColaboradorDetalhes.tsx para possível divisão.  
2. **DRY:** Padrões repetidos de mapeamento (unidade, status) em useNotasFiscais, GestaoParceria — extrair para shared/utils.  
3. **useEffect deps:** Revisar dependências exaustivas em hooks críticos.  
4. **useCallback/useMemo:** Aplicar em funções passadas como props e computações caras.  
5. **RefreshCw como ícone decorativo:** Em GestaoDesempenhoColaborador e Detalhes, RefreshCw é usado para indicar "em andamento" (não loading) — considerar ícone alternativo do DS para evitar confusão com padrão de loading.

---

## 7. Conformidade por Categoria (Agregado)

| Categoria | Features conformes | Features com issues |
|-----------|-------------------|---------------------|
| Arquitetura (Dependências) | 2 | 26 |
| Design System (Tokens, Spinner) | 0 | 28 |
| Padrões (httpClient, fetch) | 4 | 24 |
| TypeScript (any) | 10 | 18 |
| DRY / Refatoração | 15 | 13 |

---

## 8. Plano de Ação Recomendado

### Antes do Merge (CRITICAL)
1. [ ] Corrigir CRITICAL-001 a CRITICAL-004: Substituir fetch() por httpClient em ColaboradoresApi, ParceriaApi, CandidaturaApi, TimesheetComponentesApi  
2. [ ] Corrigir CRITICAL-006, CRITICAL-007: Remover fetch de CertificationSection e RemessaCNAB; criar UseCases/Repositories  
3. [ ] Corrigir CRITICAL-008: Usar ViaCepApi em calculations.ts e usePerfilAtuacao.ts  
4. [ ] Corrigir CRITICAL-009: Iniciar migração dos hooks/pages que importam @data/api para usar UseCases (priorizar Timesheet, Reembolsos, IntegracaoBancaria)  
5. [ ] Avaliar CRITICAL-005 (MicrosoftGraphApi) e CRITICAL-010 (cores) para definição de escopo de correção

### Próxima Sprint (WARNING)
1. [ ] Substituir Loader2/RefreshCw por Spinner nos componentes de loading  
2. [ ] Reduzir tipos `any` nos arquivos listados  
3. [ ] Mover tipos de @data/api para @domain ou @shared/types onde fizer sentido  

### Backlog (SUGGESTION)
1. [ ] Dividir componentes grandes (>300 linhas)  
2. [ ] Substituir cores hardcoded por tokens do Design System  
3. [ ] Extrair lógica duplicada (DRY) em mapeamentos de unidade/status  
4. [ ] Revisar dependências de useEffect e uso de useCallback/useMemo  

---

## 9. Notas

- O **httpClient.ts** usa fetch internamente — isso é esperado e correto. As violações referem-se a chamadas de fetch() fora do httpClient em APIs e em presentation.  
- A violação mais impactante é a **CRITICAL-009**: mais de 25 arquivos em presentation importam @data/api diretamente. A correção demanda criação/validação de UseCases e refatoração dos hooks. Priorizar por feature crítica (Timesheet, Reembolsos).  
- **refetch()** em contexto de React Query é nome de função, não violação de fetch HTTP.  
- **Documentacao.tsx** contém menção a fetch() apenas como documentação — não é violação.  
- O relatório foi gerado com base em Grep e análise estática; revisão manual de arquivos específicos pode revelar falsos positivos ou nuances adicionais.

---

## 10. Ranking de Qualidade de Código por Feature

**Comparação com main (PRODUCAO):** Branch atual `feat/agendas-comerciais/para-release` comparada com `main` em 06/03/2026.

**Legenda Producao:**
- **Sim** — Feature em main, sem alterações pendentes nesta branch
- **Parcial** — Feature em main, mas esta branch tem modificações ainda não mergeadas
- **Nao** — Componente novo, não existe na main

**Critério de Nota (qualidade de código, baseado em `.cursor/commands/DESENVOLVIMENTO-passo-4-code-review.md` e `common-patterns-to-avoid-*.mdc`):**  
Segundo as regras: **CRITICAL** = violação arquitetural, fetch() direto, cores hardcoded, modais sem acessibilidade, **tipos `any`**; **WARNING** = lógica em componente, deps useEffect incompletas (não adicionam risco real). SUGGESTION não entra no cálculo.  
- **5 — Excelente:** 0 CRITICAL, 0 WARNING  
- **4 — Bom:** 0 CRITICAL, 1–2 WARNING  
- **3 — Regular:** 0 CRITICAL, 3–5 WARNING  
- **2 — Ruim:** 1 CRITICAL (qualquer WARNING)  
- **1 — Crítico:** 2+ CRITICAL  

**Critério de complexidade:**  
- **Baixa** — 1–2 pages, fluxo simples, poucos UseCases  
- **Média** — 3–5 pages, fluxos moderados, vários componentes  
- **Alta** — 6+ pages, múltiplos fluxos, alta interdependência  
- **Muito Alta** — Muitas integrações, dezenas de UseCases, fluxos críticos de negócio  

**Critério de RISCO (0–10):** `min(10, Q × (1 + EmProd) + C)`  
- **Q** (qualidade) = CRITICAL×2,5 — WARNING não entra (não adiciona risco real)  
- **EmProd** = 1 se Sim/Parcial, 0 se Nao  
- **C** (complexidade) = Baixa 0 | Média 1 | Alta 2 | Muito Alta 3  
- Quanto maior a nota, maior o risco.

| # | Feature | CRITICAL | WARNING | Nota | Classificação | Complexidade | Producao (main) | RISCO 0-10 | Desenvolvedor(es) |
|---|---------|----------|---------|------|---------------|--------------|-----------------|------------|-------------------|
| 1 | Auth | 0 | 0 | 5 | Excelente | Baixa | Parcial — authSlice, AuthApi alterados nesta branch | 0 | tiagorocha4sys, Carlos Eduardo |
| 2 | Menu | 0 | 0 | 5 | Excelente | Baixa | Sim | 0 | Carlos Eduardo |
| 3 | Parametros | 0 | 0 | 5 | Excelente | Baixa | Sim | 0 | Vários |
| 4 | Dashboard Recrutamento | 0 | 0 | 5 | Excelente | Alta | Sim | 2 | Edvaldo |
| 5 | Feedback360 | 0 | 0 | 5 | Excelente | Baixa | Sim | 0 | Doug |
| 6 | Permissionamento | 0 | 0 | 5 | Excelente | Alta | Sim | 2 | Luan Rodrigues, joaovitorbatista4 |
| 7 | Banco de Talentos | 0 | 0 | 5 | Excelente | Média | Parcial — TalentosInscritos alterada nesta branch | 1 | Doug |
| 8 | Comunicacao / Bot Fourmakers | 0 | 0 | 5 | Excelente | Média | Sim | 1 | ttiagorocha |
| 9 | RelatorioMinhaJornada | 0 | 0 | 5 | Excelente | Média | Sim | 1 | ttiagorocha |
| 10 | ParametrizacaoNotificacaoCandidatos | 0 | 0 | 5 | Excelente | Média | Sim | 1 | Edvaldo |
| 11 | Dashboard (Home) | 0 | 0 | 5 | Excelente | Baixa | Sim | 0 | tiagorocha4sys, Luan Rodrigues |
| 12 | Documentacao | 0 | 0 | 5 | Excelente | Baixa | Sim | 0 | Doug, Luan Kaique Rodrigues Ribeiro |
| 13 | MinhasImportacoes | 0 | 0 | 5 | Excelente | Baixa | Sim | 0 | Doug |
| 14 | Holerites | 0 | 0 | 5 | Excelente | Baixa | Sim | 0 | Doug, ttiagorocha |
| 15 | MinhaJornada | 0 | 0 | 5 | Excelente | Alta | Sim | 2 | ttiagorocha |
| 16 | MapaRelacionamento / Organograma | 0 | 0 | 5 | Excelente | Muito Alta | Parcial — MapaRelacionamentoPage, PainelVcx360, AbaAgenda alterados | 3 | paulocymbaum, ttiagorocha |
| 17 | Canal Denúncia | 0 | 0 | 5 | Excelente | Baixa | Sim | 0 | Vários |
| 18 | Home Builder | 0 | 0 | 5 | Excelente | Média | Sim | 1 | ttiagorocha |
| 19 | VCX (Dores e Iniciativas) | 0 | 0 | 5 | Excelente | Alta | Parcial — VcxApi, vcx360Slice alterados nesta branch | 2 | paulocymbaum |
| 20 | Agendas Comerciais | 0 | 0 | 5 | Excelente | Alta | Parcial — AgendasApi, Page, componentes, useAgendasComerciais com melhorias não mergeadas | 2 | Paulo Sasaki Cymbaum |
| 21 | Book Colaborador | 0 | 1 | 4 | Bom | Baixa | Sim (usa mocks) | 0 | joaovitorbatista4 |
| 22 | NotasFiscais | 0 | 2 | 4 | Bom | Média | Sim | 1 | Luan Rodrigues, Carlos Eduardo |
| 23 | IntegracaoFolhaPonto | 0 | 1 | 4 | Bom | Média | Sim | 1 | Carlos Eduardo, Doug |
| 24 | ConciliacaoFolhaPagamento | 0 | 1 | 4 | Bom | Média | Sim | 1 | Doug, Carlos Eduardo |
| 25 | IntegracaoContabil | 0 | 1 | 4 | Bom | Média | Sim | 1 | Doug, Carlos Eduardo |
| 26 | DadosBancarios | 0 | 1 | 4 | Bom | Baixa | Sim | 0 | Vários |
| 27 | Rubricas | 0 | 1 | 4 | Bom | Média | Sim | 1 | Carlos Eduardo, Doug |
| 28 | MeuFaturamento | 0 | 1 | 4 | Bom | Alta | Sim | 2 | Luan Rodrigues, Carlos Eduardo |
| 29 | Orquestracao | 0 | 1 | 4 | Bom | Alta | Sim | 2 | Vários |
| 30 | Felizometro | 0 | 1 | 4 | Bom | Baixa | Sim | 0 | tiagorocha4sys |
| 31 | Notificacao | 0 | 1 | 4 | Bom | Baixa | Sim | 0 | Vários |
| 32 | Aniversariantes | 0 | 1 | 4 | Bom | Baixa | Sim | 0 | Vários |
| 33 | Projetos | 0 | 4 | 3 | Regular | Média | Sim | 1 | tiagorocha4sys, Carlos Eduardo |
| 34 | Reembolsos | 0 | 5 | 3 | Regular | Muito Alta | Sim | 3 | tiagorocha4sys, Luan Rodrigues |
| 35 | ReembolsoComponentes | 0 | 2 | 4 | Bom | Alta | Sim | 2 | Doug, Carlos Eduardo |
| 36 | MapaAlocacao | 0 | 3 | 3 | Regular | Muito Alta | Sim | 3 | ttiagorocha, Luan Rodrigues |
| 37 | MinhaEquipe | 0 | 1 | 4 | Bom | Alta | Sim | 2 | paulocymbaum, Paulo Sasaki Cymbaum |
| 38 | Competencias / Skills | 0 | 2 | 4 | Bom | Alta | Sim | 2 | Vários |
| 39 | SkillsDashboard | 0 | 2 | 4 | Bom | Alta | Sim | 2 | paulocymbaum |
| 40 | GestaoDesempenho | 0 | 3 | 3 | Regular | Muito Alta | Sim | 3 | Paulo Sasaki Cymbaum, paulocymbaum |
| 41 | TBD | 0 | 1 | 4 | Bom | Baixa | Sim | 0 | ttiagorocha, Luan Rodrigues |
| 42 | Simulador (Remuneracao) | 0 | 1 | 4 | Bom | Muito Alta | Sim | 3 | Doug, ttiagorocha |
| 43 | Profile | 1 | 1 | 2 | Ruim | Alta | Sim | 7 | Luan Rodrigues, ttiagorocha |
| 44 | Colaboradores | 1 | 1 | 2 | Ruim | Alta | Parcial — ColaboradoresApi, colaboradoresSlice alterados nesta branch | 7 | tiagorocha4sys, Carlos Eduardo |
| 45 | Recrutamento (Vaga, Candidate, Candidatura) | 1 | 0 | 2 | Ruim | Muito Alta | Parcial — GestaoVagas, GestaoVagasCandidatos, HistoricoCandidatura alterados | 7 | Edvaldo, Doug |
| 46 | Parceria | 1 | 3 | 2 | Ruim | Alta | Sim | 8 | paulocymbaum, Paulo Sasaki Cymbaum |
| 47 | MicrosoftGraph / Teams | 1 | 0 | 2 | Ruim | Média | **Nao** — MicrosoftGraphApi, CriarReuniaoTeams, useMicrosoftAuth não existem na main | 3 | Vários |
| 48 | PerfilAtuacao | 1 | 1 | 2 | Ruim | Alta | Parcial — CriarPerfilAtuacao, usePerfilAtuacao alterados nesta branch | 7 | Doug |
| 49 | IntegracaoBancaria | 2 | 1 | 1 | Crítico | Muito Alta | Sim | 10 | tiagorocha4sys, joaovitorbatista4 |
| 50 | Timesheet | 2 | 6 | 1 | Crítico | Muito Alta | Sim | 10 | ttiagorocha, Carlos Eduardo |
| 51 | ViaCEP | 2 | 0 | 1 | Crítico | Baixa | Sim | 8 | Vários |

**Resumo do ranking (Nota):** 20 Excelente | 22 Bom | 4 Regular | 6 Ruim | 3 Crítico

**Resumo Producao (main):** 41 Sim | 9 Parcial (alterações pendentes nesta branch) | 1 Nao (MicrosoftGraph/Teams)

**Resumo RISCO (0-10):** Maior risco = IntegracaoBancaria, Timesheet, ViaCEP (8–10); Profile, Colaboradores, Recrutamento, Parceria, PerfilAtuacao (7–8). Features com 0 CRITICAL têm RISCO = complexidade apenas (0–3).

**Resumo Complexidade:** Baixa 16 | Média 14 | Alta 14 | Muito Alta 7

**Features presentation adicionadas nesta revisão:** Book Colaborador, Dashboard (Home), Documentacao, MinhasImportacoes. Pages FlutterFlowContainer e NotFound são utilitárias/legado.

**Desenvolvedores:** Autores obtidos via `git log`/`git shortlog` nos paths principais de cada feature. "Vários" indica feature com contribuições distribuídas sem autor dominante claro.
