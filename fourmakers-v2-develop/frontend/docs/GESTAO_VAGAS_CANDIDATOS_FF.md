# Painel de Candidatos (Gestão de Vagas) – Análise FlutterFlow

Documentação técnica da página FlutterFlow `gestao_vagas_candidatos` para integração no React.

## 1. Parâmetros de entrada (widget)

| Parâmetro     | Tipo   | Descrição                          |
|---------------|--------|------------------------------------|
| `nomeVaga`    | String?| Título da vaga                     |
| `clienteVaga` | String | Nome do cliente (default: Nome do Cliente) |
| `gestorVaga`  | String | Nome do gestor (default: Nome do Gestor)   |
| `dataInicio`  | String | Data início filtro (default: 01/01/2025)    |
| `dataFim`     | String | Data fim filtro (default: 01/07/2025)       |
| `idVaga`      | String?| ID da vaga                         |
| `codigoVaga`  | int?   | Código da vaga                     |

A página também usa `FFAppState().dadosVagaRecrutamento` (objeto da vaga) definido ao navegar da tela de gestão de vagas.

## 2. Chamadas de API (on load e ações)

### 2.1 Listar candidatos inscritos (inscritos na vaga)

- **Action:** `listarCandidatosVagas` com `listagem: 'inscritos'`
- **Backend:** `BackendHomologGroup.listarCandidatosVagasCall`
- **Endpoint:** `GET Vaga/ListarCandidatosInscritos`
- **Params:** `busca`, `cursor`, `limite`, `dataInicio`, `dataFim`, `vagaId`, `qualificados`, `diasUltimaAlteracao`, `localizacaoCidade`, `localizacaoEstado`
- **Resposta:** `$.retorno` = lista de candidatos inscritos

### 2.2 Listar status de candidatura

- **Action:** `listarStatusCandidatos`
- **Backend:** `listarStatusCandidaturaRecrutamentoCall`
- **Endpoint:** `GET Vaga/ListarStatusCandidaturaRecrutamento`
- **Params:** (nenhum além do token)
- **Resposta:** lista de status (usados como colunas do Kanban)

### 2.3 Listar totais de inscritos

- **Action:** `listarTotaisInscritos`
- **Backend:** `listarTotaisInscritosCall`
- **Endpoint:** `GET Vaga/ObterTotaisInscritos`
- **Params:** `vagaId`
- **Resposta:** totais por status (Total de Inscritos, Aprovados, Reprovados, Declinados, etc.)

### 2.4 Listar origens de candidatos

- **Action:** `listarOrigensCandidatos(context)`
- Usado para filtros de origem.

## 3. Estado (model)

- `counterInscritosValue` / `counterInscritosValueController`: limite de candidatos na listagem
- `inputBuscaCandidatosTextController`: busca por nome ou descrição do status
- `listaPadraoTabela` (FFAppState): lista de candidatos inscritos
- `listaAderentes` (FFAppState): lista de aderentes (quando listagem aderentes)
- `totalAderentes`, `totalCandidatos` (FFAppState): totais
- Filtros: qualificados, origem, data admissão, cidade/estado
- Colunas Kanban: visibilidade por status (`col1`–`col4` etc.)

## 4. Componentes de UI

- **Header:** botão voltar + título (nome da vaga) + ícone info
- **Informações sobre a vaga:** painel colapsável com detalhes (código, cliente, gestor, abertura, contratação, proposta, tipo, custo, modelo de trabalho) + cards de totais (Total de Inscritos, Candidatos Aprovados, Reprovados, Declinados)
- **Candidatos:** título + subtítulo; barra com busca (“Buscar Nome ou Desc.Status”), botão buscar, filtro, botão “Gerar match”, dropdowns “Opções de tela” e “Mostrar todos”
- **Kanban:** colunas por status de candidatura; em cada coluna, cards de candidato (nome, “Inscrito nesta e em outras X vagas”, % match, badge Colaborador, criado por, candidatura em, ícones de ação, dropdown de status, tempo decorrido, modificado em)
- **Coluna “Aderentes e Qualificados”:** botão “Ver Aderentes”
- Colunas vazias: placeholder “0 Candidatos nesta fase”

## 5. Comportamentos

- Ao carregar: `showMeCall`, depois `listarCandidatosVagas` (inscritos), `listarStatusCandidatos`, `listarTotaisInscritos`, `listarOrigensCandidatos`; `idVaga` e `dadosVagaRecrutamento` vêm do estado/parâmetros.
- Busca: aplica filtro na lista (busca por nome ou descrição do status).
- Kanban: candidatos agrupados por status de candidatura; arrastar/alterar status (dropdown no card) atualiza a fase.
- “Gerar match”: ação de match com IA (BuscarMatchComIACall).

## 6. Integração React

- **Rota:** `/gestaodevagas/candidatos`
- **Navegação:** a partir do ícone de olho no card da vaga em `/gestaodevagas`, navegar com `state: { vagaId, vagaTitle, vagaRaw }` (e opcionalmente cliente, gestor, datas).
- **APIs a expor no React:** `ListarCandidatosInscritos`, `ListarStatusCandidaturaRecrutamento`, `ObterTotaisInscritos` (e, se necessário, detalhes da vaga por `ObterVagaRecrutamentoPorId`).
- **Layout:** replicar header, painel de informações da vaga (colapsável), cards de totais, barra de busca/filtros, Kanban por status com cards de candidato no mesmo estilo da GestaoVagas (cards, bordas, tipografia).
