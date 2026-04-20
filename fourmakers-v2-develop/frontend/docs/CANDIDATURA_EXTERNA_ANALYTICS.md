# Analytics – Candidatura externa (ContentSquare / Hotjar)

## Conformidade com arquitetura e frontend

- **Camadas:** Script e data-attributes ficam na **presentation** (página e markup). Nenhuma lógica de analytics no domain nem no data.
- **Script:** Carregamento centralizado em `@shared/utils/candidaturaAnalyticsScript.ts` (única fonte da URL, possível `VITE_CANDIDATURA_ANALYTICS_SCRIPT_URL` no build).
- **Observabilidade:** O projeto usa **Firebase Analytics** para eventos e page_view (`logUserAction`, `logPageView`). A rota `/public/vaga` está em `usePageTracking` (PAGE_TITLES) para registro de page_view. O ContentSquare é **complementar** (heatmaps, session replay, cliques). As ações críticas da candidatura externa disparam **logUserAction** com flow `CandidaturaExterna` e feature names em PascalCase (constantes em `@shared/constants/candidaturaExternaAnalytics.ts`), conforme `.cursor/rules/common-patterns-to-avoid-specific.mdc`.
- **Design System:** Apenas atributos `data-*` e componentes existentes; sem cores hardcoded nem loading fora do padrão (Spinner).

## Script

O script ContentSquare é carregado **apenas na página de candidatura** (`/public/vaga`), via `loadCandidaturaAnalyticsScript()` em `PublicVagaDetalhePage`:

- **URL padrão:** `https://t.contentsquare.net/uxa/e93bd8a52019a.js`
- **Alteração:** variável de ambiente `VITE_CANDIDATURA_ANALYTICS_SCRIPT_URL` no build ou editar `CANDIDATURA_ANALYTICS_SCRIPT_URL` em `src/shared/utils/candidaturaAnalyticsScript.ts`.

## Atributos `data-candidatura-*` para heatmaps e funis

Todos os atributos usam o prefixo **`data-candidatura-`** para segmentação em ContentSquare/Hotjar.

### Passos do formulário

| Atributo | Valores | Uso |
|----------|---------|-----|
| `data-candidatura-step` | `"1"`, `"2"`, `"3"`, `"4"` | Passo atual: 1 = Login/Cadastro, 2 = Dados para vaga, 3 = Atualize competências, 4 = Resultado (sucesso/ja-candidato) |
| `data-candidatura-section` | `"dados-vaga"`, `"competencias"` | Dentro do passo 2: bloco de dados (CPF, CEP, etc.) ou bloco de competências |
| `data-candidatura-resultado` | `"sucesso"`, `"inscricao-ok"`, `"ja-candidato"` | Tela de fim (step 4): sucesso pós-inscrição, inscrição ok, ou já tinha candidatura |

### Indicador de etapas

| Atributo | Uso |
|----------|-----|
| `data-candidatura-step-indicator` | Container do indicador visual (bolinhas 1, 2, 3) |
| `data-candidatura-current-step` | Número do passo atual (1, 2 ou 3) |
| `data-candidatura-step-num` | Número de cada bolinha (1, 2, 3) |

### Campos do formulário

| Atributo | Uso |
|----------|-----|
| `data-candidatura-field` | Identificador do campo (sem valor sensível). Valores: `email`, `cpf`, `nomeCompleto`, `senha`, `confirmaSenha`, `aceita-termos`, `cep`, `cidade`, `estado`, `pretensao-salarial`, `modelo-trabalho`, `preferencia-contato`, `skill-incluir-perfil` |

Assim dá para ver em heatmaps e funis **quais campos são preenchidos ou abandonados** (ex.: filtro por “elementos com data-candidatura-field”).

### Ações (botões / cliques)

| Atributo | Valores | Uso |
|----------|---------|-----|
| `data-candidatura-action` | `entrar`, `cadastrar`, `cadastre-se`, `voltar`, `realizar-inscricao`, `incluir-skill`, `alterar-nivel-skill`, `entrar-plataforma`, `acessar-plataforma`, `abrir-formulario-mobile`, `fechar-formulario-mobile` | Ação do botão/link para análise de cliques e funil |

### Competências (passo 3)

| Atributo | Uso |
|----------|-----|
| `data-candidatura-skill-card` | Card de cada habilidade |
| `data-candidatura-skill-id` | ID da skill no catálogo (numérico), para saber **quais habilidades** foram marcadas/alteradas (sem PII) |

## Firebase (logUserAction) – eventos já implementados

| featureName | Quando |
|-------------|--------|
| `SolicitarCodigoLogin` | Usuário solicitou código de login por e-mail (tipoAcesso 0). |
| `Entrar` | Login concluído com sucesso (OTP validado). |
| `Cadastrar` | Cadastro de novo usuário concluído com sucesso. |
| `VisualizarStepDadosVaga` | Usuário visualizou o passo "Dados para vaga" (formulário de inscrição), uma vez por sessão. |
| `RealizarInscricao` | Inscrição na vaga enviada com sucesso (`additionalData.codigoVaga`). |
| `RealizarInscricaoErro` | Erro ao realizar inscrição (`additionalData.codigoVaga`, `additionalData.erro`). |
| `IncluirSkillPerfil` | Skill incluída no perfil 360 (`additionalData.skillId`). |
| `RemoverSkillPerfil` | Skill removida do perfil 360 (`additionalData.skillId`). |
| `AtualizarNivelSkillPerfil` | Nível da skill alterado no perfil (`additionalData.skillId`, `additionalData.nivelId`). |

Constantes: `CANDIDATURA_EXTERNA_FLOW`, `CANDIDATURA_EXTERNA_ACTIONS` em `src/shared/constants/candidaturaExternaAnalytics.ts`.

---

## Como usar no ContentSquare / Hotjar

1. **Heatmaps:** filtrar por página `/public/vaga` e, se possível, por elementos que tenham `data-candidatura-step`, `data-candidatura-field` ou `data-candidatura-action` para ver onde há mais cliques e scroll.
2. **Funis:** criar funil por passos usando `data-candidatura-step="1"` → `"2"` → `"3"` → `"4"` (e opcionalmente por `data-candidatura-resultado`).
3. **Até onde o candidato foi:** eventos/cliques em `data-candidatura-action="realizar-inscricao"` vs. abandono antes; uso de `data-candidatura-field` para ver último campo tocado.
4. **Skills:** segmentar sessões ou cliques por `data-candidatura-skill-id` para analisar quais competências são mais incluídas no perfil.

## Página identificada

O container principal do formulário (coluna direita) tem **`data-candidatura-page="true"`**, para facilitar segmentação de “página de candidatura” em qualquer ferramenta.
