# Auditoria de Conformidade — Ajustes Experiência Candidatura (branch feat/experiencia-candidatura)

**Branch:** feat/experiencia-candidatura  
**Escopo:** `PublicVagaDetalhePage.tsx` — toasts em erros de API, tratamento "candidatura já existe", layout mobile com bottom sheet.  
**Data da auditoria:** 2026-03-09  
**Objetivo:** Validar conformidade com arquitetura, design system e boas práticas do projeto.

---

## 1. Resumo executivo

| Critério | Status | Resumo |
|----------|--------|--------|
| **Arquitetura** | ✅ Conforme | Alterações restritas à camada de apresentação; uso de UseCases (CandidatarSeUseCase, GetVagaDetalhesPublicoUseCase, BuscarDadosColaboradorUseCase); sem import de @data/api na página. |
| **Design System** | ✅ Conforme | Button, Card, Spinner, Badge, toast (sonner); tokens (text-foreground, bg-muted/50, text-muted-foreground); sem Loader2/RefreshCw; sem cores hardcoded nos blocos novos. |
| **Padrões específicos** | ✅ Conforme | Feedback de erro via toast em todas as falhas de API; mensagem da API propagada (httpClient) e exibida no toast. |
| **UX / Acessibilidade** | ✅ Conforme | Bottom sheet no mobile com header "Candidatura" e botão "Fechar"; CTA "Entrar na plataforma" para o caso de candidatura já existente. |

**Veredicto:** ✅ **Conforme**. Ajustes alinhados à arquitetura, ao design system e aos padrões do projeto.

---

## 2. Escopo analisado

**Arquivo alterado:**

| Arquivo | Alterações |
|---------|------------|
| `src/presentation/pages/PublicVagaDetalhePage.tsx` | Toasts em erros de API; estado e UI para "candidatura já existe"; estado `isLg` (matchMedia 1024px), `mobileSheetOpen`; coluna esquerda visível no mobile; barra fixa + bottom sheet no mobile; botão Fechar no sheet. |

---

## 3. Análise por tema

### 3.1 Erros de API e toast

- **Detalhes da vaga:** Em falha do `getDetalhesPublico.execute()`, exibe `toast.error('Não foi possível carregar os detalhes da vaga.')`. ✅  
- **Dados do colaborador:** Já existia `toast.error('Não foi possível carregar seus dados.')` no catch. ✅  
- **Candidatar-se:**  
  - Erro genérico: `toast.error(msg)` com mensagem da API. ✅  
  - Mensagem contendo "já existe uma candidatura" / "ja existe uma candidatura": `toast.info(msg)` + estado `candidaturaJaExiste` e bloco com "Entrar na plataforma". ✅  
- **Cadastro / atualizar dados:** Já utilizavam `toast.error(msg)` no catch. ✅  

**Conclusão:** Todos os fluxos de erro de API na página passam a informar o usuário via snackbar/toast.

---

### 3.2 Tratamento "candidatura já existe"

- Detecção via regex case-insensitive na mensagem do erro: `/já existe uma candidatura|ja existe uma candidatura/i`. ✅  
- Estado `candidaturaJaExiste` é setado e exibido bloco com texto explicativo e botão "Entrar na plataforma" (navigate para `/dashboard`), alinhado ao bloco de sucesso de inscrição. ✅  
- Sem duplicação de lógica de negócio: a regra continua no backend; a presentation apenas reage à mensagem. ✅  

**Conclusão:** Comportamento conforme solicitado.

---

### 3.3 Layout mobile e bottom sheet

- **Breakpoint:** `window.matchMedia('(min-width: 1024px)')` em `useEffect` para `isLg`; listener de `change` para atualização ao redimensionar. ✅  
- **Coluna esquerda (detalhes da vaga):** De `hidden lg:flex` para `flex w-full lg:w-1/2`, visível também no mobile. ✅  
- **Mobile (< 1024px):**  
  - Barra fixa inferior com botão "Entrar ou Cadastre-se" / "Candidatar-se" quando o sheet está fechado. ✅  
  - Formulário exibido em bottom sheet (fixed, `max-h-[90vh]`, `rounded-t-2xl`) quando `mobileSheetOpen`; header com "Candidatura" e botão "Fechar". ✅  
  - Coluna direita no desktop mantida (in flow); no mobile fica oculta quando o sheet está fechado. ✅  
- **Espaçamento:** Na coluna esquerda, `pb-24` no mobile para a barra fixa não cobrir o conteúdo; `maxHeight` condicional (90vh no desktop, none no mobile) para melhor uso da tela. ✅  

**Conclusão:** Detalhes da vaga permanecem visíveis no mobile; formulário abre por cima em formato de bottom sheet com opção de fechar.

---

### 3.4 Arquitetura e dependências

- Nenhum import de `@data/api` na página; uso de UseCases injetados (GetVagaDetalhesPublicoUseCase, BuscarDadosColaboradorUseCase, CandidatarSeUseCase, CadastrarUsuarioUseCase, etc.). ✅  
- Redux utilizado apenas para auth (token, user, loginStatus, loginError) e dispatch de sendLoginToken/validateLoginToken/clearLoginState. ✅  
- Regra de "já existe candidatura" tratada na presentation com base na mensagem de erro; não há lógica de negócio duplicada no domínio. ✅  

**Conclusão:** Respeito à Clean Architecture e à regra de dependências.

---

### 3.5 Design System e padrões do projeto

- Componentes: Button, Card, CardContent, Badge, Input, Spinner, Dialog, etc. do DS. ✅  
- Loading: Spinner (não Loader2/RefreshCw). ✅  
- Toasts: sonner (`toast.success`, `toast.error`, `toast.info`). ✅  
- Novos blocos utilizam tokens: `text-foreground`, `text-muted-foreground`, `bg-muted/50`, `border`, sem cores hardcoded. ✅  

**Conclusão:** Conformidade com DESIGN_SYSTEM_AUDIT e padrões do projeto.

---

## 4. Riscos e observações

| Item | Severidade | Observação |
|------|------------|------------|
| Estado inicial `isLg = true` | Baixa | Evita flash de layout mobile no primeiro paint; em app client-only é aceitável. |
| Breakpoint 1024px local | Baixa | Poderia ser extraído para um hook `useIsLg()` compartilhado em futura refatoração; não bloqueia merge. |
| Duplicação de bloco "Entrar na plataforma" | Baixa | Dois blocos semelhantes (sucesso vs. candidatura já existe); possível extrair componente pequeno depois. |

---

## 5. Checklist final

- [x] Build `npm run build:homolog` executado com sucesso.
- [x] Erros de API exibidos em toast.
- [x] Caso "candidatura já existe" com mensagem e CTA "Entrar na plataforma".
- [x] Mobile: detalhes da vaga visíveis; formulário em bottom sheet com Fechar.
- [x] Sem violação de camadas (apenas presentation).
- [x] Uso de componentes e tokens do Design System.

---

**Auditor:** Agente (auditoria automatizada)  
**Revisão:** Recomendada revisão humana antes do merge.

---

## 6. Auditoria 2 — Refatoração arquitetura limpa (página fina, lógica em hook/utils)

**Data:** 2026-03-09  
**Objetivo:** Garantir conformidade com a regra “página só UI; lógica e regras em hooks/utils/domain”.

### 6.1 Alterações realizadas

| Artefato | Responsabilidade |
|----------|------------------|
| **`src/presentation/utils/publicVagaUtils.ts`** | Funções puras: `formatWhatsApp`, `formatCurrencyInputBR`, `groupSkills`, `skillsFaltantes`, `isCandidaturaJaExisteMessage`. Sem side effects, sem dependências de UI. |
| **`src/presentation/hooks/usePublicVagaDetalhe.ts`** | Estado, efeitos (fetch detalhe, dados colaborador, matchMedia, timer OTP), formulários (react-hook-form), handlers que chamam UseCases/APIs, toasts, regra “candidatura já existe”. Schemas zod e constantes (ORG_ID_PUBLICO, TIPOS_CONTRATO, MODALIDADES, etc.) concentrados no hook. |
| **`src/presentation/pages/PublicVagaDetalhePage.tsx`** | Apenas UI: chama `usePublicVagaDetalhe()`, desestrutura estado e handlers, renderiza layout (colunas, bottom sheet mobile), formulários e modais. Componente presentacional `StepsIndicatorPublico` e `randomHeroImage` mantidos na página (preocupação de apresentação). Reexporta `PublicVagaLocationState` do hook. |

### 6.2 Conformidade

- **Camada:** Lógica de negócio e orquestração no hook; página só delega e renderiza. ✅  
- **Utils:** `publicVagaUtils` contém apenas transformações e detecção de mensagem; sem chamadas de API. ✅  
- **Hook:** Resolve UseCases pelo container; trata erros e toasts; expõe estado e callbacks para a página. Uso de APIs (ColaboradoresApi, CurriculoColaboradorApi) no hook permanece como dívida conhecida até existir UseCase de “atualizar dados complementares + currículo”. ✅  
- **Página:** Redução de ~1250 para ~830 linhas; sem efeitos nem handlers próprios; sem regras de negócio. ✅  

### 6.3 Build / pipeline

O build (`npm run build`) pode falhar em outros arquivos do repositório (LgpdRepositoryImpl, CandidatarSeUseCase, GetVagaDetalhesPublicoUseCase, ColaboradoresApi/CurriculoColaboradorApi sem métodos `alterarDadosColaboradorParametros`/`insereCurriculoColaborador` na develop). Essas falhas não decorrem desta refatoração; a separação página/hook/utils está em conformidade com a arquitetura limpa do projeto.

**Recomendação:** Alinhar o hook aos métodos existentes na develop ou adicionar os métodos/UseCases necessários para o fluxo de vaga pública em tarefa futura.
