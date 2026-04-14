# Code Review – Template de Contratação do Candidato

**Escopo:** Implementação da tela "Template de Contratação" acessada pelo ícone na tela de Gestão de Vagas / Candidatos.  
**Referências:** ARCHITECTURE.md, design-toolkit.md (public/design-toolkit.md).  
**Observação:** Não foi localizado IMPLEMENTATION.md no repositório; a análise foi feita com base no código implementado e nos padrões definidos na arquitetura e no design toolkit.

---

## 1. Arquivos envolvidos

| Arquivo | Papel |
|--------|--------|
| `src/presentation/pages/TemplateContratacaoCandidato.tsx` | Página do template (formulário, carregamento, estado) |
| `src/data/api/CandidaturaApi.ts` | Novo método `obterCandidaturaPorId` e tipos `ObterCandidaturaPorIdRetorno/Response` |
| `src/data/api/VagaApi.ts` | Novo método `obterTemplatePorCandidatura` e tipos `TemplateContratacaoRetorno`, `TemplateContratacaoEndereco`, `TemplateContratacaoSaude`, etc. |
| `src/app/routes/AppRoutes.tsx` | Nova rota `/gestaodevagas/template-contratacao/:idCandidatura` e import de `TemplateContratacaoCandidato` |
| `src/presentation/pages/GestaoVagasCandidatos.tsx` | Link do ícone "Template de contratação" para a nova rota + `state: { vagaId }` |

Nenhum arquivo novo em `domain/` (entities, repositories, usecases) ou `data/repositories/` foi criado para esta feature.

---

## 2. Verificação de sucesso da implementação

- **Rota e navegação:** A rota existe e o ícone no card do candidato navega para `/gestaodevagas/template-contratacao/{idCandidatura}` com `state: { vagaId }`.
- **APIs:** As chamadas usam `httpClient` (sem `fetch()` direto). Endpoints: `GET api/Vaga/Contratacao/ObterTemplatePorCandidatura/{id}` e `GET api/Candidatura/ObterCandidaturaPorId?idCandidatura=...`.
- **Dados na tela:** O template e os detalhes da candidatura são carregados em paralelo e preenchem os blocos (informações da vaga, formulário, saúde, acessórios, etc.).
- **Modo criação vs edição:** Definido corretamente pelo `id` do template (`00000000-0000-0000-0000-000000000000` = criação).
- **Voltar:** O link "Voltar" usa `vagaId` do `location.state` quando existir, preservando o contexto da vaga.

Conclusão: a implementação atende ao comportamento esperado da feature no que foi entregue.

---

## 3. Inconsistências com ARCHITECTURE.md e design-toolkit.md

### 3.1 Página chamando a camada de API diretamente (violação de fluxo)

**Regra (ARCHITECTURE.md e design-toolkit):**  
O fluxo de dados deve ser: **Page → Store → UseCase → Repository (interface) → Repository (impl) → API → httpClient**.  
Páginas **não** devem fazer chamadas diretas a APIs; devem usar Use Cases (via Redux ou hooks que resolvem Use Cases).

**O que está implementado:**  
Em `TemplateContratacaoCandidato.tsx`, a página resolve as APIs no container e chama os métodos diretamente:

```ts
const vagaApi = container.resolve(DiTokens.vagaApi) as VagaApi;
const candidaturaApi = container.resolve(DiTokens.candidaturaApi) as CandidaturaApi;
// ...
vagaApi.obterTemplatePorCandidatura(token, idCandidatura),
candidaturaApi.obterCandidaturaPorId(token, idCandidatura),
```

**Decisão:** Isso é **inconsistente** com a arquitetura. No restante do projeto (ex.: `HistoricoCandidatura`, `useGestaoVagasCandidatos`, modais de gestão de vagas), a camada de apresentação resolve **Use Cases**, que por sua vez dependem de repositórios; as páginas não resolvem `VagaApi` nem `CandidaturaApi` diretamente.

---

### 3.2 Lógica de negócio e estado na página

**Regra (design-toolkit, seção Pages):**  
Páginas devem **apenas orquestrar UI** (chamar componentes e hooks). **Não** devem conter: lógica de negócio, chamadas diretas a APIs, cálculos complexos.

**O que está implementado:**  
A página concentra:

- Carregamento (useEffect com chamadas às APIs)
- Estado do formulário (template, candidatura, loading, saving, formErrors)
- Regras de atualização (updateTemplate, updateEndereco, updateSaude)
- Validação parcial em `handleSalvarValidar` (ex.: dataInício obrigatória)

**Decisão:** Parte disso é aceitável como “lógica de apresentação” (estado de formulário, handlers), mas **carregamento e validação** ficariam mais alinhados à arquitetura se estivessem em um **hook** (ex.: `useTemplateContratacaoCandidato`) ou em um Use Case + repositório. A chamada direta à API na página é o ponto mais forte de desvio.

---

### 3.3 Uso de httpClient e camada Data

**Regra:** Todas as requisições HTTP devem usar `httpClient` em `@data/api`; não usar `fetch()` direto.

**O que está implementado:**  
`CandidaturaApi` e `VagaApi` usam `httpClient.get` nos novos métodos.

**Decisão:** **Conforme.** Nenhuma inconsistência aqui.

---

### 3.4 Design system e componentes

**Regra (design-toolkit):** Usar componentes base do DS, tokens, labels em inputs, acessibilidade.

**O que está implementado:**  
Uso de Card, Button, Input, Label, Checkbox, Textarea, Select do `@/components/ui`, PageBreadcrumb, PageHeader. Erro de validação exibido com `text-destructive`. Botão "Salvar e Validar" no rodapé fixo.

**Decisão:** **Conforme.** Não foram identificadas cores hardcoded críticas nem ausência de labels nos campos.

---

### 3.5 Rastreamento de página (Firebase)

**Regra (ARCHITECTURE.md):** Novas páginas devem ter título no mapeamento de `usePageTracking` para analytics.

**O que está implementado:**  
A rota `/gestaodevagas/template-contratacao/:idCandidatura` não está em `PAGE_TITLES` em `usePageTracking.ts`. O `getPageTitle` cai no fallback e usa a última parte do path ("template-contratacao" capitalizada).

**Decisão:** **Pequena inconsistência.** O page_view é registrado, mas com título genérico. Incluir um caso específico (ex.: `pathname.startsWith('/gestaodevagas/template-contratacao')`) melhoraria relatórios de analytics.

---

## 4. Refatoração recomendada para conformidade

Para alinhar a implementação à ARCHITECTURE.md e ao design-toolkit, sugere-se o seguinte refactor (em ordem lógica).

### 4.1 Domain

- **Entidades (opcional mas recomendado):** Criar tipos em `domain/entities/` para o template de contratação e para o retorno de “candidatura por id” (podem espelhar os DTOs da API ou uma versão mais enxuta). Isso mantém o domínio independente do contrato exato da API.
- **Repository (interface):** Em `domain/repositories/`, criar algo como `TemplateContratacaoRepository` com:
  - `obterTemplatePorCandidatura(token, idCandidatura)`
  - `obterCandidaturaPorId(token, idCandidatura)`  
  (e, quando existirem, métodos para salvar/validar em modo criação e edição).
- **Use Case(s):** Em `domain/usecases/`, criar por exemplo:
  - `ObterTemplateContratacaoPorCandidaturaUseCase`: recebe o repository e chama os dois métodos de leitura (template + candidatura), retornando um objeto agregado para a UI.
  - Futuramente: use cases para “Salvar e Validar” em modo criação e edição.

### 4.2 Data

- **Repository (implementação):** Em `data/repositories/`, criar `TemplateContratacaoRepositoryImpl` implementando a interface do domain, injetando `VagaApi` e `CandidaturaApi` e mapeando respostas para as entidades de domínio (se você tiver criado entidades).
- **Container (DI):** Registrar a interface do repository e a implementação; registrar o(s) use case(s) recebendo o repository pela interface.

### 4.3 Presentation

- **Hook:** Criar `useTemplateContratacaoCandidato(idCandidatura)` que:
  - Resolve `ObterTemplateContratacaoPorCandidaturaUseCase` (e futuramente os use cases de salvar).
  - Executa o use case no mount (e ao salvar), gerencia loading/error e retorna `{ template, candidatura, loading, error, updateTemplate, updateEndereco, updateSaude, handleSalvarValidar }` (ou equivalente).
- **Página:** `TemplateContratacaoCandidato` passa a:
  - Chamar apenas `useTemplateContratacaoCandidato(idCandidatura)` e renderizar com os dados e handlers retornados.
  - Não resolver `VagaApi` nem `CandidaturaApi`; não chamar APIs diretamente.

Assim o fluxo fica: **Page → Hook → UseCase → Repository (interface) → Repository (impl) → API → httpClient**, em linha com ARCHITECTURE.md e design-toolkit.

### 4.4 Rastreamento

- Em `src/shared/hooks/usePageTracking.ts`, em `getPageTitle`, adicionar:
  - `if (pathname.startsWith('/gestaodevagas/template-contratacao')) return 'Template de Contratação';`  
  (ou título mais específico, se desejado).

---

## 5. Código redundante ou não utilizado

- **Estado `saving`:** É setado em `handleSalvarValidar` (true e depois false), mas o botão "Salvar e Validar" não usa `disabled={saving}`. Quando o submit for assíncrono de fato, faz sentido usar `saving` para desabilitar o botão e evitar double submit; não é código morto, apenas subutilizado no estado atual.
- **Funções locais `formatDatePtBr` e `toInputDate`:** Existem em `TemplateContratacaoCandidato.tsx` e não são compartilhadas. O projeto já tem `formatDatePtBr` em `@presentation/hooks/gestaoVagas/gestaoVagasUtils.ts` (com assinatura ligeiramente diferente: segundo parâmetro fallback). **Recomendação:** usar o `formatDatePtBr` de `gestaoVagasUtils` na página e manter `toInputDate` local ou movê-lo para um util de datas compartilhado, evitando duas implementações de formatação de data.
- **Imports e tipos:** Nenhum import ou tipo obviamente não utilizado foi identificado nos arquivos alterados.

---

## 6. Conclusão e qualidade técnica

- **Funcionalidade:** A entrega atende ao que foi pedido: tela acessível pelo ícone, carregamento por `idCandidatura`, dois endpoints utilizados, formulário com as seções descritas, modo criação/edição pelo `id` do template, botão "Salvar e Validar" preparado para futura integração.
- **Inconsistências:** A principal é a **violação do fluxo de dados**: a página chama a camada de API diretamente, sem Use Case nem Repository. Há também concentração de lógica de carregamento/validação na página e ausência de título dedicado no rastreamento da nova rota.
- **Manutenibilidade:** O arquivo da página é grande (muitos campos e seções). Extrair seções em subcomponentes (ex.: `InformacoesVaga`, `InformacoesPessoais`, `SaudeCandidato`, etc.) e concentrar a lógica em um hook + use case/repository tornaria o código mais fácil de testar e evoluir (ex.: quando forem adicionados os endpoints de salvar/validar).

**Recomendação final:** Aplicar o refactor da seção 4 para alinhar à arquitetura e ao design-toolkit; em seguida, reutilizar ou centralizar helpers de data (formatDatePtBr / toInputDate) e adicionar o título da rota em `usePageTracking`. Isso melhora a consistência do projeto e a qualidade técnica da entrega sem alterar o comportamento visível para o usuário.
