# Feedback 360 – Documentação Técnica (Regras de Negócio e Backend)

Documentação técnica da página **Feedback 360** (`/feedback360`) para suporte ao desenvolvimento do backend em .NET 8 e à integração com o frontend React. A feature está **sem integrações e APIs próprias** no backend; este documento mapeia regras, contratos e APIs necessárias para que a tela funcione com a arquitetura atual do projeto (ver `ARCHITECTURE.md`).

---

## 1. Visão geral e objetivo

**Objetivos principais (visão de produto):** Permitir que o colaborador **registre** feedbacks 360 sobre colegas e **consulte** os que enviou; **padronizar** o registro (contexto, data, nível, comentário condicional) e **garantir** que a listagem reflita apenas os feedbacks do próprio usuário.

| Item | Descrição |
|------|------------|
| **Rota** | `/feedback360` |
| **Título** | Compartilhe seu Feedback |
| **Descrição (UI)** | Inserir, solicitar e consultar feedbacks para o seu perfil. |
| **Objetivo de negócio** | Permitir que o usuário autenticado **registre feedbacks 360** sobre outros colaboradores (para quem deseja oferecer feedback) e **consulte** os feedbacks que ele mesmo **enviou** (lista “meus feedbacks enviados”). |
| **Escopo atual** | Apenas **inserir** e **consultar (listar)**. Não há edição, exclusão, nem “feedbacks que recebi” nesta primeira versão. |

---

## 2. Parâmetros de entrada e contexto

- **Autenticação:** Todas as chamadas usam o **token** do usuário logado (Bearer), obtido do estado de autenticação (Redux).
- **Sem parâmetros de rota ou query:** A página não recebe `id`, `codigo` ou filtros via URL. Filtros (data, sentimento) são apenas no cliente (front) sobre a lista já retornada pela API.
- **Dependência de API existente:** Na aba **Inserir**, a lista de colaboradores (para escolher “para quem” dar o feedback) vem da API já existente **`GET /api/MapaDeAlocacao/ListarColaboradoresOrg`** (parâmetros: `busca`, `cursor`, `limite`). O front usa `ColaboradoresApi.listarColaboradoresOrg` com `busca: '', cursor: 0, limite: 50000` para carregar a lista uma vez ao abrir a aba. Essa API retorna itens com **`codigoColaboradorInterno`**; o valor é o mesmo que deve ser enviado como **`codigoInternoColaborador`** no payload do Feedback 360 (padrão do projeto para identificador de colaborador em contratos de domínio).

---

## 2.1 Padrões de contratos do projeto (consistência)

Para manter consistência com as demais APIs e entidades do projeto (ex.: `FuncionalidadeSistemaApi`, `GrupoAcessoApi`, `VagaApi`, entidades em `domain/entities`):

| Aspecto | Padrão | Exemplo no projeto |
|---------|--------|--------------------|
| **Nomenclatura JSON** | camelCase | `codigoInternoColaborador`, `dataCriacao`, `nomeColaborador` |
| **Resposta de API** | `retorno`, `sucesso`, `mensagem`, `erros?` | Todas as APIs de listagem/criação usam esse envelope |
| **Data de criação** | `dataCriacao` (string ISO ou YYYY-MM-DD) | FuncionalidadeSistemaApi, GrupoAcessoApi, Vaga, Organograma, Tbd |
| **Data de alteração** | `dataAlteracao` (quando houver auditoria) | FuncionalidadeSistemaApi, GrupoAcessoApi, Tbd |
| **Identificador colaborador** | `codigoInternoColaborador` | CompetenciasApi, VagaApi, Feedback360 (request/response). Nota: `ListarColaboradoresOrg` retorna `codigoColaboradorInterno` (mesmo valor, nome diferente naquele contrato). |
| **Erros de validação** | `erros?: string[] \| null` no envelope da resposta | GrupoAcessoApi, FuncionalidadeSistemaApi, VagaApi, CandidaturaApi |

Os contratos do Feedback 360 abaixo seguem esses padrões. O frontend atual usa `criadoEm` na entidade; recomenda-se que o **backend** exponha **`dataCriacao`** na resposta e que o front mapeie para `criadoEm` ou que a entidade seja renomeada para `dataCriacao` para alinhar ao restante do projeto.

---

## 3. Regras de negócio

### 3.1 Níveis de avaliação (sentimento)

O feedback tem um **nível** numérico de 1 a 5, com significado fixo:

| Nível | Label (sentimento) | Comentário livre |
|-------|---------------------|------------------|
| 1 | Precisa evoluir bastante | **Obrigatório** |
| 2 | Abaixo do esperado | **Obrigatório** |
| 3 | Dentro do esperado | Opcional |
| 4 | Acima do esperado | Opcional |
| 5 | Referência para os demais | Opcional |

**Regra:** Quando o nível for **1 ou 2**, o campo **comentário** é obrigatório (validação no front e deve ser rejeitado no backend se ausente/vazio).

### 3.2 Campos obrigatórios ao inserir

| Campo | Tipo | Obrigatório | Observação |
|-------|------|-------------|------------|
| Colaborador (destinatário) | string (código interno) | Sim | Código do colaborador que recebe o feedback; deve existir na organização. |
| Contexto | string | Sim | Texto livre; front envia trim. |
| Data da interação | string (date) | Sim | Formato **YYYY-MM-DD** (ISO date, sem hora). |
| Nível (avaliação) | int (1–5) | Sim | Apenas valores 1 a 5. |
| Comentário | string | Condicional | Obrigatório se nível = 1 ou 2; opcional caso contrário. Front envia trim. |

O front envia também o **nome do colaborador** (para exibição), mas a referência canônica é o **código interno**.

### 3.3 Quem pode dar / sobre quem / o que listar

- **Quem dá o feedback:** Usuário autenticado (identificado pelo token). O backend deve associar o feedback ao “autor” (por exemplo, usuário/colaborador do token).
- **Sobre quem:** Qualquer colaborador retornado por `ListarColaboradoresOrg` (mesma organização/contexto do usuário). O backend deve validar se o `codigoInternoColaborador` existe e, se houver regra de negócio, se pode receber feedback (ex.: mesma org, ativo, etc.).
- **O que listar:** A listagem retorna **apenas os feedbacks que o próprio usuário (autor) enviou**. Não há, na tela atual, “feedbacks que eu recebi” nem listagem por colaborador destinatário.

### 3.4 Comportamento da lista (consultar)

- **Filtros:** Aplicados **no front** sobre a lista completa retornada pela API:
  - **Por data:** Comparação com `dataInteracao` (string YYYY-MM-DD); o front usa `slice(0, 10)` da string para comparar com o valor do input date.
  - **Por sentimento:** Comparação com o label do nível (ex.: “Dentro do esperado”); o front deriva do `nivel` (1–5).
- **Ordenação:** O front não envia parâmetros de ordenação; o backend pode definir o padrão (ex.: mais recente primeiro por `dataCriacao` ou `dataInteracao`).

---

## 4. Funcionalidades e experiência do usuário

### 4.1 Aba “Inserir feedback”

1. Usuário escolhe **colaborador** em um combobox (busca por nome ou código); origem: `ListarColaboradoresOrg`.
2. Preenche **contexto**, **data da interação**, **nível** (1–5) e **comentário** (obrigatório se nível 1 ou 2).
3. Clica em **Enviar Feedback**.
4. Front valida; envia **POST** para a API de criação; em sucesso mostra toast de sucesso, limpa o formulário e muda para a aba **Consultar Feedbacks**, recarregando a lista.

### 4.2 Aba “Consultar Feedbacks”

1. Ao abrir a aba, o front chama **GET** da API de listagem (sem parâmetros além do token).
2. Exibe lista de cards (nome do colaborador, data, sentimento, trecho de contexto/comentário).
3. Filtros opcionais: **data** e **sentimento** (aplicados no cliente).
4. Clique em **Ver Detalhes** abre um modal com: Para (nome), Data, Avaliação (emoji + label), Contexto, Comentário.

### 4.3 Modal de detalhes

- Apenas leitura; exibe todos os campos do item (nomeColaborador, dataInteracao, nivel, contexto, comentario, dataCriacao). Sem edição ou exclusão.

### 4.4 Regras de sucesso, erro e bloqueios (UX)

- **Sucesso (Enviar Feedback):** Toast de sucesso; formulário é limpo; usuário é levado à aba **Consultar Feedbacks** e a lista é recarregada.
- **Erro:** Em falha de validação (front ou backend), exibir mensagem (toast ou inline); em `sucesso: false`, usar `mensagem` ou `erros` do envelope para texto amigável (ex.: “Colaborador é obrigatório”, “Comentário é obrigatório quando a avaliação é 1 ou 2”). Em erro de rede, mensagem genérica e possibilidade de tentar novamente.
- **Bloqueios:** Não há avanço de etapa; o bloqueio é por validação no envio (campos obrigatórios e comentário condicional). Botão **Enviar Feedback** pode permanecer habilitado e a validação exibir erros nos campos ou em toast.

### 4.5 Linguagem e tom

- Mensagens em tom neutro e profissional (colaborador interno). Labels de nível já são amigáveis (“Precisa evoluir bastante”, “Dentro do esperado”). Evitar jargões em mensagens de erro; preferir “Comentário é obrigatório quando a avaliação é baixa” a siglas ou termos técnicos.

---

## 5. APIs necessárias (backend .NET 8)

A stack front usa `httpClient` (ver `ARCHITECTURE.md`): requisições com **Bearer token** no header. Base URL da API conforme configuração do projeto.

### 5.1 Listar feedbacks (do usuário autenticado)

- **Método e rota:** `GET /api/Feedback360/Listar`
- **Headers:** `Authorization: Bearer {token}`
- **Parâmetros de query:** Nenhum (a listagem é “todos os feedbacks que eu enviei”).
- **Resposta esperada (200):**

```json
{
  "retorno": [
    {
      "id": "string (guid ou id único)",
      "codigoInternoColaborador": "string",
      "nomeColaborador": "string",
      "contexto": "string",
      "dataInteracao": "YYYY-MM-DD",
      "nivel": 1,
      "comentario": "string",
      "dataCriacao": "YYYY-MM-DDTHH:mm:ss (ISO 8601)"
    }
  ],
  "sucesso": true,
  "mensagem": null,
  "erros": null
}
```

- **Contrato sugerido (C#):**
  - Response: `Feedback360ListResponse` com `List<Feedback360ItemDto> Retorno`, `bool Sucesso`, `string Mensagem`, `List<string> Erros` (opcional).
  - Item: `Feedback360ItemDto` com `Id`, `CodigoInternoColaborador`, `NomeColaborador`, `Contexto`, `DataInteracao` (string YYYY-MM-DD), `Nivel` (int), `Comentario`, `DataCriacao` (string ISO ou DateTime) — alinhado ao padrão do projeto (dataCriacao).

### 5.2 Criar feedback

- **Método e rota:** `POST /api/Feedback360/Criar`
- **Headers:** `Authorization: Bearer {token}`, `Content-Type: application/json`
- **Corpo (JSON):**

```json
{
  "codigoInternoColaborador": "string",
  "nomeColaborador": "string",
  "contexto": "string",
  "dataInteracao": "YYYY-MM-DD",
  "nivel": 1,
  "comentario": "string"
}
```

- **Resposta esperada (200/201):**

```json
{
  "retorno": {
    "id": "string",
    "codigoInternoColaborador": "string",
    "nomeColaborador": "string",
    "contexto": "string",
    "dataInteracao": "YYYY-MM-DD",
    "nivel": 1,
    "comentario": "string",
    "dataCriacao": "YYYY-MM-DDTHH:mm:ss"
  },
  "sucesso": true,
  "mensagem": null,
  "erros": null
}
```

- **Contrato sugerido (C#):**
  - Request: `Feedback360CreateRequest` (ou command) com `CodigoInternoColaborador`, `NomeColaborador`, `Contexto`, `DataInteracao`, `Nivel`, `Comentario`.
  - Validação backend: `Nivel` entre 1 e 5; se `Nivel` é 1 ou 2, `Comentario` não pode ser nulo/vazio; `CodigoInternoColaborador` deve existir (e, se aplicável, estar ativo na mesma org). Autor = usuário do token.
  - Response: `Feedback360CreateResponse` com `Feedback360ItemDto Retorno`, `bool Sucesso`, `string Mensagem`, `List<string> Erros` (opcional).

---

## 6. Modelos de dados (contratos)

### 6.1 Entidade / DTO – item de feedback

| Propriedade | Tipo | Obrigatório | Descrição |
|-------------|------|-------------|-----------|
| id | string (guid) | Sim (na resposta) | Identificador único do registro. |
| codigoInternoColaborador | string | Sim | Código do colaborador que **recebe** o feedback (padrão do projeto; mesma referência que `codigoColaboradorInterno` em `ListarColaboradoresOrg`). |
| nomeColaborador | string | Sim | Nome do colaborador (exibição). |
| contexto | string | Sim | Contexto do feedback. |
| dataInteracao | string (YYYY-MM-DD) | Sim | Data da interação. |
| nivel | int (1–5) | Sim | Nível de avaliação. |
| comentario | string | Condicional | Obrigatório se nivel 1 ou 2. |
| dataCriacao | string (ISO 8601) | Sim (na resposta) | Data/hora de criação do registro (padrão do projeto; ex.: FuncionalidadeSistemaApi, GrupoAcessoApi usam `dataCriacao`). |

### 6.2 Payload de criação (request)

| Propriedade | Tipo | Obrigatório | Descrição |
|-------------|------|-------------|-----------|
| codigoInternoColaborador | string | Sim | Código do colaborador destinatário. |
| nomeColaborador | string | Sim | Nome (pode ser validado/atualizado no backend a partir do código). |
| contexto | string | Sim | Texto; trim no front. |
| dataInteracao | string (YYYY-MM-DD) | Sim | Data da interação. |
| nivel | int (1–5) | Sim | Nível. |
| comentario | string | Condicional | Obrigatório se nivel 1 ou 2. |

### 6.3 Respostas padrão (envelope)

- **retorno:** objeto ou array conforme endpoint (item na criação, lista na listagem).
- **sucesso:** boolean.
- **mensagem:** string (opcional; ex.: erro ou mensagem de validação).
- **erros:** string[] | null (opcional; lista de mensagens de validação ou erro), alinhado a FuncionalidadeSistemaApi, GrupoAcessoApi, VagaApi, CandidaturaApi.

Em caso de erro (4xx/5xx), o front hoje trata de forma genérica (toast de erro, lista vazia na listagem). O backend deve retornar o mesmo envelope com `sucesso: false`, `mensagem` com texto amigável e, quando aplicável, `erros` com a lista de falhas de validação.

---

## 7. Dependências de APIs e dados existentes

- **Colaboradores (destinatário do feedback):** A tela usa **`GET /api/MapaDeAlocacao/ListarColaboradoresOrg`** com `busca`, `cursor`, `limite`. Essa API retorna itens com **`codigoColaboradorInterno`**; o front envia esse mesmo valor como **`codigoInternoColaborador`** no POST do Feedback 360 (padrão de nomenclatura nos contratos de domínio). O backend deve validar o `codigoInternoColaborador` contra a mesma base de colaboradores usada pelo Mapa de Alocação. Dependência com:
  - Tabela(s) de colaboradores / usuários já usadas por Mapa de Alocação, Colaborador, ou RH.
- **Autor do feedback:** Identificado pelo token (usuário logado). O backend precisará do identificador do colaborador/usuário associado ao token para persistir “quem enviou” o feedback (para listar apenas “meus feedbacks enviados”).

---

## 8. Fluxo resumido (backend)

1. **Listar:** Obter identificador do usuário a partir do token → buscar registros de Feedback 360 onde “autor” = usuário → retornar lista (ex.: ordenada por `dataCriacao` desc).
2. **Criar:** Obter identificador do usuário a partir do token → validar payload (nível 1–5; comentário obrigatório se 1 ou 2; código do colaborador existente) → persistir registro associando autor = usuário do token → retornar item criado (com `id` e `dataCriacao`).

---

## 9. Propostas de melhorias (evolução da feature)

- **Listar “feedbacks que recebi”:** Endpoint ou parâmetro para listar por colaborador destinatário (ex.: eu), com possíveis regras de privacidade/visibilidade.
- **Filtros no servidor:** Enviar filtros de data e sentimento (nível) na **GET** para reduzir payload e permitir paginação (ex.: `dataInicio`, `dataFim`, `nivel`).
- **Paginação:** Incluir `cursor`/`limite` (ou `page`/`pageSize`) na listagem para muitas entradas.
- **Edição/Exclusão:** Se houver regra de negócio (ex.: permitir editar/excluir em até X horas), definir endpoints PUT e DELETE e contratos.
- **Auditoria:** Registrar quem criou e quando (`dataCriacao`); eventualmente `dataAlteracao` e usuário de alteração se houver edição (padrão do projeto).
- **Validação de nome:** Backend pode ignorar ou sobrescrever `nomeColaborador` com o nome atual do colaborador a partir de `codigoInternoColaborador`, para consistência.

---

## 10. Cenários de erro e pontos de atenção

### 10.1 Erros possíveis não mapeados no front

- **Colaborador inativo ou inexistente:** Backend deve rejeitar criação com mensagem clara (ex.: “Colaborador não encontrado ou inativo”).
- **Data da interação no futuro:** Regra de negócio a definir (rejeitar ou permitir); hoje o front não bloqueia.
- **Limite de feedbacks por período:** Não há regra no front; backend pode impor (ex.: máximo X feedbacks por colaborador por mês).
- **Token inválido/expirado:** Retorno 401; front trata como erro genérico (pode redirecionar para login).
- **Conflito de dados (ex.: colaborador excluído após carregar a lista):** Na criação, validar sempre no servidor.

### 10.2 Impacto em desenvolvimento (tabelas e dependências)

- **Tabelas sugeridas (exemplo):**
  - **Feedback360:** id (PK), autor_colaborador_id ou autor_user_id (FK), codigo_interno_colaborador_destino (ou FK para colaborador; alinhar ao campo usado em Mapa de Alocação/Colaborador), nome_colaborador_destino (snapshot ou desnormalizado), contexto, data_interacao, nivel, comentario, data_criacao, data_alteracao (opcional). Nomes em API: camelCase (`dataCriacao`, `dataAlteracao`, `codigoInternoColaborador`). Índices: autor, data_interacao, destinatário.
- **Dependências:**
  - **Colaborador/Usuário:** Para validar destinatário e para obter autor a partir do token. Alinhar com a mesma fonte usada por `ListarColaboradoresOrg` (Mapa de Alocação) para evitar inconsistência (código inexistente ou de outra org).
  - **Organização (org):** Se a listagem de colaboradores for por org, o feedback deve restringir destinatários à mesma org (ou à regra já usada no Mapa de Alocação).
- **Segurança:** Garantir que a listagem retorne **somente** feedbacks cujo autor seja o usuário do token; não expor feedbacks de outros autores por falha de filtro.

---

## 11. Resumo para o time

- **Feature:** Feedback 360 – usuário envia feedback sobre outro colaborador (nível 1–5, contexto, data, comentário obrigatório se 1 ou 2) e consulta apenas os feedbacks que **ele enviou**.
- **APIs a implementar:** `GET /api/Feedback360/Listar` (lista do usuário) e `POST /api/Feedback360/Criar` (criação com validação de nível/comentário e colaborador).
- **API já usada:** `GET /api/MapaDeAlocacao/ListarColaboradoresOrg` para preencher o combobox de “para quem” dar o feedback.
- **Contratos:** Objeto com id, codigoInternoColaborador, nomeColaborador, contexto, dataInteracao, nivel, comentario, dataCriacao; resposta padrão com `retorno`, `sucesso`, `mensagem`, `erros` (alinhado aos padrões do projeto: FuncionalidadeSistemaApi, GrupoAcessoApi, VagaApi).
- **Pontos de atenção:** Validar destinatário e autor; alinhar com tabelas de colaborador já existentes; considerar filtros e paginação futuros e cenários de erro (colaborador inválido, data futura, limites de negócio).

*Documento alinhado à stack React (ARCHITECTURE.md) e ao padrão de APIs do projeto. Última atualização: Fev 2026.*
