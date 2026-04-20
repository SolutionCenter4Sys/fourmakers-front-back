# Mapa de Relacionamento – Documentação Técnica (Regras de Negócio e Backend)

Documentação técnica da página **Mapa de Relacionamento** (`/mapa-relacionamento`) para suporte ao desenvolvimento do backend em .NET 8 e à integração com o frontend React. A feature está **totalmente integrada com backend** através de múltiplas APIs do módulo Organograma e VCX.

**Última atualização:** 27 de Fevereiro de 2026

---

## 1. Visão Geral e Objetivo

### Objetivos Principais (Visão de Produto)

O Mapa de Relacionamento é uma ferramenta estratégica que transforma a gestão organizacional de um processo burocrático e fragmentado em uma **ferramenta centralizada**. Os principais objetivos são:

- **Centralizar** toda a estrutura organizacional em uma única fonte de verdade
- **Automatizar** a construção de hierarquias organizacionais a partir de dados reais
- **Fornecer visibilidade** completa da estrutura através de visualizações interativas (árvore e lista)
- **Garantir rastreabilidade** de posições, alocações, departamentos e perfis
- **Permitir gestão ágil** com criação, edição e reorganização de estruturas via drag-and-drop
- **Conectar pessoas ao contexto** através do Painel VCX 360° (Dores, Iniciativas, Histórico)

| Item | Descrição |
|------|------------|
| **Rota** | `/mapa-relacionamento` |
| **Título** | Mapa de Relacionamento |
| **Descrição (UI)** | Visualize e gerencie a estrutura organizacional hierárquica |
| **Objetivo de negócio** | Hub central que consolida estrutura organizacional completa, permitindo visualização dinâmica e gestão interativa de posições, departamentos, alocações e contexto VCX (dores, iniciativas) |
| **Escopo atual** | Visualização em árvore/lista, CRUD de posições/departamentos/alocações, filtro C-Levels, drag-and-drop para reorganização, Painel VCX 360° com Dores, Iniciativas e Histórico |

### Personas

**Usuário Principal:** Gestores de RH, Líderes de Departamento, Executivos (C-Level)

**Experiência esperada:**
- Visualizar toda a estrutura organizacional de um cliente
- Criar e editar posições e departamentos rapidamente
- Associar colaboradores a posições (alocações)
- Reorganizar hierarquia via drag-and-drop intuitivo
- Identificar e gerenciar contexto VCX (dores e iniciativas) por posição
- Acompanhar histórico de alterações

---

## 2. Parâmetros de Entrada e Contexto

### Autenticação
- Todas as chamadas usam o **token** do usuário logado (Bearer), obtido do estado de autenticação (Redux)
- O **orgId** é extraído de `user.colaboradorOrg.orgId`
- Validações de permissões (ex.: criar departamento) baseadas em `user.isGestor`

### Parâmetros de Rota e Query
- **Sem parâmetros de rota:** A página não recebe `id`, `codigo` ou filtros via URL
- **Cliente:** Selecionado via dropdown após carregar a página
- **Persistência:** Cliente selecionado e modo de visualização persistidos em `localStorage`

### Dependências de APIs Existentes

| API | Endpoint | Uso |
|-----|----------|-----|
| **ListarColaboradoresOrg** | `GET /api/MapaDeAlocacao/ListarColaboradoresOrg` | Lista colaboradores para seleção no modal de edição (busca, cursor, limite) |
| **BuscarPerfisPorOrg** | `GET /api/Organograma/PerfilCorporativoListarPorOrg` | Lista perfis corporativos para seleção no modal |
| **ListarDepartamentos** | `GET /api/Organograma/Organograma/DepartamentoListarPorCliente` | Lista departamentos do cliente para dropdown |

---

## 3. Padrões de Contratos do Projeto (Consistência)

Para manter consistência com as demais APIs e entidades do projeto:

| Aspecto | Padrão | Exemplo no Projeto |
|---------|--------|--------------------|
| **Nomenclatura JSON** | camelCase | `codigoInternoColaborador`, `dataCriacao`, `organogramaPosicaoId` |
| **Envelope de resposta** | `retorno`, `sucesso`, `mensagem`, `erros?` | Todas as APIs de listagem/criação/atualização |
| **Data de criação** | `dataCriacao` (string ISO YYYY-MM-DD ou ISO 8601) | VcxDores, VcxIniciativas, Organograma |
| **Data de alteração** | `dataAlteracao` (string ISO 8601) | VcxHistorico, VcxDores, VcxIniciativas |
| **Identificador de colaborador** | `codigoInternoColaborador` nos contratos de domínio | Request/response VCX; nota: `ListarColaboradoresOrg` retorna `codigoColaboradorInterno` |
| **Identificador de posição** | `organogramaPosicaoId` (string GUID) | Todas as APIs de Organograma e VCX |
| **Identificador de departamento** | `organogramaDepartamentoId` (string GUID) | APIs de Departamento |
| **Erros de validação** | `erros?: string[] \| null` no envelope | Respostas de erro ou validação |

---

## 4. Regras de Negócio

### 4.1 Estrutura Hierárquica e Relacionamentos

#### Hierarquia de Posições
- A hierarquia é determinada pelo campo `organogramaPosicaoIdSuperior` (FK para a posição pai)
- Posições sem `organogramaPosicaoIdSuperior` (ou valor `null`) são raízes
- **Múltiplas raízes:** Sistema cria nó virtual `root-virtual` para agregar múltiplas raízes lado a lado
- **Validação crítica:** Não é permitido criar ciclos (mover nó para seus próprios descendentes)

#### Posições Vagas
Uma posição é considerada "Vaga" quando:
1. Não há alocações (`alocacoes: []` ou `null`)
2. Alocação existe mas `nomeColaborador` é `null`, `undefined` ou string vazia
3. Alocação existe mas `codigoInternoColaborador` é `null`, `undefined` ou string vazia
4. `codigoInternoColaborador` é o GUID vazio: `'00000000-0000-0000-0000-000000000000'`

**Tratamento:**
```typescript
const employeeName = hasValidAlocacao ? alocacaoAtiva.nomeColaborador : 'Vago'
const employeeId = hasValidAlocacao ? alocacaoAtiva.codigoInternoColaborador : 'vacant'
```

### 4.2 Lógica Central (Flags de Controle)

O sistema utiliza **flags de controle** para determinar o fluxo de operações ao salvar uma posição:

| Flag | Condição | Significado |
|------|----------|-------------|
| `modalCriacao` | `node.posicaoId === null` ou `undefined` | `true` = criar nova posição; `false` = editar existente |
| `criouDepartamento` | Campo "Novo Departamento" preenchido **E** `isGestor === true` | `true` = criar novo departamento |
| `alterouColaborador` | Colaborador selecionado diferente do original **E** não é "Vago" | `true` = criar/atualizar alocação |
| `criouPerfil` | Campo de perfil manual preenchido | `false` atualmente (não implementado) |

### 4.3 Filtro C-Level

O filtro C-Level funciona baseado **exclusivamente** no campo booleano `isCLevel` retornado pelo backend, **não** na posição hierárquica do nó.

**Lógica de filtro:**
- **Coleta recursiva**: Percorre toda a árvore coletando todos os nós onde `isCLevel === true`
- **Promoção de C-Levels**: C-Levels filhos de não-C-Levels são "promovidos" na estrutura filtrada
- **Garantia**: EM NENHUM CASO um C-Level deve estar escondido quando o filtro está ativo
- **Reset automático**: Filtro desativado (`mostrarApenasCLevels = false`) quando:
  - Estrutura é recarregada (`recarregarEstrutura`)
  - Novo cliente é selecionado (`handleSelecionarCliente`)

**Implementação:** `src/domain/services/mapaRelacionamentoTreeService.ts` - função `filtrarCLevelsMapa()`

**Edge Cases tratados:**
- Filtro sem C-Levels encontrados: retorna `null` (UI exibe "Estrutura vazia")
- Nó virtual (root-virtual): mantido como container, seus filhos são filtrados
- 1 C-Level encontrado: retorna direto (sem nó virtual)
- Múltiplos C-Levels: cria nó virtual para agregar

### 4.4 Validações e Restrições

| Validação | Condição | Comportamento | Mensagem de Erro |
|-----------|----------|---------------|------------------|
| **Criar Departamento sem Permissão** | `isGestor === false` E campo "Novo Departamento" preenchido | Campo desabilitado, `criouDepartamento = false` | Campo desabilitado (não permite digitar) |
| **Mover Nó sem posicaoId** | `origem.posicaoId === null` ou `undefined` | Bloqueia drag-and-drop | "Salve o nó antes de movê-lo" |
| **Mover para Descendente** | `destino` é descendente de `origem` | Bloqueia drop | "Não é possível mover para um descendente" |
| **Mover para Si Mesmo** | `origem.id === destino.id` | Bloqueia drop | "Não é possível mover um nó para si mesmo" |
| **Criar Alocação com "Vago"** | `colaboradorNovo === 'vacant'` | Não cria alocação | Nenhuma (comportamento silencioso) |
| **Editar Departamento sem isGestor** | `isGestor === false` E tentativa de criar/editar departamento | Campo oculto/desabilitado | Campo não exibido ou desabilitado |
| **Token expirado** | `!token` | Bloqueia todas as operações | Toast: "Erro: usuário ou cliente não selecionado" |
| **Cliente não selecionado** | `!clienteSelecionado` | Bloqueia salvamento | Toast: "Erro: usuário ou cliente não selecionado" |
| **orgId ausente** | `!user.colaboradorOrg?.orgId` | Bloqueia salvamento | Toast: "Erro: orgId não encontrado no usuário" |
| **Salvar posição sem perfil** | `!profileId || profileId.trim() === ''` | Bloqueia salvamento no modal | Toast: "Erro: Todos os campos obrigatórios devem ser preenchidos" |
| **Dor sem Impacto ou Urgência** | `!vcxUrgenciasDescricao || !vcxImpactosDescricao` | Bloqueia gravação | "Selecione uma opção válida para Prioridade e Impacto" |
| **Dor sem título** | `!formData.title?.trim()` | Bloqueia gravação | "O título é obrigatório" |
| **Dor sem descrição** | `!formData.description?.trim()` | Bloqueia gravação | "A descrição é obrigatória" |
| **Iniciativa sem título, tema ou Objetivo/KPI** | `!titulo?.trim() || !objetivoKpi?.trim() || !temaInput.trim()` | Bloqueia gravação | Botão Gravar desabilitado até preencher |

### 4.5 Matriz de Decisão: Alocação

| Estado Original | Estado Novo | Ação | Endpoint |
|-----------------|-------------|------|----------|
| Sem alocação (`originalId === null`) | Colaborador selecionado (`novoId !== 'vacant'`) | **Criar** nova alocação | `POST /api/Organograma/Organograma/AlocacaoInserir` |
| Sem alocação (`originalId === null`) | "Vago" (`novoId === 'vacant'`) | **Nenhuma ação** | - |
| Com alocação (`originalId` existe) | Colaborador diferente (`novoId !== originalId`) | **Atualizar** alocação existente | `POST /api/Organograma/Organograma/AlocacaoAtualizar` |
| Com alocação (`originalId` existe) | "Vago" (`novoId === 'vacant'`) | **Deletar** alocação existente | `DELETE /api/Organograma/Organograma/AlocacaoDeletar` |
| Com alocação (`originalId` existe) | Mesmo colaborador (`novoId === originalId`) | **Nenhuma ação** | - |

### 4.6 Matriz de Decisão: Departamento

| Estado Original | Ação do Usuário | Flag | Ação do Sistema | Endpoint |
|-----------------|-----------------|------|-----------------|----------|
| Sem departamento | Digitar nome novo (isGestor) | `foiCriado === true` | **Criar** novo departamento | `POST /api/Organograma/Organograma/DepartamentoInserir` |
| Sem departamento | Selecionar existente | `foiEditado === true` | **Atualizar** posição com departamento | `POST /api/Organograma/Organograma/PosicaoAtualizar` |
| Sem departamento | Não selecionar | - | **Manter** `organogramaDepartamentoId = null` | - |
| Com departamento A | Digitar nome novo (isGestor) | `foiCriado === true` | **Criar** novo departamento B | `POST /api/Organograma/Organograma/DepartamentoInserir` |
| Com departamento A | Selecionar departamento B | `foiEditado === true` | **Atualizar** posição para departamento B<br>**Atualizar** departamento B (se isGestor) | `POST PosicaoAtualizar` → `POST DepartamentoAtualizar` |
| Com departamento A | Editar nome via modal | - | **Atualizar** departamento A | `POST /api/Organograma/Organograma/DepartamentoAtualizar` |
| Com departamento A | Remover seleção | - | **Atualizar** posição com `organogramaDepartamentoId = null` | `POST /api/Organograma/Organograma/PosicaoAtualizar` |

**Nota sobre Pré-preenchimento:** Após editar um departamento, o campo de departamento no modal de edição é automaticamente atualizado. Isso é garantido através de `ultimoDepartamentoEditado` que previne que o `useEffect` sobrescreva o valor editado.

### 4.7 VCX - Dores e Iniciativas

#### Dores (VCX)
- **Obrigatoriedade:** Título, Descrição, Impacto e Urgência obrigatórios
- **Ordenação:** Por `dataCriacao` descendente (mais recente primeiro)
- **Datas inválidas:** Itens com `dataCriacao` inválida (`null`, `undefined`, string inválida) são colocados no final
- **Operações:** Criar, Editar, Deletar, Reordenar (drag-and-drop local, sem persistência de ordem)

#### Iniciativas (VCX)
- **Obrigatoriedade:** Título, Tema e Objetivo/KPI obrigatórios; Status opcional
- **Tema:** Autocomplete com criação dinâmica (se tema não existe, é criado via `POST CriarTema`). O campo Tema é obrigatório no frontend (deve ser preenchido ou selecionado)
- **Ordenação:** Por `dataCriacao` descendente (mais recente primeiro)
- **Datas inválidas:** Tratamento idêntico às Dores
- **Operações:** Criar, Editar, Deletar, Reordenar (drag-and-drop local)

### 4.8 Profissional Externo

O campo `isExternal` (Profissional Externo) está **sempre `true`** e **sempre inativo** (disabled) no modal de edição.
- Switch sempre marcado e desabilitado
- Container com `opacity-60` para indicar visualmente que está inativo
- Valor sempre salvo como `true` no backend

---

## 5. Fluxos por Persona

### 5.1 Fluxo Principal: Gestor/Líder

#### Visualizar e Gerenciar Estrutura
1. Usuário acessa `/mapa-relacionamento`
2. Seleciona um cliente no seletor de clientes
3. Sistema carrega estrutura organizacional do cliente
4. Visualiza em modo Diagrama (árvore) ou Lista
5. Pode filtrar apenas C-Levels
6. Pode criar, editar, mover e deletar posições
7. Pode exportar estrutura como JSON

#### Criar Nova Posição
1. Clica em "Adicionar Filho" em um nó existente OU "Adicionar Primeira Posição"
2. Modal de edição abre
3. Preenche:
   - Nome do Perfil/Posição (texto livre ou seleção de perfil existente)
   - Departamento (seleciona existente ou cria novo - apenas gestores)
   - Colaborador/Profissional (seleciona ou deixa "Vago")
   - Flags: C-Level (checkbox), Profissional Externo (sempre true e inativo)
4. Clica em "Salvar"
5. Sistema valida e executa sequência de endpoints
6. Estrutura recarregada e atualizada na visualização

#### Mover Posição (Drag-and-Drop)
1. Clica e segura ícone de grip em um card
2. Arrasta para outro card (novo pai) ou para zona de raiz
3. Sistema valida:
   - Nó origem tem `posicaoId` válido
   - Destino não é descendente da origem
   - Destino não é a própria origem
4. Se válido, atualiza `organogramaPosicaoIdSuperior` no backend
5. Estrutura recarregada

#### Gerenciar Contexto VCX
1. Clica no ícone Eye em um card
2. Painel VCX 360° abre (384px fixo à direita)
3. Navega entre abas: Contexto, Agenda (simplificada), Histórico
4. Na aba Contexto:
   - **Dores:** Adiciona, edita, deleta dores do profissional
   - **Iniciativas:** Adiciona, edita, deleta iniciativas estratégicas
5. Na aba Histórico: Visualiza logs de alterações

---

## 6. Funcionalidades, Experiência do Usuário e Eventos

### 6.1 Carregar Página

**Gatilho:** Acesso à rota `/mapa-relacionamento`

**Ações desencadeadas:**
1. Carregar lista de clientes (`GET RetornarClientesPorOrgId`)
2. Verificar `localStorage` para cliente e modo salvos
3. Se cliente salvo existe, carregar estrutura automaticamente
4. Exibir hero com seletor de cliente se nenhum cliente selecionado

**Estados da interface:**
- **Loading:** `MapaRelacionamentoLoadingState` com Spinner centralizado
- **Vazio:** Hero com mensagem "Comece o Mapeamento"
- **Erro:** Alert com mensagem de erro e botão para tentar novamente

### 6.2 Selecionar Cliente

**Gatilho:** Seleção no dropdown de clientes

**Ações desencadeadas:**
1. Salvar cliente em Redux (`setClienteSelecionado`)
2. Salvar em `localStorage` (`fourmakers_mapa_rel_data_${codigoCliente}`)
3. Carregar estrutura organizacional (`GET OrganogramaCompletoPorCliente`)
4. Converter estrutura para formato de árvore (`converterPosicoesParaArvore`)
5. Firebase Analytics: `logUserAction('MapaRelacionamento', 'SelecionarCliente')`
6. Resetar filtro C-Levels automaticamente
7. Carregar listas de perfis e departamentos

**Confirmação:** Nenhuma (ação imediata)

**Efeitos:**
- Hero desaparece
- Toolbar aparece
- Visualização carrega (árvore ou lista)
- `isLoadingEstrutura = true` durante carregamento

### 6.3 Criar Posição

**Gatilho:** Botão "Adicionar Filho" ou "Adicionar Primeira Posição"

**Ações desencadeadas:**
1. Abre modal de edição (`EditModalMapa`)
2. Modal em modo criação (`modalCriacao = true`)
3. Campos vazios para preenchimento
4. Ao salvar, executa sequência de endpoints (ver seção 4.8)
5. Firebase Analytics: `logUserAction('MapaRelacionamento', 'CriarPosicao')`
6. Recarrega estrutura completa

**Confirmação:** Botão "Salvar" no modal

**Efeitos:**
- Toast de sucesso: "Posição criada com sucesso"
- Modal fecha automaticamente
- Estrutura recarregada
- Novo nó visível na árvore/lista

### 6.4 Editar Posição

**Gatilho:** Ícone Edit no card OU selecionar nó e clicar "Editar"

**Ações desencadeadas:**
1. Abre modal com dados pré-preenchidos
2. Modal em modo edição (`modalCriacao = false`)
3. Ao salvar, executa sequência de endpoints conforme flags
4. Firebase Analytics: `logUserAction('MapaRelacionamento', 'EditarPosicao')`
5. Recarrega estrutura

**Confirmação:** Botão "Salvar" no modal

**Efeitos:**
- Toast de sucesso: "Posição atualizada com sucesso"
- Modal fecha
- Estrutura recarregada
- Alterações visíveis

### 6.5 Mover Posição

**Gatilho:** Drag-and-drop do ícone de grip

**Ações desencadeadas:**
1. Validações de hierarquia (não permite ciclos)
2. Atualiza `organogramaPosicaoIdSuperior` via `POST PosicaoAtualizar`
3. Firebase Analytics: `logUserAction('MapaRelacionamento', 'MoverPosicao')`
4. Recarrega estrutura

**Confirmação:** Nenhuma (ação imediata ao drop)

**Efeitos:**
- Visual feedback durante drag (ghost card)
- Toast de sucesso: "Posição movida com sucesso"
- Estrutura atualizada com nova hierarquia

### 6.6 Deletar Posição

**Gatilho:** Ícone Trash no card

**Ações desencadeadas:**
1. Abre `AlertDialog` de confirmação
2. Exibe mensagem: "Tem certeza que deseja deletar esta posição?"
3. Se confirmar: `DELETE PosicaoDeletar?organogramaPosicaoId={guid}`
4. Firebase Analytics: `logUserAction('MapaRelacionamento', 'DeletarPosicao')`
5. Recarrega estrutura

**Confirmação:** Modal de confirmação com botões "Cancelar" e "Deletar"

**Efeitos:**
- Toast de sucesso: "Posição deletada com sucesso"
- Nó removido da visualização
- Descendentes podem ser órfãos ou removidos (depende do backend)

### 6.7 Gerenciar Dores (Aba Contexto)

**Gatilho:** Clicar ícone Eye em card → Aba Contexto → Seção "DORES DO PROFISSIONAL NO DEPARTAMENTO"

#### Adicionar Dor
1. Clica botão `+` no cabeçalho da seção
2. Formulário de criação expande
3. Preenche: Título (obrigatório), Descrição (obrigatória), Impacto (obrigatório), Urgência (obrigatória)
4. Clica "GRAVAR"
5. `POST /api/MapaDeRelacionamento/VCX/CriarDor`
6. Firebase Analytics: `logUserAction('MapaRelacionamento', 'CriarDor')`
7. Lista atualizada via Redux

**Toast sucesso:** "Dor criada com sucesso"

#### Editar Dor
1. Hover sobre card de dor → ícone Edit aparece
2. Clica ícone Edit
3. Formulário de edição expande com dados pré-preenchidos
4. Modifica campos
5. Clica "GRAVAR"
6. `PUT /api/MapaDeRelacionamento/VCX/AtualizarDor`
7. Firebase Analytics: `logUserAction('MapaRelacionamento', 'AtualizarDor')`
8. Lista atualizada

**Toast sucesso:** "Dor atualizada com sucesso"

#### Deletar Dor
1. Hover sobre card → ícone Trash aparece
2. Clica ícone Trash
3. `DELETE /api/MapaDeRelacionamento/VCX/ExcluirDor?id={guid}`
4. Firebase Analytics: `logUserAction('MapaRelacionamento', 'ExcluirDor')`
5. Card removido da lista

**Toast sucesso:** "Dor excluída com sucesso"

### 6.8 Gerenciar Iniciativas (Aba Contexto)

**Gatilho:** Mesma aba Contexto, seção "INICIATIVAS ESTRATÉGICAS" (abaixo das Dores)

#### Adicionar Iniciativa
1. Clica botão `+` no cabeçalho da seção
2. Formulário de criação expande
3. Preenche: Título (obrigatório), Tema (obrigatório – autocomplete, criado dinamicamente se não existir), Objetivo/KPI (obrigatório), Status (opcional)
4. Se tema não existe, sistema cria automaticamente via `POST CriarTema`
5. Clica "GRAVAR"
6. `POST /api/MapaDeRelacionamento/VCX/CriarIniciativa`
7. Firebase Analytics: `logUserAction('MapaRelacionamento', 'CriarIniciativa', { temaDescricao })`
8. Lista atualizada

**Toast sucesso:** "Iniciativa criada com sucesso"

#### Editar/Deletar Iniciativa
Fluxo idêntico às Dores (endpoints e analytics diferentes)

**Toast sucesso editar:** "Iniciativa atualizada com sucesso"  
**Toast sucesso deletar:** "Iniciativa excluída com sucesso"

### 6.9 Visualizar Histórico

**Gatilho:** Aba Histórico no Painel VCX 360°

**Ações desencadeadas:**
1. `GET /api/MapaDeRelacionamento/VCX/ListarHistoricoDoresIniciativasPorPosicaoId?posicaoId={guid}`
2. Mescla `doresLog` e `iniciativasLog` em lista única
3. Ordena por `dataAlteracao` descendente
4. Renderiza cards de log com:
   - Badge de ação (INSERT/UPDATE/DELETE)
   - Tipo (Dor ou Iniciativa)
   - Título do item
   - Data formatada: "dd 'de' MMMM 'de' yyyy 'às' HH:mm"
   - Para UPDATE: exibe "Antes" e "Depois"

**Estados:**
- **Loading:** Spinner + "Carregando histórico..."
- **Vazio:** "Nenhum histórico disponível"
- **Erro:** Alert com mensagem de erro

---

## 7. Regras de Sucesso, Erro e Bloqueios

### 7.1 Sucesso

| Operação | O que acontece | Toast | Redirecionamento |
|----------|----------------|-------|------------------|
| **Criar Posição** | Posição criada no backend, estrutura recarregada, modal fecha | "Posição criada com sucesso" | Não |
| **Editar Posição** | Posição atualizada, estrutura recarregada, modal fecha | "Posição atualizada com sucesso" | Não |
| **Mover Posição** | Hierarquia atualizada, estrutura recarregada | "Posição movida com sucesso" | Não |
| **Deletar Posição** | Posição removida, estrutura recarregada | "Posição deletada com sucesso" | Não |
| **Criar Departamento** | Departamento criado, lista de departamentos recarregada | "Departamento criado com sucesso" | Não |
| **Criar Dor** | Dor adicionada ao Redux, lista atualizada | "Dor criada com sucesso" | Não |
| **Criar Iniciativa** | Iniciativa adicionada, tema criado se necessário | "Iniciativa criada com sucesso" | Não |

### 7.2 Erro

| Cenário de Falha | O que o usuário vê | O que pode fazer | Mensagem Sugerida |
|------------------|---------------------|------------------|-------------------|
| **Token expirado** | Toast de erro | Fazer login novamente | "Sessão expirada. Por favor, faça login novamente." |
| **Cliente não selecionado** | Toast de erro | Selecionar cliente | "Selecione um cliente para continuar." |
| **Erro ao carregar estrutura** | Alert com mensagem | Tentar novamente, voltar para seleção | "Erro ao carregar estrutura organizacional. Tente novamente." |
| **Erro ao salvar posição** | Toast de erro, modal permanece aberto | Corrigir campos, tentar novamente | "Erro ao salvar posição: {mensagem do backend}" |
| **Departamento duplicado** | Toast com mensagem do backend | Escolher nome diferente | "Já existe um departamento com este nome." |
| **Campo obrigatório vazio** | Mensagem de erro no campo | Preencher campo | "Este campo é obrigatório." |
| **Erro de rede** | Toast de erro, estado anterior mantido | Verificar conexão, tentar novamente | "Erro de conexão. Verifique sua internet e tente novamente." |
| **Erro ao carregar dados VCX** | Alert na aba, lista vazia | Tentar reabrir painel, recarregar página | "Erro ao carregar dores/iniciativas. Tente novamente." |
| **Validação de hierarquia** | Toast de erro, drop bloqueado | Escolher destino válido | "Não é possível mover para um descendente." |

### 7.3 Bloqueios

| Condição | O que a tela mostra | O que desbloqueia |
|----------|---------------------|-------------------|
| **Sem cliente selecionado** | Hero com seletor de cliente, toolbar oculta | Selecionar um cliente |
| **Sem permissão para criar departamento** | Campo "Novo Departamento" oculto ou desabilitado | Ter `isGestor === true` |
| **Posição sem posicaoId** | Drag-and-drop desabilitado | Salvar posição primeiro |
| **Sem token** | Todas as operações bloqueadas, mensagem de erro | Fazer login |
| **Estrutura vazia (sem C-Levels no filtro)** | Mensagem "Estrutura vazia", sugestão de desativar filtro | Desativar filtro C-Levels |

---

## 8. Linguagem e Tom (Visão de Produto)

### Orientações para Mensagens

**Tom:** Profissional, direto e amigável

**Princípios:**
- **Clareza:** Mensagens devem explicar claramente o que aconteceu/acontecerá
- **Ação:** Sempre indicar o que o usuário pode fazer
- **Positivo:** Usar linguagem positiva mesmo em erros

**Exemplos de mensagens recomendadas:**

✅ **Boas:**
- "Posição criada com sucesso"
- "Erro ao carregar estrutura. Tente novamente."
- "Selecione um cliente para visualizar a estrutura."
- "Não é possível mover para um descendente."

❌ **Evitar:**
- "Erro 500" (muito técnico)
- "Falha na requisição HTTP" (jargão técnico)
- "Operação inválida" (vago, sem contexto)

**Termos do domínio:**
- **Posição** (não "cargo" ou "função")
- **C-Level** (não "executivo" ou "liderança sênior")
- **Vago** (não "sem colaborador" ou "disponível")
- **Departamento** (não "área" ou "setor")
- **Mapa de Relacionamento** (não "organograma" ou "hierarquia")

---

## 9. APIs Necessárias (Backend .NET 8)

### 9.1 Organograma API

**Base URL:** `/api/Organograma`

#### 9.1.1 Listar Clientes

**Método e Rota:**
```
GET /api/Organograma/RetornarClientesPorOrgId?orgId={number}&limite={number}&cursor={number}&nomeCliente={string}
```

**Headers:**
```
Authorization: Bearer {token}
FRONTEND_TRACE_ID: {traceId} (automático via httpClient)
```

**Parâmetros Query:**
- `orgId` (number, obrigatório): ID da organização
- `limite` (number, opcional): Limite de resultados (padrão: 50)
- `cursor` (number, opcional): Cursor de paginação (padrão: 0)
- `nomeCliente` (string, opcional): Filtro por nome

**Exemplo de Response:**
```json
{
  "sucesso": true,
  "mensagem": null,
  "erros": null,
  "retorno": [
    {
      "code": "CLI001",
      "nomeFantasia": "Acme Corporation",
      "razaoSocial": "Acme Corp LTDA"
    }
  ]
}
```

**Contrato Sugerido (C#):**
```csharp
public record ClienteMapaRelacionamentoDto
{
    public string Code { get; init; }
    public string NomeFantasia { get; init; }
    public string RazaoSocial { get; init; }
}
```

#### 9.1.2 Listar Organograma Completo

**Método e Rota:**
```
GET /api/Organograma/OrganogramaCompletoPorCliente?codigoCliente={string}&orgId={number}
```

**Headers:**
```
Authorization: Bearer {token}
FRONTEND_TRACE_ID: {traceId}
```

**Parâmetros Query:**
- `codigoCliente` (string, obrigatório): Código do cliente (ex.: "CLI001")
- `orgId` (number, obrigatório): ID da organização

**Exemplo de Response:**
```json
{
  "sucesso": true,
  "mensagem": null,
  "erros": null,
  "retorno": [
    {
      "id": "guid-posicao-1",
      "posicaoIdSuperior": null,
      "perfilCorporativoId": "guid-perfil-1",
      "perfilCorporativoNome": "Diretor de TI",
      "departamentoId": "guid-dept-1",
      "departamentoNome": "Tecnologia",
      "cLevel": true,
      "profissionalExterno": true,
      "ativo": true,
      "alocacoes": [
        {
          "id": "guid-alocacao-1",
          "codigoInternoColaborador": "guid-colab-1",
          "nomeColaborador": "João Silva",
          "ativo": true,
          "dataFim": null
        }
      ]
    }
  ]
}
```

**Contrato Sugerido (C#):**
```csharp
public record PosicaoCompletaDto
{
    public string Id { get; init; }
    public string? PosicaoIdSuperior { get; init; }
    public string? PerfilCorporativoId { get; init; }
    public string? PerfilCorporativoNome { get; init; }
    public string? DepartamentoId { get; init; }
    public string? DepartamentoNome { get; init; }
    public bool CLevel { get; init; }
    public bool ProfissionalExterno { get; init; }
    public bool Ativo { get; init; }
    public List<AlocacaoDto> Alocacoes { get; init; }
}

public record AlocacaoDto
{
    public string? Id { get; init; }
    public string? CodigoInternoColaborador { get; init; }
    public string? NomeColaborador { get; init; }
    public bool Ativo { get; init; }
    public DateTime? DataFim { get; init; }
}
```

#### 9.1.3 Criar Posição

**Método e Rota:**
```
POST /api/Organograma/Organograma/PosicaoInserir
```

**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
FRONTEND_TRACE_ID: {traceId}
```

**Body:**
```json
{
  "organogramaPosicaoIdSuperior": "guid-pai",
  "organogramaDepartamentoId": "guid-dept",
  "perfilCorporativoId": "guid-perfil",
  "isCLevel": true,
  "isExternal": true
}
```

**Exemplo de Response:**
```json
{
  "sucesso": true,
  "mensagem": "Posição criada com sucesso",
  "erros": null,
  "retorno": {
    "organogramaPosicaoId": "guid-novo",
    "organogramaPosicaoIdSuperior": "guid-pai",
    "organogramaDepartamentoId": "guid-dept",
    "perfilCorporativoId": "guid-perfil",
    "isCLevel": true,
    "isExternal": true
  }
}
```

**Contrato Sugerido (C#):**
```csharp
public record CriarPosicaoRequest
{
    public string? OrganogramaPosicaoIdSuperior { get; init; }
    public string? OrganogramaDepartamentoId { get; init; }
    public string? PerfilCorporativoId { get; init; }
    public bool IsCLevel { get; init; }
    public bool IsExternal { get; init; }
}

public record PosicaoResponse
{
    public string OrganogramaPosicaoId { get; init; }
    public string? OrganogramaPosicaoIdSuperior { get; init; }
    public string? OrganogramaDepartamentoId { get; init; }
    public string? PerfilCorporativoId { get; init; }
    public bool IsCLevel { get; init; }
    public bool IsExternal { get; init; }
}
```

#### 9.1.4 Atualizar Posição

**Método e Rota:**
```
POST /api/Organograma/Organograma/PosicaoAtualizar
```

**Body:** Mesmo formato de `PosicaoInserir`, incluindo `organogramaPosicaoId`

**Response:** Mesmo formato de resposta

#### 9.1.5 Criar Departamento

**Método e Rota:**
```
POST /api/Organograma/Organograma/DepartamentoInserir
```

**Body:**
```json
{
  "nome": "Tecnologia da Informação",
  "organogramaPosicaoIdLider": "guid-posicao-lider"
}
```

**Response:**
```json
{
  "sucesso": true,
  "mensagem": "Departamento criado com sucesso",
  "erros": null,
  "retorno": {
    "organogramaDepartamentoId": "guid-dept-novo",
    "nome": "Tecnologia da Informação",
    "organogramaPosicaoIdLider": "guid-posicao-lider"
  }
}
```

#### 9.1.6 Atualizar Departamento

**Método e Rota:**
```
POST /api/Organograma/Organograma/DepartamentoAtualizar
```

**Body:** Mesmo formato de `DepartamentoInserir`, incluindo `organogramaDepartamentoId`

#### 9.1.7 Listar Departamentos por Cliente

**Método e Rota:**
```
GET /api/Organograma/Organograma/DepartamentoListarPorCliente?codigoCliente={string}
```

**Response:**
```json
{
  "sucesso": true,
  "mensagem": null,
  "erros": null,
  "retorno": [
    {
      "organogramaDepartamentoId": "guid-dept-1",
      "nome": "Tecnologia",
      "organogramaPosicaoIdLider": "guid-posicao-1"
    }
  ]
}
```

#### 9.1.8 Criar Alocação

**Método e Rota:**
```
POST /api/Organograma/Organograma/AlocacaoInserir
```

**Body:**
```json
{
  "organogramaPosicaoId": "guid-posicao",
  "codInternoColaborador": "guid-colaborador"
}
```

**Response:**
```json
{
  "sucesso": true,
  "mensagem": "Alocação criada com sucesso",
  "erros": null,
  "retorno": {
    "organogramaAlocacaoId": "guid-alocacao-novo",
    "organogramaPosicaoId": "guid-posicao",
    "codInternoColaborador": "guid-colaborador"
  }
}
```

#### 9.1.9 Atualizar Alocação

**Método e Rota:**
```
POST /api/Organograma/Organograma/AlocacaoAtualizar
```

**Body:** Mesmo formato de `AlocacaoInserir`, incluindo `organogramaAlocacaoId`

#### 9.1.10 Deletar Alocação

**Método e Rota:**
```
DELETE /api/Organograma/Organograma/AlocacaoDeletar?organogramaAlocacaoId={guid}
```

**Response:**
```json
{
  "sucesso": true,
  "mensagem": "Alocação removida com sucesso",
  "erros": null,
  "retorno": true
}
```

### 9.2 VCX API

**Base URL:** `/api/MapaDeRelacionamento/VCX`

#### 9.2.1 Listar Dores por Posição

**Método e Rota:**
```
GET /api/MapaDeRelacionamento/VCX/ListarDoresPorPosicaoId?posicaoId={guid}
```

**Response:**
```json
{
  "sucesso": true,
  "mensagem": null,
  "erros": null,
  "retorno": [
    {
      "id": "guid-dor-1",
      "organogramaPosicaoId": "guid-posicao",
      "titulo": "Falta de visibilidade de métricas",
      "descricao": "Não temos dashboards consolidados",
      "dataCriacao": "2026-02-20T10:30:00Z",
      "dataAlteracao": "2026-02-20T10:30:00Z",
      "vcxImpactosId": "guid-impacto-alto",
      "vcxUrgenciasId": "guid-urgencia-alta",
      "vcxImpactosDescricao": "Impacto Alto",
      "vcxUrgenciasDescricao": "Urgência Alta"
    }
  ]
}
```

**Contrato Sugerido (C#):**
```csharp
public record DorResponse
{
    public string Id { get; init; }
    public string OrganogramaPosicaoId { get; init; }
    public string Titulo { get; init; }
    public string? Descricao { get; init; }
    public DateTime DataCriacao { get; init; }
    public DateTime DataAlteracao { get; init; }
    public string VcxImpactosId { get; init; }
    public string VcxUrgenciasId { get; init; }
    public string VcxImpactosDescricao { get; init; }
    public string VcxUrgenciasDescricao { get; init; }
}
```

#### 9.2.2 Criar Dor

**Método e Rota:**
```
POST /api/MapaDeRelacionamento/VCX/CriarDor
```

**Body:** Todos os campos são obrigatórios (validados no frontend antes do envio).
```json
{
  "organogramaPosicaoId": "guid-posicao",
  "titulo": "Falta de visibilidade de métricas",
  "descricao": "Não temos dashboards consolidados",
  "vcxImpactosId": "guid-impacto-alto",
  "vcxUrgenciasId": "guid-urgencia-alta"
}
```

**Response:** Mesmo formato de `DorResponse` acima

**Contrato Sugerido (C#):**
```csharp
public record CriarDorRequest
{
    [Required]
    public string OrganogramaPosicaoId { get; init; }
    
    [Required]
    public string Titulo { get; init; }
    
    [Required]
    public string Descricao { get; init; }
    
    [Required]
    public string VcxImpactosId { get; init; }
    
    [Required]
    public string VcxUrgenciasId { get; init; }
}
```

#### 9.2.3 Atualizar Dor

**Método e Rota:**
```
PUT /api/MapaDeRelacionamento/VCX/AtualizarDor
```

**Body:** Mesmo formato de `CriarDor`, incluindo `id`

#### 9.2.4 Deletar Dor

**Método e Rota:**
```
DELETE /api/MapaDeRelacionamento/VCX/ExcluirDor?id={guid}
```

**Response:**
```json
{
  "sucesso": true,
  "mensagem": "Dor excluída com sucesso",
  "erros": null,
  "retorno": true
}
```

#### 9.2.5 Listar Impactos de Dores

**Método e Rota:**
```
GET /api/MapaDeRelacionamento/VCX/ListarImpactosDores
```

**Response:**
```json
{
  "sucesso": true,
  "mensagem": null,
  "erros": null,
  "retorno": [
    {
      "id": "guid-impacto-alto",
      "descricao": "Impacto Alto"
    },
    {
      "id": "guid-impacto-medio",
      "descricao": "Impacto Médio"
    },
    {
      "id": "guid-impacto-baixo",
      "descricao": "Impacto Baixo"
    }
  ]
}
```

#### 9.2.6 Listar Urgências de Dores

**Método e Rota:**
```
GET /api/MapaDeRelacionamento/VCX/ListarUrgenciasDores
```

**Response:** Formato idêntico a Impactos (id, descricao)

#### 9.2.7 Iniciativas - Endpoints

**CRUD de Iniciativas:**
```
GET  /api/MapaDeRelacionamento/VCX/ListarIniciativasPorPosicaoId?posicaoId={guid}
POST /api/MapaDeRelacionamento/VCX/CriarIniciativa
PUT  /api/MapaDeRelacionamento/VCX/AtualizarIniciativa
DELETE /api/MapaDeRelacionamento/VCX/ExcluirIniciativa?id={guid}
```

**Dados de Referência:**
```
GET /api/MapaDeRelacionamento/VCX/ListarStatusIniciativas
GET /api/MapaDeRelacionamento/VCX/ListarTemas
POST /api/MapaDeRelacionamento/VCX/CriarTema
```

**Exemplo Body CriarIniciativa:**
```json
{
  "organogramaPosicaoId": "guid-posicao",
  "titulo": "Implementar Dashboard de Métricas",
  "descricao": "Dashboard consolidado de KPIs principais",
  "vcxStatusId": "guid-status-ativa",
  "vcxTemasId": "guid-tema-ou-novo"
}
```

**Response IniciativaResponse:**
```json
{
  "id": "guid-iniciativa",
  "organogramaPosicaoId": "guid-posicao",
  "titulo": "Implementar Dashboard de Métricas",
  "descricao": "Dashboard consolidado de KPIs principais",
  "dataCriacao": "2026-02-20T10:30:00Z",
  "dataAlteracao": "2026-02-20T10:30:00Z",
  "vcxStatusId": "guid-status",
  "vcxTemasId": "guid-tema",
  "vcxStatusDescricao": "Ativa",
  "vcxTemasDescricao": "Tecnologia"
}
```

#### 9.2.8 Listar Histórico de Dores e Iniciativas

**Método e Rota:**
```
GET /api/MapaDeRelacionamento/VCX/ListarHistoricoDoresIniciativasPorPosicaoId?posicaoId={guid}
```

**Response:**
```json
{
  "sucesso": true,
  "mensagem": null,
  "erros": null,
  "retorno": {
    "doresLog": [
      {
        "id": "guid-log-1",
        "colaboradorCodigoInternoColaboradorAlterador": "guid-colab",
        "organogramaPosicaoId": "guid-posicao",
        "acao": "INSERT",
        "objeto": {
          "titulo": "Nova dor",
          "descricao": "Descrição da dor",
          "vcxImpactosDescricao": "Impacto Alto",
          "vcxUrgenciasDescricao": "Urgência Média"
        },
        "alteracao": null,
        "dataAlteracao": "2026-02-20T10:30:00Z"
      },
      {
        "id": "guid-log-2",
        "colaboradorCodigoInternoColaboradorAlterador": "guid-colab",
        "organogramaPosicaoId": "guid-posicao",
        "acao": "UPDATE",
        "objeto": {
          "titulo": "Dor antiga",
          "vcxImpactosDescricao": "Impacto Médio"
        },
        "alteracao": {
          "titulo": "Dor atualizada",
          "vcxImpactosDescricao": "Impacto Alto"
        },
        "dataAlteracao": "2026-02-20T11:00:00Z"
      }
    ],
    "iniciativasLog": [
      {
        "id": "guid-log-3",
        "colaboradorCodigoInternoColaboradorAlterador": "guid-colab",
        "organogramaPosicaoId": "guid-posicao",
        "acao": "INSERT",
        "objeto": {
          "titulo": "Nova iniciativa",
          "vcxStatusDescricao": "Em Planejamento",
          "vcxTemasDescricao": "Tecnologia"
        },
        "alteracao": null,
        "dataAlteracao": "2026-02-20T12:00:00Z"
      }
    ]
  }
}
```

**Contrato Sugerido (C#):**
```csharp
public record HistoricoDoresIniciativasResponse
{
    public List<DorLogDto> DoresLog { get; init; }
    public List<IniciativaLogDto> IniciativasLog { get; init; }
}

public record DorLogDto
{
    public string Id { get; init; }
    public string ColaboradorCodigoInternoColaboradorAlterador { get; init; }
    public string OrganogramaPosicaoId { get; init; }
    public string Acao { get; init; } // INSERT, UPDATE, DELETE
    public DorLogItemDto? Objeto { get; init; }
    public DorLogItemDto? Alteracao { get; init; }
    public DateTime DataAlteracao { get; init; }
}

public record DorLogItemDto
{
    public string? Titulo { get; init; }
    public string? Descricao { get; init; }
    public string? VcxImpactosDescricao { get; init; }
    public string? VcxUrgenciasDescricao { get; init; }
}
```

---

## 10. Modelos de Dados (Contratos)

### 10.1 Entidades de Domínio (Frontend)

#### NoMapaRelacionamento
```typescript
export interface NoMapaRelacionamento {
  id: string                                  // ID interno do frontend
  posicaoId?: string                          // organogramaPosicaoId do backend
  profileId: string | null                    // perfilCorporativoId ou gestorExternoPerfilId
  employeeId: string | null                   // codigoInternoColaborador ou 'vacant'
  profileName?: string                        // perfilCorporativoNome
  employeeName?: string                       // nomeColaborador ou 'Vago'
  employeeEmail?: string                      // email do colaborador
  employeeAvatarUrl?: string                  // URL do avatar
  departmentId?: string                       // ID interno do frontend
  departmentName?: string                     // departamentoNome
  isCLevel?: boolean                          // cLevel (posição executiva)
  isExternal?: boolean                        // profissionalExterno (sempre true)
  wasConnected?: boolean                      // tinha colaborador alocado
  alocacaoId?: string                         // organogramaAlocacaoId
  departamentoBackendId?: string              // organogramaDepartamentoId do backend
  perfilCorporativoBackendId?: string         // perfilCorporativoId do backend
  children: NoMapaRelacionamento[]            // filhos na hierarquia
}
```

#### DorResponse (VCX)
```typescript
export interface DorResponse {
  id: string                          // GUID
  organogramaPosicaoId: string        // FK para posição
  titulo: string                      // Obrigatório
  descricao: string | null            // Opcional
  dataCriacao: string                 // ISO 8601
  dataAlteracao: string               // ISO 8601
  vcxImpactosId: string               // FK para impacto
  vcxUrgenciasId: string              // FK para urgência
  vcxImpactosDescricao: string        // "Impacto Alto/Médio/Baixo"
  vcxUrgenciasDescricao: string       // "Urgência Alta/Média/Baixa"
}
```

#### IniciativaResponse (VCX)
```typescript
export interface IniciativaResponse {
  id: string                          // GUID
  organogramaPosicaoId: string        // FK para posição
  titulo: string                      // Obrigatório
  descricao: string | null            // Opcional (Objetivo/KPI)
  dataCriacao: string                 // ISO 8601
  dataAlteracao: string               // ISO 8601
  vcxStatusId: string                 // FK para status
  vcxTemasId: string                  // FK para tema
  vcxStatusDescricao: string          // "Ativa", "Em Planejamento", etc.
  vcxTemasDescricao: string           // Nome do tema
}
```

### 10.2 Payloads (Request)

#### PosicaoPayload
```typescript
export interface PosicaoPayload {
  organogramaPosicaoId?: string                  // GUID (presente em UPDATE)
  organogramaPosicaoIdSuperior?: string | null   // FK para pai (null = raiz)
  organogramaDepartamentoId?: string | null      // FK para departamento
  perfilCorporativoId?: string | null            // FK para perfil
  isCLevel?: boolean                             // Posição executiva
  isExternal?: boolean                           // Sempre true
}
```

#### DepartamentoPayload
```typescript
export interface DepartamentoPayload {
  organogramaDepartamentoId?: string       // GUID (presente em UPDATE)
  nome: string                             // Nome do departamento (obrigatório)
  organogramaPosicaoIdLider?: string | null // FK para posição líder
}
```

#### AlocacaoPayload
```typescript
export interface AlocacaoPayload {
  organogramaAlocacaoId?: string     // GUID (presente em UPDATE)
  organogramaPosicaoId: string       // FK para posição (obrigatório)
  codInternoColaborador: string      // Código do colaborador (obrigatório)
}
```

#### DorPayload (VCX)
```typescript
export interface DorPayload {
  id?: string                         // Presente em UPDATE
  organogramaPosicaoId: string        // Obrigatório
  titulo: string                      // Obrigatório
  descricao: string | null           // Obrigatório (frontend valida antes de enviar)
  vcxImpactosId: string               // Obrigatório
  vcxUrgenciasId: string              // Obrigatório
}
```

#### IniciativaPayload (VCX)
```typescript
export interface IniciativaPayload {
  id?: string                         // Presente em UPDATE
  organogramaPosicaoId: string        // Obrigatório
  titulo: string                      // Obrigatório
  descricao: string | null            // Obrigatório (Objetivo/KPI – frontend valida)
  vcxStatusId?: string | null         // Opcional
  vcxTemasId?: string | null          // Resolvido pelo backend (tema criado se não existir); Tema é obrigatório no formulário
}
```

### 10.3 Envelope Padrão de Resposta

```typescript
interface ApiGenericResult<T> {
  sucesso: boolean              // true = sucesso, false = erro
  mensagem: string | null       // Mensagem geral (sucesso ou erro)
  erros: string[] | null        // Lista de erros de validação
  retorno: T                    // Dados retornados (tipo genérico)
}
```

---

## 11. Dependências de APIs e Dados Existentes

### 11.1 APIs Reutilizadas

| API Existente | Endpoint | Alinhamento de Nomes |
|---------------|----------|----------------------|
| **ColaboradoresApi** | `GET /api/MapaDeAlocacao/ListarColaboradoresOrg` | Retorna `codigoColaboradorInterno`; usar como `codigoInternoColaborador` em payloads |
| **PerfilCorporativoApi** | `GET /api/Organograma/PerfilCorporativoListarPorOrg` | Retorna `perfilCorporativoId` e `nome` |

### 11.2 Tabelas Impactadas

- **OrganogramaPosicao**: Criação, atualização, movimentação
- **OrganogramaDepartamento**: Criação, atualização
- **OrganogramaAlocacao**: Criação, atualização, deleção
- **VcxDores**: CRUD completo
- **VcxIniciativas**: CRUD completo
- **VcxDoresLog**: Histórico de alterações de dores (INSERT, UPDATE, DELETE)
- **VcxIniciativasLog**: Histórico de alterações de iniciativas

---

## 12. Fluxo Resumido (Backend)

### 12.1 Salvar Posição (Criação)

**Ordem de Execução Obrigatória:**

1. **POST PosicaoInserir**
   - Criar posição no backend
   - Retorna `organogramaPosicaoId`
   
2. **Se criou departamento:** **POST DepartamentoInserir**
   - Criar departamento associado
   - `organogramaPosicaoIdLider` = `organogramaPosicaoId` retornado
   - Retorna `organogramaDepartamentoId`
   
3. **Se criou departamento:** **POST PosicaoAtualizar**
   - Atualizar posição com `organogramaDepartamentoId`
   
4. **Se selecionou colaborador:** **POST AlocacaoInserir**
   - Criar alocação colaborador → posição
   - `organogramaPosicaoId` e `codInternoColaborador`

**Motivo da ordem:** `posicaoId` é necessário para criar departamento; `departamentoId` é necessário para atualizar posição.

### 12.2 Salvar Posição (Edição)

**Ordem de Execução Obrigatória:**

1. **Se criou departamento:** **POST DepartamentoInserir**
   - `posicaoId` já existe
   
2. **POST PosicaoAtualizar**
   - Atualizar campos da posição
   
3. **Se editou departamento E isGestor:** **POST DepartamentoAtualizar**
   - Atualizar departamento com novo líder
   
4. **Se alterou colaborador:**
   - **Cenário 1:** Sem alocação → Colaborador: **POST AlocacaoInserir**
   - **Cenário 2:** Com alocação → Colaborador diferente: **POST AlocacaoAtualizar**
   - **Cenário 3:** Com alocação → "Vago": **DELETE AlocacaoDeletar**

### 12.3 Criar Dor/Iniciativa

1. Obter `token` do usuário autenticado
2. Validar `organogramaPosicaoId` existe
3. **Se Iniciativa E tema não existe:** Criar tema primeiro
4. Validar campos obrigatórios: **Dores** – titulo, descricao, vcxImpactosId, vcxUrgenciasId; **Iniciativas** – titulo, descricao (Objetivo/KPI), tema (obrigatório no frontend)
5. Persistir dor/iniciativa no banco
6. Criar log de auditoria (INSERT)
7. Retornar dados com `dataCriacao` e `dataAlteracao`

### 12.4 Atualizar Dor/Iniciativa

1. Obter `token` do usuário autenticado
2. Buscar registro existente por `id`
3. Capturar estado "antes" para auditoria
4. Atualizar campos
5. Atualizar `dataAlteracao` com timestamp atual
6. Criar log de auditoria (UPDATE) com `objeto` (antes) e `alteracao` (depois)
7. Retornar dados atualizados

---

## 13. Propostas de Melhorias (Evolução da Feature)

### 13.1 Curto Prazo

1. **Confirmação antes de deletar Dores/Iniciativas**
   - Atual: Deleção imediata ao clicar ícone Trash
   - Proposta: AlertDialog com confirmação

2. **Persistência de ordem de Dores/Iniciativas**
   - Atual: Drag-and-drop local, sem persistência
   - Proposta: Campo `ordem` no backend, atualizar ao reordenar

3. **Limites de caracteres com contador**
   - Atual: Campos sem limite visual
   - Proposta: Contador de caracteres (ex.: "50/200")

4. **Validação mais robusta de formulários**
   - Atual: Validação básica de obrigatoriedade
   - Proposta: Validação de formato, comprimento, caracteres especiais

### 13.2 Médio Prazo

5. **Paginação de logs no Histórico**
   - Atual: Carrega todos os logs
   - Proposta: Paginação server-side (cursor, limite)

6. **Filtros no Histórico**
   - Atual: Sem filtros
   - Proposta: Filtrar por tipo (Dor/Iniciativa), ação (INSERT/UPDATE/DELETE), período

7. **Busca textual no Histórico**
   - Atual: Sem busca
   - Proposta: Buscar por título, descrição, colaborador

8. **Implementação completa da Aba Agenda**
   - Atual: Simplificada ("Em Breve")
   - Proposta: Agendas, interações comerciais, ações (componentes já existem)

9. **Informações do colaborador alterador no Histórico**
   - Atual: Apenas `codigoInternoColaborador`
   - Proposta: Nome, email, avatar do colaborador

### 13.3 Longo Prazo

10. **Visualização de diferenças side-by-side (UPDATE)**
    - Atual: Exibe "Antes" e "Depois" em blocos separados
    - Proposta: Diff visual colorido (verde = adição, vermelho = remoção)

11. **Exportação de histórico**
    - Atual: Sem exportação
    - Proposta: Exportar histórico como CSV/PDF

12. **Suporte a drag-and-drop via teclado**
    - Atual: Apenas mouse
    - Proposta: Navegação por teclado (Tab, Enter, Setas)

13. **Animações de entrada/saída para cards**
    - Atual: Sem animações
    - Proposta: Fade in/out ao adicionar/remover

---

## 14. Cenários de Erro e Pontos de Atenção

### 14.1 Erros Não Mapeados no Front

| Erro Backend | Tratamento Frontend Atual | Sugestão de Melhoria |
|--------------|---------------------------|----------------------|
| **Colaborador já alocado em outra posição** | Toast genérico com mensagem do backend | Mensagem específica: "Este colaborador já está alocado em outra posição." |
| **Departamento duplicado** | Toast genérico | "Já existe um departamento com este nome." |
| **Posição não encontrada (404)** | Toast genérico | "Posição não encontrada. A estrutura pode ter sido alterada." |
| **Validação de FK inválido** | Toast genérico | "Dados inválidos. Por favor, recarregue a página." |

### 14.2 Impacto em Desenvolvimento

**Banco de Dados:**
- Tabelas: `OrganogramaPosicao`, `OrganogramaDepartamento`, `OrganogramaAlocacao`, `VcxDores`, `VcxIniciativas`, `VcxDoresLog`, `VcxIniciativasLog`
- FKs críticas: `organogramaPosicaoIdSuperior`, `organogramaDepartamentoId`, `perfilCorporativoId`, `vcxImpactosId`, `vcxUrgenciasId`, `vcxStatusId`, `vcxTemasId`
- Índices recomendados: `organogramaPosicaoId`, `organogramaPosicaoIdSuperior`, `codigoInternoColaborador`

**Segurança:**
- Validar permissões de `isGestor` para criar/editar departamentos
- Validar que colaborador pertence à mesma organização
- Validar que posição destino (mover) não cria ciclos
- Não permitir deletar posição com dependentes (ou deletar em cascata)

**Performance:**
- **OrganogramaCompletoPorCliente:** Pode retornar muitos registros (100+); considerar cache
- **Histórico:** Pode crescer muito; implementar paginação
- **Índices:** Criar índices em FKs para queries rápidas

---

## 15. Edge Cases Tratados (Robustez)

### 15.1 Arrays Vazios e Valores Null

**Problema:** Backend pode retornar arrays vazios, `null` ou respostas com `sucesso: false`

**Tratamento no Frontend:**
```typescript
// VcxApi.ts
if (!response || !response.sucesso || !Array.isArray(response.retorno)) {
  return []  // Retorna array vazio em vez de lançar erro
}
```

**Benefício:** Interface não quebra; exibe "Sem dados" em vez de erro

### 15.2 GUID Vazio

**Problema:** Backend pode retornar `codigoInternoColaborador` como `'00000000-0000-0000-0000-000000000000'`

**Tratamento:**
```typescript
const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'
const hasValidAlocacao = !!(
  alocacaoAtiva && 
  alocacaoAtiva.codigoInternoColaborador !== EMPTY_GUID
)
```

**Benefício:** Posição tratada como "Vago" corretamente

### 15.3 Múltiplas Raízes no Organograma

**Problema:** Estrutura pode ter múltiplas posições sem `posicaoIdSuperior` (múltiplas raízes)

**Tratamento:**
```typescript
// organogramaTreeMapper.ts
if (raiz.length === 1) {
  return raiz[0]
} else if (raiz.length > 1) {
  return {
    id: 'root-virtual',
    profileId: null,
    employeeId: null,
    employeeName: 'Organograma',
    children: raiz,
  }
}
```

**Benefício:** Interface exibe múltiplas raízes lado a lado sem erro

### 15.4 Filtro C-Levels Sem Resultados

**Problema:** Ao filtrar C-Levels, pode não haver nenhum resultado

**Tratamento:**
```typescript
// mapaRelacionamentoTreeService.ts
export function filtrarCLevelsMapa(no: NoMapaRelacionamento): NoMapaRelacionamento | null {
  const cLevels = coletarCLevelsRecursivo(no)
  if (cLevels.length === 0) return null
  // ...
}
```

**Benefício:** Interface exibe "Estrutura vazia" com mensagem clara

### 15.5 Promise.allSettled para Dados de Referência

**Problema:** Se um endpoint falha (ex.: `listarTemas` retorna vazio), o outro não deve ser bloqueado

**Tratamento:**
```typescript
// useVcxIniciativas.ts
const [statusResult, temasResult] = await Promise.allSettled([
  dispatch(listarStatusIniciativas({ token })).unwrap(),
  dispatch(listarTemas({ token })).unwrap(),
])
if (statusResult.status === 'rejected') {
  toast.error('Erro ao carregar status de iniciativas')
}
```

**Benefício:** Campos não ficam em loading eterno; cada um falha independentemente

### 15.6 Ordenação com Datas Inválidas

**Problema:** `dataCriacao` pode ser `null`, `undefined` ou string inválida; `new Date()` retorna `Invalid Date` e `getTime()` retorna `NaN`

**Tratamento Recomendado:**
```typescript
.sort((a, b) => {
  const dateA = a.dataCriacao ? new Date(a.dataCriacao).getTime() : 0
  const dateB = b.dataCriacao ? new Date(b.dataCriacao).getTime() : 0
  
  if (isNaN(dateA) && isNaN(dateB)) return 0
  if (isNaN(dateA)) return 1  // Invalid dates vão para o final
  if (isNaN(dateB)) return -1
  return dateB - dateA
})
```

**Benefício:** Itens com datas inválidas no final; ordenação robusta

---

## 16. Resumo para o Time

### Feature em uma frase
Ferramenta centralizada para visualização e gestão interativa de estruturas organizacionais hierárquicas com contexto VCX (dores e iniciativas).

### APIs a Implementar (Backend)
Todas as APIs VCX estão **já implementadas** no backend. APIs de Organograma (posição, departamento, alocação) também estão implementadas.

### APIs Já Usadas
- `GET RetornarClientesPorOrgId`
- `GET OrganogramaCompletoPorCliente`
- `POST/PUT/DELETE` Posição, Departamento, Alocação
- `GET/POST/PUT/DELETE` VCX Dores, Iniciativas
- `GET` VCX Histórico, Impactos, Urgências, Status, Temas

### Contratos em uma linha
camelCase, envelope `{sucesso, mensagem, erros, retorno}`, `dataCriacao`/`dataAlteracao` ISO 8601, `codigoInternoColaborador` para identificar colaborador.

### Pontos de Atenção
1. **Ordem de endpoints é crítica** (ver seção 12)
2. **Validar ciclos** ao mover posições
3. **GUID vazio** deve ser tratado como "Vago"
4. **Múltiplas raízes** são possíveis e devem ser suportadas
5. **Arrays vazios** devem retornar `[]`, não erro
6. **isExternal sempre true** (frontend força esse valor)
7. **Histórico:** ordenar por `dataAlteracao` descendente
8. **Promise.allSettled:** permite que endpoints de referência falhem independentemente

---

## 17. Arquitetura e Implementação Técnica

### 17.1 Clean Architecture

A implementação segue rigorosamente os princípios da Clean Architecture:

```
Presentation Layer (UI)
  └── Pages, Components, Hooks
       └── App Layer (Redux Store)
            └── Domain Layer (Use Cases + Repository Interfaces)
                 └── Data Layer (Repository Implementations + APIs)
                      └── httpClient Factory
                           └── Backend API
```

### 17.2 Fluxo de Dados

#### Carregar Estrutura
```
MapaRelacionamentoPage
    ↓
useMapaRelacionamentoPage (hook)
    ↓
dispatch(buscarEstruturaMapa)
    ↓
mapaRelacionamentoSlice (Redux)
    ↓
BuscarEstruturaMapaUseCase
    ↓
MapaRelacionamentoRepository (interface)
    ↓
MapaRelacionamentoRepositoryImpl
    ↓
OrganogramaApi.listarOrganogramaCompleto()
    ↓
httpClient.get() (factory)
    ↓
Backend API
```

#### Gerenciar Dores VCX
```
DoresList (componente)
    ↓
useVcxDores (hook)
    ↓
dispatch(criarDor)
    ↓
vcxDoresSlice (Redux)
    ↓
CriarDorUseCase
    ↓
VcxRepository (interface)
    ↓
VcxRepositoryImpl
    ↓
VcxApi.criarDor()
    ↓
httpClient.post()
    ↓
Backend API
```

### 17.3 Dependency Injection (TSyringe)

**Tokens:**
```typescript
export const DiTokens = {
  organogramaApi: Symbol.for('OrganogramaApi'),
  vcxApi: Symbol.for('VcxApi'),
  mapaRelacionamentoRepository: Symbol.for('MapaRelacionamentoRepository'),
  vcxRepository: Symbol.for('VcxRepository'),
}
```

**Registro no Container:**
```typescript
// APIs
container.registerSingleton(DiTokens.organogramaApi, OrganogramaApi)
container.registerSingleton(DiTokens.vcxApi, VcxApi)

// Repositories
container.registerSingleton<MapaRelacionamentoRepository>(
  DiTokens.mapaRelacionamentoRepository,
  MapaRelacionamentoRepositoryImpl
)
container.registerSingleton<VcxRepository>(
  DiTokens.vcxRepository,
  VcxRepositoryImpl
)

// Use Cases (auto-inject dependencies)
container.registerSingleton(BuscarEstruturaMapaUseCase, BuscarEstruturaMapaUseCase)
container.registerSingleton(SalvarPosicaoUseCase, SalvarPosicaoUseCase)
// ... outros use cases ...
```

### 17.4 Estrutura de Pastas

```
src/
├── presentation/
│   ├── pages/
│   │   └── MapaRelacionamentoPage.tsx
│   ├── components/
│   │   └── mapa-relacionamento/
│   │       ├── seletores/
│   │       │   └── ClientSelector.tsx
│   │       ├── visualizacoes/
│   │       │   ├── arvore-mapa-d3/
│   │       │   │   ├── ArvoreMapaD3.tsx
│   │       │   │   ├── arvoreMapaD3Helpers.ts (helpers extraídos)
│   │       │   │   ├── RootDropZone.tsx
│   │       │   │   ├── GhostCard.tsx
│   │       │   │   └── InstructionMessage.tsx
│   │       │   ├── DiagramaMapa.tsx
│   │       │   ├── ListaMapa.tsx
│   │       │   └── MapaRelacionamentoLoadingState.tsx
│   │       ├── modais/
│   │       │   ├── EditModalMapa.tsx
│   │       │   ├── EditarDepartamentoModal.tsx
│   │       │   ├── InserirGestorExternoModal.tsx
│   │       │   └── CriarEditarPerfilAtuacaoModal.tsx (removido)
│   │       └── paineis/
│   │           └── PainelVcx360.tsx
│   │               └── abas/
│   │                   ├── DoresList.tsx (componente em Contexto)
│   │                   ├── IniciativasList.tsx (componente em Contexto)
│   │                   ├── AbaAgenda.tsx (simplificada)
│   │                   └── AbaHistorico.tsx
│   └── hooks/
│       ├── useMapaRelacionamentoPage.ts
│       ├── useMapaRelacionamentoPersistencia.ts
│       ├── useMapaInteracoes.ts
│       ├── useVcxDores.ts
│       └── useVcxIniciativas.ts
├── app/
│   └── store/
│       └── slices/
│           ├── mapaRelacionamentoSlice.ts
│           ├── vcxDoresSlice.ts
│           └── vcxIniciativasSlice.ts
├── domain/
│   ├── entities/
│   │   ├── MapaRelacionamento.ts
│   │   ├── Organograma.ts
│   │   ├── VcxDores.ts
│   │   ├── VcxIniciativas.ts
│   │   └── VcxHistorico.ts
│   ├── repositories/
│   │   ├── MapaRelacionamentoRepository.ts (interface)
│   │   └── VcxRepository.ts (interface)
│   ├── usecases/
│   │   ├── BuscarEstruturaMapaUseCase.ts
│   │   ├── SalvarPosicaoUseCase.ts (orquestra múltiplos endpoints)
│   │   ├── MoverPosicaoUseCase.ts
│   │   ├── ListarDoresPorPosicaoIdUseCase.ts
│   │   ├── CriarDorUseCase.ts
│   │   ├── AtualizarDorUseCase.ts
│   │   ├── ExcluirDorUseCase.ts
│   │   ├── ListarIniciativasPorPosicaoIdUseCase.ts
│   │   ├── CriarIniciativaUseCase.ts
│   │   └── ... (outros use cases VCX)
│   └── services/
│       ├── mapaRelacionamentoTreeService.ts (helpers de árvore)
│       └── organogramaTreeMapper.ts (conversão API → Domain)
├── data/
│   ├── api/
│   │   ├── OrganogramaApi.ts
│   │   ├── VcxApi.ts
│   │   └── httpClient.ts (factory obrigatória)
│   └── repositories/
│       ├── MapaRelacionamentoRepositoryImpl.ts
│       └── VcxRepositoryImpl.ts
├── shared/
│   ├── utils/
│   │   ├── vcxHelpers.ts (cores, ícones, validação de status)
│   │   ├── firebaseAnalytics.ts
│   │   └── firebaseCrashlytics.ts
│   └── types/
│       └── mapaRelacionamentoTypes.ts
└── core/
    └── di/
        ├── container.ts (registro de dependências)
        └── tokens.ts (símbolos DI)
```

### 17.5 Utilitários Compartilhados

**vcxHelpers.ts** (`src/shared/utils/vcxHelpers.ts`):
- `getImpactBadgeColor(descricao: string | null)`: Retorna classes CSS para badge de Impacto
- `getUrgencyBadgeColor(descricao: string | null)`: Retorna classes CSS para badge de Urgência
- `getStatusColor(status: StatusIniciativa)`: Retorna classes CSS para badge de Status
- `getAcaoBadgeColor(acao: 'INSERT' | 'UPDATE' | 'DELETE')`: Retorna classes CSS para badge de Ação
- `getImpactIcon(descricao)`, `getUrgencyIcon(descricao)`, `getStatusIcon(status)`, `getAcaoIcon(acao)`: Ícones Lucide React
- `validarStatusIniciativa(descricao)`: Valida e normaliza descrição de status (evita `as any`)

**Todos usam tokens do Design System** (`destructive`, `warning`, `info`, `success`, `muted`) em vez de cores hardcoded.

### 17.6 Performance e Otimizações

**Implementadas:**
1. **Memoização:** `React.memo` para `MapaRelacionamentoHero`, `ColaboradorSelect`
2. **useCallback:** Callbacks estáveis para evitar re-renders
3. **useMemo:** Cálculos pesados (conversão D3Tree, contagem de nós)
4. **Promise.allSettled:** Chamadas paralelas independentes
5. **MutationObserver debounce:** 500ms em produção, desabilitado em DEV (previne lentidão)
6. **Observer cleanup:** Desconecta após 10s para limitar recursos
7. **localStorage:** Cache de cliente selecionado e modo de visualização

**Métricas Esperadas:**
- Tempo de carregamento: < 2s (estrutura completa)
- Renderização: < 100ms (até 100 nós)
- Tamanho do Painel VCX: 384px fixo

---

## 18. Design System e UI

### 18.1 Componentes Utilizados

**Componentes Base:**
- `Button`, `Card`, `Input`, `Select`, `Textarea`
- `Dialog`, `AlertDialog`, `Tabs`, `Badge`, `Alert`
- `Spinner`, `Skeleton`, `Switch`, `Checkbox`, `Tooltip`
- `Avatar`, `ScrollArea`, `Separator`, `Popover`, `Command`

**Ícones (lucide-react):**
- `Eye`, `Edit`, `Trash2`, `Plus`, `GripVertical`, `Crown`
- `Building`, `User`, `Calendar`, `AlertCircle`, `CheckCircle2`

### 18.2 Tokens do DS

**Cores:**
- `bg-primaryBackground`, `bg-card`, `bg-surfaceElevated`, `bg-secondaryBackground`
- `text-foreground`, `text-muted-foreground`, `text-primaryText`, `text-secondaryText`
- `border-border`, `border-borderSoft`, `border-accent`
- `bg-success`, `bg-destructive`, `bg-warning`, `bg-info`, `bg-muted`

**Radius:**
- `rounded-lg` (20px): Cards, inputs, modais
- `rounded-pill` (999px): Badges, botões

**Sombras:**
- `shadow-soft`: Cards
- `shadow-hover`: Hover state

### 18.3 Estados Visuais

| Estado | Visual | Componente |
|--------|--------|------------|
| **Loading Estrutura** | Spinner centralizado em `bg-primaryBackground` | `MapaRelacionamentoLoadingState` |
| **Loading VCX** | Spinner + texto | `Spinner` do DS |
| **Erro** | Alert destrutivo | `Alert` variant="destructive" |
| **Vazio** | Mensagem centralizada em `text-muted-foreground` | Texto padrão |
| **Hover** | Sombra + borda accent | Classes Tailwind |
| **Drag** | Ghost card + zona de drop destacada | `GhostCard` + visual feedback |

---

## 19. Testes e Validação

### 19.1 Cenários de Teste Funcionais

- [ ] Carregar página sem cliente selecionado (hero)
- [ ] Selecionar cliente e visualizar estrutura
- [ ] Alternar entre modo Diagrama e Lista
- [ ] Filtrar apenas C-Levels
- [ ] Criar nova posição (vaga)
- [ ] Criar nova posição com colaborador
- [ ] Criar nova posição com novo departamento (gestor)
- [ ] Editar posição existente (alterar colaborador)
- [ ] Editar posição (alterar departamento)
- [ ] Mover posição via drag-and-drop
- [ ] Deletar posição (com confirmação)
- [ ] Criar, editar e deletar dor no Painel VCX
- [ ] Criar, editar e deletar iniciativa no Painel VCX
- [ ] Criar tema novo ao adicionar iniciativa
- [ ] Visualizar histórico de alterações
- [ ] Exportar estrutura como JSON
- [ ] Tratamento de erros de rede
- [ ] Tratamento de arrays vazios
- [ ] Tratamento de GUID vazio
- [ ] Persistência de cliente em localStorage

### 19.2 Validação de Arquitetura

- [x] Domain não importa de Data/Presentation ✅
- [x] Presentation não importa de Data/API ✅
- [x] Usa httpClient (não fetch direto) ✅
- [x] UseCases via DI (container.resolve) ✅
- [x] Slices encapsulam UseCases ✅
- [x] Componentes usam Redux/Hooks ✅
- [x] Firebase Analytics implementado ✅
- [x] Design System usado consistentemente ✅
- [x] Null safety aplicado ✅
- [x] Edge cases tratados ✅

**Resultado:** ✅ 10/10 itens conformes (Score: 100%)

---

## 20. Limitações Conhecidas e Backlog

### Limitações Atuais

1. **Aba Agenda Simplificada**: Exibe apenas "Em Breve"; componentes órfãos existem (`ListaAgenda.tsx`, `ItemAgendaCard.tsx`, etc.) mas não são usados
2. **Reordenação de Dores/Iniciativas**: Drag-and-drop visual, mas ordem não é persistida no backend
3. **Sem confirmação ao deletar Dores/Iniciativas**: Deleção imediata (apenas posições têm confirmação)
4. **Colaborador Externo sempre true**: Campo inativo, sempre salvo como `true`
5. **Sem paginação de logs**: Histórico carrega todos os registros

### Backlog Técnico

**Refatorações:**
- [ ] Extrair NodeCard de ArvoreMapaD3.tsx em componente separado
- [ ] Extrair ContextMenu de ArvoreMapaD3.tsx
- [ ] Remover componentes órfãos da aba Agenda ou implementar aba completa
- [ ] Adicionar testes unitários para helpers e services

**Melhorias UX:**
- [ ] Confirmação ao deletar Dores/Iniciativas
- [ ] Busca textual na visualização Lista
- [ ] Filtros avançados (departamento, perfil, colaborador)
- [ ] Virtual scrolling para estruturas grandes (1000+ nós)
- [ ] Animações de transição ao criar/deletar

**Backend:**
- [ ] Paginação de histórico
- [ ] Campo `ordem` para persistir ordenação de Dores/Iniciativas
- [ ] Validação de ciclos ao mover posições
- [ ] Soft delete de posições (manter histórico)

---

*Documento alinhado à stack React (ARCHITECTURE.md) e ao padrão de APIs do projeto. Última atualização: 27 de Fevereiro de 2026.*
