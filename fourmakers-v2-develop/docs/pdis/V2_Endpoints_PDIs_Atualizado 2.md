Segue o texto revisado para você copiar e colar, com o novo endpoint e a justificativa de como "meus PDIs" e o `codigo_interno_colaborador` funcionam:

---

# Endpoints PDI e Contratos

Base URL da API Gestão de Pessoa (ex.): `https://.../api/GestaoPessoa/Pdi`

Todos os endpoints exigem **Authorization** (JWT). Em caso de erro de validação/negócio, a API retorna **400 Bad Request** com corpo no formato `ApiGenericResult` (Sucesso, Mensagem, Erros).

---

## Identificação do colaborador

- **Meus PDIs** (`/api/GestaoPessoa/Pdi/MeusPdis`): o "meu" é o **usuário logado**. O controller obtém `Cpf` e `OrgId` do JWT (`IAspNetUser.GetUsuarioLogado()`), o service usa esse **CPF como código do colaborador** (neste backend `codigo_interno_colaborador` = CPF) e o repositório filtra na tabela **tb_pdi** com `WHERE codigo_interno_colaborador = @CodigoInternoColaborador`. Ou seja, **"meus PDIs" = registros em `tb_pdi` cuja coluna `codigo_interno_colaborador` é o CPF do usuário autenticado**.
- **PDIs do time** (`/api/GestaoPessoa/Pdi/PdisDoTime`): o gestor logado só vê PDIs de colaboradores que são seus **subordinados** (consulta de hierarquia por CPF/org). O parâmetro **colaboradorId** no endpoint por colaborador é o mesmo identificador: **`codigo_interno_colaborador` da tabela `tb_pdi`** (em geral o CPF do colaborador).

**Fluxo de status:** PDI criado pelo **colaborador** → sem etapa de aprovação; após criar plano de ação (ou já com planos no body), status vai para **Em Análise** (IN_ANALYSIS). PDI criado pelo **gestor** → fica **NOT_STARTED** até o gestor chamar o endpoint **aprovar** (`PATCH .../aprovar`); após aprovar, status vai para **Andamento** (IN_PROGRESS).

---

## 1. Meus PDIs — `api/GestaoPessoa/Pdi/MeusPdis`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/GestaoPessoa/Pdi/MeusPdis` | Listar meus PDIs |
| POST | `/api/GestaoPessoa/Pdi/MeusPdis` | Criar PDI |
| PUT | `/api/GestaoPessoa/Pdi/MeusPdis/{id}` | Atualizar PDI |
| POST | `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/action-plans` | Adicionar action plan |
| PUT | `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/action-plans/{actionPlanId}` | Atualizar action plan *(a implementar)* |
| DELETE | `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/action-plans/{actionPlanId}` | Excluir action plan *(a implementar)* |
| PATCH | `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/action-plans/{actionPlanId}/complete` | Concluir action plan |
| POST | `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/evidencias` | Upload de evidência (multipart) |
| GET | `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/evidencias` | Listar evidências do PDI |
| GET | `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/evidencias/{evidenciaId}` | Baixar (ler) arquivo da evidência |

---

### GET `/api/GestaoPessoa/Pdi/MeusPdis` — Listar meus PDIs

- **Request:** sem body.
- **Response 200:** `ApiGenericResult<IEnumerable<PdiResumoDTO>>`

```json
{
  "sucesso": true,
  "mensagem": null,
  "erros": null,
  "retorno": [
    {
      "id": "guid",
      "colaboradorId": "string",
      "titulo": "string",
      "descricao": "string",
      "status": "string",
      "dataCriacao": "date-time",
      "dataAtualizacao": "date-time | null",
      "codigoInternoColaboradorCriacao": "string | null",
      "codigoInternoColaboradorAlteracao": "string | null",
      "progress": 0.0,
      "skills": [ { "id": "guid", "nomeSkill": "string", "codigoSkill": "string" } ],
      "actionPlans": [ { "id": "guid", "description": "string", "deadline": "date-time | null", "concluidoEm": "date-time | null", "codigoInternoColaboradorCriacao": "string | null", "codigoInternoColaboradorAlteracao": "string | null" } ]
    }
  ]
}
```

---

### POST `/api/GestaoPessoa/Pdi/MeusPdis` — Criar PDI

- **Request body:** `PdiCriarRequestDTO`
- **Response 201:** `ApiGenericResult<PdiCriarResponseDTO>`
- **Response 400:** erro de validação/negócio

**PdiCriarRequestDTO:**

| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| titulo | string | sim |
| descricao | string | sim |
| skills | List\<PdiSkillInputDTO\> | não (default []) |
| actionPlans | List\<PdiActionPlanInputDTO\> | não (default []) |

**PdiSkillInputDTO:** `nomeSkill`, `codigoSkill` (string)

**PdiActionPlanInputDTO:** `description` (string), `deadline` (date-time?)

**PdiCriarResponseDTO (retorno):** `id` (Guid), `status` (string), `dataCriacao` (DateTime). O `status` na criação: **NOT_STARTED** (sem planos no body ou PDI criado pelo gestor); **IN_ANALYSIS** (criado pelo colaborador já com `actionPlans` no body).

---

### PUT `/api/GestaoPessoa/Pdi/MeusPdis/{id}` — Atualizar PDI

- **Route:** `id` = Guid do PDI.
- **Request body:** `PdiAtualizarRequestDTO`
- **Response 200:** `ApiGenericResult<PdiAtualizarResponseDTO>`
- **Response 400:** erro de validação/negócio

**PdiAtualizarRequestDTO:** `titulo`, `descricao`, `status` (string)

**PdiAtualizarResponseDTO (retorno):** `id` (Guid), `dataAtualizacao` (DateTime?)

---

### POST `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/action-plans` — Adicionar action plan

- **Route:** `pdiId` = Guid do PDI.
- **Request body:** `PdiActionPlanInputDTO` — `description`, `deadline` (opcional).
- **Response 201:** `ApiGenericResult<PdiActionPlanDTO>`
- **Response 400:** erro de validação/negócio

**PdiActionPlanDTO (retorno):** `id`, `description`, `deadline`, `concluidoEm` (DateTime?), `codigoInternoColaboradorCriacao` (string?), `codigoInternoColaboradorAlteracao` (string?)

---

### PUT `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/action-plans/{actionPlanId}` — Atualizar action plan *(a implementar no backend)*

- **Route:** `pdiId`, `actionPlanId` = Guids.
- **Request body:** `PdiActionPlanInputDTO` — `description`, `deadline` (opcional).
- **Response 200:** `ApiGenericResult<PdiActionPlanDTO>`
- **Response 400:** erro de validação/negócio (ex.: plano não encontrado ou não pertence ao PDI do usuário logado).

---

### DELETE `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/action-plans/{actionPlanId}` — Excluir action plan *(a implementar no backend)*

- **Route:** `pdiId`, `actionPlanId` = Guids.
- **Request:** sem body.
- **Response 200:** `ApiGenericResult` com `sucesso: true` (ou 204 No Content).
- **Response 400:** quando o plano não existe ou não pertence ao PDI do usuário logado.

---

### PATCH `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/action-plans/{actionPlanId}/complete` — Concluir action plan

- **Route:** `pdiId`, `actionPlanId` = Guids.
- **Request:** sem body.
- **Response 200:** `ApiGenericResult<PdiConcluirActionPlanResponseDTO>`
- **Response 400:** erro de validação/negócio

**PdiConcluirActionPlanResponseDTO (retorno):** `id`, `concluidoEm`, `progress` (double), `status` (string)

---

### POST `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/evidencias` — Upload evidência

- **Content-Type:** `multipart/form-data`
- **Route:** `pdiId` = Guid do PDI.
- **Form:**
  - `arquivo`: IFormFile (obrigatório para upload)
  - `tipo`: string (default `"CERTIFICADO"`)
- **Response 201:** `ApiGenericResult<PdiEvidenciaUploadResultDTO>`
- **Response 400:** erro de validação/negócio

**PdiEvidenciaUploadResultDTO (retorno):** `id`, `pdiId`, `docName`, `docPath`, `docMime`, `docSize`, `tipo`, `createdAt`

---

### GET `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/evidencias` — Listar evidências

- **Route:** `pdiId` = Guid do PDI.
- **Response 200:** `ApiGenericResult<IEnumerable<PdiEvidenciaDTO>>`

**PdiEvidenciaDTO:** `id`, `pdiId`, `docName`, `docPath`, `docMime`, `docSize`, `tipo`, `createdAt`

---

### GET `/api/GestaoPessoa/Pdi/MeusPdis/{pdiId}/evidencias/{evidenciaId}` — Baixar (ler) arquivo da evidência

- **Route:** `pdiId` = Guid do PDI; `evidenciaId` = Guid da evidência (o mesmo `id` retornado na listagem de evidências).
- **Request:** sem body.
- **Response 200:** o **arquivo binário** (stream), com `Content-Type` igual ao tipo do documento (ex.: `application/pdf`) e `Content-Disposition: attachment; filename="..."` para download. O nome do arquivo é o `docName` da evidência.
- **Response 400:** `ApiGenericResult` com `sucesso: false` quando a evidência não existe ou não pertence ao PDI do colaborador logado (ex.: "Evidência não encontrada ou sem permissão.").

---

## 2. PDIs do Time — `api/GestaoPessoa/Pdi/PdisDoTime`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/GestaoPessoa/Pdi/PdisDoTime` | Listar PDIs do time |
| GET | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}` | Listar PDIs de um colaborador do time por colaborador_id |
| GET | `/api/GestaoPessoa/Pdi/PdisDoTime/{pdiId}` | Obter PDI completo do time |

---

### GET `/api/GestaoPessoa/Pdi/PdisDoTime` — Listar PDIs do time

- **Request:** sem body. Query: `pagina` (default 1), `tamanhoPagina` (default 10, máx. 100).
- **Response 200:** `ApiGenericResult<PdiListagemTimeResult>` (items, totalCount, pagina, tamanhoPagina, totalPaginas)

**PdiResumoTimeDTO (em items):** `colaboradorId`, `pdiId` (Guid), `titulo`, `status`, `progress` (double)

---

### GET `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}` — Listar PDIs de um colaborador do time

- **Route:** `colaboradorId` = identificador do colaborador (mesmo valor da coluna **colaborador_id** da tabela **pdi**; em geral o `codigo_interno_colaborador`, ex.: CPF). Só retorna PDIs se o colaborador for **subordinado** do usuário logado.
- **Request:** sem body.
- **Response 200:** `ApiGenericResult<IEnumerable<PdiResumoDTO>>` (mesmo contrato da listagem "meus PDIs").
- **Response 400:** quando `Sucesso == false` (ex.: colaborador não pertence ao seu time ou nenhum subordinado encontrado).

---

### GET `/api/GestaoPessoa/Pdi/PdisDoTime/{pdiId}` — Obter PDI completo do time

- **Route:** `pdiId` = Guid do PDI.
- **Response 200:** `ApiGenericResult<PdiCompletoTimeDTO>`
- **Response 400:** quando `Sucesso == false` (ex.: PDI não encontrado ou sem permissão)

**PdiCompletoTimeDTO:**  
`id`, `colaboradorId`, `titulo`, `descricao`, `status`, `progress`,  
`skills` (List\<PdiSkillDTO\>), `actionPlans` (List\<PdiActionPlanDTO\>),  
`evidencias` (List\<PdiEvidenciaResumoDTO\>)

**PdiEvidenciaResumoDTO:** `id`, `docName`, `docPath`, `tipo`

---

## 3. Gestor — PDI do colaborador (api/GestaoPessoa/Pdi/PdisDoTime)

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}` | Gestor: criar PDI para colaborador |
| PUT | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}` | Gestor: atualizar PDI |
| POST | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/action-plans` | Gestor: adicionar action plan |
| PUT | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/action-plans/{actionPlanId}` | Gestor: atualizar action plan *(a implementar)* |
| DELETE | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/action-plans/{actionPlanId}` | Gestor: excluir action plan *(a implementar)* |
| PATCH | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/action-plans/{actionPlanId}/complete` | Gestor: concluir action plan |
| PATCH | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/aprovar` | Gestor: aprovar PDI (criado pelo gestor; após aprovar → status IN_PROGRESS) |
| POST | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/evidencias` | Gestor: upload evidência |
| GET | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/evidencias` | Gestor: listar evidências |
| GET | `/api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/evidencias/{evidenciaId}` | Gestor: baixar (ler) arquivo da evidência |

---

### GET `.../evidencias/{evidenciaId}` — Gestor: baixar (ler) arquivo da evidência

- **Route:** `colaboradorId`, `pdiId` como nos outros endpoints do gestor; `evidenciaId` = Guid da evidência (o mesmo `id` da listagem de evidências).
- **Request:** sem body.
- **Response 200:** o **arquivo binário** (stream), com `Content-Type` e `Content-Disposition` para download.
- **Response 400:** quando o colaborador não pertence ao time, a evidência não existe ou não pertence ao PDI (ex.: "Colaborador não pertence ao seu time." ou "Evidência não encontrada ou não pertence a este PDI.").

---

### PUT `.../action-plans/{actionPlanId}` — Gestor: atualizar action plan *(a implementar no backend)*

- **Route:** `colaboradorId`, `pdiId`, `actionPlanId` = Guids (colaboradorId = codigo_interno_colaborador).
- **Request body:** `PdiActionPlanInputDTO` — `description`, `deadline` (opcional).
- **Response 200:** `ApiGenericResult<PdiActionPlanDTO>`
- **Response 400:** quando o colaborador não pertence ao time, o plano não existe ou não pertence ao PDI.

---

### DELETE `.../action-plans/{actionPlanId}` — Gestor: excluir action plan *(a implementar no backend)*

- **Route:** `colaboradorId`, `pdiId`, `actionPlanId` como nos outros endpoints do gestor.
- **Request:** sem body.
- **Response 200:** `ApiGenericResult` com `sucesso: true` (ou 204 No Content).
- **Response 400:** quando o colaborador não pertence ao time ou o plano não existe/não pertence ao PDI.

---

### PATCH `.../aprovar` — Gestor: aprovar PDI

- **Route:** `colaboradorId`, `pdiId` como nos outros endpoints do gestor.
- **Request:** sem body.
- **Descrição:** PDI criado pelo gestor fica com status **NOT_STARTED** até o gestor aprovar. Após o colaborador criar os planos de ação, o gestor chama este endpoint; o status do PDI passa para **IN_PROGRESS** (Andamento).
- **Response 200:** `ApiGenericResult<PdiAprovarResponseDTO>` — `id` (Guid do PDI), `status` ("IN_PROGRESS"), `aprovadoEm` (DateTime).
- **Response 400:** quando o colaborador não pertence ao time, o PDI não existe, o PDI foi criado pelo próprio colaborador (não requer aprovação), ou o PDI já foi aprovado (ex.: "PDI foi criado pelo próprio colaborador; não requer aprovação do gestor." ou "PDI já foi aprovado ou não está aguardando aprovação.").

**PdiAprovarResponseDTO (retorno):** `id` (Guid), `status` (string), `aprovadoEm` (DateTime)

---

## 4. Métricas PDI — `api/GestaoPessoa/Pdi/Metricas`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/GestaoPessoa/Pdi/Metricas` | Métricas (visão colaborador) |
| GET | `/api/GestaoPessoa/Pdi/Metricas/gestor` | Métricas (visão gestor — time) |
| GET | `/api/GestaoPessoa/Pdi/Metricas/gestor/colaborador/{colaboradorId}` | Métricas (visão gestor — um colaborador) |

**Response 200:** `ApiGenericResult<PdiMetricasResultDTO>` (bigNumbers, ativos, historicos)

---

## Resumo dos DTOs (DataTransferObject.Domain.GestaoPessoa.Pdi)

| DTO | Uso |
|-----|-----|
| PdiCriarRequestDTO | POST criar PDI |
| PdiCriarResponseDTO | Resposta criar PDI |
| PdiAtualizarRequestDTO | PUT atualizar PDI |
| PdiAtualizarResponseDTO | Resposta atualizar PDI |
| PdiResumoDTO | Listagem "meus PDIs" e listagem por colaborador do time |
| PdiResumoTimeDTO | Listagem "PDIs do time" |
| PdiCompletoTimeDTO | Detalhe PDI do time (+ PdiEvidenciaResumoDTO) |
| PdiSkillInputDTO | Input skill (criar) |
| PdiSkillDTO | Skill em respostas |
| PdiActionPlanInputDTO | Input action plan (criar/adicionar) |
| PdiActionPlanDTO | Action plan em respostas |
| PdiConcluirActionPlanResponseDTO | Resposta concluir action plan |
| PdiEvidenciaDTO | Item de listagem de evidências |
| PdiEvidenciaUploadResultDTO | Resposta upload evidência |
| PdiEvidenciaDownloadDTO | Uso interno no download (Content, FileName, ContentType); a API retorna o arquivo como stream. |
| PdiAprovarResponseDTO | Resposta do endpoint Gestor: aprovar PDI (id, status, aprovadoEm) |
| PdiMetricasResultDTO | Resposta métricas (bigNumbers, ativos, historicos) |

Respostas de erro (400) usam o mesmo envelope: `StatusResult` / `ApiGenericResult` com `sucesso: false`, `mensagem` e opcionalmente `erros` (lista de strings).
