# Auditoria: branch fix/simulador-previsao-ano

**Branch:** `fix/simulador-previsao-ano`  
**Commit inicial auditado:** `325e743a` — fix(simulador): previsão 13 meses, seletores no corpo, vagas com candidatos, email alternativo, rodapé e hover  
**Data da auditoria:** 2026-03-12  

---

## 1. Escopo

- **Arquivos alterados / adicionados na branch:**
  - `src/data/api/CandidatoListApi.ts` — tipo com `emailAlternativo`
  - `src/data/api/VagaListApi.ts` — tipo com `quantidadeCandidatosPorEstagio`
  - `src/data/repositories/CandidatoListRepositoryImpl.ts` — mapeamento `emailAlternativo`
  - `src/data/repositories/VagaListRepositoryImpl.ts` — mapeamento `quantidadeCandidatosPorEstagio`
  - `src/domain/entities/CandidatoListItem.ts` — campo `emailAlternativo`
  - `src/domain/entities/RemuneracaoCalculation.ts`
  - `src/domain/entities/VagaListItem.ts` — campo `quantidadeCandidatosPorEstagio`
  - `src/presentation/components/common/PageHeader.tsx` — `titlePrefix` para botão Voltar
  - `src/presentation/components/simulator/SimulatorIdentifiersModal.tsx` — getEmailExibicao, Spinner, hover ghost
  - `src/presentation/components/simulator/SimulatorVagaCandidatoSelectors.tsx` — **novo** (seletores no corpo)
  - `src/presentation/components/simulator/index.ts` — export do novo componente
  - `src/presentation/hooks/useSimulator.ts` — remoção estado modal identificadores
  - `src/presentation/pages/GestaoVagasCandidatos.tsx` — state `from: 'kanban-candidatos'` no link simulador
  - `src/presentation/pages/Simulator.tsx` — rodapé, seletores no corpo, filtro vagas, texto rodapé, cor destrutiva
  - `src/shared/utils/calculations.ts` — parsing Data de Início (timezone dia 01)

- **Contexto:** Ajustes no simulador de remuneração: previsão 13 meses a partir da Data de Início, seleção de vaga/candidato no corpo da tela, filtro de vagas com candidatos, exibição de email (prioridade emailUsuario → emailAlternativo), rodapé e hover dos selects em conformidade com o Design System.

---

## 2. Conformidade por critério

### 2.1 Arquitetura (Clean Architecture)

| Critério | Status | Observação |
|----------|--------|------------|
| Presentation não importa `@data/api` | ✅ | Página Simulator e componentes usam apenas hooks (useSimulator), Redux e componentes; dados vêm do slice que chama UseCases. |
| Fluxo de dados Page → Store → UseCase → Repository → API | ✅ | Listagem de vagas/candidatos via `fetchVagasList` (ListVagasUseCase) e carregamento de candidatura via useSimulator; APIs em `@data/api` (VagaListApi, CandidatoListApi) usadas apenas por repositórios. |
| Domain sem dependência de Data/App | ✅ | Entidades (VagaListItem, CandidatoListItem, RemuneracaoCalculation) e repositórios em domain são interfaces; implementações em data. |
| httpClient na camada Data | ✅ | VagaListApi e CandidatoListApi usam `httpClient` (post/get); nenhum `fetch()` direto nos arquivos alterados. |

**Conclusão:** Conformidade com a regra de dependência e fluxo da ARCHITECTURE.md.

### 2.2 Design System e padrões de UI

| Critério | Status | Observação |
|----------|--------|------------|
| Loading com `Spinner` (não Loader2/RefreshCw) | ✅ | SimulatorVagaCandidatoSelectors usa `Spinner`; SimulatorIdentifiersModal corrigido de Loader2 para Spinner. RefreshCw em Simulator.tsx é ícone do botão "Limpar", não indicador de loading. |
| Cores semânticas (tokens) | ✅ | Rodapé usa `text-primary`, `text-info`, `text-success`, `text-destructive`; card de aviso "Gastos Variáveis" corrigido de `text-red-700 dark:text-red-300` para `text-destructive`. Selects: hover com `bg-btnGhostHover`/`dark:bg-white/5`, checked com `bg-primary`/`text-primary-foreground`. |
| Modais com `DialogTitle` e `DialogDescription` | ✅ | SimulatorIdentifiersModal possui DialogTitle e DialogDescription. |
| Componentes do DS (Button, Card, Input, Select, Label, etc.) | ✅ | Uso consistente de componentes em `@/components/ui`. |

**Conclusão:** Conformidade com DESIGN_SYSTEM_AUDIT.md e padrões do projeto.

### 2.3 Código morto e boas práticas

| Critério | Status |
|----------|--------|
| Estado não utilizado removido (showIdentifiersModal) | ✅ |
| Lógica de exibição de e-mail centralizada (getEmailExibicao) | ✅ |
| Filtro de vagas com candidatos (quantidadeCandidatosPorEstagio) no domínio/dados | ✅ |

### 2.4 Pontos corrigidos durante a auditoria

- **Simulator.tsx:** `text-red-700 dark:text-red-300` → `text-destructive` no card de Gastos Variáveis (visão colaborador).
- **SimulatorIdentifiersModal.tsx:** `Loader2` → `Spinner` nos estados de loading; hover dos SelectItems: `bg-violet-600` → `bg-btnGhostHover` / `dark:bg-white/5` e checked com `bg-primary`/`text-primary-foreground` para alinhar ao SimulatorVagaCandidatoSelectors e ao botão ghost do DS.

---

## 3. Observações para outros arquivos do simulador (fora do escopo do commit)

- **SimulatorUI.tsx:** Utiliza `Loader2` para loading (linha ~182) e cores hardcoded `text-green-600`, `text-blue-600` em seções de Proventos/Benefícios (linhas ~494–526). Recomenda-se, em alteração futura, trocar para `Spinner` e tokens `text-success` e `text-info` para total conformidade com o Design System.

---

## 4. Resumo executivo

- **Arquitetura:** em conformidade (Presentation não acessa Data; UseCases/Repositories/APIs e httpClient utilizados corretamente).
- **Design System:** em conformidade após correções (Spinner, tokens de cor, hover/checked dos selects no estilo ghost/primary).
- **Branch:** adequada para merge após commit de auditoria com as correções aplicadas.

**Ação realizada:** Correções de conformidade aplicadas (Simulator.tsx e SimulatorIdentifiersModal.tsx) e documentadas neste relatório.
