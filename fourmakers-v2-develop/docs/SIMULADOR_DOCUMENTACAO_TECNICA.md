# Simulador – Documentação Técnica (Regras de Negócio e Backend)

Documentação técnica da página **Simulador** (`/simulador`) para suporte ao desenvolvimento do backend em .NET 8 e à integração com o frontend React. A feature utiliza APIs existentes de Vaga, Candidatura e Cálculo de Remuneração; este documento consolida regras, contratos e dependências conforme a arquitetura do projeto (ver `ARCHITECTURE.md`).

- **Criado em:** 13/03/2026  
- **Última atualização:** 13/03/2026 (criação conforme ORIENTACAO_DOCUMENTACAO_TECNICA_FEATURES.md)

---

## 1. Visão geral e objetivo

**Objetivos principais (visão de produto):** Permitir que recrutadores **negociem remuneração e benefícios** com candidatos com base em dados da vaga e da candidatura; **simular** o impacto de valores (líquido pretendido, benefícios, ajuda de custo, km) e **visualizar** conformidade com a política; **registrar** dados demográficos e de pretensão na candidatura para auditoria e acompanhamento.

| Item | Descrição |
|------|------------|
| **Rota** | `/simulador` |
| **Título** | Formulário para Negociação de Remuneração e Benefícios |
| **Descrição (UI)** | Conduza conversas com candidatos com mais segurança, registre dados relevantes e gere insumos para futuras negociações. |
| **Objetivo de negócio** | Apoiar o recrutador na **simulação de remuneração** (CLT, benefícios, ajuda de custo) a partir da vaga e do candidato selecionados; **salvar** dados da negociação na candidatura; **exibir** resultado do cálculo (proposta vs pretendido) e indicador de conformidade com a política. |
| **Escopo atual** | Seleção de vaga/candidato/candidatura; formulário recrutador (remuneração, benefícios, dias presenciais, modelo de trabalho); Gerar Cálculo (POST SimularRemuneracaoTotal); Salvar Dados (POST EditarCandidaturaColaboradorComDadosDemograficos); previsão 13 meses; botão Voltar quando origem é kanban de candidatos; tag conformidade e tooltips. |

**Personas:** Recrutador (principal) — seleciona vaga e candidato, preenche valores, gera cálculo e salva dados na candidatura. Acesso pode ser direto pela rota ou a partir do kanban de candidatos (com botão Voltar).

---

## 2. Parâmetros de entrada e contexto

- **Autenticação:** Todas as chamadas usam o **token** do usuário logado (Bearer), obtido do estado de autenticação (Redux).
- **Parâmetros de URL (opcionais):** `idVaga`, `idCandidatura`, `codigoInternoColaborador` — quando presentes, a tela pré-seleciona vaga, candidatura e/ou candidato.
- **Dependências de APIs existentes:**
  - **Listar vagas:** `ListarVagasRecrutamento` ou equivalente (lista de vagas para o combobox).
  - **Listar candidatos (por vaga):** `ListarCandidatosInscritos` (candidatos da vaga selecionada).
  - **Detalhes da vaga:** `GET /api/Vaga/ObterVagaRecrutamentoPorId/{vagaId}` — fornece `custoProfissional` (custo hora vaga), modelo de trabalho, etc.
  - **Detalhes do candidato/candidatura:** `GET /api/Candidatura/ObterDadosParaEditarCandidaturaColaboradorComDadosDemograficos` — preenche formulário (pretensão, benefícios, pessoas com quem mora, CEP, etc.).
  - **Simular remuneração:** `POST /api/Vaga/Calculos/SimularRemuneracaoTotal`.
  - **Salvar dados da candidatura:** `POST /api/Candidatura/EditarCandidaturaColaboradorComDadosDemograficos`.
- **Gestores:** API de gestores (GetManagersUseCase) para preenchimento de gestor na candidatura quando aplicável.

---

## 2.1 Padrões de contratos do projeto (consistência)

Conforme `ORIENTACAO_DOCUMENTACAO_TECNICA_FEATURES.md` e padrão do projeto:

| Aspecto | Padrão | Exemplo no simulador |
|---------|--------|----------------------|
| **Nomenclatura JSON** | camelCase | `idVaga`, `liquidoPretendido`, `codigoInternoColaborador`, `emConformidadeComAPolitica` |
| **Envelope de resposta** | `retorno`, `sucesso`, `mensagem`, `erros?` | SimularRemuneracaoTotal, ObterVagaRecrutamentoPorId, EditarCandidaturaColaboradorComDadosDemograficos |
| **Data** | `dataCriacao` / `dataAlteracao` (ISO ou YYYY-MM-DD) | Entidades de candidatura e vaga |
| **Identificador colaborador** | `codigoInternoColaborador` | Payload de candidatura e detalhes |
| **Erros de validação** | `erros?: string[] \| null` no envelope | Respostas de erro das APIs |

---

## 3. Regras de negócio

### 3.1 Custo Hora Vaga e Remuneração Vaga (168h)

- **Fonte:** Exclusivamente da API **ObterVagaRecrutamentoPorId** — campo **`custoProfissional`**.
- **Custo Hora Vaga** = `custoProfissional`.
- **Remuneração Vaga (168h)** = `custoProfissional × 168` (constante `HOURS_PER_MONTH` em `shared/utils/calculations.ts`).
- Esses campos são **somente leitura** no front; não são alterados por Salvar Dados nem por Gerar Cálculo. Detalhe em `docs/SIMULADOR_CUSTO_HORA_E_REMUNERACAO_VAGA.md`.

### 3.2 Limpeza de feedback ao trocar contexto

- Ao trocar **vaga**, **candidato** ou **candidatura** nos seletores, o resultado do último **Gerar Cálculo** e as mensagens de validação (verde/vermelho) são **limpos** até que um novo Gerar Cálculo seja executado.

### 3.3 Conformidade com a política

- O backend pode retornar no retorno do **SimularRemuneracaoTotal** o campo **`emConformidadeComAPolitica`** (boolean). O front usa esse valor quando disponível; caso contrário, deriva conformidade quando o salário líquido da proposta é >= líquido desejado.

### 3.4 Modelos de trabalho e dias presenciais

- Modelos de trabalho vêm da vaga e da configuração do sistema (ex.: remoto, híbrido, presencial). Dias presenciais (ex.: código 2 para híbrido) são enviados no payload de candidatura quando aplicável.
- **Cargo de confiança:** Switch pode ser desabilitado conforme regra da vaga.

### 3.5 Validações ao salvar

- **Pessoas com quem mora:** Não pode ser 0 em certos fluxos; validação no front e recomendável no back.
- **Outros custos:** Descrição e valor validados; lista inválida enviada como `null`.
- **CEP:** Apenas dígitos no POST; máscara na exibição; ViaCEP com debounce para preenchimento de endereço.

### 3.6 Botão Voltar

- Quando a origem do acesso é o **kanban de candidatos** (parâmetro ou estado), exibe-se botão **Voltar** no header para retornar à página anterior (navigate(-1)).

---

## 4. Funcionalidades e experiência do usuário

### 4.1 Ao carregar

1. Usuário acessa `/simulador` (ou abre a partir do kanban).
2. Se houver `idVaga`, `idCandidatura` ou `codigoInternoColaborador` na URL, os seletores são pré-preenchidos e os detalhes da vaga/candidatura carregados.
3. Listas de vagas e candidatos (por vaga) são carregadas via APIs existentes.
4. Custo Hora Vaga e Remuneração Vaga (168h) são preenchidos quando os detalhes da vaga estão disponíveis (`custoProfissional`).

### 4.2 Modo Recrutador / formulário

1. Recrutador seleciona **Vaga**, **Candidato** e **Candidatura** (se houver mais de uma).
2. Formulário é preenchido com dados da candidatura (pretensão salarial, ajuda de custo, benefícios, CEP, pessoas com quem mora, etc.) quando os detalhes são carregados.
3. Campos editáveis: líquido pretendido, benefícios (VR, VA, educação, mobilidade, ajuda de custo, km), dias presenciais, modelo de trabalho (conforme vaga), dados demográficos.
4. Campos somente leitura (além de Custo Hora Vaga e Remuneração 168h): faixa etária (read-only), frequência/dias presenciais quando definidos pela vaga.

### 4.3 Gerar Cálculo

1. Recrutador preenche valores e clica em **Gerar Cálculo**.
2. Front envia **POST /api/Vaga/Calculos/SimularRemuneracaoTotal** com payload (idVaga, liquidoPretendido, quantidadeDependentes, alimentacao, mobilidade, educacao, ajudaDeCusto, km).
3. Resposta preenche **proposta** e **pretendido** (remuneração proposta vs pretendida), validações por campo (dentroDaPolitica, mensagem) e **emConformidadeComAPolitica**.
4. Mensagens de feedback (verde/vermelho) são exibidas nos campos; tag de conformidade e tooltips com detalhes de descontos/info quando aplicável.
5. Botão **Detalhes** é habilitado após o primeiro Gerar Cálculo e invalidado ao mudar vaga/candidato/candidatura.

### 4.4 Salvar Dados

1. Recrutador clica em **Salvar Dados**.
2. Front valida (pessoas com quem mora, outros custos, CEP, etc.) e envia **POST /api/Candidatura/EditarCandidaturaColaboradorComDadosDemograficos** com o payload da candidatura.
3. Sucesso: toast de sucesso; erro: apenas toast (mensagem dinâmica da API), sem erro fixo no rodapé.

### 4.5 Regras de sucesso, erro e bloqueios (UX)

- **Sucesso (Gerar Cálculo):** Resultado exibido (proposta, pretendido, previsão 13 meses); feedback por campo; tag conformidade.
- **Sucesso (Salvar Dados):** Toast de sucesso.
- **Erro:** Toast com mensagem da API (`mensagem` ou `erros`); em erro de rede, mensagem genérica e possibilidade de tentar novamente.
- **Bloqueios:** Botão Detalhes desabilitado até primeiro cálculo; ao trocar contexto, resultado e feedback são limpos.

### 4.6 Linguagem e tom

- Mensagens em tom neutro e profissional (recrutador interno). Evitar jargões; preferir "Preencha o CEP" ou "Valor inválido" a termos técnicos em excesso.

---

## 5. APIs necessárias (backend .NET 8)

Todas as requisições usam **Bearer token** no header. Base URL conforme configuração do projeto.

### 5.1 Obter detalhes da vaga

- **Método e rota:** `GET /api/Vaga/ObterVagaRecrutamentoPorId/{vagaId}`
- **Headers:** `Authorization: Bearer {token}`
- **Resposta:** Envelope com `retorno` contendo dados da vaga, incluindo **`custoProfissional`** (number) usado para Custo Hora Vaga e Remuneração 168h.

### 5.2 Simular remuneração total

- **Método e rota:** `POST /api/Vaga/Calculos/SimularRemuneracaoTotal`
- **Headers:** `Authorization: Bearer {token}`, `Content-Type: application/json`
- **Corpo (JSON):**

```json
{
  "idVaga": "string (guid)",
  "liquidoPretendido": 0,
  "quantidadeDependentes": 0,
  "alimentacao": 0,
  "mobilidade": 0,
  "educacao": 0,
  "ajudaDeCusto": 0,
  "km": 0
}
```

- **Resposta esperada (200):**

```json
{
  "retorno": {
    "remuneracaoProposta": { "clt": {}, "validacaoCLT": {}, "valeRefeicao": 0, "remuneracaoTotalLiquidaMensal": 0, "previsaoAnual": 0, "custoTotalEmpresa": 0, ... },
    "remuneracaoPretendida": { ... },
    "emConformidadeComAPolitica": true
  },
  "sucesso": true,
  "mensagem": null,
  "erros": null
}
```

- Contratos detalhados em `src/domain/entities/RemuneracaoCalculation.ts` (RemuneracaoProposta, RemuneracaoPretendida, RemuneracaoValidation, RemuneracaoCltDetails).

### 5.3 Obter dados da candidatura (para edição/demográficos)

- **Método e rota:** `GET /api/Candidatura/ObterDadosParaEditarCandidaturaColaboradorComDadosDemograficos?idVaga=...&idCandidatura=...`
- **Headers:** `Authorization: Bearer {token}`
- **Resposta:** Envelope com `retorno` contendo dados da candidatura e demográficos (pretensão, benefícios, CEP, pessoas com quem mora, outros custos, etc.).

### 5.4 Salvar dados da candidatura (edição com demográficos)

- **Método e rota:** `POST /api/Candidatura/EditarCandidaturaColaboradorComDadosDemograficos`
- **Headers:** `Authorization: Bearer {token}`, `Content-Type: application/json`
- **Corpo:** Payload de edição (idCandidatura, idVaga, codigoInternoColaborador, modelo de trabalho, pretensão, benefícios, demográficos, etc.) conforme contrato existente no projeto (CandidaturaApi, CandidaturaEditPayload).

---

## 6. Modelos de dados (contratos)

- **RemuneracaoCalculationPayload / RemuneracaoCalculationResponse:** Ver `src/domain/entities/RemuneracaoCalculation.ts`.
- **Vaga (detalhes):** Contrato que inclua `custoProfissional` (number); demais campos conforme VagaApi/ObterVagaRecrutamentoPorId.
- **Candidatura (edição/demográficos):** Conforme CandidaturaDetails e CandidaturaEditPayload em `src/domain/entities/CandidaturaDetails.ts`.
- **Envelope padrão:** `retorno`, `sucesso`, `mensagem`, `erros?` em todas as respostas.

---

## 7. Dependências de APIs e dados existentes

| API / recurso | Uso no simulador |
|---------------|------------------|
| GET ObterVagaRecrutamentoPorId | Detalhes da vaga; `custoProfissional` para Custo Hora e Remuneração 168h |
| POST SimularRemuneracaoTotal | Cálculo de proposta vs pretendido; conformidade |
| GET ObterDadosParaEditarCandidaturaColaboradorComDadosDemograficos | Preenchimento do formulário a partir da candidatura |
| POST EditarCandidaturaColaboradorComDadosDemograficos | Salvar Dados |
| ListarVagasRecrutamento / ListarVagasRecrutamentoEPerfis | Lista de vagas no seletor |
| ListarCandidatosInscritos | Lista de candidatos da vaga |
| Gestores (GetManagersUseCase) | Campo gestor quando aplicável |

---

## 8. Fluxo resumido (backend)

1. Front obtém token do usuário logado.
2. Ao selecionar vaga: GET ObterVagaRecrutamentoPorId → preenche custoProfissional, modelo de trabalho, etc.
3. Ao selecionar candidato/candidatura: GET ObterDadosParaEditarCandidaturaColaboradorComDadosDemograficos → preenche formulário.
4. Gerar Cálculo: POST SimularRemuneracaoTotal → retorno com remuneracaoProposta, remuneracaoPretendida, emConformidadeComAPolitica e validações por campo.
5. Salvar Dados: validação no front → POST EditarCandidaturaColaboradorComDadosDemograficos → toast sucesso/erro.

---

## 9. Propostas de melhorias (evolução da feature)

- Paginação nos listagens de vagas/candidatos quando houver muitos registros.
- Auditoria: registrar data/hora e usuário nas alterações de candidatura (Salvar Dados).
- Filtros no servidor para listagem de vagas (status, cliente, etc.).
- Histórico de simulações (opcional) para análise.

---

## 10. Cenários de erro e pontos de atenção

- **Vaga/candidatura inexistente ou inacessível:** Backend deve retornar 404 ou `sucesso: false` com mensagem clara.
- **Token inválido/expirado:** 401; front trata como erro genérico (pode redirecionar para login).
- **Validação de negócio no SimularRemuneracaoTotal:** Campos obrigatórios e limites; retornar `erros` ou `mensagem` no envelope.
- **CEP inválido:** Front usa ViaCEP para sugestão; backend pode validar formato e existência.
- **Conformidade:** Garantir que `emConformidadeComAPolitica` reflita a regra de política salarial do cliente.

---

## 11. Resumo para o time

- **Feature:** Simulador de remuneração e benefícios — recrutador seleciona vaga e candidato, preenche formulário (pretensão, benefícios, demográficos), gera cálculo (proposta vs pretendido, conformidade) e salva dados na candidatura.
- **APIs usadas:** GET ObterVagaRecrutamentoPorId; POST SimularRemuneracaoTotal; GET ObterDadosParaEditarCandidaturaColaboradorComDadosDemograficos; POST EditarCandidaturaColaboradorComDadosDemograficos; listagens de vagas e candidatos.
- **Contratos:** RemuneracaoCalculation (payload/response com retorno.remuneracaoProposta, remuneracaoPretendida, emConformidadeComAPolitica); envelope padrão `retorno`, `sucesso`, `mensagem`, `erros`.
- **Pontos de atenção:** Custo Hora Vaga e Remuneração 168h somente da vaga (`custoProfissional`); limpeza de feedback ao trocar vaga/candidato; validações ao salvar (pessoas com quem mora, outros custos, CEP).
- **Documento complementar:** `docs/SIMULADOR_CUSTO_HORA_E_REMUNERACAO_VAGA.md` (lógica de custo hora e remuneração vaga e limpeza de feedback).

*Documento alinhado à stack React (ARCHITECTURE.md) e ao padrão de APIs do projeto. Última atualização: 13/03/2026.*
