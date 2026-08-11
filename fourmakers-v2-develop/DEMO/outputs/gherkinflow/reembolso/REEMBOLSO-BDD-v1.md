# Cenários BDD — Módulo de Reembolso

| Tipo | Qtd |
|---|:---:|
| ✅ Positivo | 6 |
| ❌ Negativo | 3 |
| 🔁 Regressivo | 1 |
| **Total** | **10** |

## Reembolso (Dashboard)

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| R-01 | [Positivo] Acesso à lista de reembolsos | um colaborador autenticado | acessa o módulo de Reembolso | visualiza a aba Meus Reembolsos com a lista de solicitações | os painéis de resumo exibem os totais do período | `Reembolso.tsx:60 — useReembolsos()` · `Reembolso.tsx:317 — Meus Reembolsos` |
| R-02 | [Negativo] Abas restritas ocultas sem permissão | um colaborador sem permissão de gestor nem de aprovação | acessa o módulo de Reembolso | visualiza apenas a aba Meus Reembolsos | as abas Gestão ADM e Aprovações não aparecem | `Reembolso.tsx:322 — souGestor` · `Reembolso.tsx:331 — souAprovador` |
| R-03 | [Negativo] Lista vazia com mensagem orientativa | um colaborador sem solicitações no período filtrado | acessa o módulo de Reembolso | a lista aparece vazia com mensagem orientativa | — | `Reembolso.tsx:506 — emptyMessage` |
| R-04 | [Positivo] Navegação para nova solicitação | um colaborador na tela de Reembolso | solicita um novo reembolso | é direcionado à tela de inserção de solicitação | a rota de nova solicitação é a configurada no módulo | `Reembolso.tsx:44 — ROTA_NOVA_SOLICITACAO_REEMBOLSO` · `Reembolso.tsx:300 — navigate()` |

## Inserir Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| I-08 | [Positivo] Inclusão de despesa no carrinho | um colaborador com objetivo, período, projeto, categoria, data, valor, descrição e comprovante quando exigido | adiciona a despesa à solicitação | o item aparece no painel de itens para envio com valor e descrição | o total da solicitação é atualizado | `InserirReembolso.tsx:543 — handleAdicionarCarrinho()` |
| I-09 | [Negativo] Bloqueio por campos obrigatórios vazios | um colaborador com formulário incompleto | tenta adicionar a despesa à solicitação | uma notificação de erro informa os campos obrigatórios | o item não entra no painel de itens | `InserirReembolso.tsx:543 — handleAdicionarCarrinho()` · `InserirReembolso.tsx:611 — toast` |
| I-10 | [Negativo] Comprovante obrigatório ausente | um colaborador em categoria que exige comprovante e sem arquivo anexado | tenta adicionar a despesa à solicitação | o sistema impede a inclusão e sinaliza o comprovante | — | `InserirReembolso.tsx:603 — exigirComprovante` |
| I-13 | [Positivo] Envio da solicitação com itens | um colaborador com ao menos um item no painel de envio | envia a solicitação de reembolso | uma janela de sucesso confirma o envio | o colaborador retorna à lista de reembolsos | `InserirReembolso.tsx:773 — handleEnviarSolicitacoes()` · `InserirReembolso.tsx:922 — navigate()` |

## Aprovar Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| AP-01 | [Positivo] Aprovação de solicitações selecionadas | um colaborador com permissão de aprovação e itens selecionados | confirma a aprovação das solicitações | as solicitações são aprovadas com sucesso | a lista de pendências é atualizada | `AprovarReembolso.tsx:371 — handleAprovar()` · `AprovarReembolso.tsx:380 — confirmarAprovar()` |

## Parâmetros de Reembolso

| # | Funcionalidade (Feature) | Dado (Given) | Quando (When) | Então (Then) | E… (And) | Referência do Código |
|---|---|---|---|---|---|---|
| RP-01 | [Regressivo] Visualização das regras de verba | um gestor acessando os parâmetros do módulo | abre a aba de regras | visualiza os cartões de verba com limites e exigências | as alterações locais atualizam o cartão correspondente | `RegrasTab.tsx:11 — RegrasTab` · `VerbaCard.tsx:13 — VerbaCard` |
