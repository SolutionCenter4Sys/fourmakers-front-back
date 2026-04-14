# Guia para documentação técnica de protótipos externos (autocontido)

Este documento é **autocontido**: não depende de outros arquivos do projeto. Use-o quando o protótipo foi criado **fora** do repositório (Firebase Studio, Lovable, Gemini Studio, FlutterFlow, Figma + código, etc.). Ao seguir este guia, a plataforma generativa produz uma documentação técnica no padrão esperado, permitindo que o time front/back valide e integre com esforço mínimo.

**Uso:** Envie este guia + a descrição da tela/fluxo do protótipo para a ferramenta de geração. O resultado deve ser um único arquivo Markdown pronto para ser salvo como documentação da feature e usado pelo backend .NET 8 e pelo front para integração.

---

## Parte A – Regras fixas de contratos (obrigatório em toda documentação gerada)

Toda documentação técnica gerada **deve** seguir estas regras. Inclua-as no próprio documento gerado (em uma seção “Padrões de contratos”) para o time validar.

### A.1 Nomenclatura em JSON e APIs

- **Sempre camelCase** em propriedades JSON (request e response).  
  Exemplos: `codigoInternoColaborador`, `dataCriacao`, `nomeColaborador`, `dataInteracao`, `orgId`.

- **Datas:**  
  - Data de criação do registro: propriedade **`dataCriacao`** (string em formato ISO 8601 ou YYYY-MM-DD).  
  - Data de última alteração (quando houver auditoria): **`dataAlteracao`**.  
  - Evitar nomes como `criadoEm`, `createdAt`; preferir **`dataCriacao`**.

- **Identificador de colaborador (pessoa/usuário):**  
  Nos contratos da feature (request/response), usar **`codigoInternoColaborador`** para o código único do colaborador.  
  Se o protótipo ou outra API usar outro nome (ex.: `codigoColaboradorInterno`), documentar que é o **mesmo valor** e que na integração o backend/front deve expor/mapear como **`codigoInternoColaborador`**.

- **Identificadores numéricos:**  
  Podem ser `id` (number ou string/GUID). Preferir tipo consistente em toda a feature (ex.: string para GUID).

### A.2 Envelope padrão de resposta HTTP (todas as APIs)

Toda resposta de API (listagem, criação, atualização, exclusão) deve seguir este formato:

```json
{
  "retorno": <objeto ou array conforme o endpoint>,
  "sucesso": true,
  "mensagem": null,
  "erros": null
}
```

- **retorno:** objeto (ex.: item criado) ou array (ex.: lista). Em erro, pode ser null ou omitido.
- **sucesso:** boolean. `false` em falha de validação ou regra de negócio.
- **mensagem:** string ou null. Mensagem geral (ex.: “Colaborador não encontrado”).
- **erros:** array de strings ou null. Lista de mensagens de validação (ex.: `["Campo X é obrigatório", "Data inválida"]`).

Em erro (4xx/5xx), retornar o mesmo envelope com `sucesso: false`, `mensagem` preenchida e, se aplicável, `erros` com a lista de falhas.

### A.3 Autenticação

- Assumir que as chamadas são autenticadas com **Bearer token** no header:  
  `Authorization: Bearer {token}`.  
- O backend obtém o usuário/colaborador a partir do token para associar “autor”, “responsável”, etc.

### A.4 Resumo da tabela de padrões (incluir no doc gerado)

| Aspecto              | Regra                                                                 |
|----------------------|-----------------------------------------------------------------------|
| Nomenclatura JSON    | camelCase                                                             |
| Envelope de resposta | `retorno`, `sucesso`, `mensagem`, `erros` (todos os endpoints)        |
| Data de criação      | `dataCriacao` (string ISO ou YYYY-MM-DD)                             |
| Data de alteração    | `dataAlteracao` (quando houver auditoria)                             |
| Código colaborador   | `codigoInternoColaborador` nos contratos da feature                   |
| Erros de validação   | `erros`: array de strings ou null no envelope                        |

---

## Parte B – Estrutura do documento a ser gerado

Gere um único arquivo Markdown com as seções abaixo. Preencha cada seção com base na descrição do protótipo/tela. Não invoque nomes de arquivos ou pastas do projeto; descreva tudo em termos de “a tela”, “a API”, “o backend”, “o front”.

### 1. Título e introdução

- Título: **`[Nome da Feature] – Documentação Técnica (Regras de Negócio e Backend)`**.
- Parágrafo curto: para que serve a documentação (suporte ao backend .NET 8 e à integração com o front), e se a feature já está integrada ou ainda é apenas protótipo/esperada.

### 2. Visão geral e objetivo

- **Objetivos principais (visão de produto):** Lista em tópicos do que a feature busca alcançar (ex.: centralizar processo, automatizar coleta, guiar o usuário passo a passo, dar visibilidade ao time, garantir conformidade). Use bullet points com verbos no infinitivo (centralizar, automatizar, guiar, fornecer, garantir).
- **Tabela resumida:**

| Item            | Descrição |
|-----------------|-----------|
| **Rota(s)**     | Ex.: `/minha-feature` ou múltiplas rotas se houver portal/painel separados. |
| **Título**      | Título exibido na tela. |
| **Descrição (UI)** | Frase curta que aparece na interface ou descreve a tela. |
| **Objetivo de negócio** | O que o usuário ou o negócio alcança com essa tela. |
| **Escopo atual** | O que está na primeira versão (ex.: apenas listar e criar; sem edição). E opcionalmente o que não está (ex.: sem exclusão, sem relatório). |

- **Personas (se aplicável):** Se a feature tiver mais de um tipo de usuário (ex.: equipe RH vs candidato), nomear cada um e qual experiência cada um tem (ex.: “Painel de controle para RH”; “Portal de autoatendimento para o candidato”).

### 3. Parâmetros de entrada e contexto

- **Autenticação:** Todas as chamadas usam Bearer token (header `Authorization`).
- **Parâmetros de rota ou query:** Listar se a tela recebe algo pela URL (ex.: `id`, `codigo`, `filtro`). Se não receber, informar “Nenhum”.
- **Dependências de outras APIs:** Se a tela precisar de dados de outra API (ex.: lista de colaboradores, lista de status), descrever: método, rota sugerida, parâmetros (query ou corpo). Não use nomes de arquivos; use “API de colaboradores”, “endpoint de listagem de X”, etc.

### 4. Padrões de contratos (consistência)

Reproduzir a **Parte A** de forma resumida: tabela com nomenclatura (camelCase), envelope (`retorno`, `sucesso`, `mensagem`, `erros`), `dataCriacao`/`dataAlteracao`, `codigoInternoColaborador`, e que os exemplos de API abaixo seguem essas regras.

### 5. Regras de negócio

- Para cada conceito importante da tela (ex.: “nível”, “status”, “quem pode fazer o quê”):
  - Descrever em tópicos ou tabelas.
  - Incluir obrigatoriedade condicional (ex.: “se campo A = X, então campo B é obrigatório”).
- Definir **quem** pode executar cada ação (ex.: usuário logado, por token) e **sobre o quê** (ex.: apenas itens da mesma organização).
- Se houver listagem: **o que** é listado (ex.: “apenas itens criados pelo usuário”) e se filtros são no cliente ou no servidor.
- **Regras de negócio críticas (em destaque):**
  - **Bloqueios de avanço:** Em que condições o usuário **não pode** avançar (ex.: “não pode ir para etapa X se os itens Y e Z não estiverem aprovados”). Documentar a regra e a mensagem ou estado visual que o usuário vê quando bloqueado.
  - **Obrigatoriedade de motivo:** Se alguma ação exige justificativa (ex.: reprovação, cancelamento), deixar explícito que o motivo é obrigatório e como é exibido para o outro lado (ex.: candidato vê o motivo da reprovação).
  - **Auditoria:** Quais ações devem gerar registro de log (usuário, ação, data/hora). Ex.: “Convite enviado por {usuário} em {data/hora}”; “Item aprovado/reprovado por {usuário}”.

### 6. Fluxos por persona (quando houver mais de um perfil)

Se a feature tiver perfis diferentes (ex.: RH vs candidato), descrever **um fluxo por persona** com subtítulos (ex.: “Fluxo do RH (Painel de Controle)”, “Experiência do Candidato (Portal)”). Para cada um: visão geral da experiência (ex.: “Kanban”, “fluxo linear passo a passo”) e depois os subfluxos e telas. Se houver apenas um perfil, manter uma única seção “Funcionalidades e experiência do usuário”.

### 7. Funcionalidades, experiência do usuário e eventos

- **Passo a passo por fluxo:** Para cada fluxo (ex.: “Ao abrir a tela”, “Ao submeter o formulário”, “Ao clicar em Enviar Convite”):
  - Descrever em passos numerados o que o usuário vê e faz.
- **Eventos e ações desencadeadas:** Para ações críticas (botão que muda estado, envia convite, aprova, reprova):
  - **Gatilho:** O que o usuário faz (ex.: clica em “Enviar Convite” no card).
  - **Confirmação:** Se houver modal de confirmação, indicar título e descrição (ex.: “Iniciar contratação e enviar convite” / “Ao enviar o convite, o candidato terá acesso ao portal…”).
  - **Ações desencadeadas:** Lista do que acontece em sequência (ex.: item move de estágio, portal é liberado, registro de auditoria é criado, toast de sucesso é exibido).
- **Estados da interface:** Documentar quando a tela ou uma seção está **bloqueada** (ex.: “Seção bloqueada com mensagem clara até o convite ser enviado”), **em carregamento** (ex.: “Lendo informações do documento…”), **vazia** ou **em erro**. Indicadores visuais (ex.: SLA verde/amarelo/vermelho, badges de status) devem ser descritos (o que cada estado significa).
- **Explicação de itens e módulos:** Para listas, checklists, abas e seções importantes: descrever o **conteúdo** de cada um (ex.: “Aba Documentos: checklist de documentos e dados necessários; status por item: Pendente, Em revisão, Aprovado, Reprovado; botão Ver para itens Em revisão”). Incluir labels, tooltips ou mensagens que o usuário vê (ex.: “Documento oficial com foto”, “Precisa de ajuste”).
- Mencionar filtros, abas, modais, mensagens de sucesso/erro (toast, alerta). Não referenciar componentes ou arquivos; descrever apenas comportamento e dados.

### 8. Regras de sucesso, erro e bloqueios (UX)

- **Sucesso:** O que deve acontecer quando a ação é concluída com sucesso (ex.: mensagem exibida, redirecionamento, atualização da lista, item muda de status). Documentar texto de sucesso (toast, título do modal) quando relevante.
- **Erro:** Para cada cenário de falha (validação, rede, regra de negócio), indicar o que o **usuário vê** (mensagem, estado da tela) e o que ele pode fazer (ex.: corrigir campo, tentar novamente). Sugerir mensagens amigáveis para o backend retornar em `mensagem` ou `erros`.
- **Bloqueios:** Listar situações em que o usuário **não pode** prosseguir (ex.: “Não pode assinar contrato se documentos X e Y não estiverem aprovados”). Para cada bloqueio: condição, o que a tela mostra (mensagem, botão desabilitado, seção bloqueada) e, se aplicável, o que desbloqueia.

### 9. Linguagem e tom (visão de produto)

- **Orientações para mensagens ao usuário:** Tom positivo e simples; evitar jargões (ex.: preferir “Precisa de ajuste” a “Reprovado”; explicar termos técnicos em linguagem clara). Se a feature for para usuários não técnicos (ex.: candidato), indicar que termos como siglas de RH ou técnicos devem ser evitados ou explicados.
- **Exemplos:** Listar frases ou termos que a interface deve usar (ex.: “Começar”, “Confirmar Informações”, “Lendo informações do seu documento…”) e, se aplicável, o que evitar.

### 10. APIs necessárias (backend .NET 8)

Para **cada** endpoint que a tela precisa:

- **Método e rota:** ex. `GET /api/Recurso/Listar`, `POST /api/Recurso/Criar`.
- **Headers:** `Authorization: Bearer {token}`; se POST/PUT, `Content-Type: application/json`.
- **Parâmetros:** query (para GET) ou corpo (para POST/PUT), com nome e tipo.
- **Exemplo de resposta (200):** JSON completo com o **envelope** (`retorno`, `sucesso`, `mensagem`, `erros`) e o conteúdo de `retorno` já no padrão (camelCase, `dataCriacao`, `codigoInternoColaborador`, etc.).
- **Contrato sugerido (C#):** nomes de classes/DTOs (ex.: `RecursoListResponse`, `RecursoItemDto`, `RecursoCreateRequest`) e propriedades em PascalCase (ex.: `DataCriacao`, `CodigoInternoColaborador`), alinhadas ao JSON.

Se a tela tiver criação, edição, exclusão ou listagem com filtros, descrever um endpoint para cada operação.

**Serviços/operações em abstração (quando aplicável):** Para operações que vão além de CRUD (ex.: OCR, confirmação por IA, processamento assíncrono), descrever em formato: **Nome do serviço** — *Entrada:* (parâmetros); *Saída:* (tipo ou objeto esperado); *Lógica:* uma frase (ex.: “Executa OCR no arquivo e retorna dados estruturados”). Isso ajuda o backend a definir contratos e integrações sem fixar rota no documento.

### 11. Modelos de dados (contratos)

- **Tabela da entidade / DTO principal:** para cada propriedade: nome (camelCase no JSON), tipo, se é obrigatória, descrição curta. Incluir sempre `dataCriacao` (e `dataAlteracao` se houver auditoria) e `codigoInternoColaborador` quando for caso de colaborador.
- **Payload de criação (e edição, se houver):** mesma tabela para o corpo do request.
- **Envelope de respostas:** repetir a regra: toda resposta tem `retorno`, `sucesso`, `mensagem`, `erros`.

### 12. Dependências de APIs e dados existentes

- Listar quais **dados** a feature depende (ex.: “lista de colaboradores”, “lista de status”). Descrever de forma genérica (ex.: “API que lista pessoas da organização”); não é obrigatório citar rota se for externa/indefinida.
- Se o protótipo usar algum identificador (ex.: código de colaborador), explicar que o backend deve validar contra a mesma fonte (ex.: mesma base de colaboradores) para consistência.
- Mencionar se a listagem ou a criação depende de “usuário do token” (autor, responsável) e como o backend deve persistir isso.

### 13. Fluxo resumido (backend)

Para cada operação principal (ex.: Listar, Criar):

- Passos em texto ou lista numerada: obter usuário do token → validar parâmetros → (opcional) validar regras de negócio → persistir ou buscar dados → retornar resposta no envelope padrão.

### 14. Propostas de melhorias (evolução da feature)

- Filtros no servidor (em vez de só no cliente).
- Paginação (cursor/limite ou página/tamanho).
- Edição e exclusão, se não existirem na primeira versão.
- Auditoria (quem criou, quem alterou, `dataAlteracao`).
- Outras evoluções desejáveis (ex.: exportar, notificações).

### 15. Cenários de erro e pontos de atenção

- **Erros possíveis:** lista de situações que o backend deve tratar (ex.: “registro não encontrado”, “campo obrigatório ausente”, “usuário sem permissão”, “data futura inválida”). Para cada um, sugerir mensagem em `mensagem` ou em `erros`.
- **Impacto em desenvolvimento:** se aplicável, sugerir nomes de entidades/tabelas (genéricos, ex.: “Recurso”, “RecursoHistorico”) e relacionamentos (ex.: “autor do registro”, “colaborador destinatário”). Mencionar segurança (ex.: listar apenas itens do usuário do token).

### 16. Resumo para o time

- Uma frase descrevendo a feature.
- Lista de APIs a implementar (método e rota).
- Lista de APIs já existentes ou externas que a tela usa (quando aplicável).
- Contratos em uma linha (envelope + principais campos).
- Pontos de atenção (validações, segurança, dependências).

### 17. Rodapé

- Frase: *“Documento gerado para integração com backend .NET 8 e front. Última atualização: [data].”*

---

## Parte C – Instruções para a ferramenta generativa

Ao receber este guia junto com a descrição do protótipo:

1. **Entrada:** O usuário fornecerá uma descrição da tela ou do fluxo (texto, capturas, ou link). Pode incluir: nome da feature, rota, formulários, listagens, filtros, modais, ações (criar, editar, excluir, listar).
2. **Regras:** Aplique **sempre** as regras da Parte A (camelCase, envelope com `retorno`/`sucesso`/`mensagem`/`erros`, `dataCriacao`/`dataAlteracao`, `codigoInternoColaborador`). Não use `createdAt`, `criadoEm`, ou nomes em snake_case no JSON.
3. **Estrutura:** Gere um único documento Markdown seguindo a Parte B, na ordem das seções 1 a 17. Adapte subseções ao contexto (ex.: “Aba Inserir” e “Aba Consultar” só se a tela tiver abas). As seções 6 a 9 (fluxos por persona, eventos, regras de sucesso/erro/bloqueios, linguagem e tom) tornam a documentação mais rica em visão de produto e UX; preencha-as sempre que a descrição do protótipo permitir.
4. **Conteúdo:** Preencha cada seção com base na descrição do protótipo. Se algo não estiver definido (ex.: rota exata), use um nome genérico e deixe indicado que pode ser ajustado na integração.
5. **Sem referências internas:** Não inclua caminhos de arquivos, nomes de pastas ou referências a documentos do repositório. Tudo deve ser compreensível apenas com este guia e a descrição do protótipo.
6. **Output:** O resultado deve ser um arquivo Markdown completo, pronto para ser copiado, salvo como `[NOME_DA_FEATURE]_DOCUMENTACAO_TECNICA.md` e usado pelo time de front e back para validar e implementar a integração com esforço mínimo.

---

*Este guia é autocontido e não referencia outros arquivos do projeto. Pode ser usado em ambientes externos (Firebase Studio, Lovable, Gemini Studio, etc.) para gerar documentação técnica alinhada ao padrão de contratos e à estrutura esperada pelo time.*
