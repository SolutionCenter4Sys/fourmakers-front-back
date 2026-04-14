# Feedback (MVP) – Documentação Técnica de Desenvolvimento

**Feature**: 15916 – MVP Feedback  
**Rota**: `/feedback360`  
**Última atualização**: Fevereiro 2026

---

## 1. Arquitetura

### 1.1 Clean Architecture

A implementação segue a Clean Architecture do projeto:

```
Presentation (Feedback360.tsx, useFeedback360)
  → App (Redux: auth token)
  → Domain (Entities, Repository Interface, Use Cases)
  ← Data (Feedback360Api, Feedback360RepositoryImpl, Colaboradores via ListarColaboradoresOrg)
  → httpClient (todas as chamadas HTTP)
```

- **Páginas/Components** não acessam `data/` diretamente; usam Use Cases via hook.
- **API**: uso obrigatório de `httpClient` de `@data/api/httpClient`; não usar `fetch()` ou `axios` direto.

### 1.2 Estrutura de Pastas Envolvidas

```
src/
├── domain/
│   ├── entities/
│   │   ├── Feedback360.ts       # Feedback360Item, Feedback360CreatePayload, Feedback360Nivel, labels/emojis
│   │   └── ColaboradorCch.ts    # contrato do item de ListarColaboradoresOrg
│   ├── repositories/
│   │   └── Feedback360Repository.ts
│   └── usecases/
│       ├── ListarFeedback360UseCase.ts
│       ├── CriarFeedback360UseCase.ts
│       └── ListarColaboradoresOrgUseCase.ts
├── data/
│   ├── api/
│   │   ├── httpClient.ts
│   │   ├── Feedback360Api.ts
│   │   └── (MapaDeAlocacao/Colaboradores via API existente)
│   └── repositories/
│       └── Feedback360RepositoryImpl.ts
├── presentation/
│   ├── pages/
│   │   └── Feedback360.tsx
│   └── hooks/
│       └── useFeedback360.ts
└── core/
    └── di/
        ├── tokens.ts    # feedback360Api, feedback360Repository
        └── container.ts # registro API, Repository, Use Cases
```

---

## 2. APIs e Contratos

### 2.1 API Existente – Colaboradores (Autocomplete)

- **Endpoint**: `GET /api/MapaDeAlocacao/ListarColaboradoresOrg`
- **Parâmetros**: `busca`, `cursor`, `limite` (ex.: `busca: '', cursor: 0, limite: 50000`).
- **Uso**: Preencher combobox/autocomplete "para quem" dar o feedback; somente colaboradores ativos na org (conforme contrato do backend).
- **Resposta**: Itens com `codigoColaboradorInterno`, `nm_Profissional`; front envia `codigoColaboradorInterno` como `codigoInternoColaborador` no payload do Feedback 360.
- **Acesso**: Via `ListarColaboradoresOrgUseCase` (não chamar API direto na apresentação).

### 2.2 APIs Feedback 360 (Backend a Implementar)

- **Listar**: `GET /api/Feedback360/Listar`  
  - Headers: `Authorization: Bearer {token}`  
  - Sem query params na versão atual (listagem "meus feedbacks enviados" e, quando houver, "recebidos").  
  - Resposta: envelope `{ retorno: Feedback360Item[], sucesso, mensagem, erros? }`.

- **Criar**: `POST /api/Feedback360/Criar`  
  - Headers: `Authorization: Bearer {token}`, `Content-Type: application/json`  
  - Corpo: `Feedback360CreatePayload` (camelCase).  
  - Resposta: envelope `{ retorno: Feedback360Item, sucesso, mensagem, erros? }`.

- **Estado atual**: Sem API de Feedback 360 própria; persistência temporária apenas no front para testes de usabilidade (conforme escopo informado). Autocomplete de colaborador usa o endpoint acima.

### 2.3 Envelope Padrão de Resposta

- `retorno`: objeto ou array.
- `sucesso`: boolean.
- `mensagem`: string (opcional).
- `erros`: string[] | null (opcional).

### 2.4 Entidade e Payload (Domínio)

- **Feedback360Item**: id, codigoInternoColaborador, nomeColaborador, contexto, dataInteracao, nivel (1–5), comentario, criadoEm (string ISO).
- **Feedback360CreatePayload**: codigoInternoColaborador, nomeColaborador, contexto, dataInteracao, nivel, comentario (sem id/criadoEm).
- **Feedback360Nivel**: 1 | 2 | 3 | 4 | 5; labels e emojis em `FEEDBACK360_NIVEL_LABELS` e `FEEDBACK360_NIVEL_EMOJIS`.

---

## 3. Regras Técnicas

- **HTTP**: Todas as chamadas via `httpClient`; nenhum `fetch()`/`axios` direto.
- **Rastreabilidade**: Usar `logUserAction` (Firebase) para ações significativas do fluxo Feedback.
- **DI**: Feedback360Api, Feedback360Repository, ListarFeedback360UseCase, CriarFeedback360UseCase registrados no container TSyringe; hook resolve Use Cases com `container.resolve(...)`.
- **Colaboradores**: Resolvidos via `ListarColaboradoresOrgUseCase` no hook quando aba "inserir" está ativa; token e activeTab nas dependências.

---

## 4. Integrações e Dependências

- **Autenticação**: Token do Redux (`state.auth.token`).
- **Colaboradores**: `ListarColaboradoresOrgUseCase` → API Mapa de Alocação (ListarColaboradoresOrg).
- **Feedback 360**: Quando backend existir, `ListarFeedback360UseCase` e `CriarFeedback360UseCase` → `Feedback360Repository` → `Feedback360Api` → `httpClient`.

---

## 5. Estado Atual da Implementação (alinhado ao código)

- **Entidade e payload**: `Feedback360Item` e `Feedback360CreatePayload` **não** possuem `relacionamento` nem `tipoInteracao` (apenas contexto como texto livre).
- **Colaboradores**: Contrato `ColaboradorCch` possui `cd_Profissional`, `nm_Profissional`, `codigoColaboradorInterno`; **não** possui campo e-mail. A listagem de colaboradores é carregada uma vez (busca vazia, limite 50000) e o filtro é feito no cliente (Command); não há chamada à API com parâmetro `busca` durante a digitação (busca preditiva server-side).
- **APIs Feedback 360**: Listar e Criar referenciadas na doc; em ambiente sem backend a persistência é temporária no front para testes de usabilidade. Autocomplete usa `ListarColaboradoresOrg`.

---

## 6. Pontos de Atenção para Evolução

- **Abas Recebidos/Enviados**: Backend precisará de endpoint(s) ou parâmetros para "recebidos" vs "enviados".
- **Relacionamento e Tipo de Interação**: Entidade e payload atuais não possuem campos "relacionamento" (Colega, Liderança, Pares, Outro) nem "tipo de interação" categórico; serão necessários no contrato e na API.
- **Edição em 60 min**: Endpoints PUT e regra de janela de edição ainda não implementados.
- **Assunto no card**: Se "Assunto" for campo próprio, incluir na entidade e no payload.
- **Sinalizador em tempo real**: Possível evolução com WebSocket ou polling; não implementado hoje.
- **E-mail no autocomplete**: Para "exibir e-mail para evitar homônimos" (PBI 16005), verificar se a API ListarColaboradoresOrg retorna e-mail e incluir em `ColaboradorCch` e na UI.
